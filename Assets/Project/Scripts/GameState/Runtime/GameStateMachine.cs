using UnityEngine;
using Project.Board.Runtime;
using Project.StateMachine.Runtime;
using Project.Data.Runtime;
using Project.GameUI.Runtime; 

namespace Project.GameState.Runtime
{
    public class GameStateMachine : BaseStateMachine
    {
        [Header("Data Container")] [SerializeField]
        private DataContainer _dataContainer;

        [Header("Component References")] [SerializeField]
        private BoardManager _boardManager;

        [SerializeField] private GameUIStateMachine gameUIStateMachine;

        [Header("Visual Effects")] [SerializeField]
        private LineRenderer _selectionLineRenderer;

        private IBoardService _boardService;
        private IScoreService _scoreService;
        private IMoveService _moveService;
        private IUIService _uiService;

        private PlayingState _playingState;
        private WinState _winState;
        private LoseState _loseState;

        public IBoardService BoardService => _boardService;
        public IScoreService ScoreService => _scoreService;
        public IMoveService MoveService => _moveService;
        public IUIService UIService => _uiService;
        public DataContainer DataContainer => _dataContainer;

        public ScoreData ScoreData { get; private set; }
        public MoveData MoveData { get; private set; }

        private void Awake()
        {
            Initialize();
        }

        protected override void Initialize()
        {
            InitializeServices();
            CreateStates();
            StartGame();
        }

        private void InitializeServices()
        {
            if (_boardManager != null) _boardManager.Init();
            ScoreData = new ScoreData(_dataContainer.gameData.TargetScore);
            MoveData = new MoveData(_dataContainer.gameData.MovesLimit);

            if (gameUIStateMachine != null) gameUIStateMachine.Init(ScoreData, MoveData);

            _boardService = new BoardServiceAdapter(_boardManager);
            _scoreService = new ScoreServiceAdapter(ScoreData);
            _moveService = new MoveServiceAdapter(MoveData);
            _uiService = new UIServiceAdapter(gameUIStateMachine);
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

        public void DetermineGameResult()
        {
            if (ScoreService.IsGoalReached) WinGame();
            else LoseGame();
        }
    }
}