﻿using System;
using System.Collections.Generic;
using System.Linq;
using Klaesh.Core;
using Klaesh.Core.Message;
using Klaesh.Game.Job;
using Klaesh.GameEntity;
using Klaesh.GameEntity.Component;
using Klaesh.Hex;
using Klaesh.Hex.Navigation;
using Klaesh.Network;
using Klaesh.Utility;
using UnityEngine;

namespace Klaesh.Game.Input
{
    public class EntitySelectedInputState : AbstractInputState
    {
        private IGameManager _gm;
        private IHexMap _map;

        private IHexNav _navField;

        public Entity Entity { get; }

        private HexMovementComp _moveComp;

        public EntitySelectedInputState(InputStateMachine context, Entity entity)
            : base(context)
        {
            Entity = entity;
            _moveComp = entity.GetComponent<HexMovementComp>();

            _gm = ServiceLocator.Instance.GetService<IGameManager>();
            _map = ServiceLocator.Instance.GetService<IHexMap>();

            _navField = _map.GetNav(new HexNavSettings(_moveComp.Position, _moveComp.jumpHeight));
        }

        public override void Enter()
        {
            var originTile = _map.GetTile(_navField.Settings.Origin);
            originTile.SetColor(Colors.TileOrigin);

            foreach (var tile in _map.Tiles(_navField.Reachable(_moveComp.MovementLeft)))
            {
                int dist = _navField.GetDistance(tile.Position).Value;
                tile.SetColor(tile.HasEntityOnTop ? Colors.TileOccupied : Colors.TileDistances[dist - 1]);
            }

            ServiceLocator.Instance.GetService<IMessageBus>().Publish(new FocusCameraMessage(this, originTile.GetTop()));
        }

        public override void Exit()
        {
            _map.DeselectAllTiles();
        }

        public override void OnClick(GameObject go)
        {
            ForwardCall<Entity>(go, DoEntity);
            ForwardCall<HexTile>(go, DoHexTile);
        }

        public void DoHexTile(HexTile tile)
        {
            if (tile.HasEntityOnTop)
            {
                Debug.LogFormat("[EntitySelected Input] unable to move there.");
                DoEntity(tile.Entity);
                return;
            }

            var dist = _navField.GetDistance(tile.Position);
            bool reachable = dist != null && dist <= _moveComp.MovementLeft;

            if (reachable)
            {
                // path is reversed!
                var coordPath = _navField.PathToOrigin(tile.Position)
                    .Select(c => c.CubeCoord)
                    .Reverse()
                    .Skip(1)
                    .ToList();
                var job = new MoveUnitJob(Entity, coordPath);

                Context.SetState(new WaitForJobState(Context, job, new IdleInputState(Context)));

                var jm = ServiceLocator.Instance.GetService<IJobManager>();
                jm.AddJob(job);
                jm.ExecuteJobs();
                ServiceLocator.Instance.GetService<INetworker>().SendData(EventCode.DoJob, job);
                return;
            }
            Debug.LogFormat("[EntitySelected Input] can't move there. aborting movement");

            Context.SetState(new IdleInputState(Context));
        }

        public void DoEntity(Entity entity)
        {
            if (_gm.IsPartOfActiveSquad(entity))
            {
                var moveComp = entity.GetComponent<HexMovementComp>();
                if (moveComp != null && moveComp.MovementLeft > 0)
                {
                    Context.SetState(new EntitySelectedInputState(Context, entity));
                    return;
                }
            }

            // ATTAC other unit?
        }
    }
}
