using System;
using System.Collections.Generic;
using System.Linq;

namespace Klaesh.Hex.Navigation
{
    public interface IHexNav
    {
        IHexMap Map { get; }
        HexNavSettings Settings { get; }
        bool OutOfDate { get; }

        int? GetDistance(IHexCoord coord);
        IEnumerable<IHexCoord> Reachable(int maxDistance);
        IEnumerable<IHexCoord> PathToOrigin(IHexCoord coord);
    }

    public class HexNav : IHexNav
    {
        private int _mapVersion;
        private int?[,] _field;

        public IHexMap Map { get; }
        public HexNavSettings Settings { get; }
        public bool OutOfDate => Map.Version != _mapVersion;

        public HexNav(IHexMap map, HexNavSettings settings)
        {
            Map = map;
            _mapVersion = Map.Version;

            Settings = settings;

            FloodFill();
        }

        private void FloodFill()
        {
            if (_field != null)
                return;
            _field = new int?[Map.Columns, Map.Rows];

            var frontLine = new Queue<HexCubeCoord>();
            var visited = new HashSet<HexCubeCoord>();

            var origin = Settings.Origin.CubeCoord;
            frontLine.Enqueue(origin);
            visited.Add(origin);
            SetDistance(origin, 0);

            while (frontLine.Count > 0)
            {
                var pos = frontLine.Dequeue();
                var posDist = GetDistance(pos);

                foreach (var tile in Map.Tiles(HexFun.Ring(pos)))
                {
                    if (tile.HasEntityOnTop)
                        continue;
                    var p = tile.Position;
                    if (visited.Contains(p))
                        continue;
                    int heightDiff = Math.Abs(Map.GetTile(pos).Height - tile.Height);
                    if (heightDiff > Settings.MaxHeightDiff)
                        continue;

                    frontLine.Enqueue(p);
                    visited.Add(p);
                    SetDistance(p, posDist + 1);
                }
            }
        }

        private void SetDistance(IHexCoord coord, int? dist)
        {
            var offset = coord.OffsetCoord;
            if (offset.col < 0 || offset.col >= Map.Columns || offset.row < 0 || offset.row >= Map.Rows)
                return;
            _field[offset.col, offset.row] = dist;
        }

        public int? GetDistance(IHexCoord coord)
        {
            // Is des velicht a kle heftig, wenn des do imma abgfrogt wird?
            if (OutOfDate)
                throw new HexNavOutOfDateException(this, _mapVersion);

            var offset = coord.OffsetCoord;
            if (offset.col < 0 || offset.col >= Map.Columns || offset.row < 0 || offset.row >= Map.Rows)
                return null;
            return _field[offset.col, offset.row];
        }

        public IEnumerable<IHexCoord> Reachable(int maxDistance)
        {
            if (OutOfDate)
                throw new HexNavOutOfDateException(this, _mapVersion);

            var frontLine = new Queue<HexCubeCoord>();
            var visited = new HashSet<HexCubeCoord>();

            frontLine.Enqueue(Settings.Origin.CubeCoord);
            visited.Add(Settings.Origin.CubeCoord);

            while (frontLine.Count > 0)
            {
                var pos = frontLine.Dequeue();

                yield return pos;

                foreach (var neighbor in HexFun.Ring(pos))
                {
                    var coord = neighbor.CubeCoord;
                    if (visited.Contains(coord))
                        continue;
                    var dist = GetDistance(neighbor);
                    if (dist == null || dist > maxDistance)
                        continue;

                    frontLine.Enqueue(coord);
                    visited.Add(coord);
                }
            }
        }

        public IEnumerable<IHexCoord> PathToOrigin(IHexCoord coord)
        {
            if (OutOfDate)
                throw new HexNavOutOfDateException(this, _mapVersion);

            var dist = GetDistance(coord);
            if (dist == null)
                yield break;

            yield return coord;

            while (dist > 0)
            {
                foreach (var neighbor in HexFun.Ring(coord))
                {
                    var neighborDist = GetDistance(neighbor);
                    if (neighborDist == null || neighborDist != dist - 1)
                        continue;
                    coord = neighbor;
                    dist = neighborDist;
                    yield return coord;
                    break;
                }
            }
        }
    }

    public class HexNavOutOfDateException : Exception
    {
        public IHexNav Nav { get; }
        public int OriginalVersion { get; }

        public override string Message => $"Usage of HexNav prohibited. HexNav is out of date! Original Version {OriginalVersion}, Current Version: {Nav.Map.Version}";

        public HexNavOutOfDateException(IHexNav nav, int originalVersion)
        {
            Nav = nav;
            OriginalVersion = originalVersion;
        }
    }
}
