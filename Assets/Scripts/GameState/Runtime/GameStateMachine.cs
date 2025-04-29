using UnityEngine;
using System;
using AgaveCase.Board.Runtime;
using AgaveCase.StateMachine.Runtime;

namespace AgaveCase.GameState.Runtime
{ 
    public class GameStateMachine : BaseStateMachine
    {  
        [SerializeField] private BoardManager _boardManager;
        private PlayingState _playingState;
        private WinState _winState;
        private LoseState _loseState;
        
        public PlayingState PlayingState => _playingState;
        public WinState WinState => _winState;
        public LoseState LoseState => _loseState;

        private void Start()
        {
            Initialize();
        }

        protected override void Initialize()
        { 
            _playingState = new PlayingState(this, _boardManager);
            _winState = new WinState(this);
            _loseState = new LoseState(this);  
            
            AddState(_playingState);
            AddState(_winState);
            AddState(_loseState);
             
            ChangeState(_playingState);
        }
         
        public void StartGame()
        {
            ChangeState<PlayingState>();
        } 
        
        public void WinGame()
        {
            ChangeState<WinState>();
        }
         
        public void LoseGame()
        {
            ChangeState<LoseState>();
        }
         
        public void RestartGame()
        { 
            StartGame();
        }
    }
}