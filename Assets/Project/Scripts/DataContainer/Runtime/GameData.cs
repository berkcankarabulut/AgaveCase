using UnityEngine;

namespace AgaveCase.Data.Runtime
{
    [System.Serializable]
    public class GameData
    {
        [Header("Score Settings")]    
        [SerializeField] private int _targetScore = 500;  
        
        [Range(5, 50)]
        [SerializeField] private int _movesLimit = 20; 
        [Range(3, 10)]
        [SerializeField] private int _minMatchCount = 3; 
        public int MovesLimit => _movesLimit; 
        public int MinMatchCount => _minMatchCount; 
        public int TargetScore => _targetScore;

        public void SetTargetScore(int targetScore)
        {
           _targetScore = targetScore;
        }
    }
}