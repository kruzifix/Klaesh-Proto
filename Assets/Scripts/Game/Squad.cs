﻿using System.Collections;
using System.Collections.Generic;
using Klaesh.Entity;
using Klaesh.Entity.Module;
using Klaesh.Game.Config;
using Klaesh.Hex;
using UnityEngine;

namespace Klaesh.Game
{
    public interface ISquad
    {
        ISquadConfiguration Config { get; }

        List<IGameEntity> Members { get; }

        void CreateMembers(IGameEntityManager man);
    }

    public class Squad : ISquad
    {
        public ISquadConfiguration Config { get; private set; }

        public List<IGameEntity> Members { get; private set; }

        public Squad(ISquadConfiguration config)
        {
            Config = config;
        }

        public void CreateMembers(IGameEntityManager gem)
        {
            if (Members != null)
                return;

            Members = new List<IGameEntity>();

            foreach (var unit in Config.Units)
            {
                var ent = gem.CreateEntity(unit.EntityId);
                ent.AddModule(this);

                ent.InitModules();

                ent.GetModule<HexPosModule>().TryMoveTo(Config.Origin.CubeCoord + unit.Position.CubeCoord);

                Members.Add(ent);
            }
        }
    }
}
