using System;
namespace AgaveCase.Data.Runtime
{
    public class MoveData
    { 
        private int _movesRemaining; 
        public int MovesRemaining => _movesRemaining;   
        
        public event Action<int> OnMovesChanged; 
        public event Action OnOutOfMoves;   
        public MoveData(int movesLimit)
        { 
            AddMoves(movesLimit); 
        }
 
        public void DecrementMove()
        {
            _movesRemaining--;
            OnMovesChanged?.Invoke(_movesRemaining);
             
            if (_movesRemaining <= 0)
            {
                OnOutOfMoves?.Invoke();
            }
        } 
         
        public void AddMoves(int additionalMoves)
        {
            if (additionalMoves <= 0) return;
            
            _movesRemaining += additionalMoves;
            OnMovesChanged?.Invoke(_movesRemaining);
        }  
    }
}