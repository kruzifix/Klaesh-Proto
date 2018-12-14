using System;
using System.Collections.Generic;
using System.Linq;
using Klaesh.Core;
using Klaesh.Core.Message;
using Klaesh.Game.Job;
using Klaesh.GameEntity;
using Klaesh.GameEntity.Component;
using Klaesh.Hex;
using Klaesh.Network;
using Klaesh.Utility;
using UnityEngine;

namespace Klaesh.Game.Input
{
    public class EntitySelectedInputState : AbstractInputState
    {
        private IGameManager _gm;
        private IHexMap _map;

        private List<Tuple<HexTile, int>> _reachableTiles;

        public Entity Entity { get; private set; }

        public EntitySelectedInputState(InputStateMachine context, Entity entity)
            : base(context)
        {
            Entity = entity;

            _gm = ServiceLocator.Instance.GetService<IGameManager>();
            _map = ServiceLocator.Instance.GetService<IHexMap>();
        }

        public override void Enter()
        {
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

        public override void ProcessHexTile(HexTile tile)
        {
            if (_reachableTiles.Any(tup => tup.Item1 == tile))
            {
                if (!Entity.GetComponent<HexMovementComp>().CanMoveTo(tile.Position, out List<HexTile> path))
                {
                    Debug.LogFormat("[EntitySelected Input] unable to move there.");
                    if (tile.HasEntityOnTop)
                        ProcessEntity(tile.Entity);
                    return;
                }

                var coordPath = path.Select(t => t.Position).ToList();
                var job = new MoveUnitJob(Entity, coordPath);

                var jm = ServiceLocator.Instance.GetService<IJobManager>();
                jm.AddJob(job);
                jm.ExecuteJobs();
                ServiceLocator.Instance.GetService<INetworker>().SendData(EventCode.DoJob, job);

                Context.SetState(new IdleInputState(Context));
            }
            Debug.LogFormat("[EntitySelected Input] can't move there. out of range");

            Context.SetState(new IdleInputState(Context));
        }

        public override void ProcessEntity(Entity entity)
        {
            if (_gm.IsPartOfActiveSquad(entity))
            {
                var moveComp = entity.GetComponent<HexMovementComp>();
                if (moveComp != null && moveComp.MovementLeft > 0)
                {
                    Exit();
                    Entity = entity;
                    Enter();
                    return;
                }
            }

            // ATTAC other unit!
        }
    }
}
