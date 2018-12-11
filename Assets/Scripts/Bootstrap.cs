﻿using System;
using System.Collections.Generic;
using Klaesh.Core;
using Klaesh.Core.Message;
using Klaesh.Game;
using Klaesh.Game.Config;
using Klaesh.GameEntity;
using Klaesh.Hex;
using Klaesh.Network;
using Klaesh.Utility;
using Newtonsoft.Json;
using UnityEngine;

namespace Klaesh
{
    public class Bootstrap : MonoBehaviour, ICoroutineStarter
    {
        public static Bootstrap Instance { get; private set; }

        public bool useServer = false;

        private ServiceManager _serviceLocator;

        private void Awake()
        {
            if (Instance != null)
                throw new Exception("multiple Bootstrap instances!");
            Instance = this;
            Create();
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            Application.targetFrameRate = 60;

            var bus = _serviceLocator.GetService<IMessageBus>();

            // TODO: get from current webgl context/provider?
            // or from config file!
            string url = "ws://localhost:3000";

            // setup game board
            bus.Subscribe<EntityPrefabsLoadedMessage>(msg =>
            {
                if (useServer)
                {
                    var networker = _serviceLocator.GetService<INetworker>();
                    networker.ConnectTo(url);
                }
                else
                {
                    var map = _serviceLocator.GetService<IHexMap>();

                    int marg = 1;

                    var config = new GameConfiguration
                    {
                        MapConfig = new HexMapConfiguration
                        {
                            Columns = 7,
                            Rows = 12,
                            NoiseOffset = 0,
                            NoiseScale = 0.5f,
                            HeightScale = 7f
                        },
                        SquadsConfig = new List<SquadConfiguration>
                        {
                            new SquadConfiguration
                            {
                                Name = "Rowdy3",
                                Color = Colors.SquadColors[0],
                                OffsetOrigin = new HexOffsetCoord(map.Columns / 2, marg),
                                UnitsConfig = new List<UnitConfiguration>
                                {
                                    new UnitConfiguration
                                    {
                                        EntityId = "knight",
                                        OffsetPos = new HexOffsetCoord()
                                    },
                                    new UnitConfiguration
                                    {
                                        EntityId = "knight",
                                        OffsetPos = HexCubeCoord.Offset(HexDirection.West, 2).OffsetCoord
                                    },
                                    new UnitConfiguration
                                    {
                                        EntityId = "knight",
                                        OffsetPos = HexCubeCoord.Offset(HexDirection.East, 2).OffsetCoord
                                    }
                                }
                            },
                            new SquadConfiguration
                            {
                                Name = "Mighty Drei",
                                Color = Colors.SquadColors[1],
                                OffsetOrigin = new HexOffsetCoord(map.Columns / 2, map.Rows - marg - 1),
                                UnitsConfig = new List<UnitConfiguration>
                                {
                                    new UnitConfiguration
                                    {
                                        EntityId = "skeleton",
                                        OffsetPos = new HexOffsetCoord()
                                    },
                                    new UnitConfiguration
                                    {
                                        EntityId = "skeleton",
                                        OffsetPos = HexCubeCoord.Offset(HexDirection.West, 2).OffsetCoord
                                    },
                                    new UnitConfiguration
                                    {
                                        EntityId = "skeleton",
                                        OffsetPos = HexCubeCoord.Offset(HexDirection.East, 2).OffsetCoord
                                    }
                                }
                            }
                        }
                    };

                    var data = _serviceLocator.GetService<IJsonConverter>().SerializeObject(config, Formatting.Indented);
                    Debug.Log(data);
                    config = _serviceLocator.GetService<IJsonConverter>().DeserializeObject<GameConfiguration>(data);

                    _serviceLocator.GetService<IGameManager>().StartGame(config);
                }
            });
        }

        private void Create()
        {
            // load config

            _serviceLocator = new ServiceManager();
            ServiceLocator.Instance = _serviceLocator;
            _serviceLocator.RegisterSingleton<ICoroutineStarter>(this);
            _serviceLocator.RegisterSingleton<IMessageBus>(new MessageBus());
            _serviceLocator.RegisterSingleton<IJsonConverter>(new CustomJsonConverter());
        }
    }
}
