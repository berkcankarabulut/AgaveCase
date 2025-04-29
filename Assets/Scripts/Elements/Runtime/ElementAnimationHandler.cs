using UnityEngine;
using DG.Tweening;
using System;

namespace AgaveCase.Elements.Runtime
{
    /// <summary>
    /// Element animasyonlarını yöneten sınıf
    /// </summary>
    public class ElementAnimationHandler
    {
        // Element referansları
        private readonly ElementBase _element;
        private readonly SpriteRenderer _spriteRenderer;
        private readonly Transform _elementTransform;
         
        private Sequence _currentAnimation;
        
        public ElementAnimationHandler(ElementBase element, SpriteRenderer spriteRenderer)
        {
            _element = element;
            _spriteRenderer = spriteRenderer;
            _elementTransform = element.transform;
        }
         
        public void PlaySelectAnimation(float duration, float scale)
        {
            if (_spriteRenderer == null) return;
            
            // Önceki animasyonları temizle
            KillAnimations();
            
            // Yarı saydam efekti
             _spriteRenderer.material.DOColor(new Color(1f, 1f, 1f, 0.7f), duration); 
            
            // Hafif bir scale efekti ekle
            _elementTransform.DOScale(scale, duration).SetEase(Ease.OutBack);
        }
        
        /// <summary>
        /// Element seçimden çıkarıldığında çağrılır, vurgulamayı kaldırır
        /// </summary>
        public void PlayDeselectAnimation(float duration)
        {
            if (_spriteRenderer == null) return;
            
            // Önceki animasyonları temizle
            KillAnimations();
            
            // Normal renge dön
            
            _spriteRenderer.material.DOColor(Color.white, duration);
             
            
            // Normal ölçeğe dön
            _elementTransform.DOScale(1.0f, duration).SetEase(Ease.OutBack);
        }
        
        /// <summary>
        /// Element eşleştiğinde çağrılır, eşleşme efekti ekler ve sonra yok eder
        /// </summary>
        public void PlayMatchAnimation(float highlightDuration, float scaleUpFactor, float destroyDuration, Action onCompleted = null)
        {
            if (_spriteRenderer == null) return;
            
            // Önceki animasyonları temizle
            KillAnimations();
            
            // Eşleşme animasyonu (büyütüp küçültme, parıldama ve yok olma)
            _currentAnimation = DOTween.Sequence();
            
            // Önce büyüt
            _currentAnimation.Append(_elementTransform.DOScale(Vector3.one * scaleUpFactor, highlightDuration)
                .SetEase(Ease.OutBack));
            
            // Sonra parılda (rengi değiştir)
            _currentAnimation.Join(_spriteRenderer.material.DOColor(Color.white, highlightDuration)
                .SetEase(Ease.Flash, 3)); 
            // Sonra küçülterek yok et
            _currentAnimation.Append(_elementTransform.DOScale(Vector3.zero, destroyDuration)
                .SetEase(Ease.InBack));
            
            // Aynı zamanda soluklaştır
            _currentAnimation.Join(_spriteRenderer.material.DOFade(0, destroyDuration)
                .SetEase(Ease.InQuad));
            
            // Animasyon bitince callback'i çağır
            _currentAnimation.OnComplete(() => {
                onCompleted?.Invoke();
            });
        }
        
        /// <summary>
        /// Yeni element oluşturulduğunda düşme animasyonu oynatır
        /// </summary>
        public void PlaySpawnAnimation(Vector3 startPosition, Vector3 endPosition, 
                                    float duration, Ease easeType, Action onCompleted = null)
        {
            if (_elementTransform == null) return;
            
            // Önceki animasyonları temizle
            KillAnimations();
            
            // Elementi başlangıç pozisyonuna yerleştir
            _elementTransform.position = startPosition;
            
            // Düşme animasyonu
            _elementTransform.DOMove(endPosition, duration)
                .SetEase(easeType)
                .OnComplete(() => {
                    onCompleted?.Invoke();
                });
        }
        
        /// <summary>
        /// Element düşürme animasyonu oynatır
        /// </summary>
        public void PlayFallAnimation(Vector3 targetPosition, float duration, 
                                   Ease easeType, float delay = 0f, Action onCompleted = null)
        {
            if (_elementTransform == null) return;
            
            // Önceki animasyonları temizle
            KillAnimations();
            
            // Düşme animasyonu
            _elementTransform.DOMove(targetPosition, duration)
                .SetEase(easeType)
                .SetDelay(delay)
                .OnComplete(() => {
                    onCompleted?.Invoke();
                });
        }
        
        /// <summary>
        /// Tüm aktif animasyonları durdurur ve temizler
        /// </summary>
        public void KillAnimations()
        {
            if (_currentAnimation != null)
            {
                _currentAnimation.Kill();
                _currentAnimation = null;
            }
            
            if (_elementTransform != null)
            {
                DOTween.Kill(_elementTransform);
            }
            
            if (_spriteRenderer != null)
            {
                DOTween.Kill(_spriteRenderer);
            }
        }
        
        /// <summary>
        /// Sınıf temizlenirken çağrılır (ElementBase OnDestroy içinden)
        /// </summary>
        public void Cleanup()
        {
            KillAnimations();
        }
    }
}