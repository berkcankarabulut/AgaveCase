using System;
using System.Collections.Generic;
using Grid.Runtime;
using AgaveCase.Elements.Runtime;
using AgaveCase.Data.Runtime;
using AgaveCase.Pooler.Runtime;
using UnityEngine;

namespace AgaveCase.Board.Runtime
{ 
    public class BoardManager : MonoBehaviour
    { 
        [Header("Data Container")]
        [SerializeField] private DataContainer _dataContainer;
        [Header("Prefabs")] 
        [SerializeField] private GridCell _gridPrefab;
        [SerializeField] private ElementBase _elementPrefab;
        [Header("Grid Setup")]
        [SerializeField] private Transform _gridContainer;
        [Header("Element Setup")] 
        [SerializeField] private ElementDataSO[] _availableElements;
        [SerializeField] private ObjectPooler _objectPooler;
        
        [Header("Animation Settings")]
        [SerializeField] private float _fallingDuration = 0.3f;
        [SerializeField] private float _newElementFallingDuration = 0.5f;
        [SerializeField] private DG.Tweening.Ease _fallingEase = DG.Tweening.Ease.OutBounce;
        [SerializeField] private float _delayBetweenFalls = 0.05f;
        [SerializeField] private float _matchAnimationDelay = 0.5f;
        [SerializeField] private float _shuffleAnimationDuration = 0.5f;
        
        [Header("Shuffle Settings")]
        [SerializeField] private int _maxShuffleAttempts = 5;
         
        private GridCell[,] _gridCells;
        private Camera _mainCamera;
         
        private GridHandler _gridHandler;
        private ElementHandler _elementHandler;
        private BoardAnimationHandler _boardAnimationHandler;
        private MatchDetectionHandler _matchDetectionHandler;
        private ShuffleHandler _shuffleHandler;
         
        private bool _isShuffling = false;
        private int _currentShuffleAttempt = 0;
         
        public int GridWidth => _dataContainer != null ? _dataContainer.gridData.Width : 8;
        public int GridHeight => _dataContainer != null ? _dataContainer.gridData.Height : 8;
        public float FallDuration => _fallingDuration; 
        public DG.Tweening.Ease FallingEase => _fallingEase;
        public float DelayBetweenFalls => _delayBetweenFalls; 
         
        public event Action OnBoardShuffleStarted;
        public event Action OnBoardShuffleCompleted;
         
        public void Init()
        {  
            _mainCamera = Camera.main; 
        
            _gridHandler = new GridHandler(
                _gridPrefab, 
                _gridContainer, 
                _dataContainer.gridData.CellSize,
                _dataContainer.gridData.ScreenFillPercentage,
                _mainCamera
            );
         
            _elementHandler = new ElementHandler(
                _availableElements,
                _elementPrefab,
                _objectPooler
            );
        
            _boardAnimationHandler = new BoardAnimationHandler(this);
            _matchDetectionHandler = new MatchDetectionHandler(this);
            _shuffleHandler = new ShuffleHandler(this, _matchDetectionHandler);
            
            _gridCells = _gridHandler.CreateGrid(GridWidth, GridHeight);
         
            _elementHandler.SetupElements(_gridCells);
         
            Invoke(nameof(CheckForPotentialMatches), 0.1f);
        }
         
        public GridCell GetGridAt(int x, int y)
        {
            return _gridHandler.GetCellAt(x, y, _gridCells);
        }
 
        public GridCell GetGridAt(Vector2Int position)
        {
            return GetGridAt(position.x, position.y);
        } 
         
        public void ProcessMatchedElementsWithCallback(List<Vector2Int> positions, Action onCompleted)
        {
            if (positions == null || positions.Count == 0)
            {
                onCompleted?.Invoke();
                return;
            }
            
            _boardAnimationHandler.ProcessMatchedElementsWithCallback(positions, _matchAnimationDelay, () => {
                CheckForPotentialMatches();
                onCompleted?.Invoke();
            });
        }
         
        public void SpawnElementAtCellWithAnimation(GridCell cell, int spawnOrder, Action onSpawned = null, Action onCompleted = null)
        {
            _elementHandler.SpawnElementAtCellWithAnimation(
                cell, 
                spawnOrder, 
                _newElementFallingDuration, 
                _fallingEase,
                _delayBetweenFalls,
                onSpawned, 
                onCompleted
            );
        }
         
        public void CheckForPotentialMatches()
        {
            if (_isShuffling) return;
            
            if (!_matchDetectionHandler.HasPotentialMatches()) ShuffleBoard();
            else _currentShuffleAttempt = 0;
        }
         
        private void ShuffleBoard()
        {
            if (_isShuffling) return;
    
            _isShuffling = true;
            _currentShuffleAttempt++;
    
            OnBoardShuffleStarted?.Invoke();
     
            _shuffleHandler.StartShuffleAnimation(_gridCells, _shuffleAnimationDuration, () => {
                _shuffleHandler.ShuffleBoard();
        
                bool hasPotentialMatches = _matchDetectionHandler.HasPotentialMatches();
        
                if (!hasPotentialMatches && _currentShuffleAttempt < _maxShuffleAttempts)
                { 
                    _shuffleHandler.ShuffleBoard();
                    hasPotentialMatches = _matchDetectionHandler.HasPotentialMatches();
                }
        
                OnBoardShuffleCompleted?.Invoke();
                _isShuffling = false; 
            });
        }
         
        public void AddAnimationCompletedCallback(Action callback)
        {
            _boardAnimationHandler.AddAnimationCompletedCallback(callback);
        }
         
        public void RemoveElementAt(Vector2Int position)
        {
            _elementHandler.RemoveElementAt(position, this);
        }
    }
}