using UnityEngine;
using Project.Board.Runtime;
using Project.StateMachine.Runtime;
using Project.Data.Runtime;
using Project.GameUI.Runtime;
using UnityEngine.Serialization;

namespace Project.GameState.Runtime
{
    public class GameStateMachine : BaseStateMachine
    {
        [Header("Data Container")] [SerializeField]
        private DataContainer _dataContainer;

        [Header("Component References")] [SerializeField]
        private BoardFacade _boardFacade;

        [SerializeField] private GameUIStateMachine _gameUIStateMachine;

        [Header("Visual Effects")] [SerializeField]
        private LineRenderer _selectionLineRenderer;

        private IScoreService _scoreService;
        private IMoveService _moveService;
        private IUIService _uiService;

        private PlayingState _playingState;
        private WinState _winState;
        private LoseState _loseState;

        public IScoreService ScoreService => _scoreService;
        public IMoveService MoveService => _moveService;
        public IUIService UIService => _uiService;
        public DataContainer DataContainer => _dataContainer;

        public BoardFacade BoardFacade => _boardFacade;
        public ScoreData ScoreData { get; private set; }
        public MoveData MoveData { get; private set; }

        public void InitializeStates()
        {
            Initialize();
        } 
        
        protected override void Initialize()
        {
            _playingState = new PlayingState(this, FindObjectOfType<LineRenderer>());
            _winState = new WinState(this);
            _loseState = new LoseState(this);
            
            AddState(_playingState);
            AddState(_winState);
            AddState(_loseState);
        }

        public void InitializeServices(ScoreData scoreData, MoveData moveData, BoardFacade boardFacade, GameUIStateMachine gameUIStateMachine)
        {
            _boardFacade = boardFacade;
            _gameUIStateMachine = gameUIStateMachine;
            _boardFacade?.InitializeBoardAsync();
            ScoreData = scoreData;
            MoveData = moveData;

            if (_gameUIStateMachine != null) _gameUIStateMachine.Init(ScoreData, MoveData);

            _scoreService = new ScoreServiceAdapter(ScoreData);
            _moveService = new MoveServiceAdapter(MoveData);
            _uiService = new UIServiceAdapter(_gameUIStateMachine);
        }

        private void CreateStates()
        {
            _playingState = new PlayingState(this, _selectionLineRenderer);
            _winState = new WinState(this);
            _loseState = new LoseState(this);

            AddState(_playingState);
            AddState(_winState);
            AddState(_loseState);
        }

        public void StartGame()
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

        public void DetermineGameResult()
        {
            if (ScoreService.IsGoalReached) WinGame();
            else LoseGame();
        } 
    }
}