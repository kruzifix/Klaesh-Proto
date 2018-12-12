using Klaesh.GameEntity;
using Klaesh.Hex;
using UnityEngine;

namespace Klaesh.Game.Input
{
    public interface IInputState
    {
        void OnEnabled();
        void OnDisabled();

        IInputState OnInputActivated(string id, object data);
        IInputState OnPickHexTile(HexTile tile);
        IInputState OnPickGameEntity(Entity entity);
    }

    public abstract class BaseInputState : IInputState
    {
        public virtual void OnDisabled() { }

        public virtual void OnEnabled() { }

        public virtual IInputState OnInputActivated(string id, object data)
        {
            return null;
        }

        public virtual IInputState OnPickHexTile(HexTile tile)
        {
            return null;
        }

        public virtual IInputState OnPickGameEntity(Entity entity)
        {
            return null;
        }
    }

    public class NullInputState : BaseInputState
    {
        private static NullInputState _instance;
        public static NullInputState Instance { get { return _instance ?? (_instance = new NullInputState()); } }

        private NullInputState()
        {
        }
    }
}
