using Newtonsoft.Json;

namespace Klaesh.Game.Data
{
    public class SquadEntityRefData
    {
        [JsonProperty("squad")]
        public int SquadId { get; set; }
        [JsonProperty("member")]
        public int MemberId { get; set; }
    }
}
