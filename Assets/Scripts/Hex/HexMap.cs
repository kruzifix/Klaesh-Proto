using Klaesh.Core;
using Klaesh.Core.Message;
using System;
using System.Collections.Generic;
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

        void BuildMap();

        HexTile GetTile(int col, int row);
        HexTile GetTile(IHexCoord coord);
        List<HexTile> GetNeighbors(IHexCoord origin);
        List<Tuple<HexTile, int>> GetReachableTiles(IHexCoord origin, int maxDistance, int maxHeightDifference);

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

        private HexTile[,] _tiles;

        #region Mono-Events

        protected override void OnAwake()
        {
            _locator.RegisterSingleton<IHexMap, HexMap>(this);
        }

        private void Start()
        {
            ClearMap();
        }

        private void OnDestroy()
        {
            _locator.DeregisterSingleton<IHexMap>();
        }

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
                    tile.Height = Mathf.CeilToInt(height * genParams.heightScale);
                    tile.Position = new HexOffsetCoord(c, r).CubeCoord;
                    tile.Refresh();

                    float offset = (r % 2) * CellWidth * 0.5f;
                    go.transform.position = new Vector3(c * CellWidth + offset, 0f, r * CellHeight);

                    go.name = string.Format("Cell {0}", tile.Position);

                    _tiles[r, c] = tile;
                }
            }

            _bus.Publish(new HexMapInitializedMessage(this, this));
        }

        public void ClearMap()
        {
            _tiles = null;
            var children = new List<GameObject>();
            foreach (Transform t in transform) children.Add(t.gameObject);
            children.ForEach(c => DestroyImmediate(c));
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

        public List<HexTile> GetNeighbors(IHexCoord origin)
        {
            var tiles = new List<HexTile>();

            var center = origin.CubeCoord;

            foreach (var offset in HexCubeCoord.Offsets)
            {
                var c = (center + offset);
                var tile = GetTile(c);
                if (tile != null)
                    tiles.Add(tile);
            }
            return tiles;
        }

        public List<Tuple<HexTile, int>> GetReachableTiles(IHexCoord origin, int maxDistance, int maxHeightDifference)
        {
            var result = new List<Tuple<HexTile, int>>();
            var queue = new Queue<HexCubeCoord>();
            queue.Enqueue(origin.CubeCoord);

            var lookedAt = new Dictionary<HexCubeCoord, int>();
            lookedAt.Add(origin.CubeCoord, 0);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                var tile = GetTile(current);
                if (tile == null)
                    continue;
                int distFromOrigin = lookedAt[current];
                if (distFromOrigin > 0)
                    result.Add(Tuple.Create(tile, distFromOrigin));

                distFromOrigin += 1;

                if (distFromOrigin > maxDistance)
                    continue;

                var neighbors = GetNeighbors(current);
                foreach (var n in neighbors)
                {
                    int heightDiff = Mathf.Abs(n.Height - tile.Height);
                    if (heightDiff > maxHeightDifference)
                        continue;

                    if (lookedAt.ContainsKey(n.Position))
                    {
                        int previousDist = lookedAt[n.Position];
                        if (previousDist <= distFromOrigin)
                            continue;

                        lookedAt[n.Position] = distFromOrigin;
                    }
                    else
                    {
                        lookedAt.Add(n.Position, distFromOrigin);
                    }

                    queue.Enqueue(n.Position);
                }
            }

            return result;
        }

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
