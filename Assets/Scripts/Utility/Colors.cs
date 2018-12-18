using System;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine;

namespace Klaesh.Utility
{
    public class Colors
    {
        public static Color Forest = new Color32(14, 178, 14, 255);

        public static Color TileOrigin;
        public static Color TileOccupied;
        public static Color[] TileDistances;

        public static Color TileHighlight = new Color32(214, 214, 214, 255);
        public static Color ValidMovementTarget = Color.green;
        public static Color InValidMovementTarget = new Color32(255, 172, 170, 255);

        public static Color[] SquadColors = new Color[] { new Color32(66, 134, 244, 255), new Color32(244, 172, 65, 255) };

        public static Color HighlightOrange = new Color32(255, 185, 94, 255);

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

    public class ColorConverter : JsonConverter
    {
        public override bool CanRead => false;
        public override bool CanWrite => true;

        public override bool CanConvert(Type objectType)
        {
            return typeof(Color).IsEquivalentTo(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var col = (Color)value;

            Action<string, float> w = (string n, float v) => {
                writer.WritePropertyName(n);
                writer.WriteValue(Math.Round(v, 3));
            };

            writer.WriteStartObject();

            w("r", col.r);
            w("g", col.g);
            w("b", col.b);
            w("a", col.a);

            writer.WriteEndObject();
        }
    }
}
