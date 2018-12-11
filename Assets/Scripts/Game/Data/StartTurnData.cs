using Newtonsoft.Json;

namespace Klaesh.Game.Data
{
    public class StartTurnData
    {
        [JsonProperty("turn_num")]
        public int TurnNumber { get; set; }

        [JsonProperty("active_squad")]
        public int ActiveSquadIndex { get; set; }
    }
}
