using GuidSystem.Runtime;
using UnityEngine;

namespace Project.Pooler.Runtime 
{
    [CreateAssetMenu(fileName = "Pool Object Type", menuName = "Game ScriptableObjects/Pool System/Pool Object Type")]
    public class PoolObjectTypeSO : ScriptableObject
    {
        public SerializableGuid Id = SerializableGuid.NewGuid();
 
        private void AssignNewGuid()
        {
            Id = SerializableGuid.NewGuid();
        }
    }
}