using UnityEngine;

namespace Pooler
{
    [System.Serializable]
    public class PooledObjectData
    {
        public GameObject prefab;
        public int defaultCapacity = 10;
        public int maxSize = 100;
    }
}