using System;

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

    [Serializable]
    public struct HexOffsetCoord
    {
        // horizontal (Unity X axis)
        public int col;
        // vertical (Unity Z axis)
        public int row;

        public HexOffsetCoord(int col, int row)
        {
            this.col = col;
            this.row = row;
        }

        public override string ToString()
        {
            return string.Format("({0}, {1})", col, row);
        }

        public HexCubeCoord ToCube()
        {
            //var x = hex.col - (hex.row - (hex.row & 1)) / 2
            //var z = hex.row
            //var y = -x - z
            int x = col - (row - (row & 1)) / 2;
            int z = row;
            int y = -x - z;
            return new HexCubeCoord(x, y, z);
        }

        public HexAxialCoord ToAxial()
        {
            int x = col - (row - (row & 1)) / 2;
            int z = row;
            return new HexAxialCoord(x, z);
        }
    }

    public struct HexAxialCoord
    {
        // skewed vertical
        public int q;
        // horizontal (Unity X axis)
        public int r;

        public HexAxialCoord(int q, int r)
        {
            this.q = q;
            this.r = r;
        }

        public override string ToString()
        {
            return string.Format("({0}, {1})", q, r);
        }
    }

    public struct HexCubeCoord
    {
        // skewed vertical (to the left)
        public int x;
        // skewed vertical (to the right)
        public int y;
        // horizontal (Unity X axis)
        public int z;

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

        public HexOffsetCoord ToOffset()
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
            new HexCubeCoord(-1, 1, 0),
            new HexCubeCoord(0, 1, -1),
            new HexCubeCoord(1, 0, -1),
            new HexCubeCoord(1, -1, 0),
            new HexCubeCoord(0, -1, 1),
            new HexCubeCoord(-1, 0, 1)
        };
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
