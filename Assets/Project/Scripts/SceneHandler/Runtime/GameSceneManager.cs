using System;
using System.Collections.Generic;
using UnityEngine;
using Project.EventChannel.Runtime;

namespace Project.SceneHandler.Runtime
{
    public class GameSceneManager : MonoBehaviour
    {
        [Serializable]
        public class SceneData
        {
            public SceneLoader sceneLoader;
            [HideInInspector] public bool isLoaded;
            [HideInInspector] public bool isActive;
        }

        [Header("Scene Configuration")] [SerializeField]
        private SceneData mainMenuScene;

        [SerializeField] private SceneData gameScene;

        [Header("Scene Logic")] [SerializeField]
        private bool closeMainMenuWhenGameLoads = true;

        [SerializeField] private bool autoLoadMainMenuOnStart = true;

        [Header("Event Channels")] [SerializeField]
        private VoidEventChannelSO _onMainMenuRequested;

        [SerializeField] private VoidEventChannelSO _onGameSceneRequested;
        [SerializeField] private VoidEventChannelSO _onGameExitRequested;

        private Dictionary<string, SceneData> _sceneRegistry = new Dictionary<string, SceneData>();

        public event Action<string> OnSceneLoadStarted;
        public event Action<string> OnSceneLoadCompleted;
        public event Action<string> OnSceneUnloadStarted;
        public event Action<string> OnSceneUnloadCompleted;

        private void Awake()
        {
            RegisterScene(mainMenuScene);
            RegisterScene(gameScene);
        }

        private void Start()
        {
            if (autoLoadMainMenuOnStart)
            {
                LoadScene(mainMenuScene);
            }
        }

        private void OnEnable()
        {
            _onMainMenuRequested.onEventRaised += HandleMainMenuRequested;

            _onGameSceneRequested.onEventRaised += HandleGameSceneRequested;

            _onGameExitRequested.onEventRaised += HandleGameExitRequested;
        }

        private void OnDisable()
        {
            _onMainMenuRequested.onEventRaised -= HandleMainMenuRequested;

            _onGameSceneRequested.onEventRaised -= HandleGameSceneRequested;

            _onGameExitRequested.onEventRaised -= HandleGameExitRequested;
        }

        private void RegisterScene(SceneData sceneData)
        {
            if (string.IsNullOrEmpty(sceneData.sceneLoader.SceneName))
            {
                Debug.LogWarning("Cannot register a scene with an empty name!");
                return;
            }

            if (!_sceneRegistry.ContainsKey(sceneData.sceneLoader.SceneName))
            {
                _sceneRegistry.Add(sceneData.sceneLoader.SceneName, sceneData);

                if (sceneData.sceneLoader != null)
                {
                    sceneData.sceneLoader.SceneName = sceneData.sceneLoader.SceneName;

                    sceneData.sceneLoader.OnSceneLoadStarted +=
                        () => OnSceneLoadStarted?.Invoke(sceneData.sceneLoader.SceneName);
                    sceneData.sceneLoader.OnSceneLoadCompleted += (scene) =>
                    {
                        sceneData.isLoaded = true;
                        sceneData.isActive = scene == UnityEngine.SceneManagement.SceneManager.GetActiveScene();
                        OnSceneLoadCompleted?.Invoke(sceneData.sceneLoader.SceneName);
                    };
                    sceneData.sceneLoader.OnSceneUnloadStarted += (scene) =>
                        OnSceneUnloadStarted?.Invoke(sceneData.sceneLoader.SceneName);
                    sceneData.sceneLoader.OnSceneUnloadCompleted += () =>
                    {
                        sceneData.isLoaded = false;
                        sceneData.isActive = false;
                        OnSceneUnloadCompleted?.Invoke(sceneData.sceneLoader.SceneName);
                    };
                }
                else
                {
                    Debug.LogError(
                        $"SceneLoader for {sceneData.sceneLoader.SceneName} is not assigned! This scene will not be loaded.");
                }
            }
            else
            {
                Debug.LogWarning($"Scene {sceneData.sceneLoader.SceneName} is already registered!");
            }
        }

