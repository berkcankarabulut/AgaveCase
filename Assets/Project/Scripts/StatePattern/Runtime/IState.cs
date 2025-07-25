namespace Project.StateMachine.Runtime
{ 
    public interface IState
    {
        void Enter();
        void Update();
        void Exit();
    }
}