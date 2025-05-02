using AgaveCase.EventChannel.Runtime;
using CommandHandler.Runtime; 
using UnityEngine;

namespace AgaveCase.GameInitilazer.Runtime
{
    public class GameSceneInitilazer : CommandExecuteHandler
    {  
        [Header("Broadcasting")]
        [SerializeField] private VoidEventChannelSO _onGameSceneRequested;

        private void Start()
        {
            ExecuteAllCommands();
        }

        protected override void OnAllCommandsExecuted()
        {
            base.OnAllCommandsExecuted(); 
            _onGameSceneRequested?.RaiseEvent();
        }
    }
}
