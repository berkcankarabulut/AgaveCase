using AgaveCase.Elements.Runtime;
using UnityEngine;

namespace Grid.Runtime
{
    public class GridCell : MonoBehaviour
    {  
        [SerializeField] private SpriteRenderer _spriteRenderer;
        private Vector2Int _position;
        private ElementBase _currentElement;
        public Vector2Int Position => _position;

        public SpriteRenderer SpriteRenderer => _spriteRenderer;

        public void Initialize(Vector2Int position)
        {
            _position = position; 
        }

        public void SetCandy(ElementBase element)
        {
            _currentElement = element;
            if (element == null) return; 
        }

        public ElementBase GetElement()
        {
            return _currentElement;
        }  
    }
}