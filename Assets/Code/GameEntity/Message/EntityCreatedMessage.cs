using Klaesh.Core.Message;

namespace Klaesh.GameEntity.Message
{
    public class EntityCreatedMessage : MessageBase<Entity>
    {
        public EntityCreatedMessage(object sender, Entity value) : base(sender, value)
        {
        }
    }
}
