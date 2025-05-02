using UnityEngine; 
using System.Collections;
using AgaveCase.Pooler.Runtime;

namespace AgaveCase.Elements.Runtime
{
    public class PoolableEffect : MonoBehaviour, IPoolable
    {
        [SerializeField] private float _effectDuration = 1.0f;
        [SerializeField] private ParticleSystem _particleSystem;
        
        private Coroutine _returnToPoolCoroutine; 
        public PoolObjectTypeSO PoolTypeSO { get; set; } 
        public ObjectPooler Pooler { get; set; }
        public GameObject GameObject => gameObject;
        
        public void OnGetFromPool()
        {
            gameObject.SetActive(true);
            
            if (_particleSystem != null)
            {
                _particleSystem.Play();
            }
             
            if (_returnToPoolCoroutine != null)
            {
                StopCoroutine(_returnToPoolCoroutine);
            }
            _returnToPoolCoroutine = StartCoroutine(ReturnToPoolAfterDuration());
        }
        
        public void OnReturnToPool()
        {
            if (_particleSystem != null)
            {
                _particleSystem.Stop();
            }
            
            gameObject.SetActive(false);
        }
        
        private IEnumerator ReturnToPoolAfterDuration()
        {
            yield return new WaitForSeconds(_effectDuration);
            
            if (Pooler != null)
            {
                Pooler.Release(this);
            }
            else
            {
                gameObject.SetActive(false);
            }
            
            _returnToPoolCoroutine = null;
        }
    }
}