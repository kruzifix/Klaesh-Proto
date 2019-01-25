using System;
using System.Collections.Generic;
using System.Linq;
using Klaesh.Core;
using Klaesh.Core.Message;
using Klaesh.Utility;

namespace Klaesh.Game.Cards
{
    public interface ICardDeck
    {
        ISquad Owner { get; }
        int MaximumHandSize { get; set; }

        void DrawCard();
        void UseCard(int cardId);
    }

    public struct Card
    {
        public int Id { get; }
        public ICardData Data { get; }

        public Card(int id, ICardData data)
        {
            Id = id;
            Data = data;
        }
    }

    public class CardDeck : ICardDeck
    {
        private IMessageBus _bus;

        private Stack<Card> _drawPile;
        private List<Card> _discardPile;

        private List<Card> _handCards;

        public ISquad Owner { get; }
        public int MaximumHandSize { get; set; }

        public CardDeck(ISquad owner, IEnumerable<ICardData> cards)
        {
            Owner = owner;
            MaximumHandSize = 10;

            _drawPile = new Stack<Card>(cards.OrderBy(_ => NetRand.Next()).Select((c, i) => new Card(i, c)));

            _discardPile = new List<Card>();
            _handCards = new List<Card>();

            _bus = ServiceLocator.Instance.GetService<IMessageBus>();
        }

        public void DrawCard()
        {
            if (_handCards.Count >= MaximumHandSize)
                return;

            if (_drawPile.Count == 0)
            {
                foreach (var c in _discardPile.OrderBy(_ => NetRand.Next()))
                    _drawPile.Push(c);
                _discardPile.Clear();
            }

            var card = _drawPile.Pop();
            _handCards.Add(card);

            _bus.Publish(new CardDrawnMessage(this, card));
        }

        public void UseCard(int cardId)
        {
            var cards = _handCards.Where(c => c.Id == cardId).ToList();
            if (cards.Count != 1)
                throw new Exception("Used card not in hand!");

            var card = cards[0];
            _handCards.Remove(card);
            _discardPile.Add(card);

            _bus.Publish(new CardUsedMessage(this, card));
        }
    }
}
