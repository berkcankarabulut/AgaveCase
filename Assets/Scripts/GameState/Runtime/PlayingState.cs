using System.Collections.Generic;
using AgaveCase.Board.Runtime;
using AgaveCase.Elements.Runtime;
using Grid.Runtime;
using UnityEngine;

namespace AgaveCase.GameState.Runtime
{ 
    public class PlayingState : BaseGameState
    {
        private Camera _mainCamera;
        
        // Element selection tracking
        private List<GridCell> _selectedCells = new List<GridCell>();
        private ElementDataSO _currentElementType = null;
        private GridCell _lastSelectedCell = null;
        
        // Selection settings
        private readonly float _minDragDistance = 0.1f;
        private Vector3 _lastMousePosition;
        private bool _isSelecting = false;
        
        // Board manager referansı
        private BoardManager _boardManager;
        
        // Kullanıcı girişini etkinleştir/devre dışı bırak
        private bool _inputEnabled = true;

        public PlayingState(GameStateMachine stateMachine, BoardManager boardManager) 
            : base(stateMachine)
        {
            _mainCamera = Camera.main;
            _boardManager = boardManager;
        }

        public override void Enter()
        {
            Debug.Log("Playing State'e girildi");
            ResetSelection();
            SetInputEnabled(true);
        }

        public override void Update()
        { 
            HandleInput();
        }

        private void HandleInput()
        {
            // Input devre dışı bırakıldıysa işleme
            if (!_inputEnabled) return;
            
            // Mouse Down - Start Selection
            if (Input.GetMouseButtonDown(0))
            {
                StartSelection();
            }
            // Mouse Hold - Continue Selection
            else if (Input.GetMouseButton(0) && _isSelecting)
            {
                ContinueSelection();
            }
            // Mouse Up - End Selection
            else if (Input.GetMouseButtonUp(0) && _isSelecting)
            {
                EndSelection();
            }
        }
        
        private void StartSelection()
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            
            if (hit.collider != null)
            {
                GridCell cell = hit.collider.GetComponent<GridCell>();
                if (cell != null && cell.GetCandy() != null)
                {
                    _isSelecting = true;
                    _lastMousePosition = Input.mousePosition;
                    
                    // Store the element type we're looking for
                    _currentElementType = cell.GetCandy().ElementData;
                    
                    // Add first cell to selection
                    AddCellToSelection(cell);
                    
                    Debug.Log($"Started selection with element: {_currentElementType.ElementName}");
                }
            }
        }
        
        private void ContinueSelection()
        {
            // Check if mouse moved enough for a new selection
            if (Vector3.Distance(Input.mousePosition, _lastMousePosition) < _minDragDistance)
                return;
                
            _lastMousePosition = Input.mousePosition;
            
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            
            if (hit.collider != null)
            {
                GridCell cell = hit.collider.GetComponent<GridCell>();
                if (CanSelectCell(cell))
                {
                    AddCellToSelection(cell);
                    Debug.Log($"Added cell to selection: {cell.Position}");
                }
            }
        }
        
        private void EndSelection()
        {
            _isSelecting = false;
            
            // Process the selection if we have at least 3 matching elements
            if (_selectedCells.Count >= 3)
            {
                ProcessSelection();
            }
            else
            {
                // Not enough elements selected, clear selection
                Debug.Log("Not enough elements selected, clearing selection");
                ResetSelection();
            }
        }
        
        private bool CanSelectCell(GridCell cell)
        {
            // Basic validation
            if (cell == null || cell.GetCandy() == null || cell.GetCandy().ElementData != _currentElementType)
                return false;
                
            // Check if cell is already selected
            if (_selectedCells.Contains(cell))
                return false;
                
            // If this is the first selection, we can select it
            if (_lastSelectedCell == null)
                return true;

            // Check if it's adjacent to the last selected cell
            return IsAdjacent(cell, _lastSelectedCell);
        }
        
        private bool IsAdjacent(GridCell cell1, GridCell cell2)
        {
            Vector2Int diff = cell1.Position - cell2.Position;
            // Adjacent if they differ by 1 in only one coordinate
            return (Mathf.Abs(diff.x) + Mathf.Abs(diff.y)) == 1;
        }
        
        private void AddCellToSelection(GridCell cell)
        {
            _selectedCells.Add(cell);
            _lastSelectedCell = cell;
            
            // Visual feedback - DoTween ile seçim efekti
            ElementBase element = cell.GetCandy();
            if (element != null)
            {
                element.OnSelected();
            }
        }
        
        private void ProcessSelection()
        {
            Debug.Log($"Processing selection of {_selectedCells.Count} elements");
            
            // Seçim işlenirken kullanıcı girişini devre dışı bırak
            SetInputEnabled(false);
            
            // Calculate score based on the number of elements
            int score = _selectedCells.Count * _currentElementType.BaseValue;
            Debug.Log($"Score: {score}");
            
            // Seçilen hücrelerin pozisyonlarını topla
            List<Vector2Int> positions = new List<Vector2Int>();
            
            // Reset visual feedback for all selected cells and collect positions
            foreach (GridCell cell in _selectedCells)
            {
                // Pozisyonu listeye ekle
                positions.Add(cell.Position);
            }
            
            // BoardManager varsa, eşleşen elementleri işle
            if (_boardManager != null)
            {
                // Tüm animasyonlar tamamlandığında kullanıcı girişini tekrar etkinleştir
                _boardManager.AddAnimationCompletedCallback(() => {
                    SetInputEnabled(true);
                });
                
                // Element kaldırma, yer çekimi ve boşluk doldurma işlemlerini yap
                _boardManager.ProcessMatchedElements(positions);
            }
            else
            {
                // BoardManager yoksa, manuel olarak elementleri kaldır
                foreach (GridCell cell in _selectedCells)
                {
                    ElementBase element = cell.GetCandy();
                    if (element != null)
                    {
                        // Return to pool and set cell's element to null
                        element.ReturnToPool();
                        cell.SetCandy(null);
                    }
                }
                
                // Kullanıcı girişini hemen etkinleştir
                SetInputEnabled(true);
            }
            
            // Here you would update the game state, add score, etc.
            
            // Reset the selection
            ResetSelection();
        }
        
        private void SetInputEnabled(bool enabled)
        {
            _inputEnabled = enabled;
        }
        
        private void ResetSelection()
        {
            // Reset visual feedback for all selected cells
            foreach (GridCell cell in _selectedCells)
            {
                ElementBase element = cell.GetCandy();
                if (element != null)
                {
                    element.OnDeselected();
                }
            }
            
            // Clear the selection
            _selectedCells.Clear();
            _currentElementType = null;
            _lastSelectedCell = null;
            _isSelecting = false;
        }

        public override void Exit()
        {
            Debug.Log("Playing State'den çıkılıyor");
            ResetSelection();
        }
    }
}