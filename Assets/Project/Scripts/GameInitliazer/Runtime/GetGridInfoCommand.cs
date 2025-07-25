 
using CommandHandler.Runtime;
using Project.Data.Runtime;
using TMPro;
using UnityEngine; 
using UnityEngine.UI;

namespace Project.GameInitilazer.Runtime
{
    public class GetGridInfoCommand : Command
    {  
        [Header("Settings")]
        [SerializeField] private DataContainer _dataContainer;   
        [Header("Panel")] 
        [SerializeField] private GameObject _panelGO;
        
        [Space(10), Header("Move Elements")]
        [SerializeField] private TextMeshProUGUI _moveTitleText;
        [SerializeField] private Slider _moveInput;
        
        [Space(10),Header("Width Elements")] 
        [SerializeField] private TextMeshProUGUI _widthTitleText;
        [SerializeField] private Slider _widthInput;
        
        [Space(10), Header("Height Elements")]
        [SerializeField] private TextMeshProUGUI _heightTitleText;
        [SerializeField] private Slider _heightInput;

        [Space(10), Header("Height Elements")] 
        [SerializeField] private TMP_InputField _targetScoreField;
        
        [Space(10)]
        [SerializeField] private Button _createButton;
        
        public override void Execute()
        {
            _panelGO.SetActive(true);
            Undo();
            _targetScoreField.placeholder.GetComponent<TextMeshProUGUI>().text =
                _dataContainer.gameData.TargetScore.ToString();
            AddEvents();
        }

        public override void Undo()
        {  
            _widthInput.value = _dataContainer.gridData.Width;
            _heightInput.value = _dataContainer.gridData.Height;
            _moveInput.value = _dataContainer.gameData.MovesLimit;
            _moveTitleText.text = "Move Limit: " + _dataContainer.gameData.MovesLimit;
            _widthTitleText.text = "Width:" + _widthInput.value; 
            _heightTitleText.text = "Height:" +_heightInput.value;
        }

        private void AddEvents()
        {
            _moveInput.onValueChanged.AddListener(OnMoveSliderValueChanged);
            _widthInput.onValueChanged.AddListener(OnWidthSliderValueChanged);
            _heightInput.onValueChanged.AddListener(OnHeightSliderValueChanged);
            _createButton.onClick.AddListener(OnCreateButtonClicked);
        }

        private void RemoveEvents()
        {
            _moveInput.onValueChanged.RemoveListener(OnMoveSliderValueChanged);
            _heightInput.onValueChanged.RemoveListener(OnHeightSliderValueChanged);
            _widthInput.onValueChanged.RemoveListener(OnWidthSliderValueChanged);
            _createButton.onClick.RemoveListener(OnCreateButtonClicked);
        }

        private void OnMoveSliderValueChanged(float moveLimit)
        {
            _moveTitleText.text = "Move Limit: " + _moveInput.value;
        }

        private void OnHeightSliderValueChanged(float height)
        { 
            _heightTitleText.text = "Height:" +_heightInput.value;
        }

        private void OnWidthSliderValueChanged(float width)
        { 
            _widthTitleText.text = "Width:" + _widthInput.value;
        }

        private void OnCreateButtonClicked()
        {
            int width = (int)_widthInput.value;
            int height = (int)_heightInput.value;
            
            _dataContainer.gridData.SetDimensions(width, height);
            if (!string.IsNullOrWhiteSpace(_targetScoreField.text))
            {
                int targetScore = int.Parse(_targetScoreField.text);
                if (targetScore > 0)
                {
                    _dataContainer.gameData.SetTargetScore(targetScore);
                } 
            }

            _dataContainer.gameData.SetMoveLimit((int)_moveInput.value);
            
            RemoveEvents(); 
            _panelGO.SetActive(false);    
            base.Complete();
        }
    }
}