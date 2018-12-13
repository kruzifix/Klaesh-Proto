using Klaesh.Core.Message;

namespace Klaesh.Game
{
    public class GameStartedMessage : MessageBase
    {
        public GameStartedMessage(object sender)
            : base(sender)
        {
        }
    }
}
