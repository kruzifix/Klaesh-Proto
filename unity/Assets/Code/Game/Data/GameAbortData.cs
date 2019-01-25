using Newtonsoft.Json;

namespace Klaesh.Game.Data
{
    public class GameAbortData
    {
        [JsonProperty("reason")]
        public string Reason { get; set; }
    }
}
