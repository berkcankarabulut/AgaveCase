using System.Collections.Generic;
using Project.Elements.Runtime;
using Project.GridSystem.Runtime;
using UnityEngine;

namespace Project.GameState.Runtime
{ 
    public class PlayingState : BaseGameState
    { 
        private readonly IBoardService _boardService;
        private readonly IScoreService _scoreService;
        private readonly IMoveService _moveService;
        private readonly IUIService _uiService;
         
        private readonly IInputHandler _inputHandler;
        private readonly ILineRendererHandler _lineRendererHandler; 
         
        private readonly int _minimumMatchLength;
        private readonly LayerMask _selectionLayerMask = LayerMask.GetMask("Selection");
        private const int MIN_SELECTION_FOR_LINE = 2;
         
        private readonly List<GridCell> _selectedCells = new List<GridCell>();
        private ElementDataSO _currentElementType;
        private GridCell _lastSelectedCell; 

        public PlayingState(GameStateMachine stateMachine, LineRenderer lineRenderer)
            : base(stateMachine)
        { 
            Camera mainCamera = Camera.main; 
             
            _boardService = stateMachine.BoardService;
            _scoreService = stateMachine.ScoreService;
            _moveService = stateMachine.MoveService;
            _uiService = stateMachine.UIService;
            
            _minimumMatchLength = stateMachine.DataContainer.gameData.MinMatchCount;
             
            _inputHandler = new InputHandler(mainCamera, _selectionLayerMask);
            _lineRendererHandler = new LineRendererHandler(lineRenderer);
             
            _inputHandler.OnSelectionStarted += StartSelection;
            _inputHandler.OnSelectionContinued += ContinueSelection;
            _inputHandler.OnSelectionEnded += EndSelection;
        }

        public override void Enter()
        {
            ResetSelection();
            _inputHandler.EnableInput(true);
            _uiService.OpenPlayingPanel();
            
            RegisterEvents();
        }

        public override void Update()
        {
            _inputHandler.ProcessInput();
        }

        public override void Exit()
        {
            ResetSelection();
            UnregisterEvents(); 
            
            _lineRendererHandler.HideSelectionLine();
        }
 
        private void RegisterEvents()
        {
            _moveService.OnOutOfMoves += OnOutOfMoves;
            _boardService.OnBoardShuffleStarted += OnBoardShuffleStarted;
            _boardService.OnBoardShuffleCompleted += OnBoardShuffleCompleted;
        }

        private void UnregisterEvents()
        {
            _moveService.OnOutOfMoves -= OnOutOfMoves;
            _boardService.OnBoardShuffleStarted -= OnBoardShuffleStarted;
            _boardService.OnBoardShuffleCompleted -= OnBoardShuffleCompleted;
        }

        private void OnOutOfMoves()
        { 
        }

        private void OnBoardShuffleStarted()
        {
            _inputHandler.EnableInput(false);
        }

        private void OnBoardShuffleCompleted()
        {
            _inputHandler.EnableInput(true);
        }
  
        private void StartSelection()
        {
            GridCell cell = _inputHandler.GetCellUnderCursor();
            if (!IsValidCellForSelection(cell)) return;
 
            _currentElementType = cell.GetElement().ElementData;
            AddCellToSelection(cell);
        }

        private void ContinueSelection(Vector3 mousePosition)
        {
            GridCell cell = _inputHandler.GetCellUnderCursor();
     
            if (_selectedCells.Count >= 2 && cell == _selectedCells[^2])
            {
                RemoveLastSelectedCell();
                return;
            }

            if (CanSelectCell(cell))
            {
                AddCellToSelection(cell);
            }
        }

        private void EndSelection()
        { 
            if (_selectedCells.Count >= _minimumMatchLength)
            {
                ProcessMatch();
            }
            else
            {
                ResetSelection();
            }
        }

        private bool IsValidCellForSelection(GridCell cell)
        {
            return cell != null && 
                   cell.GetElement() != null && 
                   cell.GetElement() is DefaultElement;
        }
        
        private void RemoveLastSelectedCell()
        {
            if (_selectedCells.Count == 0) return;
     
            GridCell lastCell = _selectedCells[^1];
            if (lastCell.GetElement() is DefaultElement defaultElement)
                defaultElement.OnDeselected();
            
            _selectedCells.RemoveAt(_selectedCells.Count - 1);
            _lastSelectedCell = _selectedCells.Count > 0 ? _selectedCells[^1] : null;
            
            if (_lastSelectedCell.GetElement() is DefaultElement element) 
                element.OnSelected(); 
            UpdateSelectionVisuals();
        }
        
        private bool CanSelectCell(GridCell cell)
        {
            if (!IsValidCellForSelection(cell) || 
                cell.GetElement().ElementData != _currentElementType ||
                _selectedCells.Contains(cell))
            {
                return false;
            } 
            return _lastSelectedCell == null || AreAdjacent(cell, _lastSelectedCell);
        }

        private bool AreAdjacent(GridCell cell1, GridCell cell2)
        {
            Vector2Int diff = cell1.Position - cell2.Position;
            return (Mathf.Abs(diff.x) + Mathf.Abs(diff.y)) == 1;
        }

        private void AddCellToSelection(GridCell cell)
        {
            DeselectAllCells();

            _selectedCells.Add(cell);
            _lastSelectedCell = cell;

            if (cell.GetElement() is DefaultElement defaultElement)
            {
                defaultElement.OnSelected();
            }

            UpdateSelectionVisuals();
        }

        private void DeselectAllCells()
        {
            foreach (GridCell selectedCell in _selectedCells)
            {
                if (selectedCell.GetElement() is DefaultElement defaultElement)
                {
                    defaultElement.OnDeselected();
                }
            }
        }

        private void ResetSelection()
        {
            DeselectAllCells();
            _selectedCells.Clear();
            _currentElementType = null;
            _lastSelectedCell = null; 
            
            _lineRendererHandler.HideSelectionLine();
        }

        private void UpdateSelectionVisuals()
        {
            if (_selectedCells.Count >= MIN_SELECTION_FOR_LINE)
            {
                _lineRendererHandler.UpdateSelectionLine(_selectedCells);
            }
            else
            {
                _lineRendererHandler.HideSelectionLine();
            }
        }
 
        private void ProcessMatch()
        {
            _inputHandler.EnableInput(false);
             
            if (_currentElementType != null)
            {
                int score = _selectedCells.Count * _currentElementType.PointValue;
                _scoreService.AddScore(score);
            } 
            List<Vector2Int> positions = new List<Vector2Int>();
            foreach (GridCell cell in _selectedCells)
            {
                positions.Add(cell.Position);
            }
 
            _moveService.DecrementMove();
            _boardService.AddAnimationCompletedCallback(() =>
            {
                if(_moveService.MovesRemaining == 0) GameStateMachine.DetermineGameResult();
                else _inputHandler.EnableInput(true);
            });
 
            _boardService.ProcessMatchedElementsWithCallback(positions, () => { });

            ResetSelection();
        } 
    }
}