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
using Klaesh.Input;

namespace Klaesh.Game
{
    public interface IGameManager : IInputProcessor
    {
        int TurnNumber { get; }
        int ActiveSquadIndex { get; }
        ISquad ActiveSquad { get; }
        bool HomeSquadActive { get; }

        IGameConfiguration GameConfig { get; }

        bool TurnEnded { get; }

        void StartGame(IGameConfiguration game);

        void StartNextTurn(StartTurnData data);
        void EndTurn();

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

        private InputStateMachine _inputMachine;
        private List<Squad> _squads;

        public int TurnNumber { get; private set; }
        public int ActiveSquadIndex { get; private set; }
        public ISquad ActiveSquad => _squads?[ActiveSquadIndex] ?? null;
        public bool HomeSquadActive => ActiveSquadIndex == GameConfig.HomeSquadId;

        public IGameConfiguration GameConfig { get; private set; }

        public bool TurnEnded { get; private set; }

        protected override void OnAwake()
        {
            _locator.RegisterSingleton<IGameManager>(this);
        }

        private void Start()
        {
            _gem = _locator.GetService<IEntityManager>();
            _map = _locator.GetService<IHexMap>();

            _inputMachine = new InputStateMachine();
            _inputMachine.SetState(null);

            var input = _locator.GetService<IGameInputManager>();
            input.RegisterProcessor(_inputMachine);

            _jconverter = _locator.GetService<IJsonConverter>();
            _jobManager = _locator.GetService<IJobManager>();
            _networker = _locator.GetService<INetworker>();

            var networkDataHandler = new Dictionary<EventCode, Action<string>>();

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

            networkDataHandler.Add(EventCode.GameStart, MakeHandlerBridge<GameConfiguration>(StartGame));
            networkDataHandler.Add(EventCode.GameAbort, MakeHandlerBridge<GameAbortData>(GameAborted));
            networkDataHandler.Add(EventCode.StartTurn, MakeHandlerBridge<StartTurnData>(StartNextTurn));
            networkDataHandler.Add(EventCode.DoJob, MakeHandlerBridge<IJob>(OnDoJob));
        }

        //private void OnDestroy()
        //{
        //    _locator.DeregisterSingleton<IGameManager>();
        //}

        private Action<string> MakeHandlerBridge<T>(Action<T> action)
        {
            return data =>
            {
                var jdata = _jconverter.DeserializeObject<T>(data);
                action(jdata);
            };
        }

        public void StartGame(IGameConfiguration game)
        {
            GameConfig = game;

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

            // Initialize Random with seed!
            NetRand.Seed(GameConfig.RandomSeed);

            // spawn debris on map
            int debrisCount = NetRand.Range(5, 11);
            for (int i = 0; i < debrisCount; i++)
            {
                // find empty position
                var pos = NetRand.HexOffset(_map.Columns, _map.Rows);
                while (_map.GetTile(pos).HasEntityOnTop)
                    pos = NetRand.HexOffset(_map.Columns, _map.Rows);

                var deb = _gem.CreateEntity("debris-stone");
                deb.GetComponent<HexPosComp>().SetPosition(pos);
            }

            TurnNumber = 0;
            TurnEnded = true;

            Debug.Log($"[GameManager] Starting game! id: {GameConfig.ServerId}");
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

            _inputMachine.SetState(null);

            if (HomeSquadActive)
            {
                _inputMachine.SetState(new IdleInputState(_inputMachine));
                TurnEnded = false;
            }

            ActiveSquad.Members.ForEach(ge => ge.GetComponent<HexMovementComp>().OnNextTurn());

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

            _inputMachine.SetState(null);

            _networker.SendData(EventCode.EndTurn, new EndTurnData { TurnNumber = TurnNumber });
            _bus.Publish(new TurnBoundaryMessage(this, false));
        }

        private void GameAborted(GameAbortData data)
        {
            Debug.Log($"[GameManager] game aborted. reason: {data.Reason}");

            _networker.Disconnect();

            _inputMachine.SetState(null);

            // cleanup!
            GameConfig = null;
            // kill all entities
            _gem.KillAll();

            // clear map
            _map.ClearMap();

            _bus.Publish(new GameAbortedMessage(this, data.Reason));
        }

        public bool IsPartOfActiveSquad(Entity entity)
        {
            return ActiveSquad.Members.Contains(entity);
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

        public void ProcessInput(InputCode code, object data)
        {
            _inputMachine.ProcessInput(code, data);
        }
    }
}
