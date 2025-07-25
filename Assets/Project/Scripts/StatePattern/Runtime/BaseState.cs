namespace Project.StateMachine.Runtime
{ 
    public abstract class BaseState : IState
    {
        protected BaseStateMachine StateMachine;

        protected BaseState(BaseStateMachine stateMachine)
        {
            this.StateMachine = stateMachine;
        }
        public virtual void Enter() { }
        public virtual void Update() { }
        public virtual void Exit() { }
    }
}