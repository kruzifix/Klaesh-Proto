using System;
using Klaesh.Hex;

namespace Klaesh.Utility
{
    public static class NetRand
    {
        private static int _seed;
        private static Random _rand;

        public static void Seed(int seed)
        {
            _seed = seed;
            _rand = new Random(seed);
        }

        public static int Next()
        {
            return _rand.Next();
        }

        /// <summary>
        /// Returns random integer in range [min, max[
        /// </summary>
        public static int Range(int min, int max)
        {
            return _rand.Next(min, max);
        }

        // OBACHT! isch des denn wirklich deterministisch? mit verschiedene architekturen und so
        public static float Range(float min, float max)
        {
            return min + (max - min) * (float)_rand.NextDouble();
        }

        public static HexOffsetCoord HexOffset(int maxCol, int maxRow)
        {
            int c = Range(0, maxCol);
            int r = Range(0, maxRow);
            return new HexOffsetCoord(c, r);
        }

        /// <summary>
        /// Returns true in probability / precision cases.
        /// Example: Chance(3, 10) => 3 in 10 => 30%
        /// </summary>
        public static bool Chance(int probability, int precision)
        {
            return probability < Range(0, precision);
        }
    }
}
