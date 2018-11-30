using System.Collections;
using System.Collections.Generic;
using Klaesh.Core;
using Klaesh.Entity;
using Klaesh.Hex;
using UnityEngine;

namespace Klaesh.Game
{
    public interface IGameManager
    {

    }

    public class GameManager : ManagerBehaviour, IGameManager, IPickHandler<GameEntity>, IPickHandler<HexTile>
    {
        private IInputState _currentState;

        protected override void OnAwake()
        {
            _locator.RegisterSingleton<IGameManager, GameManager>(this);
        }

        private void Start()
        {
            _currentState = new IdleInputState();

            var picker = _locator.GetService<IObjectPicker>();
            picker.RegisterHandler<GameEntity>(KeyCode.Mouse0, "Entity", this);
            picker.RegisterHandler<HexTile>(KeyCode.Mouse0, "HexTile", this);
        }

        private void OnDestroy()
        {
            _locator.DeregisterSingleton<IGameManager>();
        }

        public void OnPick(HexTile comp, RaycastHit hit)
        {
            SwitchTo(_currentState.OnPickHexTile(comp, hit));
        }

        public void OnPick(GameEntity comp, RaycastHit hit)
        {
            SwitchTo(_currentState.OnPickGameEntity(comp, hit));
        }

        private void SwitchTo(IInputState newState)
        {
            if (newState != null)
            {
                _currentState.OnDisabled();
                _currentState = newState;
                _currentState.OnEnabled();
            }
        }
    }
}
