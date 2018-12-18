using System;
using System.Collections.Generic;
using System.Linq;
using Klaesh.Core;
using Klaesh.Core.Message;
using Klaesh.Game.Job;
using Klaesh.GameEntity;
using Klaesh.GameEntity.Component;
using Klaesh.GameEntity.Module;
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

            // Rechable includes origin tile, we dont want that
            foreach (var tile in _map.Tiles(_navField.Reachable(_moveComp.MovementLeft).Skip(1)))
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

                ExecuteAndSendJob(job);
                return;
            }
            Debug.LogFormat("[EntitySelected Input] can't move there. aborting movement");

            Context.SetState(new IdleInputState(Context));
        }

        public void DoEntity(Entity entity)
        {
            if (entity == Entity)
                return;

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
            var weapon = Entity.GetComponent<WeaponComp>();
            if (weapon == null)
                return;
            // can other be attacked?
            var otherVitality = entity.GetComponent<VitalityComp>();
            if (otherVitality == null)
                return;
            // is it in the enemy squad?
            if (entity.GetModule<SquadMember>() == null)
                return;

            // check range
            var pos = entity.GetComponent<HexPosComp>().Position;
            if (HexCubeCoord.Distance(_moveComp.Position, pos) > weapon.range)
                return;
            if (weapon.UsesLeft <= 0)
                return;

            // TODO: CONSIDER HEIGHT DIFFERENCE!!!

            // ATACK!
            var job = new AttackUnitJob(Entity, entity);
            Context.SetState(new WaitForJobState(Context, job, new IdleInputState(Context)));
            ExecuteAndSendJob(job);
        }

        public override void OnEnter(GameObject go)
        {
            ForwardCall<HexTile>(go, tile =>
            {
                var dist = _navField.GetDistance(tile.Position);
                bool reachable = dist != null && dist <= _moveComp.MovementLeft;
                if (reachable && dist == 0)
                    return;

                tile.SetColor(reachable ? Colors.ValidMovementTarget : Colors.InValidMovementTarget);
            });
        }

        public override void OnExit(GameObject go)
        {
            ForwardCall<HexTile>(go, tile =>
            {
                var dist = _navField.GetDistance(tile.Position);
                bool reachable = dist != null && dist <= _moveComp.MovementLeft;
                if (reachable && dist == 0)
                    return;

                tile.SetColor(reachable ? Colors.TileDistances[dist.Value - 1] : Color.white);
            });
        }
    }
}
