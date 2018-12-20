using System.Collections.Generic;
using System.Linq;
using Klaesh.Game;
using Klaesh.Game.Config;
using Klaesh.GameEntity;
using Klaesh.GameEntity.Component;
using Klaesh.Hex;
using Klaesh.Hex.Navigation;
using Klaesh.Utility;
using UnityEngine;

namespace Klaesh
{
    public static class MapGenerator
    {
        public static bool Generate(IGameConfiguration game, IHexMap _map, IEntityManager _gem, out List<Squad> _squads)
        {
            _map.Columns = game.Map.Columns;
            _map.Rows = game.Map.Rows;

            _map.GenParams.noiseOffset = game.Map.NoiseOffset;
            _map.GenParams.noiseScale = game.Map.NoiseScale;
            _map.GenParams.heightScale = game.Map.HeightScale;

            _map.BuildMap();

            _squads = new List<Squad>();
            foreach (var config in game.Squads)
            {
                var squad = new Squad(config);
                squad.CreateMembers(_gem);

                _squads.Add(squad);
            }

            var blacklist = new HashSet<HexOffsetCoord>();
            foreach (var s in _squads)
            {
                foreach (var e in s.Members)
                {
                    blacklist.UnionWith(HexCubeCoord.Ring(e.GetComponent<HexPosComp>().Position).Select(c => c.OffsetCoord));
                }
            }

            // Initialize Random with seed!
            NetRand.Seed(game.RandomSeed);

            // hole in the middle
            var center = _map.Center.CubeCoord + HexCubeCoord.Offset(HexDirection.West);
            //for (int i = 0; i < 2; i++)
            {
                var pos = center;// + HexCubeCoord.Offset(NetRand.Enum<HexDirection>(), 1);
                foreach (var c in HexCubeCoord.Spiral(pos))
                    if (NetRand.Chance(7, 10))
                        _map.RemoveTile(c);
            }
            {
                var pos = center + HexCubeCoord.Offset(HexDirection.SouthEast) + HexCubeCoord.Offset(HexDirection.East);
                foreach (var c in HexCubeCoord.Spiral(pos))
                    if (NetRand.Chance(7, 10))
                        _map.RemoveTile(c);
            }

            // cut of corners with no altar
            // hard code this for now
            for (int c = 0; c < _map.Columns; c++)
            {
                for (int r = 0; r < _map.Rows; r++)
                {
                    var coord = new HexOffsetCoord(c, r).CubeCoord;
                    if (Mathf.Abs(coord.x - 2) > NetRand.Range(6, 8))
                        //_map.GetTile(coord)?.SetColor(Color.green);
                        _map.RemoveTile(coord);
                }
            }

            //foreach (var c in blacklist)
            //    _map.GetTile(c)?.SetColor(Color.red);

            // spawn debris on map
            int debrisCount = NetRand.Range(7, 10);
            for (int i = 0; i < debrisCount; i++)
            {
                // find empty position
                var pos = NetRand.HexOffset(_map.Columns, _map.Rows);
                var tile = _map.GetTile(pos);
                while (tile == null || blacklist.Contains(pos) || tile.HasEntityOnTop)
                {
                    pos = NetRand.HexOffset(_map.Columns, _map.Rows);
                    tile = _map.GetTile(pos);
                }

                var deb = _gem.CreateEntity("mountain");
                deb.GetComponent<HexPosComp>().SetPosition(pos);

                foreach (var npos in HexCubeCoord.Ring(pos, 2))
                {
                    var neighbor = _map.GetTile(npos);
                    if (neighbor == null || blacklist.Contains(npos.OffsetCoord) || neighbor.HasEntityOnTop)
                        continue;
                    if (NetRand.Chance(3, 10))
                    {
                        var ndeb = _gem.CreateEntity("mountain");
                        ndeb.GetComponent<HexPosComp>().SetPosition(npos);
                    }
                }
            }

            // generate forest patches
            int treeCount = NetRand.Range(7, 16);
            for (int i = 0; i < debrisCount; i++)
            {
                var pos = NetRand.HexOffset(_map.Columns, _map.Rows);
                var tile = _map.GetTile(pos);
                while (tile == null || tile.Terrain != HexTerrain.Plain || tile.HasEntityOnTop)
                {
                    pos = NetRand.HexOffset(_map.Columns, _map.Rows);
                    tile = _map.GetTile(pos);
                }

                tile.Terrain = HexTerrain.Forest;

                foreach (var npos in HexCubeCoord.Ring(pos))
                {
                    var neighbor = _map.GetTile(npos);
                    if (neighbor == null || neighbor.Terrain != HexTerrain.Plain || neighbor.HasEntityOnTop)
                        continue;
                    if (NetRand.Chance(3, 10))
                    {
                        neighbor.Terrain = HexTerrain.Forest;
                    }
                }
            }

            // check that there is a path from base to base
            var altarPos = _squads[0].Members[0].GetComponent<HexPosComp>().Position;
            var nav = _map.GetNav(new HexNavSettings(altarPos));

            var otherAltar = _squads[1].Members[0].GetComponent<HexPosComp>().Position;

            var target = otherAltar;
            int minDist = int.MaxValue;
            foreach (var n in HexCubeCoord.Ring(otherAltar))
            {
                var dist = nav.GetDistance(n);
                if (dist == null)
                {
                    Debug.Log("NO PATH BETWEEN ALTARS!");
                    return false;
                }
                if (dist.Value < minDist)
                {
                    minDist = dist.Value;
                    target = n.CubeCoord;
                }
            }

            //var path = nav.PathToOrigin(target).ToList();
            //foreach (var c in path)
            //{
            //    _map.GetTile(c).SetColor(new Color32(163, 198, 255, 255));
            //}

            return true;
        }
    }
}
