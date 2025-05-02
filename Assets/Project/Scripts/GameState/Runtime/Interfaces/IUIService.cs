namespace AgaveCase.GameState.Runtime
{ 
    public interface IUIService
    {
        void SetStatusGamePanel(bool status);
        void SetStatusWinPanel(bool status);
        void SetStatusLosePanel(bool status);
    }
}