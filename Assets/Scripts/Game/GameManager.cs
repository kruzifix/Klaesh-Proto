using System.Collections;
using System.Collections.Generic;
using Klaesh.Core;
using Klaesh.Entity;
using Klaesh.Entity.Module;
using Klaesh.Game.Config;
using Klaesh.Hex;
using UnityEngine;

namespace Klaesh.Game
{
    public interface IGameManager
    {
        ISquad ActiveSquad { get; }

        void StartGame(IGameConfiguration game);

        void StartNextTurn();
        void EndTurn();
    }

    public class GameManager : ManagerBehaviour, IGameManager, IPickHandler<GameEntity>, IPickHandler<HexTile>
    {
        private IGameEntityManager _gem;
        private IHexMap _map;

        private IInputState _currentState;
        private IGameConfiguration _currentGameConfig;

        private List<Squad> _squads;
        private int _activeSquadIndex;

        public ISquad ActiveSquad => _squads?[_activeSquadIndex] ?? null;

        protected override void OnAwake()
        {
            _locator.RegisterSingleton<IGameManager, GameManager>(this);
        }

        private void Start()
        {
            _gem = _locator.GetService<IGameEntityManager>();
            _map = _locator.GetService<IHexMap>();

            _currentState = new IdleInputState();

            var picker = _locator.GetService<IObjectPicker>();
            picker.RegisterHandler<GameEntity>(KeyCode.Mouse0, "Entity", this);
            picker.RegisterHandler<HexTile>(KeyCode.Mouse0, "HexTile", this);

            // TODO: Deregister Handler!!!
        }

        private void OnDestroy()
        {
            _locator.DeregisterSingleton<IGameManager>();
        }

        public void StartGame(IGameConfiguration game)
        {
            _currentGameConfig = game;

            _map.Columns = game.Map.Columns;
            _map.Rows = game.Map.Rows;

            _map.GenParams.noiseOffset = game.Map.NoiseOffset;
            _map.GenParams.noiseScale = game.Map.NoiseScale;
            _map.GenParams.heightScale = game.Map.HeightScale;

            _map.BuildMap();

            _squads = new List<Squad>();
            foreach (var config in game.Squads)
            {
                var squad = new Squad(config);
                squad.CreateMembers(_gem);

                _squads.Add(squad);
            }

            _activeSquadIndex = -1;
            StartNextTurn();
        }

        public void StartNextTurn()
        {
            _activeSquadIndex = (_activeSquadIndex + 1) % _squads.Count;

            ActiveSquad.Members.ForEach(ge => ge.GetModule<MovementModule>().Reset());

            _bus.Publish(new FocusCameraMessage(this, ActiveSquad.Members[0].Position));
        }

        public void EndTurn()
        {

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
