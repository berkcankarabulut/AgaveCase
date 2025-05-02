using AgaveCase.Pooler.Runtime;
using UnityEngine;

namespace AgaveCase.Elements.Runtime
{
    public class ElementBase : MonoBehaviour, IPoolable
    {
        [Header("Core Properties")]
        [SerializeField] protected SpriteRenderer spriteRenderer; 
        public PoolObjectTypeSO PoolTypeSO { get; set; }  
        public ObjectPooler Pooler { get; set; }
        public GameObject GameObject => gameObject;
     
        private ElementDataSO _elementData;
        public ElementDataSO ElementData => _elementData; 
        private ElementPoolHandler _poolHandler;
     
        public virtual void Initialize(ElementDataSO elementDataSO)
        {
            _elementData = elementDataSO;
            if (spriteRenderer == null) return;
            spriteRenderer.sprite = _elementData.Icon; 
        }
     
        public virtual void OnGetFromPool()
        {
            gameObject.SetActive(true);
           
            _poolHandler ??= new ElementPoolHandler(this, spriteRenderer);
            _poolHandler.OnGetFromPool();
        }
    
        public virtual void OnReturnToPool()
        {
            _poolHandler.OnReturnToPool();
            gameObject.SetActive(false);
        }
     
        public virtual void ReturnToPool()
        {
            _poolHandler.ReturnToPool();
        }
    }
}