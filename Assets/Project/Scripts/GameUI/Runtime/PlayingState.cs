 
using Project.StateMachine.Runtime;
using DG.Tweening;
using System;
using Project.Data.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.GameUI.Runtime
{
    public class PlayingState : BaseState
    {
        private readonly GameObject _panel;
        private readonly TextMeshProUGUI _scoreText;
        private readonly TextMeshProUGUI _movesText;
        private readonly ScoreData _scoreData;
        private readonly MoveData _moveData;
        private readonly Button _resetInGameButton;
        private readonly Action _resetCallback;
        
        private readonly GameUIStateMachine _stateMachine;
        
        private Color _defaultColor = Color.white;
        private Color _goalReachedColor = Color.green;
         
        private float _bounceAnimationDuration = 0.3f;
        private float _bounceScale = 1.2f;
        
        public PlayingState(
            GameUIStateMachine stateMachine,
            GameObject panel,
            TextMeshProUGUI scoreText,
            TextMeshProUGUI movesText,
            ScoreData scoreData,
            MoveData moveData,
            Button resetInGameButton,
            Action resetCallback) 
            : base(stateMachine)
        {
            _stateMachine = stateMachine;
            _panel = panel;
            _scoreText = scoreText;
            _movesText = movesText;
            _scoreData = scoreData;
            _moveData = moveData;
            _resetInGameButton = resetInGameButton;
            _resetCallback = resetCallback;
        }
        
        public override void Enter()
        {
            _panel.SetActive(true); 
            
            _scoreData.OnScoreChanged += UpdateScoreUI;
            _moveData.OnMovesChanged += UpdateMovesUI;
            
            if (_resetInGameButton != null)
            {
                _resetInGameButton.onClick.AddListener(OnResetClicked);
            }
            
            UpdateUI();
        }
        
        public override void Exit()
        {
            _panel.SetActive(false);
            
            _scoreData.OnScoreChanged -= UpdateScoreUI;
            _moveData.OnMovesChanged -= UpdateMovesUI;
            
            if (_resetInGameButton != null)
            {
                _resetInGameButton.onClick.RemoveListener(OnResetClicked);
            }
             
            if (_scoreText != null)
            {
                DOTween.Kill(_scoreText.transform);
            }
            
            if (_movesText != null)
            {
                DOTween.Kill(_movesText.transform);
            }
        }
        
        private void UpdateUI()
        {
            UpdateScoreUI(_scoreData.CurrentScore);
            UpdateMovesUI(_moveData.MovesRemaining);
        }
        
        private void UpdateScoreUI(int score)
        {
            if (_scoreText == null) return;
             
            _scoreText.text = score + "/" + _scoreData.TargetScore; 
            _scoreText.color = score >= _scoreData.TargetScore ? _goalReachedColor : _defaultColor;
             
            PlayBounceAnimation(_scoreText.transform);
        }
        
        private void UpdateMovesUI(int moves)
        {
            if (_movesText == null) return;
            _movesText.text = moves.ToString();
             
            PlayBounceAnimation(_movesText.transform);
        }
        
        private void PlayBounceAnimation(Transform target)
        { 
            DOTween.Kill(target); 
            Vector3 originalScale = Vector3.one; 
            Sequence bounceSequence = DOTween.Sequence();
             
            bounceSequence.Append(target.DOScale(originalScale * _bounceScale, _bounceAnimationDuration * 0.5f)
                .SetEase(Ease.OutQuad));
             
            bounceSequence.Append(target.DOScale(originalScale, _bounceAnimationDuration * 0.5f)
                .SetEase(Ease.OutBounce));
        }
        
        private void OnResetClicked()
        {
            _resetCallback?.Invoke();
        }
    }
}