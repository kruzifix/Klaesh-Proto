using Klaesh.Hex;
using Newtonsoft.Json;

namespace Klaesh.Game.Config
{
    public interface IUnitConfiguration
    {
        IHexCoord Position { get; }
        string EntityId { get; }
    }

    public class UnitConfiguration : IUnitConfiguration
    {
        [JsonProperty("pos")]
        public HexOffsetCoord OffsetPos { get; set; }
        [JsonProperty("id")]
        public string EntityId { get; set; }

        [JsonIgnore]
        public IHexCoord Position => OffsetPos;
    }
}
