using Klaesh.Core.Message;

namespace Klaesh.Game.Message
{
    public class TurnBoundaryMessage : MessageBase
    {
        /// <summary>
        /// false -> turn ended
        /// </summary>
        public bool TurnStart { get; }

        public TurnBoundaryMessage(object sender, bool start)
            : base(sender)
        {
            TurnStart = start;
        }
    }
}
