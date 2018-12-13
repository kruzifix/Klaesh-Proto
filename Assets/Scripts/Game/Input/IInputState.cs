using Klaesh.GameEntity;
using Klaesh.Hex;

namespace Klaesh.Game.Input
{
    public interface IInputState : IInputProcessor
    {
        InputStateMachine Context { get; }

        void Enter();
        void Exit();

        void ProcessHexTile(HexTile tile);
        void ProcessEntity(Entity entity);
    }

    public abstract class AbstractInputState : IInputState
    {
        public InputStateMachine Context { get; }

        public AbstractInputState(InputStateMachine context)
        {
            Context = context;
        }

        public virtual void Enter() { }
        public virtual void Exit() { }

        public virtual void ProcessInput(InputCode code, object data) { }
        public virtual void ProcessHexTile(HexTile tile) { }
        public virtual void ProcessEntity(Entity entity) { }
    }
}
