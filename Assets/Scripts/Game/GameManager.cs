using System.Collections;
using System.Collections.Generic;
using Klaesh.Core;
using Klaesh.GameEntity;
using Klaesh.Game.Config;
using Klaesh.Hex;
using UnityEngine;
using Klaesh.GameEntity.Component;
using Klaesh.Network;
using Klaesh.Utility;
using Klaesh.Game.Data;
using System;
using Klaesh.Game.UI;

namespace Klaesh.Game
{
    public interface IGameManager
    {
        int TurnNumber { get; }
        int ActiveSquadIndex { get; }
        ISquad ActiveSquad { get; }
        bool HomeSquadActive { get; }

        IGameConfiguration CurrentConfig { get; }

        bool TurnEnded { get; }

        void StartGame(IGameConfiguration game);

        void StartNextTurn(StartTurnData data);
        void EndTurn();

        bool IsPartOfActiveSquad(Entity entity);
    }

    public class GameManager : ManagerBehaviour, IGameManager//, IPickHandler<GameEntity.GameEntity>, IPickHandler<HexTile>
    {
        private IEntityManager _gem;
        private IHexMap _map;
        private INetworker _networker;
        private IJsonConverter _jconverter;

        private IInputState _currentState;
        private IGameConfiguration _currentGameConfig;

        private List<Squad> _squads;

        public int TurnNumber { get; private set; }
        public int ActiveSquadIndex { get; private set; }
        public ISquad ActiveSquad => _squads?[ActiveSquadIndex] ?? null;
        public bool HomeSquadActive => ActiveSquadIndex == _currentGameConfig.HomeSquadId;

        public IGameConfiguration CurrentConfig => _currentGameConfig;

        public bool TurnEnded { get; private set; }

        protected override void OnAwake()
        {
            _locator.RegisterSingleton<IGameManager>(this);
        }

        private void Start()
        {
            _gem = _locator.GetService<IEntityManager>();
            _map = _locator.GetService<IHexMap>();

            _currentState = NullInputState.Instance;

            //var picker = _locator.GetService<IObjectPicker>();
            //picker.RegisterHandler<GameEntity.GameEntity>(KeyCode.Mouse0, "Entity", this);
            //picker.RegisterHandler<HexTile>(KeyCode.Mouse0, "HexTile", this);

            var input = _locator.GetService<IGameInputComponent>();
            input.RegisterHandler<Entity>("Entity", OnPickGameEntity);
            input.RegisterHandler<HexTile>("HexTile", OnPickHexTile);

            // TODO: Deregister Handler!!!

            _jconverter = _locator.GetService<IJsonConverter>();
            _networker = _locator.GetService<INetworker>();

            var networkDataHandler = new Dictionary<EventCode, Action<string>>();

            //_networker.DataReceived += OnDataReceived;
            _networker.DataReceived += (code, data) =>
            {
                if (networkDataHandler.ContainsKey(code))
                {
                    networkDataHandler[code].Invoke(data);
                }
                else
                {
                    Debug.Log($"[GameManager] unhandeled event code received: {code}\n{data}");
                }
            };

            //networkDataHandler.Add(EventCode.HeartBeat, MakeHandlerBridge<HeartBeatData>(OnHeartBeat));
            networkDataHandler.Add(EventCode.GameStart, MakeHandlerBridge<GameConfiguration>(StartGame));
            networkDataHandler.Add(EventCode.StartTurn, MakeHandlerBridge<StartTurnData>(StartNextTurn));
            networkDataHandler.Add(EventCode.MoveUnit, MakeHandlerBridge<MoveUnitData>(OnMoveUnit));
        }

        private void OnDestroy()
        {
            _locator.DeregisterSingleton<IGameManager>();
        }

        private Action<string> MakeHandlerBridge<T>(Action<T> action)
        {
            return data =>
            {
                var jdata = _jconverter.DeserializeObject<T>(data);
                action(jdata);
            };
        }

        //private void OnHeartBeat(HeartBeatData data)
        //{
        //    Debug.Log($"[HeartBeat] {data.Time}");
        //    _networker.SendData(EventCode.HeartBeat, new HeartBeatData { Time = DateTime.Now });
        //}

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

            TurnNumber = 0;
            TurnEnded = true;

            // Initialize Random with seed!


            Debug.Log($"[GameManager] Starting game! id: {_currentGameConfig.ServerId}; random seed: {_currentGameConfig.RandomSeed}");
        }

        public void StartNextTurn(StartTurnData data)
        {
            // check turn number
            if (data.TurnNumber != TurnNumber + 1)
            {
                throw new Exception($"got wrong turn number. expected {TurnNumber + 1}, got {data.TurnNumber}");
            }

            TurnNumber = data.TurnNumber;
            ActiveSquadIndex = data.ActiveSquadIndex;

            if (HomeSquadActive)
            {
                SwitchTo(new IdleInputState());
                TurnEnded = false;
            }

            _squads.ForEach(s => s.Members.ForEach(ge => ge.GetComponent<HexMovementComp>().OnNextTurn()));

            _bus.Publish(new FocusCameraMessage(this, ActiveSquad.Members[0].transform.position));
            _bus.Publish(new RefreshGameUIMessage(this));
        }

        private void OnMoveUnit(MoveUnitData data)
        {
            // check squad id, and if active
            if (data.SquadId != ActiveSquad.Config.ServerId)
            {
                throw new Exception("got move unit message, and sender is not active player!");
            }
            // get the entity and move it
            bool canMove = ActiveSquad.Members[data.MemberId]
                .GetComponent<HexMovementComp>()
                .StartMovingTo(data.Target, () => Debug.Log($"unit moved to {data.Target}"));
            if (!canMove)
            {
                throw new Exception($"unit was unable to move to {data.Target}!");
            }
        }

        public void EndTurn()
        {
            if (TurnEnded)
                return;
            TurnEnded = true;

            SwitchTo(NullInputState.Instance);

            _networker.SendData(EventCode.EndTurn, new EndTurnData { TurnNumber = TurnNumber });
        }

        public bool IsPartOfActiveSquad(Entity entity)
        {
            return ActiveSquad.Members.Contains(entity);
            //var eSquad = entity.GetModule<ISquad>();
            //if (eSquad == null)
            //    return false;
            //return ActiveSquad == eSquad;
        }

        public void OnPickHexTile(HexTile comp)
        {
            SwitchTo(_currentState.OnPickHexTile(comp));
        }

        public void OnPickGameEntity(Entity comp)
        {
            SwitchTo(_currentState.OnPickGameEntity(comp));
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
