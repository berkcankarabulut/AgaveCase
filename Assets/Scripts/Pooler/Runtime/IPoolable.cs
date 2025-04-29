using UnityEngine;

namespace Pooler
{
    public interface IPoolable
    {
        GameObject GameObject { get; }
        void OnGetFromPool();
        void OnReturnToPool();
    }
}