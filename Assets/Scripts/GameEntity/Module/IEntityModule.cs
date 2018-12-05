namespace Klaesh.GameEntity.Module
{
    public interface IEntityModule
    {
        Entity Owner { get; set; }

        void Init();
    }
}
