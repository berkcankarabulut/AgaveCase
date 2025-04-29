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
         
        [SerializeField] private float _baseSpeed = 1f; 
        [SerializeField] private int _baseValue = 10;
         
        [SerializeField] private GameObject _spawnEffectPrefab;
        [SerializeField] private GameObject _destroyEffectPrefab;
        [SerializeField] private AudioClip _spawnSound;
        [SerializeField] private AudioClip _destroySound;

        public string ElementName => _elementName;
        public Sprite Icon => _icon;
        public SerializableGuid Id => _id;
        public float BaseSpeed => _baseSpeed;  
        public int BaseValue => _baseValue;
        public GameObject SpawnEffectPrefab => _spawnEffectPrefab;
        public GameObject DestroyEffectPrefab => _destroyEffectPrefab;
        public AudioClip SpawnSound => _spawnSound;
        public AudioClip DestroySound => _destroySound;
        
        public void SetGuid(SerializableGuid newGuid)
        {
            _id = newGuid;
        } 
    }
}