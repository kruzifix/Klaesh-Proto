using Klaesh.Core.Message;

namespace Klaesh.Game.Message
{
    public class GameStartedMessage : MessageBase
    {
        public GameStartedMessage(object sender)
            : base(sender)
        {
        }
    }
}
