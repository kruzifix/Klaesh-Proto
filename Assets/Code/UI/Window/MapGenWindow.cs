using System.Collections.Generic;
using Klaesh.Game;
using Klaesh.Game.Config;
using Klaesh.Game.Input;
using Klaesh.GameEntity;
using Klaesh.GameEntity.Component;
using Klaesh.Hex;
using Klaesh.Input;
using Klaesh.Utility;
using Newtonsoft.Json;
using UnityEngine;

namespace Klaesh.UI.Window
{
    public class MapGenWindow : WindowBase
    {
        private GameConfiguration _config;
        private int _seed;

        private InputStateMachine _input;

        public TextAsset configFile;

        protected override void Init()
        {
            _config = JsonConvert.DeserializeObject<GameConfiguration>(configFile.text);
            _seed = 0;

            _input = new InputStateMachine();
            _input.SetState(new SpectateInputState(_input));

            var im = _locator.GetService<IGameInputManager>();
            im.RegisterProcessor(_input);

            Generate();
        }

        protected override void DeInit()
        {
            var im = _locator.GetService<IGameInputManager>();
            im.DeRegisterProcessor(_input);

            ClearEverything();
        }

        public void OnGenerateClicked()
        {
            Generate();
        }

        private void ClearEverything()
        {
            var _gem = _locator.GetService<IEntityManager>();
            _gem.KillAll();

            var _map = _locator.GetService<IHexMap>();
            _map.ClearMap();
        }

        public void Generate()
        {
            ClearEverything();

            var game = _config;
            game.RandomSeed = _seed++;

            var _map = _locator.GetService<IHexMap>();
            _map.Columns = game.Map.Columns;
            _map.Rows = game.Map.Rows;

            _map.GenParams.noiseOffset = game.Map.NoiseOffset;
            _map.GenParams.noiseScale = game.Map.NoiseScale;
            _map.GenParams.heightScale = game.Map.HeightScale;

            _map.BuildMap();

            var _gem = _locator.GetService<IEntityManager>();

            var _squads = new List<Squad>();
            foreach (var config in game.Squads)
            {
                var squad = new Squad(config);
                squad.CreateMembers(_gem);

                _squads.Add(squad);
            }

            // Initialize Random with seed!
            NetRand.Seed(game.RandomSeed);

            // spawn debris on map
            int debrisCount = NetRand.Range(7, 16);
            for (int i = 0; i < debrisCount; i++)
            {
                // find empty position
                var pos = NetRand.HexOffset(_map.Columns, _map.Rows);
                var tile = _map.GetTile(pos);
                while (tile.HasEntityOnTop)
                {
                    pos = NetRand.HexOffset(_map.Columns, _map.Rows);
                    tile = _map.GetTile(pos);
                }

                var deb = _gem.CreateEntity("mountain");
                deb.GetComponent<HexPosComp>().SetPosition(pos);

                foreach (var dir in HexCubeCoord.Offsets)
                {
                    var npos = pos.CubeCoord + dir * 2;
                    var neighbor = _map.GetTile(npos);
                    if (neighbor == null || neighbor.HasEntityOnTop)
                        continue;
                    if (NetRand.Chance(3, 10))
                    {
                        var ndeb = _gem.CreateEntity("mountain");
                        ndeb.GetComponent<HexPosComp>().SetPosition(npos);
                    }
                }
            }

            // generate forest patches
            int treeCount = NetRand.Range(7, 16);
            for (int i = 0; i < debrisCount; i++)
            {
                var pos = NetRand.HexOffset(_map.Columns, _map.Rows);
                var tile = _map.GetTile(pos);
                while (tile.Terrain != HexTerrain.Plain || tile.HasEntityOnTop)
                {
                    pos = NetRand.HexOffset(_map.Columns, _map.Rows);
                    tile = _map.GetTile(pos);
                }

                tile.Terrain = HexTerrain.Forest;

                foreach (var dir in HexCubeCoord.Offsets)
                {
                    var neighbor = _map.GetTile(pos.CubeCoord + dir);
                    if (neighbor == null || neighbor.Terrain != HexTerrain.Plain || neighbor.HasEntityOnTop)
                        continue;
                    if (NetRand.Chance(3, 10))
                    {
                        neighbor.Terrain = HexTerrain.Forest;
                    }
                }
            }
        }
    }
}
