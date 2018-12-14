using System.Collections;
using System.Collections.Generic;
using Klaesh.GameEntity.Module;
using Klaesh.Game.Config;
using Klaesh.GameEntity;
using Klaesh.Hex;
using UnityEngine;
using Klaesh.GameEntity.Component;
using Klaesh.Core;
using Klaesh.Utility;

namespace Klaesh.Game
{
    public interface ISquad
    {
        ISquadConfiguration Config { get; }

        List<Entity> Members { get; }

        void CreateMembers(IEntityManager man);
        void AddMember(IEntityManager gem, IHexCoord position, string entityId);
    }

    public class Squad : ISquad
    {
        private int _number;

        public ISquadConfiguration Config { get; private set; }

        public List<Entity> Members { get; private set; }

        public Squad(ISquadConfiguration config)
        {
            Config = config;
        }

        public void CreateMembers(IEntityManager gem)
        {
            if (Members != null)
                return;

            Members = new List<Entity>();
            _number = 0;

            foreach (var unit in Config.Units)
            {
                AddMember(gem, Config.Origin.CubeCoord + unit.Position.CubeCoord, unit.EntityId);
            }
        }

        public void AddMember(IEntityManager gem, IHexCoord position, string entityId)
        {
            var ent = gem.CreateEntity(entityId, e => {
                e.AddModule(new SquadMember(this, _number));
            });

            ent.GetComponent<HexMovementComp>().SetPosition(position);
            ent.GetComponent<ModelColorizer>().Colorize(Config.Color);

            var map = ServiceLocator.Instance.GetService<IHexMap>();
            var center = map.GetTile(map.Columns / 2, map.Rows / 2);
            var dir = center.GetTop() - ent.transform.position;
            dir.y = 0;
            ent.transform.rotation = Quaternion.LookRotation(dir);

            Members.Add(ent);
            _number++;
        }
    }
}
