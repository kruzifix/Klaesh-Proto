using Klaesh;
using Klaesh.Core;
using Klaesh.Core.Message;
using Klaesh.Hex;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HexMap : MonoBehaviour, IPickHandler<HexTile>
{
    private const float Sqrt3 = 1.732f;

    public float cellSize = 1f;
    public int mapColumns = 5;
    public int mapRows = 4;
    public GameObject cellPrefab;

    public float CellWidth => cellSize * 2f;
    public float CellHeight => cellSize * Sqrt3;

    private HexTile[,] _tiles;

    #region Mono-Events

    private void Awake()
    {
        ServiceLocator.Instance.RegisterSingleton<HexMap, HexMap>(this);
    }

    private void Start()
    {
        ClearMap();
        BuildMap();

        ServiceLocator.Instance.GetService<IObjectPicker>().RegisterHandler(KeyCode.Mouse0, "HexTile", this);
        ServiceLocator.Instance.GetService<IMessageBus>().Publish(new HexMapInitializedMessage(this, this));
    }

    private void OnDestroy()
    {
        ServiceLocator.Instance.DeregisterSingleton<HexMap>();
    }

    #endregion

    public void OnPick(HexTile comp, RaycastHit hit)
    {
        var _bus = ServiceLocator.Instance.GetService<IMessageBus>();
        _bus.Publish(new FocusCameraMessage(this, comp.GetTop()));

        DeselectAll();

        comp.SetColor(Color.red);

        var colorStrings = new[] {
            "#299AFF", "#5879BF", "#B6373F", "#E61600",
        };

        var colors = new Color[colorStrings.Length];
        for (int i = 0; i < colors.Length; i++)
            ColorUtility.TryParseHtmlString(colorStrings[i], out colors[i]);

        int maxDist = 2;
        foreach (var n in GetReachableTiles(comp.coord, maxDist, 1))
        {
            n.Item1.SetColor(colors[n.Item2 - 1]);
        }
    }

    public void BuildMap()
    {
        if (_tiles != null)
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
                float noiseScale = 0.5f;
                float height = Mathf.PerlinNoise(c * noiseScale, r * noiseScale);
                tile.height = Mathf.CeilToInt(height * 6f);
                tile.coord = new HexOffsetCoord(c, r).ToCube();
                tile.Refresh();

                float offset = (r % 2) * CellWidth * 0.5f;
                go.transform.position = new Vector3(c * CellWidth + offset, 0f, r * CellHeight);

                go.name = string.Format("Cell {0}", tile.coord);

                _tiles[r, c] = tile;
            }
        }
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

    public HexTile GetTile(HexOffsetCoord coord)
    {
        return GetTile(coord.col, coord.row);
    }

    public List<HexTile> GetNeighbors(HexCubeCoord origin)
    {
        var tiles = new List<HexTile>();

        foreach (var offset in HexCubeCoord.Offsets)
        {
            var c = (origin + offset).ToOffset();
            var tile = GetTile(c);
            if (tile != null)
                tiles.Add(tile);
        }
        return tiles;
    }

    public List<Tuple<HexTile, int>> GetReachableTiles(HexCubeCoord origin, int maxDistance, int maxHeightDifference)
    {
        var result = new List<Tuple<HexTile, int>>();
        var queue = new Queue<HexCubeCoord>();
        queue.Enqueue(origin);

        var lookedAt = new Dictionary<HexCubeCoord, int>();
        lookedAt.Add(origin, 0);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            var tile = GetTile(current.ToOffset());
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
                int heightDiff = Mathf.Abs(n.height - tile.height);
                if (heightDiff > maxHeightDifference)
                    continue;

                if (lookedAt.ContainsKey(n.coord))
                {
                    int previousDist = lookedAt[n.coord];
                    if (previousDist <= distFromOrigin)
                        continue;

                    lookedAt[n.coord] = distFromOrigin;
                }
                else
                {
                    lookedAt.Add(n.coord, distFromOrigin);
                }

                queue.Enqueue(n.coord);
            }
        }

        return result;
    }

    public void DeselectAll()
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
