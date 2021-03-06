﻿using System.Collections.Generic;
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
using Klaesh.Game.Cards;

namespace Klaesh.Game
{
    public interface IGameManager : IInputProcessor
    {
        int TurnNumber { get; }
        int ActiveSquadIndex { get; }
        ISquad ActiveSquad { get; }
        ISquad HomeSquad { get; }

        bool HomeSquadActive { get; }

        IGameConfiguration GameConfig { get; }

        bool TurnEnded { get; }

        void StartGame(IGameConfiguration game);

        void StartNextTurn(StartTurnData data);
        void EndTurn();

        bool IsPartOfActiveSquad(Entity entity);

        Entity ResolveEntityRef(SquadEntityRefData data);

        void UseCard(int cardId);
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

        private StartTurnData _nextTurnData;

        // TWEAK data
        private const int cardsHandStartSize = 3;
        private const int cardsHandMaximumSize = 4;

        public int TurnNumber { get; private set; }
        public int ActiveSquadIndex { get; private set; }
        public ISquad ActiveSquad => _squads?[ActiveSquadIndex] ?? null;
        public ISquad HomeSquad => _squads?[GameConfig.HomeSquadId] ?? null;

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

            while (!MapGenerator.Generate(GameConfig, _map, _gem, out _squads))
            {
                _gem.KillAll();
                _map.ClearMap();

                (GameConfig as GameConfiguration).RandomSeed += 1;
            }

            _squads.ForEach(s => s.Deck.MaximumHandSize = cardsHandMaximumSize);

            TurnNumber = 0;
            TurnEnded = true;
            _nextTurnData = null;

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

            _nextTurnData = data;

            if (_jobManager.NoJobsLeft)
            {
                DoStartTurn();
            }
            else
            {
                _jobManager.AllJobsFinished += DoStartTurn;
            }
        }

        private void DoStartTurn()
        {
            if (_nextTurnData == null)
                throw new Exception("Missing data for next turn?");

            _jobManager.AllJobsFinished -= DoStartTurn;

            TurnNumber = _nextTurnData.TurnNumber;
            ActiveSquadIndex = _nextTurnData.ActiveSquadIndex;

            _nextTurnData = null;

            _inputMachine.SetState(HomeSquadActive ? new IdleInputState(_inputMachine) : null);

            TurnEnded = false;

            // CARDS!
            if (TurnNumber == 1)
            {
                foreach (var s in _squads)
                {
                    for (int i = 0; i < cardsHandStartSize; i++)
                        s.Deck.DrawCard();
                }
            }
            else
            {
                // draw one at the start of your turn
                ActiveSquad.Deck.DrawCard();
            }

            var handler = new List<INewTurnHandler>();
            ActiveSquad.AliveMembers.ForEach(ent => handler.AddRange(ent.GetComponents<INewTurnHandler>()));
            handler.ForEach(h => h.OnNewTurn());

            _bus.Publish(new FocusCameraMessage(this, ActiveSquad.AliveMembers.Last().transform.position));

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
            _squads.Clear();
            _gem.KillAll();

            // clear map
            _map.ClearMap();

            _bus.Publish(new GameAbortedMessage(this, data.Reason));
        }

        public bool IsPartOfActiveSquad(Entity entity)
        {
            return ActiveSquad.AliveMembers.Contains(entity);
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

        public void UseCard(int cardId)
        {
            ActiveSquad.Deck.UseCard(cardId);
        }
    }
}
