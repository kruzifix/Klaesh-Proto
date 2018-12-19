using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Klaesh.Game.Config
{
    public interface IGameConfiguration
    {
        int ServerId { get; }
        IHexMapConfiguration Map { get; }
        IList<ISquadConfiguration> Squads { get; }
        int HomeSquadId { get; }
        int RandomSeed { get; }
    }

    public class GameConfiguration : IGameConfiguration
    {
        [JsonProperty("id")]
        public int ServerId { get; set; }
        [JsonProperty("map")]
        public HexMapConfiguration MapConfig { get; set; }
        [JsonProperty("squads")]
        public List<SquadConfiguration> SquadsConfig { get; set; }
        [JsonProperty("home_squad")]
        public int HomeSquadId { get; set; }
        [JsonProperty("random_seed")]
        public int RandomSeed { get; set; }

        [JsonIgnore]
        public IHexMapConfiguration Map => MapConfig;
        [JsonIgnore]
        public IList<ISquadConfiguration> Squads => SquadsConfig.ToList<ISquadConfiguration>();
    }
}
