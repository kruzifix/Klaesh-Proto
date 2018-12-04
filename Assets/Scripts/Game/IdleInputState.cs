using Klaesh.Core;
using Klaesh.Core.Message;
using Klaesh.Entity;
using Klaesh.Entity.Module;
using Klaesh.Hex;
using UnityEngine;

namespace Klaesh.Game
{
    public class IdleInputState : BaseInputState
    {
        public override void OnEnabled()
        {
            // highlight active squad units
            var map = ServiceLocator.Instance.GetService<IHexMap>();

            map.DeselectAllTiles();

            var gm = ServiceLocator.Instance.GetService<IGameManager>();
            foreach (var unit in gm.ActiveSquad.Members)
            {
                map.GetTile(unit.GetModule<HexPosModule>().Position).SetColor(gm.ActiveSquad.Config.Color);
            }
        }

        public override IInputState OnPickGameEntity(IGameEntity entity, RaycastHit hit)
        {
            var gm = ServiceLocator.Instance.GetService<IGameManager>();
            if (gm.IsPartOfActiveSquad(entity))
                return new EntitySelectedInputState(entity);

            return null;
        }

        public override IInputState OnPickHexTile(HexTile tile, RaycastHit hit)
        {
            if (tile.HasEntityOnTop)
            {
                return OnPickGameEntity(tile.Entity, hit);
            }

            ServiceLocator.Instance.GetService<IMessageBus>().Publish(new FocusCameraMessage(this, tile.GetTop()));
            return null;
        }
    }
}
