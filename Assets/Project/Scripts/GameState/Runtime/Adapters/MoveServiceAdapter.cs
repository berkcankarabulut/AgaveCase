using System;
using AgaveCase.Data.Runtime;

namespace AgaveCase.GameState.Runtime
{ 
    public class MoveServiceAdapter : IMoveService
    {
        private readonly MoveData _moveData;

        public int MovesRemaining => _moveData.MovesRemaining;

        public event Action<int> OnMovesChanged;
        public event Action OnOutOfMoves;

        public MoveServiceAdapter(MoveData moveData)
        {
            _moveData = moveData;
            _moveData.OnMovesChanged += moves => OnMovesChanged?.Invoke(moves);
            _moveData.OnOutOfMoves += () => OnOutOfMoves?.Invoke();
        }

        public void DecrementMove()
        {
            _moveData.DecrementMove();
        }
    }
}