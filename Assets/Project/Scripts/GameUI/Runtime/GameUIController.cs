using UnityEngine;
using TMPro;  
using AgaveCase.Data.Runtime;
using AgaveCase.EventChannel.Runtime;
using UnityEngine.UI;

namespace AgaveCase.GameUI.Runtime
{
    public class GameUIController : MonoBehaviour
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
        
        private ScoreUI _scoreUI;
        private MoveUI _moveUI;
        
        [Header("Broadcasting")] 
        [SerializeField] private VoidEventChannelSO _onGameExitRequested;
        
        public void Init(ScoreData scoreData, MoveData moveData)
        { 
            _scoreData = scoreData;
            _moveData = moveData;
            _scoreUI = new ScoreUI(_scoreText, _scoreData);
            _moveUI = new MoveUI(_movesText, _moveData);
            
            if (_winPanel != null) _winPanel.SetActive(false);
            if (_losePanel != null) _losePanel.SetActive(false);
            if(_gamePanel != null) _gamePanel.SetActive(true);

            UpdateAllUI();
            AddEvents();
            _inGameDynamicCanvas.worldCamera = Camera.main;
        }
        
        private void OnEnable()
        {
            AddEvents();
        }

        private void OnDisable()
        {
            DisableEvents();
        }

        private void AddEvents()
        {
            if (_scoreData == null) return;
            _scoreUI.OnEnable();
            _moveUI.OnEnable();
            _resetWinButton.onClick.AddListener(ResetGame);
            _resetInGameButton.onClick.AddListener(ResetGame);
            _resetLoseButton.onClick.AddListener(ResetGame);
        }

        private void DisableEvents()
        {
            if (_scoreData == null) return;
            _scoreUI.OnDisable();
            _moveUI.OnDisable();
            _resetWinButton.onClick.RemoveListener(ResetGame);
            _resetInGameButton.onClick.RemoveListener(ResetGame);
            _resetLoseButton.onClick.RemoveListener(ResetGame);
        }

        private void ResetGame()
        {
            _onGameExitRequested?.RaiseEvent();
        }

        private void UpdateAllUI()
        {
            if (_scoreData == null) return;
            _scoreUI.UpdateScoreUI(_scoreData.CurrentScore);
            _moveUI.UpdateMovesUI(_moveData.MovesRemaining);
        } 

        public void SetStatusLoseUI(bool status)
        {  
            _losePanel.SetActive(status);
        }
        
        public void SetStatusWinUI(bool status)
        { 
            _winPanel.SetActive(status);
        } 

        public void SetStatusGamePanel(bool status)
        { 
            _gamePanel.SetActive(status);
        }
 
    }
}