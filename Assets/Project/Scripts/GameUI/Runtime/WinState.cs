using System;
using AgaveCase.StateMachine.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace AgaveCase.GameUI.Runtime
{
    public class WinState : BaseState
    {
        private readonly GameObject _panel;
        private readonly Button _resetButton;
        private readonly Action _resetCallback;
        private readonly GameUIStateMachine _uiStateMachine;
        
        public WinState(
            GameUIStateMachine uiStateMachine,
            GameObject panel,
            Button resetButton,
            Action resetCallback) 
            : base(uiStateMachine)
        {
            _uiStateMachine = uiStateMachine;
            _panel = panel;
            _resetButton = resetButton;
            _resetCallback = resetCallback;
        }
        
        public override void Enter()
        {
            _panel.SetActive(true);
            
            if (_resetButton != null)
            {
                _resetButton.onClick.AddListener(OnResetClicked);
            }
        }
        
        public override void Exit()
        {
            _panel.SetActive(false);
            
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