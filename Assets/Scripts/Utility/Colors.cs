using UnityEngine;

namespace Klaesh.Utility
{
    public class Colors
    {
        public static Color Forest = new Color32(14, 178, 14, 255);

        public static Color TileOrigin;
        public static Color TileOccupied;
        public static Color[] TileDistances;

        public static Color[] SquadColors = new Color[] { new Color32(66, 134, 244, 255), new Color32(244, 172, 65, 255) };

        static Colors()
        {
            ColorUtility.TryParseHtmlString("#6D80EC", out TileOrigin);
            ColorUtility.TryParseHtmlString("#EC7B6C", out TileOccupied);

            string[] tileDists = new[] { "#7EEF7B", "#65C364", "#4F984F", "#407B40", "#315E31" };
            TileDistances = new Color[tileDists.Length];
            for (int i = 0; i < tileDists.Length; i++)
                ColorUtility.TryParseHtmlString(tileDists[i], out TileDistances[i]);
        }
    }
}
