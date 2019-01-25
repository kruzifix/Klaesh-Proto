using System;
using System.Collections.Generic;
using System.Linq;
using Klaesh.Game.Cards;
using Klaesh.Game.Job;
using Klaesh.GameEntity;
using Klaesh.GameEntity.Component;
using Klaesh.Hex;
using Klaesh.Utility;
using UnityEngine;

namespace Klaesh.Game.Input
{
    public class IdleInputState : AbstractInputState
    {
        private IGameManager _gm;
        private IHexMap _map;

        private List<HexTile> _tiles;

        public IdleInputState(InputStateMachine context)
            : base(context)
        {
            _gm = _locator.GetService<IGameManager>();
            _map = _locator.GetService<IHexMap>();
        }

        public override void Enter()
        {
            _tiles = _gm.ActiveSquad.AliveMembers
                .Select(m => _map.GetTile(m.GetComponent<HexMovementComp>().Position))
                .ToList();
            // highlight active squad units
            _tiles.ForEach(t => t.SetColor(_gm.ActiveSquad.Config.Color));
        }

        public override void Exit()
        {
            _map.DeselectAllTiles();
        }

        public override void ProcessInput(InputCode code, object data)
        {
            if (code == InputCode.AttackMode && data != null && data is Entity)
            {
                Context.SetState(new EntityAttackInputState(Context, data as Entity));
            }
            else if (code == InputCode.Card && data != null && data is Card card)
            {
                if (card.Data is SpawnUnitCardData spawnUnitData)
                {
                    DoSpawnUnit(card, spawnUnitData);
                }
                else if (card.Data is TerraformCardData)
                {
                    Context.SetState(new TerraformInputState(Context, card));
                }
            }
        }

        public override void OnClick(GameObject go)
        {
            ForwardCall<Entity>(go, DoEntity);
            ForwardCall<HexTile>(go, DoHexTile);
        }

        public void DoEntity(Entity entity)
        {
            if (_gm.IsPartOfActiveSquad(entity))
            {
                var moveComp = entity.GetComponent<HexMovementComp>();
                if (moveComp != null && moveComp.MovementLeft > 0)
                {
                    Context.SetState(new EntityMovementInputState(Context, entity));
                    return;
                }
            }
        }

        public void DoHexTile(HexTile tile)
        {
            _bus.Publish(new FocusCameraMessage(this, tile.GetTop()));
            if (tile.HasEntityOnTop)
            {
                DoEntity(tile.Entity);
                return;
            }
        }

        public override void OnEnter(GameObject go)
        {
            ForwardCall<HexTile>(go, tile =>
            {
                if (_tiles.Contains(tile))
                    return;

                tile.SetColor(Colors.TileHighlight);
            });
        }

        public override void OnExit(GameObject go)
        {
            ForwardCall<HexTile>(go, tile =>
            {
                if (_tiles.Contains(tile))
                    return;

                tile.SetColor(Color.white);
            });
        }

        private void DoSpawnUnit(Card card, SpawnUnitCardData data)
        {
            var originCoord = _gm.ActiveSquad.Config.Origin;

            _bus.Publish(new FocusCameraMessage(this, _map.GetTile(originCoord).GetTop()));

            var pickableTiles = _map.Tiles(HexFun.Ring(originCoord));

            var state = new HexPickState(Context, pickableTiles, (tile) =>
            {
                if (tile.HasEntityOnTop)
                    return;
                // spawn unit at that tile
                var job = new SpawnUnitJob
                {
                    Position = tile.Position.OffsetCoord,
                    EntityId = data.EntityId,
                    CardId = card.Id
                };

                ExecuteAndSendJob(job);

                // callback
                Context.SetState(this);
            }, () =>
            {
                Context.SetState(this);
            });
            Context.SetState(state);
        }

        //private void DoTerraForm(Card card, TerraformCardData data)
        //{
        //    // get tiles in range of my units
        //    var tiles = new HashSet<HexCubeCoord>();

        //    var gm = _locator.GetService<IGameManager>();
        //    foreach (var mem in gm.HomeSquad.AliveMembers)
        //    {
        //        var pos = mem.GetComponent<HexPosComp>().Position;

        //        tiles.UnionWith(HexFun.Spiral(pos, 2));
        //    }

        //    var pickableTiles = _map.Tiles(tiles);

        //    var state = new HexPickState(Context, pickableTiles, (tile) =>
        //    {
        //        var job = new TerraformJob
        //        {
        //             Origin = tile.Position,
        //             Changes = data.GetTerrainChanges(),
        //             CardId = card.Id
        //        };

        //        // callback
        //        Context.SetState(new WaitForJobState(Context, job, this));
        //    }, () =>
        //    {
        //        Context.SetState(this);
        //    });
        //    Context.SetState(state);
        //}
    }
}
