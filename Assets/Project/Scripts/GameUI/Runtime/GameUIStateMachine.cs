using UnityEngine;
using TMPro;  
using AgaveCase.Data.Runtime;
using AgaveCase.EventChannel.Runtime;
using AgaveCase.StateMachine.Runtime;
using UnityEngine.UI;

namespace AgaveCase.GameUI.Runtime
{
    public class GameUIStateMachine : BaseStateMachine
    {  
        [Header("UI Elements")] 
        [SerializeField] private TextMeshProUGUI _scoreText;  
        [SerializeField] private TextMeshProUGUI _movesText;

        [Header("Game End UI")] 
        [SerializeField] private Canvas _inGameDynamicCanvas;
        [SerializeField] private GameObject _winPanel; 
        [SerializeField] private GameObject _losePanel;
        [SerializeField] private GameObject _gamePanel;  
        
        [Header("Buttons")]
        [SerializeField] private Button _resetWinButton;
        [SerializeField] private Button _resetLoseButton;
        [SerializeField] private Button _resetInGameButton;
      
        private ScoreData _scoreData;
        private MoveData _moveData;
        
        [Header("Broadcasting")] 
        [SerializeField] private VoidEventChannelSO _onGameExitRequested;  
        
        public void Init(ScoreData scoreData, MoveData moveData)
        { 
            _scoreData = scoreData;
            _moveData = moveData;
            
            Initialize();
            
            _inGameDynamicCanvas.worldCamera = Camera.main;
        }
        
        protected override void Initialize()
        { 
            var playingState = CreateStates(out var winState, out var loseState);

            AddState(playingState);
            AddState(winState);
            AddState(loseState);
             
            ChangeState<PlayingState>();
        }

        private PlayingState CreateStates(out WinState winState, out LoseState loseState)
        {
            PlayingState playingState = new PlayingState(
                this, 
                _gamePanel, 
                _scoreText, 
                _movesText, 
                _scoreData, 
                _moveData, 
                _resetInGameButton, 
                ResetGame);
                
            winState = new WinState(
                this, 
                _winPanel, 
                _resetWinButton, 
                ResetGame);
                
            loseState = new LoseState(
                this, 
                _losePanel, 
                _resetLoseButton, 
                ResetGame);
            return playingState;
        }

        private void ResetGame()
        {
            _onGameExitRequested?.RaiseEvent();
        } 
        
        public void PlayingState()
        {
            ChangeState<PlayingState>();
        }
        
        public void WinState()
        {
            ChangeState<WinState>();
        }
        
        public void LoseState()
        {
            ChangeState<LoseState>();
        }
    }
}