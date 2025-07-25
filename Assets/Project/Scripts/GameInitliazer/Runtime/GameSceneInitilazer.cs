using CommandHandler.Runtime;
using Project.EventChannel.Runtime; 
using UnityEngine;

namespace Project.GameInitilazer.Runtime
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
