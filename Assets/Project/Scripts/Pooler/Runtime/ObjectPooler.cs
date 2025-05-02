using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using GuidSystem.Runtime;

namespace AgaveCase.Pooler.Runtime
{
    public class ObjectPooler : MonoBehaviour
    {
        [Header("Pool Settings")] 
        public List<PooledObjectData> objectsToPool;

        private Transform _poolContainer;
 
        private Dictionary<SerializableGuid, ObjectPool<IPoolable>> _pools = 
            new Dictionary<SerializableGuid, ObjectPool<IPoolable>>();

        private void Awake()
        {
            _poolContainer = transform;
            InitializePools();
        }

        private void InitializePools()
        {
            foreach (var data in objectsToPool)
            {
                if (data.prefab == null || data.poolObjectTypeSo == null) continue;
                
                SerializableGuid typeId = data.poolObjectTypeSo.Id;
                 
                if (_pools.ContainsKey(typeId)) continue;

                IPoolable prefabPoolable = data.prefab.GetComponent<IPoolable>();
                if (prefabPoolable == null) continue;

                Transform poolParent;
                if (data.parentObject != null)
                {
                    poolParent = data.parentObject;
                    Debug.Log($"[ObjectPooler] Using custom parent for {data.prefab.name}: {poolParent.name}");
                }
                else
                {
                    poolParent = new GameObject($"Pool_{data.prefab.name}_{typeId.ToHexString()}").transform;
                    poolParent.SetParent(_poolContainer);
                    Debug.Log($"[ObjectPooler] Created new parent for {data.prefab.name}: {poolParent.name}");
                }

                var pool = new ObjectPool<IPoolable>(
                    createFunc: () =>
                    {
                        var obj = Instantiate(data.prefab, poolParent);
                        obj.name = $"{data.prefab.name}_Instance";
                        var poolable = obj.GetComponent<IPoolable>();
                        return poolable;
                    },
                    actionOnGet: obj =>
                    {
                        GameObject go = obj.GameObject;
                        go.SetActive(true);
                        obj.OnGetFromPool();
                    },
                    actionOnRelease: obj =>
                    {
                        obj.OnReturnToPool();
                        GameObject go = obj.GameObject;
                        go.SetActive(false);
                        go.transform.SetParent(poolParent);
                    },
                    actionOnDestroy: obj => 
                    { 
                        Destroy(obj.GameObject); 
                    },
                    collectionCheck: false,
                    defaultCapacity: data.defaultCapacity,
                    maxSize: data.maxSize
                );

                _pools[typeId] = pool;

                PreloadPool(pool, data.defaultCapacity, poolParent);
            }
        }

        private void PreloadPool(ObjectPool<IPoolable> pool, int count, Transform container)
        {
            List<IPoolable> instances = new List<IPoolable>();

            for (int i = 0; i < count; i++)
            {
                var instance = pool.Get();
                instances.Add(instance);
            }

            foreach (var instance in instances)
            {
                pool.Release(instance);
            }
        }
 
        public GameObject Spawn(PoolObjectTypeSO poolObjectTypeSo, Vector3 position, Quaternion rotation)
        {
            SerializableGuid typeId = poolObjectTypeSo.Id;
            
            if (!_pools.ContainsKey(typeId))
            {
                Debug.LogWarning($"[ObjectPooler] Type not registered: {poolObjectTypeSo.name} with ID {typeId.ToHexString()}");
                return null;
            }

            var poolable = _pools[typeId].Get();
            var go = poolable.GameObject;
            poolable.Pooler = this;
            poolable.PoolTypeSO = poolObjectTypeSo;
            go.transform.SetPositionAndRotation(position, rotation);

            return go;
        } 
        
        public void Release(IPoolable poolable)
        {
            if (poolable == null)
            {
                Debug.LogError("[ObjectPooler] Tried to release null poolable object");
                return;
            }
            
            SerializableGuid typeId = poolable.PoolTypeSO.Id;
            
            if (_pools.ContainsKey(typeId))
            {
                _pools[typeId].Release(poolable);
            }
            else
            {
                Debug.LogWarning($"[ObjectPooler] Type not registered: {poolable.PoolTypeSO.name} with ID {typeId.ToHexString()}. Destroying object instead.");
                Destroy(poolable.GameObject);
            }
        }

        public void ClearPools()
        {
            _pools.Clear();
            
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Transform child = transform.GetChild(i);
                if (child.name.StartsWith("Pool_"))
                {
                    Destroy(child.gameObject);
                }
            }
        }
    }
}