using AgaveCase.GameUI.Runtime;

namespace AgaveCase.GameState.Runtime
{ 
    public class UIServiceAdapter : IUIService
    {
        private readonly GameUIStateMachine _gameUIStateMachine;

        public UIServiceAdapter(GameUIStateMachine gameUIStateMachine)
        {
            _gameUIStateMachine = gameUIStateMachine;
        }

        public void OpenPlayingPanel()
        {
            _gameUIStateMachine.PlayingState();
        }
        
        public void OpenWinPanel()
        {
            _gameUIStateMachine.WinState();
        }
        
        public void OpenLosePanel()
        {
            _gameUIStateMachine.LoseState();
        }
    }
}