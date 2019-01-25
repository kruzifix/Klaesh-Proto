using System.Linq;
using Klaesh.Game.Job;
using Klaesh.GameEntity;
using Klaesh.GameEntity.Component;
using Klaesh.Hex;
using Klaesh.Hex.Navigation;
using Klaesh.Utility;
using UnityEngine;

namespace Klaesh.Game.Input
{
    public class EntityMovementInputState : AbstractInputState
    {
        private IGameManager _gm;
        private IHexMap _map;

        private IHexNav _navField;

        private Entity Entity { get; }

        private HexMovementComp _moveComp;

        public EntityMovementInputState(InputStateMachine context, Entity entity)
            : base(context)
        {
            Entity = entity;
            _moveComp = entity.GetComponent<HexMovementComp>();

            _gm = _locator.GetService<IGameManager>();
            _map = _locator.GetService<IHexMap>();

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

            _bus.Publish(new FocusCameraMessage(this, originTile.GetTop()));
        }

        public override void Exit()
        {
            _map.DeselectAllTiles();
        }

        public override void ProcessInput(InputCode code, object data)
        {
            if (code == InputCode.AttackMode && data != null && data is Entity)
            {
                Context.SetState(new EntityAttackInputState(Context, data as Entity));
            }
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
                if (entity.CanMove())
                {
                    Context.SetState(new EntityMovementInputState(Context, entity));
                    return;
                }
            }
        }

        public override void OnEnter(GameObject go)
        {
            ForwardCall<HexTile>(go, tile =>
            {
                var dist = _navField.GetDistance(tile.Position);
                bool reachable = dist != null && dist <= _moveComp.MovementLeft;
                if (reachable && dist == 0)
                    return;

                if (reachable)
                {
                    // highlight path
                    foreach (var c in _navField.PathToOrigin(tile.Position).Reverse().Skip(1))
                    {
                        _map.GetTile(c).SetColor(Colors.ValidMovementTarget);
                    }
                }
                else
                    tile.SetColor(Colors.InValidMovementTarget);
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

                if (reachable)
                {
                    foreach (var c in _navField.PathToOrigin(tile.Position).Reverse().Skip(1))
                    {
                        var t = _map.GetTile(c);
                        int d = _navField.GetDistance(t.Position).Value;
                        t.SetColor(t.HasEntityOnTop ? Colors.TileOccupied : Colors.TileDistances[d - 1]);
                    }
                }
                else
                    tile.SetColor(Color.white);
            });
        }
    }
}
