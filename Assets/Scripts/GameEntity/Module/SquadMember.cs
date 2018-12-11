using Klaesh.Game;

namespace Klaesh.GameEntity.Module
{
    public class SquadMember : IEntityModule
    {
        public Entity Owner { get; set; }

        public Squad Squad { get; }
        public int Id { get; }

        public SquadMember(Squad squad, int id)
        {
            Squad = squad;
            Id = id;
        }

        public void Init()
        {
        }
    }
}
