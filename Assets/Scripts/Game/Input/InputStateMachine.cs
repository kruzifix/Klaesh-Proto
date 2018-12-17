using Klaesh.Input;
using UnityEngine;

namespace Klaesh.Game.Input
{
    public class InputStateMachine : IInputProcessor, IGameInputProcessor
    {
        private IInputState _currentState;

        public void SetState(IInputState state)
        {
            Debug.Log($"[GameManager] switching input state! {_currentState?.GetType().Name} -> {state?.GetType().Name}");

            _currentState?.Exit();
            _currentState = state;
            _currentState?.Enter();
        }

        public void ProcessInput(InputCode code, object data)
        {
            _currentState?.ProcessInput(code, data);
        }

        public void OnEnter(GameObject go)
        {
            _currentState?.OnEnter(go);
        }

        public void OnExit(GameObject go)
        {
            _currentState?.OnExit(go);
        }

        public void OnDown(GameObject go)
        {
            _currentState?.OnDown(go);
        }

        public void OnUp(GameObject go)
        {
            _currentState?.OnUp(go);
        }

        public void OnClick(GameObject go)
        {
            _currentState?.OnClick(go);
        }
    }
}
