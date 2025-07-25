using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Project.GridSystem.Runtime;
using Project.Elements.Runtime;
using UnityEngine;
using System.Threading;
using Project.Data.Runtime; 

namespace Project.Board.Runtime
{ 
    public class BoardFacade : MonoBehaviour
    {
        [Header("Configuration")] [SerializeField]
        DataContainer _dataContainer;

        [Header("Animation Settings")] [SerializeField]
        private float _matchAnimationDelay = 0.5f;

        [SerializeField] private float _fallDuration = 0.3f;
        [SerializeField] private float _delayBetweenFalls = 0.05f;

        [Header("Shuffle Settings")] [SerializeField]
        private int _maxShuffleAttempts = 5;

        // ✅ System Components - All systems in one place
        private BoardGridSystem _gridSystem;
        private BoardElementSystem _elementSystem;
        private BoardAnimationSystem _animationSystem;
        private BoardMatchDetectionSystem _matchDetectionSystem;
        private BoardShuffleSystem _shuffleSystem;

        // ✅ Cancellation support
        private CancellationTokenSource _cancellationTokenSource;

        // ✅ Public events for external systems
        public event Action OnBoardInitialized;
        public event Action<List<Vector2Int>> OnMatchProcessed;
        public event Action OnBoardShuffled;

        // ✅ Public properties for external access
        public int GridWidth => _gridSystem?.Width ?? _dataContainer.gridData.Width;
        public int GridHeight => _gridSystem?.Height ?? _dataContainer.gridData.Height;
        public GridCell[,] GridCells => _gridSystem?.GridCells;

      
        private void OnDestroy()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }
        
        public async UniTask InitializeBoardAsync()
        {
            try
            {
                Debug.Log("🚀 Initializing board...");
                InitializeSystems();
                
                _gridSystem.Initialize(GridWidth, GridHeight);
 
                await _elementSystem.PopulateGridAsync();
 
                bool hasValidMoves = _matchDetectionSystem.HasPotentialMatches();
                if (!hasValidMoves)
                {
                    await _shuffleSystem.ShuffleAsync();
                }

                Debug.Log("✅ Board initialization complete!");
                OnBoardInitialized?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ Board initialization failed: {ex}");
                throw;
            }
        }
        
        private void InitializeSystems()
        {
            // ✅ Get or add system components
            _gridSystem = GetComponent<BoardGridSystem>();
            _elementSystem = GetComponent<BoardElementSystem>();

            _animationSystem = new BoardAnimationSystem(this, _matchAnimationDelay, _fallDuration, _delayBetweenFalls);
            _matchDetectionSystem = new BoardMatchDetectionSystem(this);
            _shuffleSystem = new BoardShuffleSystem(_elementSystem, _animationSystem, _matchDetectionSystem);

            // ✅ Initialize all systems with this facade
            _elementSystem.Initialize(this);

            // ✅ Setup cancellation token
            _cancellationTokenSource = new CancellationTokenSource();
        }

        // ✅ MAIN PUBLIC API - Simple and clean facade methods

        

        public async UniTask<bool> ProcessPlayerMoveAsync(List<Vector2Int> selectedCells)
        {
            try
            {
                var ct = _cancellationTokenSource.Token;
                ct.ThrowIfCancellationRequested();

                Debug.Log($"🎮 Processing player move: {selectedCells.Count} cells");

                // 1. Validate move
                if (!_matchDetectionSystem.IsValidMatch(selectedCells))
                {
                    Debug.Log("❌ Invalid move!");
                    return false;
                }

                // 2. Process match sequence
                await ProcessMatchSequenceAsync(selectedCells, ct);

                // 3. Check for potential moves
                bool hasMoreMoves = await _matchDetectionSystem.HasPotentialMatchesAsync();
                if (!hasMoreMoves)
                {
                    Debug.Log("🔄 No more moves - shuffling board...");
                    await _shuffleSystem.ShuffleAsync();
                    OnBoardShuffled?.Invoke();
                }

                Debug.Log("✅ Player move processed successfully!");
                OnMatchProcessed?.Invoke(selectedCells);
                return true;
            }
            catch (OperationCanceledException)
            {
                Debug.Log("⏹️ Player move processing cancelled");
                return false;
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ Error processing player move: {ex}");
                return false;
            }
        }

        public async UniTask ShuffleBoardAsync()
        {
            try
            {
                Debug.Log("🔄 Manual board shuffle requested");
                await _shuffleSystem.ShuffleAsync();
                OnBoardShuffled?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ Board shuffle failed: {ex}");
                throw;
            }
        }

