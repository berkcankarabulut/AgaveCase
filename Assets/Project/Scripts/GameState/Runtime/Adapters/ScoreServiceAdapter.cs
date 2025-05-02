using System;
using AgaveCase.Data.Runtime;

namespace AgaveCase.GameState.Runtime
{ 
    public class ScoreServiceAdapter : IScoreService
    {
        private readonly ScoreData _scoreData;

        public int CurrentScore => _scoreData.CurrentScore;
        public int TargetScore => _scoreData.TargetScore;
        public bool IsGoalReached => _scoreData.IsGoalReached;

        public event Action<int> OnScoreChanged;

        public ScoreServiceAdapter(ScoreData scoreData)
        {
            _scoreData = scoreData;
            _scoreData.OnScoreChanged += score => OnScoreChanged?.Invoke(score);
        }

        public void AddScore(int points)
        {
            _scoreData.AddScore(points);
        }
    }
}