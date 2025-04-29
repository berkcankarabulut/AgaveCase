using CommandHandler.Runtime;
using SceneLoadSystem.Runtime;
using UnityEngine;

namespace AgaveCase.GameInitilazer.Runtime
{
    public class GameInitilazerHandler : CommandExecuteHandler
    {
        [SerializeField] private SceneLoader _sceneLoader;
        private void Start()
        {
            ExecuteAllCommands();
        }

        protected override void OnAllCommandsExecuted()
        {
            base.OnAllCommandsExecuted();
            _sceneLoader.LoadScene();
        }
    }
}
