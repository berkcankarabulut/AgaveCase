using AgaveCase.Data.Runtime;
using AgaveCase.GameUI.Runtime;
using UnityEngine; 

namespace AgaveCase.GameState.Runtime
{ 
    public class LoseState : BaseGameState
    {
        private ScoreData _scoreData;
        private GameUIController _UIController;
        public LoseState(GameStateMachine stateMachine) 
            : base(stateMachine)
        { 
            _scoreData = stateMachine.ScoreData;
            _UIController = stateMachine.UIController;
        }

        public override void Enter()
        {
            _UIController.SetStatusLoseUI(true);  
        } 
        
        public override void Exit()
        {
            _UIController.SetStatusLoseUI(false); 
        }
    }
}