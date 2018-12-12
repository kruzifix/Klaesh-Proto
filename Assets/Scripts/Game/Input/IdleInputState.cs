using System.Linq;
using Klaesh.Core;
using Klaesh.Core.Message;
using Klaesh.Game.Job;
using Klaesh.GameEntity;
using Klaesh.GameEntity.Component;
using Klaesh.GameEntity.Module;
using Klaesh.Hex;
using Klaesh.Network;
using UnityEngine;

namespace Klaesh.Game.Input
{
    public class IdleInputState : BaseInputState
    {
        public override void OnEnabled()
        {
            // highlight active squad units
            var map = ServiceLocator.Instance.GetService<IHexMap>();

            map.DeselectAllTiles();

            var gm = ServiceLocator.Instance.GetService<IGameManager>();
            foreach (var unit in gm.ActiveSquad.Members)
            {
                map.GetTile(unit.GetComponent<HexMovementComp>().Position).SetColor(gm.ActiveSquad.Config.Color);

                //foreach (var mod in unit.GetModules<MeshModule>().Where(m => m.Name == "WobblyCircle"))
                //{
                //    mod.Enabled = true;
                //}
            }
        }

        public override void OnDisabled()
        {
            //var gm = ServiceLocator.Instance.GetService<IGameManager>();
            //foreach (var unit in gm.ActiveSquad.Members)
            //{
            //    foreach (var mod in unit.GetModules<MeshModule>().Where(m => m.Name == "WobblyCircle"))
            //    {
            //        mod.Enabled = false;
            //    }
            //}
        }

        public override IInputState OnInputActivated(string id, object data)
        {
            switch (id)
            {
                case "recruitUnit":
                    var map = ServiceLocator.Instance.GetService<IHexMap>();
                    var gm = ServiceLocator.Instance.GetService<IGameManager>();

                    //var pickableTiles = map.GetReachableTiles(new HexOffsetCoord(3, 2), 1, 5)
                    //    .Select(t => t.Item1);
                    var pickableTiles = map.GetNeighbors(gm.ActiveSquad.Config.Origin);
                    pickableTiles.Add(map.GetTile(gm.ActiveSquad.Config.Origin));

                    return new HexPickState(pickableTiles, (tile) =>
                    {
                        if (tile.HasEntityOnTop)
                            return null;
                        // spawn unit at that tile
                        var job = new SpawnUnitJob
                        {
                            Position = tile.Position.OffsetCoord,
                            EntityId = "knight"
                        };

                        var jm = ServiceLocator.Instance.GetService<IJobManager>();
                        jm.AddJob(job);
                        jm.ExecuteJobs();
                        ServiceLocator.Instance.GetService<INetworker>().SendData(EventCode.DoJob, job);

                        // callback
                        return new IdleInputState();
                    });

                    //break;
                default:
                    return null;
            }
        }

        public override IInputState OnPickGameEntity(Entity entity)
        {
            var gm = ServiceLocator.Instance.GetService<IGameManager>();
            if (gm.IsPartOfActiveSquad(entity))
                return new EntitySelectedInputState(entity);

            return null;
        }

        public override IInputState OnPickHexTile(HexTile tile)
        {
            if (tile.HasEntityOnTop)
            {
                return OnPickGameEntity(tile.Entity);
            }

            ServiceLocator.Instance.GetService<IMessageBus>().Publish(new FocusCameraMessage(this, tile.GetTop()));
            return null;
        }
    }
}
