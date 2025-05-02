using UnityEngine;

namespace AgaveCase.Elements.Runtime
{
    public class ElementStateHandler
    {
        private readonly DefaultElement _element;
        private readonly SpriteRenderer _selectedRenderer; 

        public ElementStateHandler(DefaultElement element, SpriteRenderer selectedRenderer)
        {
            _element = element;
            _selectedRenderer = selectedRenderer;
        }

        public void OnSelected()
        { 
            if (_selectedRenderer != null)
            {
                _selectedRenderer.enabled = true;
            }

            _element.AnimationHandler.PlaySelectAnimation();
        }

        public void OnDeselected()
        {
            if (_selectedRenderer != null)
            {
                _selectedRenderer.enabled = false;
            }

            _element.AnimationHandler.PlayDeselectAnimation();
        }

        public void OnMatched()
        { 
            if (_selectedRenderer != null)
            {
                _selectedRenderer.enabled = false;
            }

            _element.Pooler.Spawn(_element.ElementData.DestroyPoolTypeSo, _element.transform.position,
                Quaternion.identity);
            _element.AnimationHandler.PlayMatchAnimation(() => _element.ReturnToPool());
        }
    }
}