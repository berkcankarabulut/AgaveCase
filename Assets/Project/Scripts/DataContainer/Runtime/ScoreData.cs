using System; 

namespace AgaveCase.Data.Runtime
{ 
    public class ScoreData
    { 
        private int _currentScore;
        private int _targetScore = 1000;
        
        public int CurrentScore => _currentScore;
        public int TargetScore => _targetScore;
        public bool IsGoalReached => _currentScore >= TargetScore;
         
        public event Action<int> OnScoreChanged; 

        public ScoreData(int targetScore)
        {
            _targetScore = targetScore;
        } 
  
        public void AddScore(int points)
        { 
            _currentScore += points;
            OnScoreChanged?.Invoke(_currentScore);   
        } 
    }
}