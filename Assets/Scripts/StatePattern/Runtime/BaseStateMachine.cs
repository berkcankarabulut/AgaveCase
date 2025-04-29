using System;
using System.Collections.Generic;
using UnityEngine;

namespace AgaveCase.StateMachine.Runtime
{  
    public abstract class BaseStateMachine : MonoBehaviour
    {
        private IState _currentState;
        private Dictionary<Type, IState> _states = new Dictionary<Type, IState>();

        protected IState CurrentState => _currentState;
        
        protected abstract void Initialize(); 
        
        protected virtual void Update()
        {  
            _currentState?.Update();
        }
  
        public void AddState(IState state)
        {
            Type stateType = state.GetType();
            if (_states.ContainsKey(stateType)) return;
            _states.Add(stateType, state);
        }
  
        public void ChangeState<T>() where T : IState
        {
            Type stateType = typeof(T);
            if (!_states.TryGetValue(stateType, out IState newState)) return; 
            _currentState?.Exit(); 
            _currentState = newState; 
            _currentState.Enter();
             
            OnStateChanged(newState);
        } 
        
        public void ChangeState(IState state)
        {
            if (state == null) return;
 
            _currentState?.Exit(); 
            _currentState = state; 
            _currentState.Enter();
             
            OnStateChanged(state);
        } 
        
        public T GetState<T>() where T : class, IState
        {
            Type stateType = typeof(T);
            return _states.TryGetValue(stateType, out IState state) ? state as T : null;
        } 
         
        public bool IsInState<T>() where T : IState
        {
            return _currentState is T;
        }
         
        protected virtual void OnStateChanged(IState newState) 
        { 
            
        }
         
    }
}