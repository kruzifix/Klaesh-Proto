using Newtonsoft.Json;

namespace Klaesh.Game.Data
{
    public class EndTurnData
    {
        [JsonProperty("turn_num")]
        public int TurnNumber { get; set; }
    }
}
