using System;
using System.Collections.Generic;
using System.Linq;
using Klaesh.Core;
using Klaesh.GameEntity.Message;
using Klaesh.Utility;
using UnityEngine;

namespace Klaesh.GameEntity
{
    public interface IEntityManager
    {
        /// <summary>
        /// </summary>
        /// <param name="addModules">is called during entity creation, before module initialization</param>
        /// <returns></returns>
        Entity CreateEntity(string type, Action<Entity> addModules = null);
        Entity GetEntity(int id);

        void KillEntity(Entity entity);
        void KillEntities(IEnumerable<Entity> entities);
        void KillAll();
    }

    public class EntityManager : ManagerBehaviour, IEntityManager
    {
        private int _idCounter = 0;
        private Dictionary<string, GameObject> _prefabs;
        private List<Entity> _entities;

        protected override void OnAwake()
        {
            _locator.RegisterSingleton<IEntityManager>(this);
        }

        private void Start()
        {
            _entities = new List<Entity>();

            // TODO & ATTENTION! loading all prefabs can maybe consume a lot of memory!
            // maybe don't do this!
            _prefabs = Resources.LoadAll<GameObject>("Entities")
                .Where(go => go.GetComponent<Entity>() != null)
                .ToDictionary(go => go.name);

            _bus.Publish(new EntityPrefabsLoadedMessage(this));
        }

        //private void OnDestroy()
        //{
        //    _locator.DeregisterSingleton<IEntityManager>();
        //}

        public Entity CreateEntity(string name, Action<Entity> addModules = null)
        {
            if (!_prefabs.ContainsKey(name))
            {
                throw new Exception($"No prefab with name {name} exists.");
            }

            var go = Instantiate(_prefabs[name], transform);
            var entity = go.GetComponent<Entity>();
            entity.Initialize(_idCounter);

            addModules?.Invoke(entity);

            _entities.Add(entity);

            entity.InitModules();

            _idCounter++;

            _bus.Publish(new EntityCreatedMessage(this, entity));

            return entity;
        }

        public Entity GetEntity(int id)
        {
            // TODO: add dict map cache for fast lookup
            return _entities.FirstOrDefault(e => e.Id == id);
        }

        public void KillEntity(Entity entity)
        {
            // TODO: entity died message?

            _entities.Remove(entity);
            Destroy(entity.gameObject);
        }

        public void KillEntities(IEnumerable<Entity> entities)
        {
            foreach (var ent in entities)
            {
                KillEntity(ent);
            }
        }

        public void KillAll()
        {
            _entities.Clear();
            transform.DestroyAllChildrenImmediate();
        }
    }
}
