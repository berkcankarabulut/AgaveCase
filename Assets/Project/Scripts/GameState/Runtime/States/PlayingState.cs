using System.Collections.Generic;
using AgaveCase.Board.Runtime;
using AgaveCase.Data.Runtime;
using AgaveCase.Elements.Runtime;
using AgaveCase.GameUI.Runtime;
using Grid.Runtime;
using UnityEngine;

namespace AgaveCase.GameState.Runtime
{
    public class PlayingState : BaseGameState
    {
        private readonly Camera _mainCamera;
        private readonly BoardManager _boardManager;
        private readonly ScoreData _scoreData;
        private readonly MoveData _moveData;
        private readonly LineRenderer _selectionLine;
        private readonly int _minimumMatchLength = 3;
        private readonly float _minDragDistance = 0.1f;
        private readonly LayerMask _selectionLayerMask= LayerMask.GetMask("Selection");
        private readonly int _minSelectionForLine = 2; 
       
        private List<GridCell> _selectedCells = new List<GridCell>();
        private ElementDataSO _currentElementType = null;
        private GridCell _lastSelectedCell = null;
        private Vector3 _lastMousePosition;
        private bool _isSelecting = false;
        private bool _inputEnabled = true;
        GameUIController _UIController;
        public PlayingState(GameStateMachine stateMachine, BoardManager boardManager, LineRenderer lineRenderer)
            : base(stateMachine)
        {
            _mainCamera = Camera.main;
            _boardManager = boardManager;
            _scoreData = stateMachine.ScoreData;
            _moveData = stateMachine.MoveData;
            _UIController = stateMachine.UIController;
            _selectionLine = lineRenderer;
            _minimumMatchLength = stateMachine.DataContainer.gameData.MinMatchCount;
            ConfigureLineRenderer();
        }

        private void ConfigureLineRenderer()
        {
            _selectionLine.positionCount = 0;
            _selectionLine.enabled = false;
        }

        public override void Enter()
        {
            ResetSelection();
            SetInputEnabled(true);
            _UIController.SetStatusGamePanel(true); 
            
            _moveData.OnOutOfMoves += OnOutOfMoves; 
            _boardManager.OnBoardShuffleStarted += OnBoardShuffleStarted;
            _boardManager.OnBoardShuffleCompleted += OnBoardShuffleCompleted; 
        }

        public override void Update()
        {
            HandleInput();
        }

        private void HandleInput()
        {
            if (!_inputEnabled) return;

            if (Input.GetMouseButtonDown(0))
            {
                StartSelection();
            }
            else if (Input.GetMouseButton(0) && _isSelecting)
            {
                ContinueSelection();
            }
            else if (Input.GetMouseButtonUp(0) && _isSelecting)
            {
                EndSelection();
            }
        }

        private void StartSelection()
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, _selectionLayerMask);

            if (hit.collider == null) return;
            GridCell cell = hit.collider.GetComponent<GridCell>();
     
            if (cell == null || cell.GetElement() == null || !(cell.GetElement() is DefaultElement)) return;
    
