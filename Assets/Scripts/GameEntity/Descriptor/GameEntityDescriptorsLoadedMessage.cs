using Klaesh.Core.Message;

namespace Klaesh.GameEntity.Descriptor
{
    public class GameEntityDescriptorsLoadedMessage : MessageBase
    {
        public GameEntityDescriptorsLoadedMessage(object sender)
            : base(sender)
        {
        }
    }
}
