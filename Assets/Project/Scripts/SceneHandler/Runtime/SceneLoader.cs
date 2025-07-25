using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.SceneHandler.Runtime
{
    public class SceneLoader : MonoBehaviour
    {
        [Header("Scene Load Settings")]
        [SceneSelector]
        [SerializeField] private string _sceneName;
        [SerializeField] private bool _loadOnAwake = false;
        [SerializeField] private LoadSceneMode _loadSceneMode = LoadSceneMode.Additive;
        [SerializeField] private bool _activateOnLoad = false;
        [SerializeField, HideInInspector] private int _selectedSceneIndex = 0;

        private Scene _loadedScene;
        private Coroutine _loadingCoroutine;
         
        public event Action<float> OnLoadProgressChanged;
        public event Action OnSceneLoadStarted;
        public event Action<Scene> OnSceneLoadCompleted;
        public event Action<Scene> OnSceneUnloadStarted;
        public event Action OnSceneUnloadCompleted;

        public int SelectedSceneIndex
        {
            get => _selectedSceneIndex;
            set => _selectedSceneIndex = value;
        }
        
        public string SceneName
        {
            get => _sceneName;
            set => _sceneName = value;
        }

        public bool IsSceneLoaded { get; private set; }
        public bool IsLoadingInProgress => _loadingCoroutine != null;

        private void Awake()
        {
            if (!_loadOnAwake) return;
            LoadScene();
        }

        public void LoadScene()
        {
            if (IsLoadingInProgress) return;
            
            OnSceneLoadStarted?.Invoke();
            _loadingCoroutine = StartCoroutine(LoadSceneAsync());
        }
        
        public void UnloadScene()
        {
            if (!IsSceneLoaded || IsLoadingInProgress) return;
            
            OnSceneUnloadStarted?.Invoke(_loadedScene);
            StartCoroutine(UnloadSceneAsync());
        }

        private IEnumerator LoadSceneAsync()
        { 
            SceneManager.sceneLoaded += OnSceneLoaded;
             
            AsyncOperation operation;
            if (!string.IsNullOrEmpty(_sceneName))
            {
                operation = SceneManager.LoadSceneAsync(_sceneName, _loadSceneMode);
            }
            else
            {
                operation = SceneManager.LoadSceneAsync(SelectedSceneIndex, _loadSceneMode);
            }
            
            if (operation == null)
            {
                Debug.LogError($"Failed to load scene: {(string.IsNullOrEmpty(_sceneName) ? SelectedSceneIndex.ToString() : _sceneName)}");
                SceneManager.sceneLoaded -= OnSceneLoaded;
                _loadingCoroutine = null;
                yield break;
            }
             
            operation.allowSceneActivation = true;
             
            while (!operation.isDone)
            { 
                OnLoadProgressChanged?.Invoke(operation.progress);
                yield return null;
            }
        }

        private IEnumerator UnloadSceneAsync()
        {
            AsyncOperation operation;
            
            if (!string.IsNullOrEmpty(_sceneName))
            {
                operation = SceneManager.UnloadSceneAsync(_sceneName);
            }
            else
            {
                operation = SceneManager.UnloadSceneAsync(SelectedSceneIndex);
            }
            
            if (operation == null)
            {
                Debug.LogError($"Failed to unload scene: {(string.IsNullOrEmpty(_sceneName) ? SelectedSceneIndex.ToString() : _sceneName)}");
                yield break;
            }
            
            while (!operation.isDone)
            {
                OnLoadProgressChanged?.Invoke(operation.progress);
                yield return null;
            }
            
            IsSceneLoaded = false;
            OnSceneUnloadCompleted?.Invoke();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {
            _loadedScene = scene;
            IsSceneLoaded = true;
            
            if (_activateOnLoad)
            {
                SceneManager.SetActiveScene(scene);
            }
            
            OnSceneLoadCompleted?.Invoke(scene);
            SceneManager.sceneLoaded -= OnSceneLoaded;
            _loadingCoroutine = null;
        }
         
        public void ActivateScene()
        {
            if (!IsSceneLoaded)
            {
                Debug.LogWarning("Cannot activate scene because it is not loaded.");
                return;
            }
            
            SceneManager.SetActiveScene(_loadedScene);
        }
    }
}