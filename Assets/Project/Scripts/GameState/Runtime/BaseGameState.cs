using Project.StateMachine.Runtime;

namespace Project.GameState.Runtime
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