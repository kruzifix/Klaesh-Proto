using Klaesh.Core;
using Klaesh.Hex;

namespace Klaesh.Entity.Module
{
    public class HexPosModule : IGameEntityModule
    {
        private bool _firstMove = true;

        public IGameEntity Owner { get; set; }

        public HexCubeCoord Position { get; private set; }

        public HexPosModule()
        {
            Position = new HexCubeCoord();
        }

        public bool TryMoveTo(HexCubeCoord position)
        {
            var map = ServiceLocator.Instance.GetService<IHexMap>();
            var tile = map.GetTile(position);

            if (tile.HasEntityOnTop)
                return false;

            if (!_firstMove)
            {
                var oldTile = map.GetTile(Position.ToOffset());
                oldTile.Entity = null;
            }
            _firstMove = false;

            Position = position;
            tile.Entity = Owner;
            (Owner as GameEntity).transform.position = tile.GetTop();

            return true;
        }
    }
}
