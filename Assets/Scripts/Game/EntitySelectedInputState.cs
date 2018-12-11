using System;
using System.Collections.Generic;
using System.Linq;
using Klaesh.Core;
using Klaesh.Core.Message;
using Klaesh.Game.Data;
using Klaesh.Game.Job;
using Klaesh.GameEntity;
using Klaesh.GameEntity.Component;
using Klaesh.GameEntity.Module;
using Klaesh.Hex;
using Klaesh.Network;
using Klaesh.Utility;
using UnityEngine;

namespace Klaesh.Game
{
    public class EntitySelectedInputState : BaseInputState
    {
        private IHexMap _map;
        private List<Tuple<HexTile, int>> _reachableTiles;

        public Entity Entity { get; private set; }

        public EntitySelectedInputState(Entity entity)
        {
            Entity = entity;
        }

        public override void OnEnabled()
        {
            _map = ServiceLocator.Instance.GetService<IHexMap>();

            var moveComp = Entity.GetComponent<HexMovementComp>();

            _map.DeselectAllTiles();

            var tile = _map.GetTile(moveComp.Position);
            tile.SetColor(Colors.TileOrigin);

            _reachableTiles = _map.GetReachableTiles(moveComp.Position, moveComp.MovementLeft, moveComp.jumpHeight);
            foreach (var t in _reachableTiles)
            {
                t.Item1.SetColor(t.Item1.HasEntityOnTop ? Colors.TileOccupied : Colors.TileDistances[t.Item2 - 1]);
            }

            ServiceLocator.Instance.GetService<IMessageBus>().Publish(new FocusCameraMessage(this, tile.GetTop()));
        }

        public override IInputState OnPickHexTile(HexTile tile)
        {
            if (_reachableTiles.Any(tup => tup.Item1 == tile))
            {
                //if (!Entity.GetComponent<HexMovementComp>().StartMovingTo(tile.Position, () => Debug.Log("arrived!")))
                //{
                //    Debug.LogFormat("[EntitySelected Input] unable to move there.");
                //    if (tile.HasEntityOnTop)
                //        return OnPickGameEntity(tile.Entity);
                //    return null;
                //}

                if (!Entity.GetComponent<HexMovementComp>().CanMoveTo(tile.Position, out List<HexTile> path))
                {
                    Debug.LogFormat("[EntitySelected Input] unable to move there.");
                    if (tile.HasEntityOnTop)
                        return OnPickGameEntity(tile.Entity);
                    return null;
                }

                var coordPath = path.Select(t => t.Position).ToList();
                var job = new MoveUnitJob(Entity, coordPath);

                var jm = ServiceLocator.Instance.GetService<IJobManager>();
                jm.AddJob(job);
                jm.ExecuteJobs();
                ServiceLocator.Instance.GetService<INetworker>().SendData(EventCode.DoJob, job);

                // TEMPORARY
                // TEMPORARY
                // TEMPORARY
                //var sm = Entity.GetModule<SquadMember>();
                //ServiceLocator.Instance.GetService<INetworker>().SendData(EventCode.MoveUnit, new MoveUnitData {
                //    SquadId = sm.Squad.Config.ServerId,
                //    MemberId = sm.Id,
                //    Target = tile.Position
                //});
                // TEMPORARY
                // TEMPORARY
                // TEMPORARY

                //ServiceLocator.Instance.GetService<IMessageBus>().Publish(new FocusCameraMessage(this, tile.GetTop()));
                return new IdleInputState();
            }
            Debug.LogFormat("[EntitySelected Input] can't move there. out of range");

            //ServiceLocator.Instance.GetService<IMessageBus>().Publish(new FocusCameraMessage(this, tile.GetTop()));
            return new IdleInputState();
        }

        public override IInputState OnPickGameEntity(Entity entity)
        {
            var gm = ServiceLocator.Instance.GetService<IGameManager>();
            if (gm.IsPartOfActiveSquad(entity))
            {
                Entity = entity;
                return this;
            }

            // ATTAC other unit!
            return null;
        }
    }
}
