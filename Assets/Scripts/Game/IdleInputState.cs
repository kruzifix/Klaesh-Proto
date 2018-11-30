using Klaesh.Core;
using Klaesh.Core.Message;
using Klaesh.Entity;
using Klaesh.Hex;
using UnityEngine;

namespace Klaesh.Game
{
    public class IdleInputState : BaseInputState
    {
        public override IInputState OnPickGameEntity(GameEntity entity, RaycastHit hit)
        {
            return new EntitySelectedInputState(entity);
        }

        public override IInputState OnPickHexTile(HexTile tile, RaycastHit hit)
        {
            ServiceLocator.Instance.GetService<IMessageBus>().Publish(new FocusCameraMessage(this, tile.GetTop()));

            return base.OnPickHexTile(tile, hit);
        }
    }
}
