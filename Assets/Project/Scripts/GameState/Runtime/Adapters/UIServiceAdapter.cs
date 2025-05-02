using AgaveCase.GameUI.Runtime;

namespace AgaveCase.GameState.Runtime
{ 
    public class UIServiceAdapter : IUIService
    {
        private readonly GameUIController _gameUIController;

        public UIServiceAdapter(GameUIController gameUIController)
        {
            _gameUIController = gameUIController;
        }

        public void SetStatusGamePanel(bool status)
        {
            _gameUIController.SetGamePanel(status);
        }
        
        public void SetStatusWinPanel(bool status)
        {
            _gameUIController.SetWinPanel(status);
        }
        
        public void SetStatusLosePanel(bool status)
        {
            _gameUIController.SetLosePanel(status);
        }
    }
}