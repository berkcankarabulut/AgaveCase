using System;
using System.Collections.Generic;
using UnityEngine;
using Project.GridSystem.Runtime; 

namespace Project.GameState.Runtime
{ 
    public interface IBoardService
    { 
        int GridWidth { get; }
        int GridHeight { get; }
 
        event Action OnBoardShuffleStarted;
        event Action OnBoardShuffleCompleted;
 
        GridCell GetGridAt(int x, int y);
        GridCell GetGridAt(Vector2Int position);
        void ProcessMatchedElementsWithCallback(List<Vector2Int> positions, Action onCompleted);
        void AddAnimationCompletedCallback(Action callback);
    }
}