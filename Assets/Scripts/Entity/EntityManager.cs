using System;
using System.Collections.Generic;
using System.Linq;
using Klaesh.Core;
using UnityEngine;

namespace Klaesh.Entity
{
    public class EntityManager : MonoBehaviour
    {
        public static EntityManager Instance { get; private set; }

        public GameObject entityPrefab;

        private Dictionary<string, EntityDescriptor> _descriptors;
        private List<Entity> _entities;

        private void Awake()
        {
            ServiceLocator.Instance.RegisterSingleton<EntityManager, EntityManager>(this);
        }

        private void Start()
        {
            _entities = new List<Entity>();
            _descriptors = Resources.LoadAll<EntityDescriptor>("Entities").ToDictionary(d => d.entityId);
        }

        private void OnDestroy()
        {
            ServiceLocator.Instance.DeregisterSingleton<EntityManager>();
        }

        public Entity CreateEntity(string id)
        {
            if (!_descriptors.ContainsKey(id))
                throw new Exception(string.Format("no entity descriptor with id '{0}'", id));

            var desc = _descriptors[id];

            var go = Instantiate(entityPrefab, transform);
            var entity = go.GetComponent<Entity>();
            entity.Initialize(desc);

            _entities.Add(entity);

            return entity;
        }
    }
}
