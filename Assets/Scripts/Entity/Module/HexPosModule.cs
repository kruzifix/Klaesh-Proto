using Klaesh.Core;
using Klaesh.Hex;

namespace Klaesh.Entity.Module
{
    public class HexPosModule : IGameEntityModule
    {
        private bool _firstMove = true;

        public IGameEntity Owner { get; set; }

        public HexCubeCoord Position { get; private set; }

        public void Init()
        {

        }

        public bool TryMoveTo(IHexCoord position)
        {
            var map = ServiceLocator.Instance.GetService<IHexMap>();
            var tile = map.GetTile(position);

            if (tile.HasEntityOnTop)
                return false;

            if (!_firstMove)
            {
                var oldTile = map.GetTile(Position);
                oldTile.Entity = null;
            }
            _firstMove = false;

            Position = position.CubeCoord;
            tile.Entity = Owner;
            (Owner as GameEntity).transform.position = tile.GetTop();

            return true;
        }
    }
}
