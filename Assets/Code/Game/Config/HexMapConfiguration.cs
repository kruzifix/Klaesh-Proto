using Newtonsoft.Json;

namespace Klaesh.Game.Config
{
    public interface IHexMapConfiguration
    {
        int Rows { get; }
        int Columns { get; }

        int NoiseOffset { get; }
        float NoiseScale { get; }
        float HeightScale { get; }
    }

    public class HexMapConfiguration : IHexMapConfiguration
    {
        [JsonProperty("rows")]
        public int Rows { get; set; }
        [JsonProperty("cols")]
        public int Columns { get; set; }

        [JsonProperty("noffset")]
        public int NoiseOffset { get; set; }
        [JsonProperty("nscale")]
        public float NoiseScale { get; set; }
        [JsonProperty("hscale")]
        public float HeightScale { get; set; }
    }
}
