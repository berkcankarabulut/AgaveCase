namespace AgaveCase.StateMachine.Runtime
{ 
    public interface IState
    {
        void Enter();
        void Update();
        void Exit();
    }
}