        #region Scene Management Methods

        private void LoadScene(string sceneName)
        {
            if (_sceneRegistry.TryGetValue(sceneName, out SceneData sceneData))
            {
                LoadScene(sceneData);
            }
            else
            {
                Debug.LogError($"Scene {sceneName} not found in registry!");
            }
        }

        private void LoadScene(SceneData sceneData)
        {
            if (sceneData.isLoaded)
            {
                ActivateScene(sceneData.sceneLoader.SceneName);
                return;
            }

            if (sceneData.sceneLoader != null)
            {
                sceneData.sceneLoader.LoadScene();
            }
            else
            {
                Debug.LogError(
                    $"SceneLoader for {sceneData.sceneLoader.SceneName} is not assigned! This scene will not be loaded.");
            }
        }

        private void UnloadScene(string sceneName)
        {
            if (_sceneRegistry.TryGetValue(sceneName, out SceneData sceneData))
            {
                UnloadScene(sceneData);
            }
            else
            {
                Debug.LogError($"Scene {sceneName} not found in registry!");
            }
        }

        private void UnloadScene(SceneData sceneData)
        {
            if (!sceneData.isLoaded) return;

            if (sceneData.sceneLoader != null)
            {
                sceneData.sceneLoader.UnloadScene();
            }
            else
            {
                Debug.LogError(
                    $"SceneLoader for {sceneData.sceneLoader.SceneName} is not assigned! This scene will not be unloaded.");
            }
        }

        private void ActivateScene(string sceneName)
        {
            if (_sceneRegistry.TryGetValue(sceneName, out SceneData sceneData))
            {
                if (!sceneData.isLoaded)
                {
                    Debug.LogWarning($"Scene {sceneName} is not loaded! Loading it first.");
                    LoadScene(sceneName);
                    return;
                }

                if (sceneData.sceneLoader != null)
                {
                    sceneData.sceneLoader.ActivateScene();
                    sceneData.isActive = true;

                    foreach (var entry in _sceneRegistry)
                    {
                        if (entry.Key != sceneName)
                        {
                            entry.Value.isActive = false;
                        }
                    }
                }
            }
            else
            {
                Debug.LogError($"Scene {sceneName} not found in registry!");
            }
        }

        private bool IsSceneLoaded(string sceneName)
        {
            if (_sceneRegistry.TryGetValue(sceneName, out SceneData sceneData))
            {
                return sceneData.isLoaded;
            }

            return false;
        }

        public bool IsSceneActive(string sceneName)
        {
            if (_sceneRegistry.TryGetValue(sceneName, out SceneData sceneData))
            {
                return sceneData.isActive;
            }

            return false;
        }

        #endregion

        #region Event Handlers

        private void HandleMainMenuRequested()
        {
            if (IsSceneLoaded(gameScene.sceneLoader.SceneName) && closeMainMenuWhenGameLoads)
            {
                UnloadScene(gameScene.sceneLoader.SceneName);
            }

            LoadScene(mainMenuScene.sceneLoader.SceneName);
        }

        private void HandleGameSceneRequested()
        {
            LoadScene(gameScene.sceneLoader.SceneName);

            if (closeMainMenuWhenGameLoads && IsSceneLoaded(mainMenuScene.sceneLoader.SceneName))
            {
                UnloadScene(mainMenuScene.sceneLoader.SceneName);
            }
        }

        private void HandleGameExitRequested()
        {
            UnloadScene(gameScene.sceneLoader.SceneName);

            if (!IsSceneLoaded(mainMenuScene.sceneLoader.SceneName)) LoadScene(mainMenuScene.sceneLoader.SceneName);
            else ActivateScene(mainMenuScene.sceneLoader.SceneName);
        }

        #endregion
    }
}