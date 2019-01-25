using Klaesh.GameEntity;
using Klaesh.Hex;
using UnityEngine;

namespace Klaesh.Game.Input
{
    public class SpectateInputState : AbstractInputState
    {
        public SpectateInputState(InputStateMachine context)
            : base(context)
        {
        }

        public override void OnClick(GameObject go)
        {
            ForwardCall<Entity>(go, DoEntity);
            ForwardCall<HexTile>(go, DoHexTile);
        }

        private void DoEntity(Entity entity)
        {
            _bus.Publish(new FocusCameraMessage(this, entity.transform.position));
        }

        private void DoHexTile(HexTile tile)
        {
            _bus.Publish(new FocusCameraMessage(this, tile.GetTop()));
        }
    }
}
