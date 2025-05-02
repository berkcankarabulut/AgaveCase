using AgaveCase.Data.Runtime;
using TMPro;
using UnityEngine;

namespace AgaveCase.GameUI.Runtime
{
    public class MoveUI : UIComponent
    {
        private TextMeshProUGUI _movesText;
        MoveData _moveData;
        public MoveUI(TextMeshProUGUI moveText, MoveData moveData)
        {
            _movesText = moveText;
            _moveData = moveData;
        }

        public override void OnEnable()
        {
            _moveData.OnMovesChanged += UpdateMovesUI;
        }

        public override void OnDisable()
        {
            _moveData.OnMovesChanged -= UpdateMovesUI;
        } 
        
        public void UpdateMovesUI(int moves)
        {
            if (_movesText == null) return;
            _movesText.text = moves.ToString();
        } 
        
    }
}