using UnityEngine;
using Pooler;

namespace AgaveCase.Elements.Runtime
{
    public abstract class ElementBase : MonoBehaviour, IPoolable
    {
        [SerializeField] protected SpriteRenderer spriteRenderer;
        protected ElementDataSO elementData;

        // Animasyon handler
        protected ElementAnimationHandler _animationHandler;

        public GameObject GameObject => gameObject;
        public ElementDataSO ElementData => elementData;

        [Header("Animation Settings")] [SerializeField]
        private float _selectAnimationDuration = 0.2f;

        [SerializeField] private float _selectAnimationScale = 1.1f;
        [SerializeField] private float _matchHighlightDuration = 0.3f;
        [SerializeField] private float _matchScaleUpFactor = 1.2f;
        [SerializeField] private float _destroyAnimationDuration = 0.4f;

        private void Awake()
        {
            // Animator handler'ı oluştur
            if (spriteRenderer != null)
            {
                _animationHandler = new ElementAnimationHandler(this, spriteRenderer);
            }
        }

        public virtual void Initialize(ElementDataSO elementDataSO)
        {
            elementData = elementDataSO;

            if (spriteRenderer == null || elementData == null) return;
            spriteRenderer.sprite = elementData.Icon;

            // Renk ve ölçek sıfırla
            spriteRenderer.color = Color.white;
            transform.localScale = Vector3.one;
        }

        public virtual void OnGetFromPool()
        {
            gameObject.SetActive(true);

            // Element animasyonunu durdur varsa
            if (_animationHandler != null)
            {
                _animationHandler.KillAnimations();
            }
        }

        public virtual void OnReturnToPool()
        {
            // Element animasyonunu durdur varsa
            if (_animationHandler != null)
            {
                _animationHandler.KillAnimations();
            }

            gameObject.SetActive(false);
        }

        /// <summary>
        /// Element seçildiğinde çağrılır
        /// </summary>
        public virtual void OnSelected()
        {
            if (_animationHandler != null)
            {
                _animationHandler.PlaySelectAnimation(_selectAnimationDuration, _selectAnimationScale);
            }
        }

        /// <summary>
        /// Element seçimden çıkarıldığında çağrılır
        /// </summary>
        public virtual void OnDeselected()
        {
            if (_animationHandler != null)
            {
                _animationHandler.PlayDeselectAnimation(_selectAnimationDuration);
            }
        }

        /// <summary>
        /// Element eşleştiğinde çağrılır
        /// </summary>
        public virtual void OnMatched()
        {
            if (_animationHandler != null)
            {
                // Eşleşme animasyonu bittiğinde havuza döndür
                _animationHandler.PlayMatchAnimation(
                    _matchHighlightDuration,
                    _matchScaleUpFactor,
                    _destroyAnimationDuration,
                    () =>
                    {
                        if (gameObject.activeInHierarchy)
                        {
                            ReturnToPool();
                        }
                    }
                );
            }
            else
            {
                // Animasyon handler yoksa direkt havuza döndür
                ReturnToPool();
            }
        }

        /// <summary>
        /// Element oluşma animasyonu oynatır
        /// </summary>
        public virtual void PlaySpawnAnimation(Vector3 startPosition, Vector3 endPosition, float duration,
            DG.Tweening.Ease easeType, System.Action onCompleted = null)
        {
            if (_animationHandler != null)
            {
                _animationHandler.PlaySpawnAnimation(startPosition, endPosition, duration, easeType, onCompleted);
            }
            else if (onCompleted != null)
            {
                onCompleted.Invoke();
            }
        }

        /// <summary>
        /// Element düşme animasyonu oynatır
        /// </summary>
        public virtual void PlayFallAnimation(Vector3 targetPosition, float duration,
            DG.Tweening.Ease easeType, float delay = 0f, System.Action onCompleted = null)
        {
            if (_animationHandler != null)
            {
                _animationHandler.PlayFallAnimation(targetPosition, duration, easeType, delay, onCompleted);
            }
            else if (onCompleted != null)
            {
                onCompleted.Invoke();
            }
        }

        public virtual void ReturnToPool()
        {
            if (_animationHandler != null) _animationHandler.KillAnimations();

            // Havuza döndürme işlemi için ObjectPooler'ı kullan
            if (ObjectPooler.Instance != null) ObjectPooler.Instance.Release(this.gameObject, this);

            else gameObject.SetActive(false);
        }

        private void OnDestroy()
        { 
            if (_animationHandler != null)
            {
                _animationHandler.Cleanup();
                _animationHandler = null;
            }
        }
    }
}