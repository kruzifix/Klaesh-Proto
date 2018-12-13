using Klaesh.Core.Message;

namespace Klaesh.Game.Message
{
    public class GameAbortedMessage : MessageBase<string>
    {
        public GameAbortedMessage(object sender, string value) : base(sender, value)
        {
        }
    }
}
