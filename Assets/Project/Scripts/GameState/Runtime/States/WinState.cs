using AgaveCase.Data.Runtime;
using AgaveCase.GameUI.Runtime;
using UnityEngine; 

namespace AgaveCase.GameState.Runtime
{ 
    public class WinState : BaseGameState
    {
        private ScoreData _scoreData; 
        private GameUIController _UIController;
        public WinState(GameStateMachine stateMachine) 
            : base(stateMachine)
        { 
            _scoreData = stateMachine.ScoreData;
            _UIController = stateMachine.UIController;
        }

        public override void Enter()
        { 
            _UIController.SetStatusWinUI(true); 
        } 

        public override void Exit()
        {
            _UIController.SetStatusWinUI(false);
        }
    }
}