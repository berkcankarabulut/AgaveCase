using System;
using System.Collections.Generic; 
using Cysharp.Threading.Tasks; 
using Project.Data.Runtime; 
using UnityEngine;

namespace Project.LevelInitialization.Runtime
{
    public class GameInitializer : MonoBehaviour
    {  
        [Header("Settings")]
        [SerializeField] private bool _showProgressUI = true;
        [SerializeField] private float _stepDelay = 0.1f;
        [SerializeField] private bool _enableParallelSteps = false;
        
        // Events
        public event Action<string> OnStepStarted;
        public event Action<string> OnStepCompleted;
        public event Action<float> OnProgressUpdated;
        public event Action OnInitializationComplete;
        public event Action<string> OnInitializationFailed;
        
        // State
        private List<InitializationStep> _initializationSteps;
        private bool _isInitialized = false;
        
        // Shared data between steps
        private ScoreData _scoreData;
        private MoveData _moveData;
        
        private async void Start()
        {
            await InitializeGameAsync();
        }
        
        public async UniTask InitializeGameAsync()
        {
            if (_isInitialized)
            {
                Debug.LogWarning("Game already initialized!");
                return;
            }
            
            try
            {
                Debug.Log("🚀 Starting game initialization...");
                
                // Build initialization steps
                BuildInitializationSteps();
                
                // Execute steps
                if (_enableParallelSteps)
                {
                    await ExecuteStepsInParallel();
                }
                else
                {
                    await ExecuteStepsSequentially();
                }
                
                _isInitialized = true;
                Debug.Log("✅ Game initialization complete!");
                
                OnInitializationComplete?.Invoke(); 
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ Game initialization failed: {ex}");
                OnInitializationFailed?.Invoke(ex.Message);
                throw;
            }
        }
        
        private void BuildInitializationSteps()
        {
            _initializationSteps = new List<InitializationStep>
            {
                // Step 1: Initialize Data
                new InitializationStep("Initialize Game Data", InitializeGameDataAsync, isRequired: true, timeout: 5f),
                
                // Step 2: Initialize Board
                new InitializationStep("Initialize Board", InitializeBoardAsync, isRequired: true, timeout: 10f),
                
                // Step 3: Initialize Services
                new InitializationStep("Initialize Services", InitializeServicesAsync, isRequired: true, timeout: 5f),
                
                // Step 4: Initialize UI
                new InitializationStep("Initialize UI", InitializeUIAsync, isRequired: true, timeout: 5f),
                
                // Step 5: Initialize Game State Machine
                new InitializationStep("Initialize Game States", InitializeGameStatesAsync, isRequired: true, timeout: 5f),
                
                // Step 6: Validate Setup
                new InitializationStep("Validate Game Setup", ValidateGameSetupAsync, isRequired: false, timeout: 2f)
            };
        }
        
        private async UniTask ExecuteStepsSequentially()
        {
            for (int i = 0; i < _initializationSteps.Count; i++)
            {
                var step = _initializationSteps[i];
                
                OnStepStarted?.Invoke(step.StepName);
                
                await step.ExecuteAsync();
                
                if (step.IsCompleted)
                {
                    OnStepCompleted?.Invoke(step.StepName);
                    OnProgressUpdated?.Invoke((float)(i + 1) / _initializationSteps.Count);
                }
                
                // Small delay for visual feedback
                if (_stepDelay > 0)
                    await UniTask.Delay((int)(_stepDelay * 1000));
            }
        }
        
        private async UniTask ExecuteStepsInParallel()
        {
            // Group steps that can run in parallel
            var parallelGroups = new List<List<InitializationStep>>
            {
                // Group 1: Data initialization (sequential dependency)
                new List<InitializationStep> { _initializationSteps[0] },
                
                // Group 2: Board and Services (can be parallel after data)
                new List<InitializationStep> { _initializationSteps[1], _initializationSteps[2] },
                
                // Group 3: UI and States (can be parallel after services)
                new List<InitializationStep> { _initializationSteps[3], _initializationSteps[4] },
                
                // Group 4: Validation (after everything)
                new List<InitializationStep> { _initializationSteps[5] }
            };
            
            int completedSteps = 0;
            
            foreach (var group in parallelGroups)
            {
                var tasks = group.Select(step => ExecuteStepWithCallback(step, () => {
                    completedSteps++;
                    OnProgressUpdated?.Invoke((float)completedSteps / _initializationSteps.Count);
                }));
                
                await UniTask.WhenAll(tasks);
            }
        }
        
        private async UniTask ExecuteStepWithCallback(InitializationStep step, Action onComplete)
        {
            OnStepStarted?.Invoke(step.StepName);
            await step.ExecuteAsync();
            
            if (step.IsCompleted)
            {
                OnStepCompleted?.Invoke(step.StepName);
                onComplete?.Invoke();
            }
        } 
    }
}