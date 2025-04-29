using UnityEngine;

namespace AgaveCase.GameState.Runtime
{ 
    public class LoseState : BaseGameState
    {
        public LoseState(GameStateMachine stateMachine) 
            : base(stateMachine)
        {
        }

        public override void Enter()
        {
            Debug.Log("Oyun kaybedildi - Lose State'e girildi"); 
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
            Debug.Log("Lose State'den çıkılıyor"); 
        }
    }
}