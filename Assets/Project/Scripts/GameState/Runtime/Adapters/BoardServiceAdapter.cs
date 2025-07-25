using System;
using System.Collections.Generic;
using Project.Board.Runtime;
using Project.GridSystem.Runtime;
using UnityEngine; 

namespace Project.GameState.Runtime
{ 
    public class BoardServiceAdapter : IBoardService
    {
        private readonly BoardManager _boardManager;

        public int GridWidth => _boardManager.GridWidth;
        public int GridHeight => _boardManager.GridHeight;

        public event Action OnBoardShuffleStarted;
        public event Action OnBoardShuffleCompleted;

        public BoardServiceAdapter(BoardManager boardManager)
        {
            _boardManager = boardManager;
             
            _boardManager.OnBoardShuffleStarted += () => OnBoardShuffleStarted?.Invoke();
            _boardManager.OnBoardShuffleCompleted += () => OnBoardShuffleCompleted?.Invoke();
        }

        public GridCell GetGridAt(int x, int y)
        {
            return _boardManager.GetGridAt(x, y);
        }

        public GridCell GetGridAt(Vector2Int position)
        {
            return _boardManager.GetGridAt(position);
        }

        public void ProcessMatchedElementsWithCallback(List<Vector2Int> positions, Action onCompleted)
        {
            _boardManager.ProcessMatchedElementsWithCallback(positions, onCompleted);
        }

        public void AddAnimationCompletedCallback(Action callback)
        {
            _boardManager.AddAnimationCompletedCallback(callback);
        }
    }
}