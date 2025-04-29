using UnityEngine;

namespace AgaveCase.GameState.Runtime
{ 
    public class WinState : BaseGameState
    {
        public WinState(GameStateMachine stateMachine) 
            : base(stateMachine)
        {
        }

        public override void Enter()
        {
            Debug.Log("Oyun kazanıldı - Win State'e girildi"); 
        }

        public override void Update()
        { 
            if (Input.GetKeyDown(KeyCode.R))
            {
                GameStateMachine.RestartGame();
            }
        }

        public override void Exit()
        { 
        }
    }
}