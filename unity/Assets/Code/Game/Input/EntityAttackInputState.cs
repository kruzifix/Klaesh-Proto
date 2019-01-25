using System.Collections.Generic;
using Klaesh.Game.Job;
using Klaesh.GameEntity;
using Klaesh.GameEntity.Component;
using Klaesh.Hex;
using Klaesh.Utility;
using UnityEngine;

namespace Klaesh.Game.Input
{
    public class EntityAttackInputState : AbstractInputState
    {
        private IHexMap _map;

        private HashSet<Entity> _possibleTargets;

        private Entity Entity { get; }

        public EntityAttackInputState(InputStateMachine context, Entity entity)
            : base(context)
        {
            Entity = entity;
            _possibleTargets = new HashSet<Entity>(Entity.AttackableEntitiesInRange());

            _map = _locator.GetService<IHexMap>();
        }

        public override void Enter()
        {
            foreach (var ent in _possibleTargets)
            {
                var pos = ent.GetComponent<HexPosComp>().Position;
                _map.GetTile(pos).SetColor(Colors.AttackTargetTileHighlight);
            }

            _bus.Publish(new FocusCameraMessage(this, Entity.transform.position));
        }

        public override void Exit()
        {
            _map.DeselectAllTiles();
        }

        public override void OnClick(GameObject go)
        {
            ForwardCall<HexTile>(go, tile => {
                if (tile.HasEntityOnTop)
                {
                    DoEntity(tile.Entity);
                }
                else
                {
                    Context.SetState(new IdleInputState(Context));
                }
            });
            ForwardCall<Entity>(go, DoEntity);
        }

        private void DoEntity(Entity entity)
        {
            if (_possibleTargets.Contains(entity))
            {
                var job = new AttackUnitJob(Entity, entity);
                Context.SetState(new WaitForJobState(Context, job, new IdleInputState(Context)));
                return;
            }

            var gm = _locator.GetService<IGameManager>();
            if (gm.IsPartOfActiveSquad(entity))
            {
                if (entity.CanMove())
                {
                    Context.SetState(new EntityMovementInputState(Context, entity));
                }
                else
                {
                    Context.SetState(new IdleInputState(Context));
                }
            }
        }

        public override void OnEnter(GameObject go)
        {
            ForwardCall<HexTile>(go, tile =>
            {
                if (tile.HasEntityOnTop && _possibleTargets.Contains(tile.Entity))
                    return;

                tile.SetColor(Colors.TileHighlight);
            });
        }

        public override void OnExit(GameObject go)
        {
            ForwardCall<HexTile>(go, tile =>
            {
                if (tile.HasEntityOnTop && _possibleTargets.Contains(tile.Entity))
                    return;

                tile.SetColor(Color.white);
            });
        }
    }
}
