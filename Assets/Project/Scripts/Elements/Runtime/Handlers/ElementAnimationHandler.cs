using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace AgaveCase.Elements.Runtime
{
    public class ElementAnimationHandler
    {
        private readonly DefaultElement _element;
        private readonly Transform _transform;
        private readonly SpriteRenderer _selectedRenderer;
        private readonly SpriteRenderer _spriteRenderer;
        private Vector3 _originalScale;

        private float _selectAnimationDuration;
        private float _selectAnimationScale;
        private float _matchHighlightDuration;
        private float _matchScaleUpFactor;
        private float _destroyAnimationDuration;

        private static Dictionary<DefaultElement, Sequence> _activeAnimations =
            new Dictionary<DefaultElement, Sequence>();

        private static bool _isInitialized = false;

        public ElementAnimationHandler(
            DefaultElement element,
            Transform transform,
            SpriteRenderer selectedRenderer)
        {
            _element = element;
            _transform = transform;
            _selectedRenderer = selectedRenderer;
            _spriteRenderer = element.GetComponent<SpriteRenderer>();
            _originalScale = transform.localScale;

            if (!_isInitialized)
            {
                DOTween.SetTweensCapacity(500, 50);
                _isInitialized = true;
            }
        }

        public void SetAnimationSettings(
            float selectAnimationDuration,
            float selectAnimationScale,
            float matchHighlightDuration,
            float matchScaleUpFactor,
            float destroyAnimationDuration)
        {
            _selectAnimationDuration = selectAnimationDuration;
            _selectAnimationScale = selectAnimationScale;
            _matchHighlightDuration = matchHighlightDuration;
            _matchScaleUpFactor = matchScaleUpFactor;
            _destroyAnimationDuration = destroyAnimationDuration;
        }

        public void PlaySelectAnimation()
        {
            if (_element == null || _transform == null) return;
            KillAnimations();

            Sequence sequence = DOTween.Sequence();
            sequence.Append(_transform.DOScale(_selectAnimationScale, _selectAnimationDuration).SetEase(Ease.OutBack));
            _activeAnimations[_element] = sequence;
        }

        public void PlayDeselectAnimation()
        {
            if (_element == null || _transform == null) return;

            KillAnimations();

            Sequence sequence = DOTween.Sequence();
            sequence.Append(_transform.DOScale(_originalScale, _selectAnimationDuration).SetEase(Ease.OutBack));
            sequence.OnComplete(() =>
            {
                if (_element != null && _transform != null)
                {
                    _transform.localScale = _originalScale;
                }

                if (_activeAnimations.ContainsKey(_element))
                {
                    _activeAnimations.Remove(_element);
                }
            });
            _activeAnimations[_element] = sequence;
        }

        public void PlayMatchAnimation(Action onCompleted = null)
        {
            if (_element == null || _transform == null) return;

            if (_spriteRenderer == null)
            {
                onCompleted?.Invoke();
                return;
            }

            KillAnimations();

            Sequence sequence = DOTween.Sequence();
            sequence.Append(_transform.DOScale(_originalScale * _matchScaleUpFactor, _matchHighlightDuration)
                .SetEase(Ease.OutBack));
            sequence.Join(_spriteRenderer.material.DOColor(Color.white, _matchHighlightDuration)
                .SetEase(Ease.Flash, 3));
            sequence.Append(_transform.DOScale(Vector3.zero, _destroyAnimationDuration)
                .SetEase(Ease.InBack));
            sequence.Join(_spriteRenderer.material.DOFade(0, _destroyAnimationDuration)
                .SetEase(Ease.InQuad));
            sequence.OnComplete(() =>
            {
                onCompleted?.Invoke();

                if (_activeAnimations.ContainsKey(_element))
                {
                    _activeAnimations.Remove(_element);
                }
            });

            _activeAnimations[_element] = sequence;
        }

        public void PlaySpawnAnimation(Vector3 startPosition, Vector3 endPosition, float duration,
            DG.Tweening.Ease easeType, Action onCompleted = null)
        {
            if (_element == null || _transform == null) return;

            KillAnimations();

            _transform.position = startPosition;

            Tween tween = _transform.DOMove(endPosition, duration)
                .SetEase(easeType)
                .OnComplete(() => { onCompleted?.Invoke(); });
        }

        public void PlayFallAnimation(Vector3 targetPosition, float duration,
            DG.Tweening.Ease easeType, float delay = 0f, Action onCompleted = null)
        {
            if (_element == null || _transform == null) return;

            KillAnimations();

            Tween tween = _transform.DOMove(targetPosition, duration)
                .SetEase(easeType)
                .SetDelay(delay)
                .OnComplete(() => { onCompleted?.Invoke(); });
        }

        public void KillAnimations()
        {
            if (_element == null) return;

            if (_activeAnimations.TryGetValue(_element, out Sequence sequence))
            {
                sequence.Kill();
                _activeAnimations.Remove(_element);
            }

            if (_transform != null)
            {
                DOTween.Kill(_transform);
            }

            if (_spriteRenderer != null && _spriteRenderer.material != null)
            {
                DOTween.Kill(_spriteRenderer.material);
                DOTween.Kill(_spriteRenderer);
            }
        }

        public Vector3 GetOriginalScale()
        {
            return _originalScale;
        }
    }
}