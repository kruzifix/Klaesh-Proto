using Klaesh.Core.Message;

namespace Klaesh.GameEntity.Message
{
    public class EntityKilledMessage : MessageBase<Entity>
    {
        public EntityKilledMessage(object sender, Entity value) : base(sender, value)
        {
        }
    }
}
