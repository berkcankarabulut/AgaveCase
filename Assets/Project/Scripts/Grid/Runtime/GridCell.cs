using Project.Elements.Runtime;
using UnityEngine;

namespace Project.GridSystem.Runtime
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
            _currentElement = element == null ? null : element;
        }

        public ElementBase GetElement()
        {
            return _currentElement;
        }

        public void EnterTest()
        {
            Debug.Log("Enter");
        }

        public void ExitTest()
        {
            Debug.Log("Exit");
        }
    }
}