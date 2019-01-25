using System.Collections.Generic;
using System.Linq;
using Klaesh.Game.Cards;
using Klaesh.Game.Job;
using Klaesh.GameEntity.Component;
using Klaesh.Hex;
using Klaesh.Utility;
using UnityEngine;

namespace Klaesh.Game.Input
{
    public class TerraformInputState : AbstractInputState
    {
        private IGameManager _gm;
        private IHexMap _map;

        private Card _card;

        private List<TerrainChange> _terrainChanges;
        private List<HexTile> _pickableTiles;

        public TerraformInputState(InputStateMachine context, Card card)
            : base(context)
        {
            _card = card;
            _terrainChanges = (card.Data as TerraformCardData).GetTerrainChanges();

            _gm = _locator.GetService<IGameManager>();
            _map = _locator.GetService<IHexMap>();

            var tiles = new HashSet<HexCubeCoord>();

            var gm = _locator.GetService<IGameManager>();
            foreach (var mem in gm.HomeSquad.AliveMembers)
            {
                var pos = mem.GetComponent<HexPosComp>().Position;

                tiles.UnionWith(HexFun.Spiral(pos, 2));
            }

            _pickableTiles = _map.Tiles(tiles).ToList();
        }

        public override void Enter()
        {
            _pickableTiles.ForEach(t => t.SetColor(Colors.HighlightOrange));
        }

        public override void Exit()
        {
            _map.DeselectAllTiles();
        }

        public override void OnClick(GameObject go)
        {
            if (!ForwardCall<HexTile>(go, DoHexTile))
            {
                Context.SetState(new IdleInputState(Context));
            }
        }

        private void DoHexTile(HexTile tile)
        {
            if (!_pickableTiles.Contains(tile))
            {
                Context.SetState(new IdleInputState(Context));
                return;
            }

            var job = new TerraformJob
            {
                Origin = tile.Position,
                Changes = _terrainChanges,
                CardId = _card.Id
            };

            // callback
            Context.SetState(new WaitForJobState(Context, job, new IdleInputState(Context)));
        }

        public override void OnEnter(GameObject go)
        {
            ForwardCall<HexTile>(go, tile =>
            {
                if (!_pickableTiles.Contains(tile))
                {
                    tile.SetColor(Colors.InValidMovementTarget);
                    return;
                }

                foreach (var c in _terrainChanges)
                {
                    var p = tile.Position + c.coord.CubeCoord;
                    _map.GetTile(p)?.SetColor(c.amount > 0 ? Colors.TerraformRaise : Colors.TerraformLower);
                }
            });
        }

        public override void OnExit(GameObject go)
        {
            ForwardCall<HexTile>(go, tile =>
            {
                if (!_pickableTiles.Contains(tile))
                {
                    tile.SetColor(Color.white);
                    return;
                }

                foreach (var c in _terrainChanges)
                {
                    var p = tile.Position + c.coord.CubeCoord;
                    var t = _map.GetTile(p);
                    if (t != null)
                        t.SetColor(_pickableTiles.Contains(t) ? Colors.HighlightOrange : Color.white);
                }
            });
        }
    }
}
