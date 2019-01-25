using Klaesh.Game;
using Klaesh.Game.Data;

namespace Klaesh.GameEntity.Module
{
    public class SquadMember : IEntityModule
    {
        public Entity Owner { get; set; }

        public Squad Squad { get; }
        public int Id { get; }

        public SquadEntityRefData RefData { get; }

        public SquadMember(Squad squad, int id)
        {
            Squad = squad;
            Id = id;

            RefData = new SquadEntityRefData { SquadId = squad.Config.ServerId, MemberId = id };
        }

        public void Init()
        {
        }
    }
}
