using System;
using System.Collections.Generic;
using System.Linq;
using Klaesh.Core;
using Klaesh.Core.Message;
using Klaesh.GameEntity;
using Klaesh.GameEntity.Module;
using Klaesh.Hex;
using Klaesh.Utility;
using UnityEngine;

namespace Klaesh.Game
{
    public class EntitySelectedInputState : BaseInputState
    {
        private IHexMap _map;
        private List<Tuple<HexTile, int>> _reachableTiles;

        public IGameEntity Entity { get; private set; }

        public EntitySelectedInputState(IGameEntity entity)
        {
            Entity = entity;
        }

        public override void OnEnabled()
        {
            _map = ServiceLocator.Instance.GetService<IHexMap>();

            var hexMod = Entity.GetModule<HexPosModule>();
            var moveMod = Entity.GetModule<MovementModule>();

            _map.DeselectAllTiles();

            var tile = _map.GetTile(hexMod.Position);
            tile.SetColor(Colors.TileOrigin);

            _reachableTiles = _map.GetReachableTiles(hexMod.Position, moveMod.MovementLeft, moveMod.JumpHeight);
            foreach (var t in _reachableTiles)
            {
                t.Item1.SetColor(t.Item1.HasEntityOnTop ? Colors.TileOccupied : Colors.TileDistances[t.Item2 - 1]);
            }

            ServiceLocator.Instance.GetService<IMessageBus>().Publish(new FocusCameraMessage(this, tile.GetTop()));
        }

        public override IInputState OnPickHexTile(HexTile tile)
        {
            if (_reachableTiles.Any(tup => tup.Item1 == tile))
            {
                if (!Entity.GetModule<MovementModule>().TryMoveTo(tile.Position))
                {
                    Debug.LogFormat("[EntitySelected Input] unable to move there.");
                    if (tile.HasEntityOnTop)
                        return OnPickGameEntity(tile.Entity);
                    return null;
                }
                ServiceLocator.Instance.GetService<IMessageBus>().Publish(new FocusCameraMessage(this, tile.GetTop()));
                return new IdleInputState();
            }
            Debug.LogFormat("[EntitySelected Input] can't move there. out of range");

            //ServiceLocator.Instance.GetService<IMessageBus>().Publish(new FocusCameraMessage(this, tile.GetTop()));
            return new IdleInputState();
        }

        public override IInputState OnPickGameEntity(IGameEntity entity)
        {
            var gm = ServiceLocator.Instance.GetService<IGameManager>();
            if (gm.IsPartOfActiveSquad(entity))
            {
                Entity = entity;
                return this;
            }

            // ATTAC other unit!
            return null;
        }
    }
}
