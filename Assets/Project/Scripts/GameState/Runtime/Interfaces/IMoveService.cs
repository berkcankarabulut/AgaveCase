using System;

namespace Project.GameState.Runtime
{ 
    public interface IMoveService
    {
        int MovesRemaining { get; }
        
        event Action<int> OnMovesChanged;
        event Action OnOutOfMoves;
        
        void DecrementMove();
    }
}