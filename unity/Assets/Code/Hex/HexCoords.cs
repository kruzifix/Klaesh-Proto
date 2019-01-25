using System;
using System.Collections.Generic;
using System.Linq;
using Klaesh.Utility;
using Newtonsoft.Json;
using UnityEngine;

namespace Klaesh.Hex
{
    // https://www.redblobgames.com/grids/hexagons/#coordinates

    public enum HexDirection
    {
        West,
        NorthWest,
        NorthEast,
        East,
        SouthEast,
        SouthWest
    }

    public interface IHexCoord
    {
        HexCubeCoord CubeCoord { get; }
        HexOffsetCoord OffsetCoord { get; }
    }

    public struct HexOffsetCoord : IHexCoord
    {
        // horizontal (Unity X axis)
        public int col { get; set; } // This has a public setter for JSON serialization! but don't use it elsewhere!
        // vertical (Unity Z axis)
        public int row { get; set; } // This has a public setter for JSON serialization! but don't use it elsewhere!

        [JsonIgnore]
        public HexCubeCoord CubeCoord => ToCube();
        [JsonIgnore]
        public HexOffsetCoord OffsetCoord => this;

        public HexOffsetCoord(int col, int row)
        {
            this.col = col;
            this.row = row;
        }

        public override string ToString()
        {
            return string.Format("({0}, {1})", col, row);
        }

        private HexCubeCoord ToCube()
        {
            //var x = hex.col - (hex.row - (hex.row & 1)) / 2
            //var z = hex.row
            //var y = -x - z
            int x = col - (row - (row & 1)) / 2;
            int z = row;
            int y = -x - z;
            return new HexCubeCoord(x, y, z);
        }
    }

    public struct HexCubeCoord : IHexCoord
    {
        // skewed vertical (to the left)
        public int x { get; set; } // This has a public setter for JSON serialization! but don't use it elsewhere!
        // skewed vertical (to the right)
        public int y { get; set; } // This has a public setter for JSON serialization! but don't use it elsewhere!
        // horizontal (Unity X axis)
        public int z { get; set; } // This has a public setter for JSON serialization! but don't use it elsewhere!

        [JsonIgnore]
        public HexCubeCoord CubeCoord => this;
        [JsonIgnore]
        public HexOffsetCoord OffsetCoord => ToOffset();

        public HexCubeCoord(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}, {2})", x, y, z);
        }

        private HexOffsetCoord ToOffset()
        {
            int col = x + (z - (z & 1)) / 2;
            int row = z;
            return new HexOffsetCoord(col, row);
        }

        public override bool Equals(object obj)
        {
            if (obj is HexCubeCoord)
            {
                return this == (HexCubeCoord)obj;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return new { x, y, z }.GetHashCode();
        }

        #region Operators

        public static HexCubeCoord operator +(HexCubeCoord a, HexCubeCoord b)
        {
            return new HexCubeCoord(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static HexCubeCoord operator *(HexCubeCoord a, int b)
        {
            return new HexCubeCoord(a.x * b, a.y * b, a.z * b);
        }

        public static bool operator ==(HexCubeCoord a, HexCubeCoord b)
        {
            return a.x == b.x && a.y == b.y && a.z == b.z;
        }

        public static bool operator !=(HexCubeCoord a, HexCubeCoord b)
        {
            return !(a == b);
        }

        #endregion
    }

    public static class HexFun
    {
        private static readonly HexCubeCoord[] Offsets = {
            new HexCubeCoord(-1, 1, 0), // west
            new HexCubeCoord(-1, 0, 1), // northwest
            new HexCubeCoord(0, -1, 1), // northeast
            new HexCubeCoord(1, -1, 0), // east
            new HexCubeCoord(1, 0, -1), // southeast
            new HexCubeCoord(0, 1, -1) // southwest
        };

        public static int Distance(HexCubeCoord a, HexCubeCoord b)
        {
            return (Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y) + Math.Abs(a.z - b.z)) / 2;
        }
        
        public static HexCubeCoord Offset(HexDirection dir, int dist = 1)
        {
            return Offsets[(int)dir] * dist;
        }

        public static IEnumerable<HexCubeCoord> Ring(IHexCoord origin, int radius = 1)
        {
            var coord = origin.CubeCoord + Offset(HexDirection.SouthEast, radius);

            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < radius; j++)
                {
                    yield return coord;
                    coord += Offset((HexDirection)i);
                }
            }
        }

        public static IEnumerable<HexCubeCoord> Spiral(IHexCoord origin, int radius = 1)
        {
            yield return origin.CubeCoord;

            for (int i = 1; i <= radius; i++)
            {
                foreach (var c in Ring(origin, i))
                    yield return c;
            }
        }

        public static IEnumerable<HexCubeCoord> Line(IHexCoord start, IHexCoord end)
        {
            var a = start.CubeCoord;
            var b = end.CubeCoord;

            var dist = Distance(a, b);

            var va = a.ToVector3();
            var vb = b.ToVector3() + new Vector3(1e-6f, 1e-6f, 1e-6f);

            for (int i = 0; i <= dist; i++)
            {
                var pos = Vector3.Lerp(va, vb, i * 1f / dist);
                yield return pos.RoundToHex();
            }
        }

        public static ISet<HexCubeCoord> Polymino(IHexCoord origin, int size)
        {
            // OBACHT!
            // use NetRand for now, should replace with some interface thingy, that can be passed as parameter!

            var center = origin.CubeCoord;

            var coords = new HashSet<HexCubeCoord>();
            coords.Add(center);

            int tries = 0;

            while (coords.Count < size)
            {
                // choose hex
                // TODO: add parameter for lengthy/bulky polyminos
                // param=1 => choose hexes farther from origin, param=0 => prefer hexes near origin

                // maybe each hex can only be chosen once / twice?
                center = coords.ElementAt(NetRand.Range(0, coords.Count));

                // add max dist parameter?

                // choose random neighbor, here the lengthy/bulky param could also be used?
                var neighbor = center + Offset(NetRand.Enum<HexDirection>());

                // add neighbor to collection
                coords.Add(neighbor);

                tries++;
            }

            //Debug.Log($"generated polymino of size {size}. took {tries} tries.");

            return coords;
        }

        public static Vector3 ToVector3(this HexCubeCoord coord)
        {
            return new Vector3(coord.x, coord.y, coord.z);
        }

        public static HexCubeCoord RoundToHex(this Vector3 vec)
        {
            var x = Mathf.RoundToInt(vec.x);
            var y = Mathf.RoundToInt(vec.y);
            var z = Mathf.RoundToInt(vec.z);

            var xd = Mathf.Abs(x - vec.x);
            var yd = Mathf.Abs(y - vec.y);
            var zd = Mathf.Abs(z - vec.z);

            if (xd > yd && xd > zd)
                x = -y - z;
            else if (yd > zd)
                y = -x - z;
            else
                z = -x - y;

            return new HexCubeCoord(x, y, z);
        }
    }
}
