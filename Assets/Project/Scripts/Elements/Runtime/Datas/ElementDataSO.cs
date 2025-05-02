using AgaveCase.Pooler.Runtime;
using GuidSystem.Runtime; 
using UnityEngine;

namespace AgaveCase.Elements.Runtime
{
    [CreateAssetMenu(fileName = "New Element Data", menuName = "Agave Case/Grid System/Element Data")]
    public class ElementDataSO : ScriptableObject
    {
        [SerializeField] private SerializableGuid _id = SerializableGuid.NewGuid();
        [SerializeField] private string _elementName;
        [SerializeField] private Sprite _icon; 
        [SerializeField] private int _pointValue = 10; 
        
        [Header("Pool Types")]
        [SerializeField] private PoolObjectTypeSO _elementPoolTypeSo;
        [SerializeField] private PoolObjectTypeSO _destroyPoolTypeSo;

        public SerializableGuid Id => _id;
        public string ElementName => _elementName;
        public Sprite Icon => _icon; 
        public int PointValue => _pointValue;  
        public PoolObjectTypeSO ElementPoolTypeSo => _elementPoolTypeSo;
        public PoolObjectTypeSO DestroyPoolTypeSo => _destroyPoolTypeSo;

        public void SetGuid(SerializableGuid newGuid)
        {
            _id = newGuid;
        } 
    }
}