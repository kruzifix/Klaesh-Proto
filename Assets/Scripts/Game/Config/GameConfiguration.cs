using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Klaesh.Game.Config
{
    public interface IGameConfiguration
    {
        IHexMapConfiguration Map { get; }
        IList<ISquadConfiguration> Squads { get; }
    }

    public class GameConfiguration : IGameConfiguration
    {
        [JsonProperty("map")]
        public HexMapConfiguration MapConfig { get; set; }
        [JsonProperty("squads")]
        public List<SquadConfiguration> SquadsConfig { get; set; }

        [JsonIgnore]
        public IHexMapConfiguration Map => MapConfig;
        [JsonIgnore]
        public IList<ISquadConfiguration> Squads => SquadsConfig.ToList<ISquadConfiguration>();
    }
}
