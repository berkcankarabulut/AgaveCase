using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using GuidSystem.Runtime;

namespace AgaveCase.Pooler.Runtime
{
    public class ObjectPooler : MonoBehaviour
    {
        [Header("Pool Settings")] 
        [SerializeField] private List<PooledObjectData> objectsToPool;
        private Dictionary<GameObject, bool> _activeObjects = new Dictionary<GameObject, bool>();

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
                if (data.Prefab == null || data.PoolObjectTypeSo == null) continue;
                
                SerializableGuid typeId = data.PoolObjectTypeSo.Id;
                 
                if (_pools.ContainsKey(typeId)) continue;

                IPoolable prefabPoolable = data.Prefab.GetComponent<IPoolable>();
                if (prefabPoolable == null) continue;

                Transform poolParent;
                if (data.ParentObject != null)
                {
                    poolParent = data.ParentObject; 
                }
                else
                {
                    poolParent = new GameObject($"Pool_{data.Prefab.name}_{typeId.ToHexString()}").transform;
                    poolParent.SetParent(_poolContainer); 
                }

                ObjectPool<IPoolable> pool = CreateObjectPool(data, poolParent); 
                _pools[typeId] = pool; 
                PreloadPool(pool, data.DefaultCapacity);
            }
        }

        private static ObjectPool<IPoolable> CreateObjectPool(PooledObjectData data, Transform poolParent)
        {
            var pool = new ObjectPool<IPoolable>(
                createFunc: () =>
                {
                    var obj = Instantiate(data.Prefab, poolParent);
                    obj.name = $"{data.Prefab.name}_Instance";
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
                defaultCapacity: data.DefaultCapacity,
                maxSize: data.MaxSize
            );
            return pool;
        }

        private void PreloadPool(ObjectPool<IPoolable> pool, int count)
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
    
            if (!_pools.ContainsKey(typeId)) return null;

            var poolable = _pools[typeId].Get();
            var go = poolable.GameObject;
     
            if (_activeObjects.ContainsKey(go) && _activeObjects[go])
            { 
                return CreateEmergencyInstance(poolObjectTypeSo, position, rotation);
            }
    
            poolable.Pooler = this;
            poolable.PoolTypeSO = poolObjectTypeSo;
            go.transform.SetPositionAndRotation(position, rotation);
     
            _activeObjects[go] = true;
     
            return go;
        }

        
        public void Release(IPoolable poolable)
        {
            if (poolable == null) return;
    
            SerializableGuid typeId = poolable.PoolTypeSO.Id;
            GameObject go = poolable.GameObject;
     
            if (_activeObjects.ContainsKey(go))
            {
                _activeObjects[go] = false;
            }
    
            if (_pools.ContainsKey(typeId))
            {
                _pools[typeId].Release(poolable);
            }
            else
            { 
                Destroy(poolable.GameObject);
            }
        }
        
        private GameObject CreateEmergencyInstance(PoolObjectTypeSO poolObjectTypeSo, Vector3 position, Quaternion rotation)
        { 
            GameObject prefab = null;
            foreach (var data in objectsToPool)
            {
                if (data.PoolObjectTypeSo == poolObjectTypeSo)
                {
                    prefab = data.Prefab;
                    break;
                }
            }
    
            if (prefab == null)  return null;
             
            var obj = Instantiate(prefab, position, rotation);
            var poolable = obj.GetComponent<IPoolable>();
            poolable.Pooler = this;
            poolable.PoolTypeSO = poolObjectTypeSo;
            poolable.OnGetFromPool();
     
            _activeObjects[obj] = true;
    
            return obj;
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