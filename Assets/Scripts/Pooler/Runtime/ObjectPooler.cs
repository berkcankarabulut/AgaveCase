using System.Collections.Generic;
using SingletonHandler.Runtime;
using UnityEngine;
using UnityEngine.Pool;

namespace Pooler
{
    public class ObjectPooler : Singleton<ObjectPooler>
    {  
        [Header("Pool Settings")]
        public List<PooledObjectData> objectsToPool;

        private Dictionary<GameObject, ObjectPool<IPoolable>> pools = new Dictionary<GameObject, ObjectPool<IPoolable>>();
        private Dictionary<GameObject, IPoolable> prefabMap = new();

        protected override void Awake()
        { 
            base.Awake(); 
            InitializePools();
        }

        private void InitializePools()
        {
            foreach (var data in objectsToPool)
            {
                if (data.prefab == null || pools.ContainsKey(data.prefab)) continue;

                var prefabPoolable = data.prefab.GetComponent<IPoolable>();
                if (prefabPoolable == null)
                {
                    Debug.LogError($"Prefab {data.prefab.name} does not implement IPoolable.");
                    continue;
                }

                prefabMap[data.prefab] = prefabPoolable;

                var pool = new ObjectPool<IPoolable>(
                    createFunc: () =>
                    {
                        var obj = Instantiate(data.prefab);
                        return obj.GetComponent<IPoolable>();
                    },
                    actionOnGet: obj =>
                    {
                        (obj as MonoBehaviour).gameObject.SetActive(true);
                        obj.OnGetFromPool();
                    },
                    actionOnRelease: obj =>
                    {
                        obj.OnReturnToPool();
                        (obj as MonoBehaviour).gameObject.SetActive(false);
                    },
                    actionOnDestroy: obj =>
                    {
                        Destroy((obj as MonoBehaviour).gameObject);
                    },
                    collectionCheck: false,
                    defaultCapacity: data.defaultCapacity,
                    maxSize: data.maxSize
                );

                for (int i = 0; i < data.defaultCapacity; i++)
                {
                    var instance = pool.Get();
                    var go = (instance as MonoBehaviour)?.gameObject;

                    if (go != null)
                    {
                        Debug.Log($"[ObjectPooler] Preloaded: {go.name}");
                    }
                    else
                    {
                        Debug.LogError($"[ObjectPooler] Failed to preload object for prefab: {data.prefab.name}");
                    }

                    pool.Release(instance);
                }

                pools[data.prefab] = pool;
            }
        }

        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            if (!pools.ContainsKey(prefab))
            {
                Debug.LogWarning($"[ObjectPooler] Prefab not registered: {prefab.name}");
                return null;
            }

            var poolable = pools[prefab].Get();
            var go = (poolable as MonoBehaviour).gameObject;
            go.transform.SetPositionAndRotation(position, rotation);
            return go;
        }

        public void Release(GameObject prefab, IPoolable poolable)
        {
            if (!pools.ContainsKey(prefab))
            {
                Destroy(poolable.GameObject);
                return;
            }

            if (poolable != null)
                pools[prefab].Release(poolable);
            else
                Destroy(poolable.GameObject);
        }
    }
}
