using AgaveCase.Data.Runtime;
using TMPro;

namespace AgaveCase.GameUI.Runtime
{
    public class ScoreUI : UIComponent
    {
        private TextMeshProUGUI _scoreText;
        ScoreData _scoreData;
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
        }
    }
}