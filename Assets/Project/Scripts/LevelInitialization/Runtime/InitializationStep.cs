using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Project.LevelInitialization.Runtime
{
    [Serializable]
    public class InitializationStep
    {
        [SerializeField] private string _stepName;
        [SerializeField] private bool _isRequired = true;
        [SerializeField] private float _timeoutSeconds = 10f;
        
        public string StepName => _stepName;
        public bool IsRequired => _isRequired;
        public float TimeoutSeconds => _timeoutSeconds;
        public bool IsCompleted { get; private set; }
        public Exception Error { get; private set; }
        
        private Func<UniTask> _initializeFunc;
        
        public InitializationStep(string stepName, Func<UniTask> initializeFunc, bool isRequired = true, float timeout = 10f)
        {
            _stepName = stepName;
            _initializeFunc = initializeFunc;
            _isRequired = isRequired;
            _timeoutSeconds = timeout;
        }
        
        public async UniTask ExecuteAsync()
        {
            try
            {
                Debug.Log($"🔄 Initializing: {_stepName}");
                
                if (_timeoutSeconds > 0)
                {
                    await _initializeFunc().TimeoutWithoutException(TimeSpan.FromSeconds(_timeoutSeconds));
                }
                else
                {
                    await _initializeFunc();
                }
                
                IsCompleted = true;
                Debug.Log($"✅ Completed: {_stepName}");
            }
            catch (Exception ex)
            {
                Error = ex;
                Debug.LogError($"❌ Failed: {_stepName} - {ex.Message}");
                
                if (_isRequired)
                    throw;
            }
        }
    }
}