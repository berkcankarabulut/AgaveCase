using UnityEngine; 
using AgaveCase.Board.Runtime;
using AgaveCase.StateMachine.Runtime; 
using AgaveCase.Data.Runtime;
using AgaveCase.GameUI.Runtime; 

namespace AgaveCase.GameState.Runtime
{ 
    public class GameStateMachine : BaseStateMachine
    {   
        [Header("Data Container")]
        [SerializeField] private DataContainer _dataContainer;
        
        [Header("Component References")]
        [SerializeField] private BoardManager _boardManager;
        [SerializeField] private GameUIController _gameUIController;
        
        [Header("Visual Effects")]
        [SerializeField] private LineRenderer _selectionLineRenderer;
        
        private ScoreData _scoreData;
        private MoveData _moveData;
        
        private PlayingState _playingState;
        private WinState _winState;
        private LoseState _loseState; 
        
        public ScoreData ScoreData => _scoreData;
        public MoveData MoveData => _moveData;
        public BoardManager BoardManager => _boardManager;
        public DataContainer DataContainer => _dataContainer;

        public GameUIController UIController => _gameUIController;

        private void Awake()
        {   
            Initialize();
        } 

        protected override void Initialize()
        { 
            if (_boardManager != null) _boardManager.Init();
            _scoreData = new ScoreData(_dataContainer.gameData.TargetScore);
            _moveData = new MoveData(_dataContainer.gameData.MovesLimit);
            
            if(UIController != null) UIController.Init(_scoreData, _moveData);
            
            CreateStates(); 
            StartGame();
        }

        private void CreateStates()
        {
            _playingState = new PlayingState(this, _boardManager, _selectionLineRenderer);
            _winState = new WinState(this);
            _loseState = new LoseState(this);  
             
            AddState(_playingState);
            AddState(_winState);
            AddState(_loseState);
        }

        private void StartGame()
        {  
            ChangeState<PlayingState>();
        } 
        
        private void WinGame()
        {
            ChangeState<WinState>();
        }
         
        private void LoseGame()
        {
            ChangeState<LoseState>(); 
        }

        public void GameEnded()
        {
             if(_scoreData.IsGoalReached) WinGame();
             else LoseGame();
        }
    }
}