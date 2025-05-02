using AgaveCase.StateMachine.Runtime;

namespace AgaveCase.GameState.Runtime
{ 
    public abstract class BaseGameState : BaseState
    {
        protected GameStateMachine GameStateMachine => StateMachine as GameStateMachine;

        protected BaseGameState(GameStateMachine stateMachine) 
            : base(stateMachine)
        {
        }
    }
}