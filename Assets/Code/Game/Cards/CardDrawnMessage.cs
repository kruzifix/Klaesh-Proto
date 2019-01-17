using Klaesh.Core.Message;

namespace Klaesh.Game.Cards
{
    public class CardDrawnMessage : MessageBase<Card>
    {
        public CardDrawnMessage(object sender, Card value)
            : base(sender, value)
        {
        }
    }
}
