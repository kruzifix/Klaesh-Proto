using System.Linq;
using Klaesh.Core;
using Klaesh.Hex;

namespace Klaesh.Entity.Module
{
    public class MovementModule : IGameEntityModule
    {
        public IGameEntity Owner { get; set; }

        public int MovementLeft { get; private set; }

        public void Init()
        {

        }

        public void Reset()
        {
            MovementLeft = Owner.Descriptor.maxDistance;
        }

        public bool TryMoveTo(IHexCoord position)
        {
            if (MovementLeft <= 0)
                return false;

            var hexMod = Owner.GetModule<HexPosModule>();

            var map = ServiceLocator.Instance.GetService<IHexMap>();
            var reachable = map.GetReachableTiles(hexMod.Position, MovementLeft, Owner.Descriptor.jumpHeight);

            var tile = reachable.Where(t => t.Item1.Position == position.CubeCoord).FirstOrDefault();
            if (tile == null)
                return false;
            int requiredMovement = tile.Item2;
            if (requiredMovement > MovementLeft)
                return false;

            if (!hexMod.TryMoveTo(position))
                return false;

            MovementLeft -= requiredMovement;
            return true;
        }
    }
}
