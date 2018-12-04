using System;
using System.Collections.Generic;
using System.Linq;
using Klaesh.Core;
using Klaesh.GameEntity.Descriptor;
using Klaesh.GameEntity.Module;
using UnityEngine;

namespace Klaesh.GameEntity
{
    public interface IGameEntityManager
    {
        IDictionary<string, GameEntityDescriptor> Descriptors { get; }
        IList<IGameEntity> Entities { get; }

        /// <summary>
        /// </summary>
        /// <param name="addModules">is called during entity creation, before module initialization</param>
        /// <returns></returns>
        IGameEntity CreateEntity(string type, Action<IGameEntity> addModules = null);
        IGameEntity GetEntity(int id);
    }

    public class GameEntityManager : ManagerBehaviour, IGameEntityManager
    {
        public GameObject entityPrefab;

        private int _idCounter = 0;
        private IGameEntityModuleFactory _moduleFactory;

        private Dictionary<string, GameEntityDescriptor> _descriptors;
        private List<IGameEntity> _entities;

        public IDictionary<string, GameEntityDescriptor> Descriptors => _descriptors;
        public IList<IGameEntity> Entities => _entities;

        protected override void OnAwake()
        {
            _locator.RegisterSingleton<IGameEntityManager, GameEntityManager>(this);

            _moduleFactory = new GameEntityModuleFactory();
            _moduleFactory.RegisterCreator<UnitModuleDescriptor>((e, desc) =>
            {
                e.AddModule(new HexPosModule());
                e.AddModule(new MovementModule(desc.maxDistance, desc.jumpHeight));
            });
            _moduleFactory.RegisterCreator<MeshModuleDescriptor>((e, desc) =>
            {
                e.AddModule(new MeshModule(desc.meshPrefab, desc.meshOffset));
            });
        }

        private void Start()
        {
            _entities = new List<IGameEntity>();
            _descriptors = Resources.LoadAll<GameEntityDescriptor>("Entities").ToDictionary(d => d.type);

            _bus.Publish(new GameEntityDescriptorsLoadedMessage(this));
        }

        private void OnDestroy()
        {
            _locator.DeregisterSingleton<IGameEntityManager>();
        }

        //public IGameEntity CreateEntity(string type)
        public IGameEntity CreateEntity(string type, Action<IGameEntity> addModules = null)
        {
            if (!_descriptors.ContainsKey(type))
                throw new Exception(string.Format("no entity descriptor with type '{0}'", type));

            var desc = _descriptors[type];

            var go = Instantiate(entityPrefab, transform);
            var entity = go.GetComponent<GameEntity>();
            entity.Initialize(_idCounter, desc);

            foreach (var modDesc in desc.modules)
            {
                _moduleFactory.CreateModules(entity, modDesc);
            }

            addModules?.Invoke(entity);

            entity.InitModules();

            _entities.Add(entity);

            _idCounter++;

            return entity;
        }

        public IGameEntity GetEntity(int id)
        {
            // TODO: add dict map cache for fast lookup
            return _entities.FirstOrDefault(e => e.Id == id);
        }
    }
}
