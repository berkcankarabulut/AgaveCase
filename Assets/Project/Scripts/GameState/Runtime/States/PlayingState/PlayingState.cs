using System.Collections.Generic; 
using AgaveCase.Elements.Runtime;
using Grid.Runtime;
using UnityEngine;

namespace AgaveCase.GameState.Runtime
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
        private bool _isSelecting;

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
            _uiService.SetStatusGamePanel(true);
            
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
            _uiService.SetStatusGamePanel(false);
            
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

            _isSelecting = true;
            _currentElementType = cell.GetElement().ElementData;
            AddCellToSelection(cell);
        }

        private void ContinueSelection(Vector3 mousePosition)
        {
            GridCell cell = _inputHandler.GetCellUnderCursor();
            if (CanSelectCell(cell))
            {
                AddCellToSelection(cell);
            }
        }

        private void EndSelection()
        {
            _isSelecting = false;
            
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

        private bool CanSelectCell(GridCell cell)
        {
            if (!IsValidCellForSelection(cell) || 
                cell.GetElement().ElementData != _currentElementType ||
                _selectedCells.Contains(cell))
            {
                return false;
            }

            if (_lastSelectedCell == null) return true;

            return AreAdjacent(cell, _lastSelectedCell);
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
            _isSelecting = false;
            
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
 
            _boardService.AddAnimationCompletedCallback(() =>
            {
                _inputHandler.EnableInput(true);
                _moveService.DecrementMove();
            });
 
            _boardService.ProcessMatchedElementsWithCallback(positions, () => { });

            ResetSelection();
        } 
    }
}