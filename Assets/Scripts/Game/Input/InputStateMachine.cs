using Klaesh.GameEntity;
using Klaesh.Hex;
using UnityEngine;

namespace Klaesh.Game.Input
{
    public class InputStateMachine
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

        public void ProcessHexTile(HexTile tile)
        {
            _currentState?.ProcessHexTile(tile);
        }

        public void ProcessEntity(Entity entity)
        {
            _currentState?.ProcessEntity(entity);
        }
    }
}
