using Klaesh.Hex;

namespace Klaesh.Game.Config
{
    public interface IUnitConfiguration
    {
        IHexCoord Position { get; }
        string EntityId { get; }
    }

    public class UnitConfiguration : IUnitConfiguration
    {
        public HexOffsetCoord OffsetPos { get; set; }
        public string EntityId { get; set; }

        public IHexCoord Position => OffsetPos;
    }
}
