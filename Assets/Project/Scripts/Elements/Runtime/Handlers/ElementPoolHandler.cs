using UnityEngine;

namespace AgaveCase.Elements.Runtime
{
    public class ElementPoolHandler
    {
        private readonly ElementBase _element;
        private readonly SpriteRenderer _spriteRenderer;

        public ElementPoolHandler(ElementBase element, SpriteRenderer spriteRenderer)
        {
            _element = element;
            _spriteRenderer = spriteRenderer;
        }

        public void OnGetFromPool()
        {
            ResetElement();
        }

        public void OnReturnToPool()
        { 
            if (_element is DefaultElement defaultElement)
            {
                defaultElement.AnimationHandler?.KillAnimations();
            }
        }

        public void ResetElement()
        { 
            if (_element is DefaultElement defaultElement)
            {
                defaultElement.AnimationHandler?.KillAnimations();
            }

            if (_element.transform != null)
            {
                _element.transform.localScale = Vector3.one;
            }

            if (_spriteRenderer != null)
            {
                Color color = _spriteRenderer.color;
                color.a = 1f;
                _spriteRenderer.color = color;

                if (_spriteRenderer.material != null)
                {
                    _spriteRenderer.material.color = Color.white;
                }
            }
        }

        public void ReturnToPool()
        { 
            ResetElement();

            if (_element.Pooler != null)
            { 
                _element.Pooler.Release(_element); 
                _element.GameObject.transform.SetParent(null);
            }
            else
            { 
                _element.GameObject.SetActive(false);
            }
        }
    }
}