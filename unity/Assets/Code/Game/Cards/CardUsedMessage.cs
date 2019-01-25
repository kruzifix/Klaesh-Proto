using Klaesh.Core.Message;

namespace Klaesh.Game.Cards
{
    public class CardUsedMessage : MessageBase<Card>
    {
        public CardUsedMessage(object sender, Card value)
            : base(sender, value)
        {
        }
    }
}
