using UnityEngine;
using UnityEngine.Serialization;

namespace AgaveCase.Pooler.Runtime
{
    [System.Serializable]
    public class PooledObjectData
    {
        [FormerlySerializedAs("poolObjectType")] public PoolObjectTypeSO poolObjectTypeSo;
        public Transform parentObject;
        public GameObject prefab;
        public int defaultCapacity = 10;
        public int maxSize = 100;
    }
}