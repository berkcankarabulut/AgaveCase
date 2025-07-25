namespace Project.GameState.Runtime
{ 
    public class WinState : BaseGameState
    {
        private readonly IScoreService _scoreService;
        private readonly IUIService _uiService;
    
        public WinState(GameStateMachine stateMachine) 
            : base(stateMachine)
        { 
            _scoreService = stateMachine.ScoreService;
            _uiService = stateMachine.UIService;
        }

        public override void Enter()
        {
            _uiService.OpenWinPanel(); 
        } 

        public override void Exit()
        { 
        }
    }
}