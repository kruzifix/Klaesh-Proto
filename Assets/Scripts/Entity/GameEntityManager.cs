using System;
using System.Collections.Generic;
using System.Linq;
using Klaesh.Core;
using UnityEngine;

namespace Klaesh.Entity
{
    public class GameEntityManager : MonoBehaviour
    {
        public static GameEntityManager Instance { get; private set; }

        public GameObject entityPrefab;

        private Dictionary<string, GameEntityDescriptor> _descriptors;
        private List<GameEntity> _entities;

        private void Awake()
        {
            ServiceLocator.Instance.RegisterSingleton<GameEntityManager, GameEntityManager>(this);
        }

        private void Start()
        {
            _entities = new List<GameEntity>();
            _descriptors = Resources.LoadAll<GameEntityDescriptor>("Entities").ToDictionary(d => d.entityId);
        }

        private void OnDestroy()
        {
            ServiceLocator.Instance.DeregisterSingleton<GameEntityManager>();
        }

        public GameEntity CreateEntity(string id)
        {
            if (!_descriptors.ContainsKey(id))
                throw new Exception(string.Format("no entity descriptor with id '{0}'", id));

            var desc = _descriptors[id];

            var go = Instantiate(entityPrefab, transform);
            var entity = go.GetComponent<GameEntity>();
            entity.Initialize(desc);

            _entities.Add(entity);

            return entity;
        }
    }
}