            _isSelecting = true;
            _lastMousePosition = Input.mousePosition;
            _currentElementType = cell.GetElement().ElementData;
            AddCellToSelection(cell);
        }

        private void ContinueSelection()
        {
            if (Vector3.Distance(Input.mousePosition, _lastMousePosition) < _minDragDistance)
                return;

            _lastMousePosition = Input.mousePosition;

            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, _selectionLayerMask);

            if (hit.collider == null) return;
            GridCell cell = hit.collider.GetComponent<GridCell>();
            if (CanSelectCell(cell))
            {
                AddCellToSelection(cell);
            }
        }

        private void EndSelection()
        {
            _isSelecting = false;
            if (_selectedCells.Count >= _minimumMatchLength) ProcessSelection();
            else ResetSelection();
        }

        private bool CanSelectCell(GridCell cell)
        {
            if (cell == null || cell.GetElement() == null || cell.GetElement().ElementData != _currentElementType)
                return false;
     
            if (!(cell.GetElement() is DefaultElement)) return false;
    
            if (_selectedCells.Contains(cell)) return false;
            if (_lastSelectedCell == null) return true;
    
            return IsAdjacent(cell, _lastSelectedCell);
        }

        private bool IsAdjacent(GridCell cell1, GridCell cell2)
        {
            Vector2Int diff = cell1.Position - cell2.Position;
            return (Mathf.Abs(diff.x) + Mathf.Abs(diff.y)) == 1;
        }

        private void AddCellToSelection(GridCell cell)
        {
            foreach (GridCell selectedCell in _selectedCells)
            {
                ElementBase element = selectedCell.GetElement();
                if (element is DefaultElement defaultElement)
                {
                    defaultElement.OnDeselected();
                }
            }

            _selectedCells.Add(cell);
            _lastSelectedCell = cell;

            ElementBase lastElement = cell.GetElement();
            if (lastElement is DefaultElement defaultLastElement)
            {
                defaultLastElement.OnSelected();
            }

            if (_selectedCells.Count >= _minSelectionForLine) UpdateSelectionLine();
            else if (_selectionLine != null) _selectionLine.enabled = false;
        }

        private void ProcessSelection()
        {
            SetInputEnabled(false);
            int score = 0;
            if (_currentElementType != null)
            {
                score = _selectedCells.Count * _currentElementType.PointValue;
                if (_scoreData != null) _scoreData.AddScore(score); 
            }

            List<Vector2Int> positions = new List<Vector2Int>();
            foreach (GridCell cell in _selectedCells)
            {
                positions.Add(cell.Position);
            }


            _boardManager.AddAnimationCompletedCallback(() =>
            {
                SetInputEnabled(true);

                if (_scoreData != null)
                {
                    _moveData.DecrementMove();
                }
            });

            _boardManager.ProcessMatchedElementsWithCallback(positions, () =>
            { 
            });

            ResetSelection();
        }

        private void OnBoardShuffleStarted()
        {
            SetInputEnabled(false);
        }

        private void OnBoardShuffleCompleted()
        {
            SetInputEnabled(true);
        }

        private void SetInputEnabled(bool enabled)
        {
            _inputEnabled = enabled;
        }

        private void ResetSelection()
        {
            foreach (GridCell cell in _selectedCells)
            {
                ElementBase element = cell.GetElement();
                if (element is DefaultElement defaultElement)
                {
                    defaultElement.OnDeselected();
                }
            }

            _selectedCells.Clear();
            _currentElementType = null;
            _lastSelectedCell = null;
            _isSelecting = false;

            if (_selectionLine == null) return;
            _selectionLine.enabled = false;
            _selectionLine.positionCount = 0;
        }

        private void UpdateSelectionLine()
        {
            if (_selectionLine == null) return;
            _selectionLine.positionCount = _selectedCells.Count;

            for (int i = 0; i < _selectedCells.Count; i++)
            {
                GridCell cell = _selectedCells[i];
                Vector3 cellPos = cell.transform.position; 
                Vector3 adjustedPos = new Vector3(cellPos.x, cellPos.y, cellPos.z - 0.1f);
                _selectionLine.SetPosition(i, adjustedPos);
            }

            _selectionLine.enabled = true;
        } 
        private void OnOutOfMoves()
        {
            GameStateMachine.GameEnded(); 
        }

        public override void Exit()
        {
            ResetSelection(); 
            DisableEvents();
            _UIController.SetStatusGamePanel(false); 

            if (_selectionLine == null) return;
            _selectionLine.enabled = false;
            _selectionLine.positionCount = 0;
        }

        private void DisableEvents()
        { 
            _moveData.OnOutOfMoves -= OnOutOfMoves;
            _boardManager.OnBoardShuffleStarted -= OnBoardShuffleStarted;
            _boardManager.OnBoardShuffleCompleted -= OnBoardShuffleCompleted;
        }
    }
}