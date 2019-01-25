namespace Klaesh.Hex.Navigation
{
    public class HexNavSettings
    {
        public IHexCoord Origin { get; }
        public int MaxHeightDiff { get; }

        public HexNavSettings(IHexCoord origin, int maxHeightDiff = 1)
        {
            Origin = origin;
            MaxHeightDiff = maxHeightDiff;
        }
    }
}
