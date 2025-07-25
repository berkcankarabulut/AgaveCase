using System;

namespace Project.GameState.Runtime
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