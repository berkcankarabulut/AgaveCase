using System;

namespace AgaveCase.GameState.Runtime
{ 
    public interface IScoreService
    {
        int CurrentScore { get; }
        int TargetScore { get; }
        bool IsGoalReached { get; }
        
        event Action<int> OnScoreChanged;
        
        void AddScore(int points);
    }
}