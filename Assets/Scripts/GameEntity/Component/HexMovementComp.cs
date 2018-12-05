using Klaesh.Core;
using Klaesh.Game;
using Klaesh.Hex;
using UnityEngine;

namespace Klaesh.GameEntity.Component
{
    public class HexMovementComp : MonoBehaviour
    {
        private IHexMap _map;
        private Entity _owner;

        private bool _firstMove = true;

        public HexCubeCoord Position { get; private set; }
        public int MovementLeft { get; private set; }

        public int maxDistance;
        public int jumpHeight;

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

        //public bool TryMoveTo(IHexCoord position)
        //{
        //    var map = ServiceLocator.Instance.GetService<IHexMap>();
        //    var tile = map.GetTile(position);

        //    if (tile.HasEntityOnTop)
        //        return false;

        //    if (!_firstMove)
        //    {
        //        var oldTile = map.GetTile(Position);
        //        oldTile.Entity = null;
        //    }
        //    _firstMove = false;

        //    Position = position.CubeCoord;
        //    tile.Entity = Owner;
        //    (Owner as Entity).transform.position = tile.GetTop();

        //    return true;
        //}
    }
}
