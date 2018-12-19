using Klaesh.Core;
using Klaesh.Core.Message;
using Klaesh.Hex.Navigation;
using Klaesh.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Klaesh.Hex
{
    [Serializable]
    public class HexMapGenParams
    {
        public int noiseOffset;
        public float noiseScale;
        public float heightScale;
    }

    public interface IHexMap
    {
        int Columns { get; set; }
        int Rows { get; set; }

        HexMapGenParams GenParams { get; }

        int Version { get; }

        void BuildMap();
        void ClearMap();

        HexTile GetTile(int col, int row);
        HexTile GetTile(IHexCoord coord);
        void GetTiles(IEnumerable<IHexCoord> coords, List<HexTile> tiles);

        IEnumerable<HexTile> Tiles(IEnumerable<IHexCoord> coords);

        //List<HexTile> GetNeighbors(IHexCoord origin);
        //List<HexTile> GetNeighbors(IHexCoord origin, int maxDist);
        //void GetNeighbors(IHexCoord origin, int maxDist, List<HexTile> tiles);

        void StateChanged();
        IHexNav GetNav(HexNavSettings settings);

        //List<Tuple<HexTile, int>> GetReachableTiles(IHexCoord origin, int maxDistance, int maxHeightDifference);
        //List<HexTile> GetPathTo(HexTile target, List<Tuple<HexTile, int>> reachableTiles, int maxHeightDifference);

        void DeselectAllTiles();
    }

    public class HexMap : ManagerBehaviour, IHexMap
    {
        private const float Sqrt3 = 1.732f;

        public float cellSize = 1f;
        public int mapColumns = 5;
        public int mapRows = 4;
        public GameObject cellPrefab;

        public HexMapGenParams genParams;

        public float CellWidth => cellSize * 2f;
        public float CellHeight => cellSize * Sqrt3;

        public int Columns
        {
            get { return mapColumns; }
            set { mapColumns = value; }
        }

        public int Rows
        {
            get { return mapRows; }
            set { mapRows = value; }
        }

        public HexMapGenParams GenParams => genParams;

        public int Version { get; private set; } = 0;

        private HexTile[,] _tiles;

        #region Mono-Events

        protected override void OnAwake()
        {
            _locator.RegisterSingleton<IHexMap>(this);
        }

        private void Start()
        {
            ClearMap();
        }

        //private void OnDestroy()
        //{
        //    _locator.DeregisterSingleton<IHexMap>();
        //}

        #endregion

        public void BuildMap()
        {
            ClearMap();

            _tiles = new HexTile[mapRows, mapColumns];

            for (int r = 0; r < mapRows; r++)
            {
                for (int c = 0; c < mapColumns; c++)
                {
#if UNITY_EDITOR
                    var go = (GameObject)PrefabUtility.InstantiatePrefab(cellPrefab);
                    go.transform.SetParent(transform);
#else
                    var go = Instantiate(cellPrefab, transform);
#endif

                    var tile = go.GetComponent<HexTile>();
                    float height = Mathf.PerlinNoise(genParams.noiseOffset + c * genParams.noiseScale, genParams.noiseOffset + r * genParams.noiseScale);
                    tile.Height = 5 + Mathf.CeilToInt(height * genParams.heightScale);
                    tile.Position = new HexOffsetCoord(c, r).CubeCoord;
                    tile.Refresh();

                    float offset = (r % 2) * CellWidth * 0.5f;
                    go.transform.position = new Vector3(c * CellWidth + offset, 0f, r * CellHeight);

                    go.name = string.Format("Cell {0}", tile.Position);

                    _tiles[r, c] = tile;
                }
            }

            StateChanged();

            _bus.Publish(new HexMapInitializedMessage(this, this));
        }

        public void ClearMap()
        {
            _tiles = null;
            transform.DestroyAllChildrenImmediate();

            StateChanged();
        }

        public HexTile GetTile(int col, int row)
        {
            if (col < 0 || row < 0 || col >= mapColumns || row >= mapRows)
                return null;
            return _tiles[row, col];
        }

        public HexTile GetTile(IHexCoord coord)
        {
            var offset = coord.OffsetCoord;
            return GetTile(offset.col, offset.row);
        }

        public void GetTiles(IEnumerable<IHexCoord> coords, List<HexTile> tiles)
        {
            foreach(var c in coords)
            {
                var tile = GetTile(c);
                if (tile != null)
                    tiles.Add(GetTile(c));
            }
        }

        public IEnumerable<HexTile> Tiles(IEnumerable<IHexCoord> coords)
        {
            foreach (var c in coords)
            {
                var tile = GetTile(c);
                if (tile != null)
                    yield return tile;
            }
        }

        public List<HexTile> GetNeighbors(IHexCoord origin)
        {
            return GetNeighbors(origin, 1);
        }

        public List<HexTile> GetNeighbors(IHexCoord origin, int maxDist)
        {
            var tiles = new List<HexTile>();
            GetNeighbors(origin, maxDist, tiles);
            return tiles;
        }

        public void GetNeighbors(IHexCoord origin, int maxDist, List<HexTile> tiles)
        {
            var center = origin.CubeCoord;

            for (int i = 1; i <= maxDist; i++)
            {
                foreach (var offset in HexCubeCoord.Offsets)
                {
                    var c = (center + offset * i);
                    var tile = GetTile(c);
                    if (tile != null)
                        tiles.Add(tile);
                }
            }
        }

        public void StateChanged()
        {
            Version++;
        }

        public IHexNav GetNav(HexNavSettings settings)
        {
            return new HexNav(this, settings);
        }

        //public List<Tuple<HexTile, int>> GetReachableTiles(IHexCoord origin, int maxDistance, int maxHeightDifference)
        //{
        //    var result = new List<Tuple<HexTile, int>>();
        //    var queue = new Queue<HexCubeCoord>();
        //    queue.Enqueue(origin.CubeCoord);

        //    var lookedAt = new Dictionary<HexCubeCoord, int>();
        //    lookedAt.Add(origin.CubeCoord, 0);

        //    while (queue.Count > 0)
        //    {
        //        var current = queue.Dequeue();
        //        var tile = GetTile(current);
        //        if (tile == null)
        //            continue;
        //        int distFromOrigin = lookedAt[current];
        //        if (distFromOrigin > 0)
        //            result.Add(Tuple.Create(tile, distFromOrigin));

        //        distFromOrigin += 1;

        //        if (distFromOrigin > maxDistance)
        //            continue;

        //        var neighbors = GetNeighbors(current);
        //        foreach (var n in neighbors)
        //        {
        //            if (n.HasEntityOnTop)
        //                continue;
        //            int heightDiff = Mathf.Abs(n.Height - tile.Height);
        //            if (heightDiff > maxHeightDifference)
        //                continue;

        //            if (lookedAt.ContainsKey(n.Position))
        //            {
        //                int previousDist = lookedAt[n.Position];
        //                if (previousDist <= distFromOrigin)
        //                    continue;

        //                lookedAt[n.Position] = distFromOrigin;
        //            }
        //            else
        //            {
        //                lookedAt.Add(n.Position, distFromOrigin);
        //            }

        //            queue.Enqueue(n.Position);
        //        }
        //    }

        //    return result;
        //}

        //public List<HexTile> GetPathTo(HexTile target, List<Tuple<HexTile, int>> reachableTiles, int maxHeightDifference)
        //{
        //    var path = new List<HexTile>();

        //    var current = reachableTiles.Where(t => t.Item1 == target).First();
        //    path.Add(current.Item1);

        //    while (current.Item2 > 1)
        //    {
        //        var neighbors = GetNeighbors(current.Item1.Position);

        //        int targetDist = current.Item2 - 1;
        //        current = reachableTiles.Where(t =>
        //            t.Item2 == targetDist &&
        //            Mathf.Abs(t.Item1.Height - current.Item1.Height) <= maxHeightDifference &&
        //            neighbors.Contains(t.Item1)
        //        ).First();
        //        path.Add(current.Item1);
        //    }

        //    path.Reverse();

        //    return path;
        //}

        public void DeselectAllTiles()
        {
            for (int r = 0; r < mapRows; r++)
            {
                for (int c = 0; c < mapColumns; c++)
                {
                    _tiles[r, c].SetColor(Color.white);
                }
            }
        }
    }
}
