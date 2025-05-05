using GuidSystem.Runtime;
using UnityEngine;

namespace AgaveCase.Pooler.Runtime 
{
    [CreateAssetMenu(fileName = "Pool Object Type", menuName = "Agave Case/Pool System/Pool Object Type")]
    public class PoolObjectTypeSO : ScriptableObject
    {
        public SerializableGuid Id = SerializableGuid.NewGuid();
 
        private void AssignNewGuid()
        {
            Id = SerializableGuid.NewGuid();
        }
    }
}