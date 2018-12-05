using System.Collections;
using System.Collections.Generic;
using Klaesh.GameEntity.Module;
using Klaesh.Game.Config;
using Klaesh.GameEntity;
using Klaesh.Hex;
using UnityEngine;
using Klaesh.GameEntity.Component;
using Klaesh.Core;

namespace Klaesh.Game
{
    public interface ISquad
    {
        ISquadConfiguration Config { get; }

        List<Entity> Members { get; }

        void CreateMembers(IEntityManager man);
    }

    public class Squad : ISquad
    {
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

            foreach (var unit in Config.Units)
            {
                var ent = gem.CreateEntity(unit.EntityId/*, e => e.AddModule(this)*/);

                ent.GetComponent<HexMovementComp>().SetPosition(Config.Origin.CubeCoord + unit.Position.CubeCoord);

                var map = ServiceLocator.Instance.GetService<IHexMap>();
                var center = map.GetTile(map.Columns / 2, map.Rows / 2);
                ent.transform.rotation = Quaternion.LookRotation(center.GetTop() - ent.transform.position);

                Members.Add(ent);
            }
        }
    }
}
