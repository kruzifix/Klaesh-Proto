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
using System.Linq;
using Klaesh.Game.Job;
using Klaesh.Game.Input;
using Klaesh.Game.Message;

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

        void ActivateInput(string id);
        void ActivateInput(string id, object data);

        bool IsPartOfActiveSquad(Entity entity);

        Entity ResolveEntityRef(SquadEntityRefData data);
    }

    public class GameManager : ManagerBehaviour, IGameManager//, IPickHandler<GameEntity.GameEntity>, IPickHandler<HexTile>
    {
        private IEntityManager _gem;
        private IHexMap _map;
        private INetworker _networker;
        private IJsonConverter _jconverter;
        private IJobManager _jobManager;

        private IInputState _baseState;
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

            _baseState = NullInputState.Instance;
            _currentState = NullInputState.Instance;

            //var picker = _locator.GetService<IObjectPicker>();
            //picker.RegisterHandler<GameEntity.GameEntity>(KeyCode.Mouse0, "Entity", this);
            //picker.RegisterHandler<HexTile>(KeyCode.Mouse0, "HexTile", this);

            var input = _locator.GetService<IGameInputComponent>();
            input.RegisterHandler<Entity>("Entity", OnPickGameEntity);
            input.RegisterHandler<HexTile>("HexTile", OnPickHexTile);

            // TODO: Deregister Handler!!!

            _jconverter = _locator.GetService<IJsonConverter>();
            _jobManager = _locator.GetService<IJobManager>();
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
            networkDataHandler.Add(EventCode.GameAbort, MakeHandlerBridge<GameAbortData>(GameAborted));
            networkDataHandler.Add(EventCode.StartTurn, MakeHandlerBridge<StartTurnData>(StartNextTurn));
            networkDataHandler.Add(EventCode.DoJob, MakeHandlerBridge<IJob>(OnDoJob));
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
            _bus.Publish(new GameStartedMessage(this));
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

            _baseState = NullInputState.Instance;

            if (HomeSquadActive)
            {
                _baseState = new IdleInputState();
                SwitchTo(_baseState);
                TurnEnded = false;
            }

            _squads.ForEach(s => s.Members.ForEach(ge => ge.GetComponent<HexMovementComp>().OnNextTurn()));

            _bus.Publish(new FocusCameraMessage(this, ActiveSquad.Members[0].transform.position));
            _bus.Publish(new TurnBoundaryMessage(this, true));
        }

        private void OnDoJob(IJob job)
        {
            _jobManager.AddJob(job);
            _jobManager.ExecuteJobs();
        }

        public void EndTurn()
        {
            if (TurnEnded)
                return;
            TurnEnded = true;

            SwitchTo(NullInputState.Instance);

            _networker.SendData(EventCode.EndTurn, new EndTurnData { TurnNumber = TurnNumber });
            _bus.Publish(new TurnBoundaryMessage(this, false));
        }

        private void GameAborted(GameAbortData data)
        {
            Debug.Log($"[GameManager] game aborted. reason: {data.Reason}");

            _networker.Disconnect();

            _baseState = NullInputState.Instance;
            _currentState = NullInputState.Instance;

            // cleanup!
            // kill all entities
            _gem.KillAll();

            // clear map
            _map.ClearMap();

            _bus.Publish(new GameAbortedMessage(this, data.Reason));
        }

        public void ActivateInput(string id)
        {
            ActivateInput(id, null);
        }

        public void ActivateInput(string id, object data)
        {
            if (id.Equals("abort") && _currentState != _baseState)
            {
                SwitchTo(_baseState);
                return;
            }

            SwitchTo(_currentState.OnInputActivated(id, data));
        }

        public bool IsPartOfActiveSquad(Entity entity)
        {
            return ActiveSquad.Members.Contains(entity);
            //var eSquad = entity.GetModule<ISquad>();
            //if (eSquad == null)
            //    return false;
            //return ActiveSquad == eSquad;
        }

        public Entity ResolveEntityRef(SquadEntityRefData data)
        {
            // this can throw so many exception, but it is intended.
            // for now....
            // ...i think
            return _squads
                .Where(s => s.Config.ServerId == data.SquadId)
                .First()
                .Members[data.MemberId];
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
                Debug.Log($"[GameManager] switching input state! {_currentState.GetType().Name} -> {newState.GetType().Name}");

                _currentState.OnDisabled();
                _currentState = newState;
                _currentState.OnEnabled();
            }
        }
    }
}
