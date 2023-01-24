using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RainyDay
{
    public class StateMachine
    {
        public IState CurrentState => _states[_currentStateName];

        string _previousStateName, _currentStateName, _nextStateName;
        Dictionary<string, IState> _states;

        public StateMachine()
        {
            _states = new Dictionary<string, IState>();
        }

        public void SetStates(string initialStateName, params IState[] states)
        {
            _states.Clear();

            _currentStateName = initialStateName;
            foreach(var state in states)
                _states.Add(state.Name, state);

            CurrentState.OnEnter();
        }

        public void AddState(IState state)
        {
            if (!_states.ContainsKey(state.Name))
            {
                Debug.LogError($"State you are trying to add ({state.Name}) already exists. Please replace the state when already existing.");
                return;
            }
            _states.Add(state.Name, state);
        }

        public bool ContainsState(string name) => _states.ContainsKey(name);

        public void ChangeState(string nextStateName)
        {
            if (nextStateName == _currentStateName)
                return;
            
            if(!_states.ContainsKey(nextStateName))
            {
                Debug.LogError($"State {nextStateName} does not exists!");
                return;
            }

            if (!string.IsNullOrEmpty(_currentStateName))
                CurrentState.OnExit();
            _previousStateName = _currentStateName;
            _currentStateName = nextStateName;

            CurrentState.OnEnter();
        }

        public void QueueNextState(string nextStateName)
        {
            _nextStateName = nextStateName;
        }

        public void OnUpdate()
        {
            if(!string.IsNullOrEmpty(_nextStateName))
            {
                ChangeState(_nextStateName);
                _nextStateName = null;
            }
            else if (!string.IsNullOrEmpty(_currentStateName))
                CurrentState.Update();
        }

        public void OnFixedUpdate()
        {
            if (!string.IsNullOrEmpty(_currentStateName))
                CurrentState.FixedUpdate();
        }

        public void OnLateUpdate()
        {
            if (!string.IsNullOrEmpty(_currentStateName))
                CurrentState.LateUpdate();
        }
    }
}
