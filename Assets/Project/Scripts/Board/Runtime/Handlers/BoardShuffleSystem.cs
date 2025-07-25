using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Project.Board.Runtime
{
    public class BoardShuffleSystem  
    {  
        private int _shuffleAttempts = 0;
        private const int MAX_ATTEMPTS = 5;

        private readonly BoardElementSystem _elementSystem;
        private readonly BoardAnimationSystem _animationSystem;
        private readonly BoardMatchDetectionSystem _matchDetection;

        public BoardShuffleSystem(BoardElementSystem elementSystem,
            BoardAnimationSystem animationSystem,
            BoardMatchDetectionSystem matchDetection)
        {
            _elementSystem = elementSystem;
            _animationSystem = animationSystem;
            _matchDetection = matchDetection;

        }

        public async UniTask ShuffleAsync()
        {
            Debug.Log("🔄 Starting board shuffle");
            _shuffleAttempts = 0;

            do
            {
                _shuffleAttempts++;
                
                // Play shuffle animation
                await _animationSystem.ShuffleAsync();
                
                // Shuffle elements
                await _elementSystem.ShuffleElementsAsync();
                
                // Check if shuffle created valid moves
                bool hasValidMoves = _matchDetection.HasPotentialMatches();
                
                if (hasValidMoves)
                {
                    Debug.Log("✅ Shuffle successful - valid moves found");
                    return;
                }
                
                Debug.Log($"⚠️ Shuffle attempt {_shuffleAttempts} - no valid moves");
                
            } while (_shuffleAttempts < MAX_ATTEMPTS);

            Debug.LogWarning("❌ Could not create valid moves after shuffling");
        }

        public bool CanShuffle()
        {
            return _shuffleAttempts < MAX_ATTEMPTS;
        }

        public void ResetShuffleCount()
        {
            _shuffleAttempts = 0;
        }
    }
}