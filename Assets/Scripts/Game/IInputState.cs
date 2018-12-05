using Klaesh.GameEntity;
using Klaesh.Hex;
using UnityEngine;

namespace Klaesh.Game
{
    public interface IInputState
    {
        void OnEnabled();
        void OnDisabled();

        IInputState OnPickHexTile(HexTile tile);
        IInputState OnPickGameEntity(Entity entity);
    }

    public abstract class BaseInputState : IInputState
    {
        public virtual void OnDisabled() { }

        public virtual void OnEnabled() { }

        public virtual IInputState OnPickHexTile(HexTile tile)
        {
            return null;
        }

        public virtual IInputState OnPickGameEntity(Entity entity)
        {
            return null;
        }
    }
}
