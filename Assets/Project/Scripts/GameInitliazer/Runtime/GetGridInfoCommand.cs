using AgaveCase.Data.Runtime; 
using CommandHandler.Runtime; 
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace AgaveCase.GameInitilazer.Runtime
{
    public class GetGridInfoCommand : Command
    { 
        [Header("Settings")]
        [SerializeField] private DataContainer dataContainer;   
        [Header("Panel")] 
        [SerializeField] private GameObject _panelGO;
        
        [Space(10),Header("Width Elements")] 
        [SerializeField] private TextMeshProUGUI _widthTitleText;
        [SerializeField] private Slider _widthInput;
        
        [Space(10), Header("Height Elements")]
        [SerializeField] private TextMeshProUGUI _heightTitleText;
        [SerializeField] private Slider _heightInput;
        
        [Space(10)]
        [SerializeField] private Button _createButton;
        
        public override void Execute()
        {
            _panelGO.SetActive(true);
            Undo();
            _widthInput.onValueChanged.AddListener(OnWidthSliderValueChanged);
            _heightInput.onValueChanged.AddListener(OnHeightSliderValueChanged);
            _createButton.onClick.AddListener(OnCreateButtonClicked);
        }

        public override void Undo()
        {  
            _widthInput.value = dataContainer.gridData.Width;
            _heightInput.value = dataContainer.gridData.Height; 
            _widthTitleText.text = "Width:" + _widthInput.value; 
            _heightTitleText.text = "Height:" +_heightInput.value;
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
            dataContainer.gridData.SetDimensions(width, height);
            
            _heightInput.onValueChanged.RemoveListener(OnHeightSliderValueChanged);
            _widthInput.onValueChanged.RemoveListener(OnWidthSliderValueChanged);
            
            _panelGO.SetActive(false);    
            base.Complete();
        }
    }
}