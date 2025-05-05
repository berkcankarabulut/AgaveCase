namespace AgaveCase.GameState.Runtime
{ 
    public class LoseState : BaseGameState
    {
        private readonly IScoreService _scoreService;
        private readonly IUIService _uiService;
        public LoseState(GameStateMachine stateMachine) 
            : base(stateMachine)
        { 
            _scoreService = stateMachine.ScoreService;
            _uiService = stateMachine.UIService;
        }

        public override void Enter()
        {
            _uiService.OpenLosePanel();  
        } 
        
        public override void Exit()
        { 
        }
    }
}