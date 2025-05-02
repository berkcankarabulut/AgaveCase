using AgaveCase.Elements.Runtime;
using UnityEngine;

namespace Grid.Runtime
{
    public class GridCell : MonoBehaviour
    {  
        [SerializeField] private SpriteRenderer _spriteRenderer;
        public SpriteRenderer SpriteRenderer => _spriteRenderer;
        
        private ElementBase _currentElement;
        
        private Vector2Int _position;
        public Vector2Int Position => _position; 

        public void Initialize(Vector2Int position)
        {
            _position = position; 
        }

        public void SetElement(ElementBase element)
        { 
            if (element == null) _currentElement = null;
            else _currentElement = element;
        }

        public ElementBase GetElement()
        {
            return _currentElement;
        }  
    }
}