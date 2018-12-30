using System.Collections.Generic;
using Klaesh.Game;
using Klaesh.Game.Config;
using Klaesh.Game.Input;
using Klaesh.GameEntity;
using Klaesh.Hex;
using Klaesh.Input;
using Klaesh.Utility;
using Newtonsoft.Json;
using UnityEngine;

namespace Klaesh.UI.Window
{
    public class MapGenWindow : WindowBase
    {
        private IHexMap _map;
        private IGameInputManager _im;
        private IEntityManager _gem;

        private GameConfiguration _config;
        private int _seed;

        private InputStateMachine _input;

        public TextAsset configFile;

        protected override void Init()
        {
            _map = _locator.GetService<IHexMap>();
            _im = _locator.GetService<IGameInputManager>();
            _gem = _locator.GetService<IEntityManager>();

            _config = JsonConvert.DeserializeObject<GameConfiguration>(configFile.text);
            _seed = 0;

            _input = new InputStateMachine();
            _input.SetState(new SpectateInputState(_input));

            _im.RegisterProcessor(_input);

            //ChanceTest();

            Generate();
        }

        protected override void DeInit()
        {
            _im.DeRegisterProcessor(_input);

            ClearEverything();
        }

        public void OnGenerateClicked()
        {
            Generate();
        }

        private void ClearEverything()
        {
            _gem.KillAll();

            _map.ClearMap();
        }

        public void Generate()
        {
            ClearEverything();

            var game = _config;
            game.RandomSeed = _seed++;
            game.MapConfig.NoiseOffset = game.RandomSeed;
            Debug.Log($"generating map for seed {game.RandomSeed}");
            if (!MapGenerator.Generate(_config, _map, _gem, out List<Squad> _squads))
            {
                Debug.Log($"map from seed {game.RandomSeed} unusable. regenerating...");
                Generate();
            }
        }

        private void ChanceTest()
        {
            NetRand.Seed(0);
            Debug.Log($"5, 10: {DoTest(10000, 5, 10)}");

            NetRand.Seed(0);
            Debug.Log($"1, 10: {DoTest(10000, 1, 10)}");

            NetRand.Seed(0);
            Debug.Log($"3, 10: {DoTest(10000, 3, 10)}");

            NetRand.Seed(0);
            Debug.Log($"2, 3: {DoTest(10000, 2, 3)}");

            NetRand.Seed(0);
            Debug.Log($"45, 100: {DoTest(10000, 45, 100)}");
        }

        private float DoTest(int runs, int probability, int precision)
        {
            int jep = 0;
            for (int i = 0; i < runs; i++)
                jep += NetRand.Chance(probability, precision) ? 1 : 0;
            return jep * 1f / runs;
        }
    }
}
