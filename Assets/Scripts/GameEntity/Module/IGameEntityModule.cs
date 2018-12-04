
namespace Klaesh.GameEntity.Module
{
    public interface IGameEntityModule
    {
        IGameEntity Owner { get; set; }

        void Init();
    }
}
