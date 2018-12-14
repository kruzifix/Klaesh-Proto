using System;
using System.Collections.Generic;
using System.Linq;
using Klaesh.Core;
using Klaesh.Hex;
using UnityEngine;

namespace Klaesh.GameEntity.Component
{
    public delegate void MovementChangedEvent();

    public class HexMovementComp : MonoBehaviour
    {
        private IHexMap _map;
        private Entity _owner;

        private int _movementLeft;

        public HexCubeCoord Position { get; set; }
        public int MovementLeft
        {
            get { return _movementLeft; }
            set { _movementLeft = value; MovementChanged?.Invoke(); }
        }

        public int maxDistance;
        public int jumpHeight;

        public event MovementChangedEvent MovementChanged;

        private void Awake()
        {
            _map = ServiceLocator.Instance.GetService<IHexMap>();
            _owner = GetComponent<Entity>();
        }

        public void SetPosition(IHexCoord position)
        {
            var tile = _map.GetTile(position);

            Position = position.CubeCoord;
            tile.Entity = _owner;
            transform.position = tile.GetTop();
        }

        public void OnNextTurn()
        {
            MovementLeft = maxDistance;
        }

        public bool CanMoveTo(IHexCoord position, out List<HexTile> path)
        {
            path = null;
            if (MovementLeft <= 0)
                return false;

            var reachable = _map.GetReachableTiles(Position, MovementLeft, jumpHeight);

            var tile = reachable.Where(t => t.Item1.Position == position.CubeCoord).FirstOrDefault();
            if (tile == null)
                return false;
            int requiredMovement = tile.Item2;
            if (requiredMovement > MovementLeft)
                return false;

            path = _map.GetPathTo(tile.Item1, reachable, jumpHeight);

            var targetTile = path.Last();
            if (targetTile.HasEntityOnTop)
                return false;

            return true;
        }
    }
}
