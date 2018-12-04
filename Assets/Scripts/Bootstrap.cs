using System;
using Klaesh;
using Klaesh.Core;
using Klaesh.Core.Message;
using Klaesh.Entity;
using Klaesh.Entity.Module;
using Klaesh.Hex;
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
            _eventBus.Subscribe<GameEntityDescriptorsLoadedMessage>(msg =>
            {
                var em = msg.Sender as IGameEntityManager;
                var map = _serviceLocator.GetService<IHexMap>();

                int marg = 1;

                //var cube = em.CreateEntity("cube-brute");
                //cube.GetModule<HexPosModule>().TryMoveTo(new HexOffsetCoord(marg, marg).ToCube());

                //cube = em.CreateEntity("cube-brute");
                //cube.GetModule<HexPosModule>().TryMoveTo(new HexOffsetCoord(map.Columns / 2, marg).ToCube());

                //cube = em.CreateEntity("cube-brute");
                //cube.GetModule<HexPosModule>().TryMoveTo(new HexOffsetCoord(map.Columns - marg - 1, marg).ToCube());

                //var round = em.CreateEntity("round-brute");
                //round.GetModule<HexPosModule>().TryMoveTo(new HexOffsetCoord(marg, map.Rows - marg - 1).ToCube());

                //round = em.CreateEntity("round-brute");
                //round.GetModule<HexPosModule>().TryMoveTo(new HexOffsetCoord(map.Columns / 2, map.Rows - marg - 1).ToCube());

                //round = em.CreateEntity("round-brute");
                //round.GetModule<HexPosModule>().TryMoveTo(new HexOffsetCoord(map.Columns - marg - 1, map.Rows - marg - 1).ToCube());

                _eventBus.Publish(new FocusCameraMessage(this, map.GetTile(map.Columns / 2, map.Rows / 2).GetTop()));
            });
        }

        private void Create()
        {
            // load config

            _serviceLocator = new ServiceManager();
            ServiceLocator.Instance = _serviceLocator;
            _serviceLocator.RegisterSingleton<IMessageBus, MessageBus>(new MessageBus());
        }
    }
}
