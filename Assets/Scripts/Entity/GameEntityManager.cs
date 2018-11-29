using System;
using System.Collections.Generic;
using System.Linq;
using Klaesh.Core;
using Klaesh.Core.Message;
using UnityEngine;

namespace Klaesh.Entity
{
    public interface IGameEntityManager
    {
        IDictionary<string, GameEntityDescriptor> Descriptors { get; }
        IList<IGameEntity> Entities { get; }

        IGameEntity CreateEntity(string id);
        IGameEntity GetEntity(int id);
    }

    public class GameEntityManager : MonoBehaviour, IGameEntityManager
    {
        public static IGameEntityManager Instance { get; private set; }

        public GameObject entityPrefab;

        private int _idCounter = 0;

        private Dictionary<string, GameEntityDescriptor> _descriptors;
        private List<IGameEntity> _entities;

        public IDictionary<string, GameEntityDescriptor> Descriptors => _descriptors;
        public IList<IGameEntity> Entities => _entities;

        private void Awake()
        {
            ServiceLocator.Instance.RegisterSingleton<IGameEntityManager, GameEntityManager>(this);
        }

        private void Start()
        {
            _entities = new List<IGameEntity>();
            _descriptors = Resources.LoadAll<GameEntityDescriptor>("Entities").ToDictionary(d => d.entityId);

            ServiceLocator.Instance.GetService<IMessageBus>().Publish(new GameEntityDescriptorsLoadedMessage(this));
        }

        private void OnDestroy()
        {
            ServiceLocator.Instance.DeregisterSingleton<IGameEntityManager>();
        }

        public IGameEntity CreateEntity(string id)
        {
            if (!_descriptors.ContainsKey(id))
                throw new Exception(string.Format("no entity descriptor with id '{0}'", id));

            var desc = _descriptors[id];

            var go = Instantiate(entityPrefab, transform);
            var entity = go.GetComponent<GameEntity>();
            entity.Initialize(_idCounter, desc);

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
