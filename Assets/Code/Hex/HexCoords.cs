using System;
using System.Collections.Generic;
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

        //public HexAxialCoord ToAxial()
        //{
        //    int x = col - (row - (row & 1)) / 2;
        //    int z = row;
        //    return new HexAxialCoord(x, z);
        //}
    }

    //public struct HexAxialCoord
    //{
    //    // skewed vertical
    //    public int q;
    //    // horizontal (Unity X axis)
    //    public int r;

    //    public HexAxialCoord(int q, int r)
    //    {
    //        this.q = q;
    //        this.r = r;
    //    }

    //    public override string ToString()
    //    {
    //        return string.Format("({0}, {1})", q, r);
    //    }
    //}

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

        public static int Distance(HexCubeCoord a, HexCubeCoord b)
        {
            return (Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y) + Math.Abs(a.z - b.z)) / 2;
        }

        public static HexCubeCoord[] Offsets = {
            new HexCubeCoord(-1, 1, 0), // west
            new HexCubeCoord(-1, 0, 1), // northwest
            new HexCubeCoord(0, -1, 1), // northeast
            new HexCubeCoord(1, -1, 0), // east
            new HexCubeCoord(1, 0, -1), // southeast
            new HexCubeCoord(0, 1, -1) // southwest
        };

        public static HexCubeCoord Offset(HexDirection dir, int dist = 1)
        {
            return Offsets[(int)dir] * dist;
        }

        public static IEnumerable<IHexCoord> Ring(IHexCoord origin, int radius = 1)
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

        public static IEnumerable<IHexCoord> Spiral(IHexCoord origin, int radius = 1)
        {
            yield return origin.CubeCoord;

            for (int i = 1; i <= radius; i++)
            {
                foreach (var c in Ring(origin, i))
                    yield return c;
            }
        }
    }

    // https://www.redblobgames.com/grids/hexagons/#conversions
    /*
    public static class HexCoordConversions
    {
        public static HexCubeCoord ToCube(this HexOffsetCoord offset)
        {
            //var x = hex.col - (hex.row - (hex.row & 1)) / 2
            //var z = hex.row
            //var y = -x - z
            int x = offset.col - (offset.row - (offset.row & 1)) / 2;
            int z = offset.row;
            int y = -x - z;
            return new HexCubeCoord(x, y, z);
        }

        public static HexAxialCoord ToAxial(this HexOffsetCoord offset)
        {
            int x = offset.col - (offset.row - (offset.row & 1)) / 2;
            int z = offset.row;
            return new HexAxialCoord(x, z);
        }

        public static HexOffsetCoord ToOffset(this HexCubeCoord cube)
        {
            int col = cube.x + (cube.z - (cube.z & 1)) / 2;
            int row = cube.z;
            return new HexOffsetCoord(col, row);
        }

        public static HexAxialCoord ToAxial(this HexCubeCoord cube)
        {
            return new HexAxialCoord(cube.x, cube.z);
        }
    }
    */
}
