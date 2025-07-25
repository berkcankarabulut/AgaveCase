using System;
using Project.GridSystem.Runtime;
using UnityEngine;

namespace Project.GameState.Runtime
{
    public class InputHandler : IInputHandler
    { 
        public event Action OnSelectionStarted;
        public event Action<Vector3> OnSelectionContinued;
        public event Action OnSelectionEnded;
         
        private const float MIN_DRAG_DISTANCE = 0.1f;
        private readonly LayerMask _selectionLayerMask;
         
        private readonly Camera _mainCamera;
         
        private Vector3 _lastMousePosition;
        private bool _isSelecting;
        private bool _inputEnabled = true;
        
        public InputHandler(Camera mainCamera, LayerMask selectionLayerMask)
        {
            _mainCamera = mainCamera;
            _selectionLayerMask = selectionLayerMask;
        }
         
        public void ProcessInput()
        {
            if (!_inputEnabled) return;

            if (Input.GetMouseButtonDown(0))
            {
                HandleSelectionStart();
            }
            else if (Input.GetMouseButton(0) && _isSelecting)
            {
                HandleSelectionContinue();
            }
            else if (Input.GetMouseButtonUp(0) && _isSelecting)
            {
                HandleSelectionEnd();
            }
        }
         
        public void EnableInput(bool enabled)
        {
            _inputEnabled = enabled;
        }
         
        public GridCell GetCellUnderCursor()
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, _selectionLayerMask);

            if (hit.collider == null) return null;
            return hit.collider.GetComponent<GridCell>();
        } 
        
        private void HandleSelectionStart()
        {
            GridCell cell = GetCellUnderCursor();
            if (cell == null) return;
            
            _isSelecting = true;
            _lastMousePosition = Input.mousePosition;
            
            OnSelectionStarted?.Invoke();
        }
        
        private void HandleSelectionContinue()
        { 
            if (Vector3.Distance(Input.mousePosition, _lastMousePosition) < MIN_DRAG_DISTANCE)
                return;

            _lastMousePosition = Input.mousePosition;
            OnSelectionContinued?.Invoke(Input.mousePosition);
        }
        
        private void HandleSelectionEnd()
        {
            _isSelecting = false;
            OnSelectionEnded?.Invoke();
        }
         
    }
}