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
        public int Rows { get; set; }
        public int Columns { get; set; }

        public int NoiseOffset { get; set; }
        public float NoiseScale { get; set; }
        public float HeightScale { get; set; }
    }
}
