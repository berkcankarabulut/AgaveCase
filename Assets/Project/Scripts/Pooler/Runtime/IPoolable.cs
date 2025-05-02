using UnityEngine;

namespace AgaveCase.Pooler.Runtime
{
    public interface IPoolable
    {
        public PoolObjectTypeSO PoolTypeSO { get; set; }
        public ObjectPooler Pooler { get; set; }
        GameObject GameObject { get; }
        void OnGetFromPool();
        void OnReturnToPool();
    }
}