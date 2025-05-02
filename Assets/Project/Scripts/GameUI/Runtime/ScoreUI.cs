using AgaveCase.Data.Runtime;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace AgaveCase.GameUI.Runtime
{
    public class ScoreUI : UIComponent
    {
        private TextMeshProUGUI _scoreText;
        ScoreData _scoreData;
        private Color _defaultColor = Color.white;
        private Color _goalReachedColor = Color.green;
        public ScoreUI(TextMeshProUGUI scoreText, ScoreData scoreData)
        {
            _scoreText = scoreText;
            _scoreData = scoreData;
        }

        public override void OnEnable()
        {
            _scoreData.OnScoreChanged += UpdateScoreUI;
        }

        public override void OnDisable()
        {
            _scoreData.OnScoreChanged -= UpdateScoreUI;
        }

        public void UpdateScoreUI(int score)
        {
            if (_scoreText == null) return;
             
            _scoreText.text = score + "/" + _scoreData.TargetScore; 
            _scoreText.color = score >= _scoreData.TargetScore ? _goalReachedColor : _defaultColor; 
            PlayBounceAnimation();
        }
        
        private void PlayBounceAnimation()
        {  
            DOTween.Kill(_scoreText.transform); 
            _scoreText.transform.localScale = Vector3.one; 
            Sequence bounceSequence = DOTween.Sequence();
            bounceSequence.Append(_scoreText.transform.DOScale(1.2f, 0.15f));
            bounceSequence.Append(_scoreText.transform.DOScale(1f, 0.15f).SetEase(Ease.OutBounce));
        }
    }
}