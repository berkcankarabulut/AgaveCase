using UnityEngine; 

namespace Project.Pooler.Runtime
{
    [System.Serializable]
    public class PooledObjectData
    {
        [SerializeField] private PoolObjectTypeSO _poolObjectTypeSo;
        [SerializeField] private Transform _parentObject;
        [SerializeField] private GameObject _prefab;
        [SerializeField] private int _defaultCapacity = 10;
        [SerializeField] private int _maxSize = 100;

        public PoolObjectTypeSO PoolObjectTypeSo => _poolObjectTypeSo;
        public Transform ParentObject => _parentObject;
        public GameObject Prefab => _prefab;
        public int DefaultCapacity => _defaultCapacity;
        public int MaxSize => _maxSize;
    }
}