using System;
using AgaveCase.StateMachine.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace AgaveCase.GameUI.Runtime
{
    public class LoseState : BaseState
    {
        private readonly GameObject _losePanel;
        private readonly Button _resetButton;
        private readonly Action _resetCallback;
        private readonly GameUIStateMachine _uiStateMachine;
        
        public LoseState(
            GameUIStateMachine uiStateMachine,
            GameObject losePanel,
            Button resetButton,
            Action resetCallback) 
            : base(uiStateMachine)
        {
            _uiStateMachine = uiStateMachine;
            _losePanel = losePanel;
            _resetButton = resetButton;
            _resetCallback = resetCallback;
        }
        
        public override void Enter()
        {
            _losePanel.SetActive(true);
            
            if (_resetButton != null)
            {
                _resetButton.onClick.AddListener(OnResetClicked);
            }
        }
        
        public override void Exit()
        {
            _losePanel.SetActive(false);
            
            if (_resetButton != null)
            {
                _resetButton.onClick.RemoveListener(OnResetClicked);
            }
        } 
        
        private void OnResetClicked()
        {
            _resetCallback?.Invoke();
        }
    }
}