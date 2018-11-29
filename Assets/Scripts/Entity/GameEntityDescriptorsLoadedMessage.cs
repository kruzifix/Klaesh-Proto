using Klaesh.Core.Message;

namespace Klaesh.Entity
{
    public class GameEntityDescriptorsLoadedMessage : MessageBase
    {
        public GameEntityDescriptorsLoadedMessage(object sender)
            : base(sender)
        {
        }
    }
}