        public async UniTask PauseBoardAsync()
        {
            Debug.Log("⏸️ Pausing board operations...");
            _animationSystem.PauseAllAnimations();
            _cancellationTokenSource.Cancel();

            // Create new token for future operations
            _cancellationTokenSource = new CancellationTokenSource();
            await UniTask.Yield();
        }

        public async UniTask ResumeBoardAsync()
        {
            Debug.Log("▶️ Resuming board operations...");
            _animationSystem.ResumeAllAnimations();
            await UniTask.Yield();
        }

        // ✅ UTILITY METHODS - Public access to grid data

        public GridCell GetCellAt(int x, int y)
        {
            return _gridSystem?.GetCellAt(x, y);
        }

        public GridCell GetCellAt(Vector2Int position)
        {
            return _gridSystem?.GetCellAt(position);
        }

        public bool IsValidPosition(Vector2Int position)
        {
            return _gridSystem?.IsValidPosition(position) ?? false;
        }

        public ElementBase GetElementAt(Vector2Int position)
        {
            return GetCellAt(position)?.GetElement();
        }

        public void SetElementAt(Vector2Int position, ElementBase element)
        {
            GetCellAt(position)?.SetElement(element);
        }

        // ✅ ELEMENT SPAWNING - Delegate to element system
        public async UniTask SpawnElementAtCellWithAnimationAsync(GridCell cell, int spawnOrder)
        {
            await _elementSystem.SpawnElementAtCellWithAnimationAsync(
                cell, spawnOrder, _fallDuration,
                DG.Tweening.Ease.OutBounce, _delayBetweenFalls);
        }

        // Legacy support for existing code
        public void SpawnElementAtCellWithAnimation(GridCell cell, int spawnOrder,
            System.Action onSpawned, System.Action onCompleted)
        {
            SpawnElementAtCellWithAnimationAsync(cell, spawnOrder).ContinueWith(() =>
            {
                onSpawned?.Invoke();
                onCompleted?.Invoke();
            });
        }

        // ✅ STATUS QUERIES - For external systems
        public bool IsAnimating()
        {
            return _animationSystem?.IsAnimating() ?? false;
        }

        public bool HasPotentialMatches()
        {
            return _matchDetectionSystem?.HasPotentialMatches() ?? false;
        }

        public async UniTask<bool> HasPotentialMatchesAsync()
        {
            if (_matchDetectionSystem == null) return false;
            return await _matchDetectionSystem.HasPotentialMatchesAsync();
        }

        // ✅ PRIVATE HELPER METHODS - Complex operations hidden

        private async UniTask ProcessMatchSequenceAsync(List<Vector2Int> positions, CancellationToken ct)
        {
            // 1. Play match animation and remove elements
            await _animationSystem.ProcessMatchAsync(positions);

            // 2. Check for cascading matches
            await CheckForCascadingMatchesAsync(ct);
        }

        private async UniTask CheckForCascadingMatchesAsync(CancellationToken ct)
        {
            // Simple cascading check - can be expanded later
            await UniTask.Delay(100, cancellationToken: ct);

            // TODO: Implement cascading match detection
            // var newMatches = FindCascadingMatches();
            // if (newMatches.Count > 0) await ProcessMatchSequenceAsync(newMatches, ct);
        }

        // ✅ DEBUG METHODS - For development and testing
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public void DebugPrintBoardState()
        {
            Debug.Log("=== BOARD STATE ===");
            Debug.Log($"Grid Size: {GridWidth}x{GridHeight}");
            Debug.Log($"Has Potential Matches: {HasPotentialMatches()}");
            Debug.Log($"Is Animating: {IsAnimating()}");

            for (int y = GridHeight - 1; y >= 0; y--)
            {
                string row = "";
                for (int x = 0; x < GridWidth; x++)
                {
                    var element = GetElementAt(new Vector2Int(x, y));
                    row += element != null ? element.ElementData.ElementName[0] : "?";
                    row += " ";
                }

                Debug.Log($"Row {y}: {row}");
            }

            Debug.Log("===================");
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public void DebugForceValidateBoard()
        {
            bool hasMatches = HasPotentialMatches();
            Debug.Log($"Board validation: {(hasMatches ? "VALID" : "INVALID")} moves available");

            if (!hasMatches)
            {
                Debug.LogWarning("⚠️ Board has no valid moves - consider shuffling!");
            }
        }
    }
}