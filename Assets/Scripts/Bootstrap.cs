using System;
using System.Collections.Generic;
using Klaesh.Core;
using Klaesh.Core.Message;
using Klaesh.Game;
using Klaesh.Game.Config;
using Klaesh.GameEntity;
using Klaesh.Hex;
using Klaesh.Utility;
using UnityEngine;

namespace Klaesh
{
    public class Bootstrap : MonoBehaviour
    {
        public static Bootstrap Instance { get; private set; }

        private ServiceManager _serviceLocator;
        private IMessageBus _eventBus;

        private void Awake()
        {
            if (Instance != null)
                throw new Exception("multiple Bootstrap instances!");
            Instance = this;
            Create();
        }

        private void Start()
        {
            Application.targetFrameRate = 60;

            _eventBus = _serviceLocator.GetService<IMessageBus>();

            DontDestroyOnLoad(gameObject);

            // setup game board
            _eventBus.Subscribe<EntityPrefabsLoadedMessage>(msg =>
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

                _serviceLocator.GetService<IGameManager>().StartGame(config);
            });
        }

        private void Create()
        {
            // load config

            _serviceLocator = new ServiceManager();
            ServiceLocator.Instance = _serviceLocator;
            _serviceLocator.RegisterSingleton<IMessageBus>(new MessageBus());
        }
    }
}
