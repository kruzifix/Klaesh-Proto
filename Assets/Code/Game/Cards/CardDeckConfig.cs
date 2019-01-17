using System;
using System.Collections.Generic;
using UnityEngine;

namespace Klaesh.Game.Cards
{
    [Serializable]
    public struct CardCount
    {
        public ScriptableObject card;
        public int count;
    }

    [CreateAssetMenu(fileName = "New Deck", menuName = "Cards/Card Deck")]
    public class CardDeckConfig : ScriptableObject
    {
        public CardCount[] cards;

        public IEnumerable<ICardData> GetCards()
        {
            foreach (var c in cards)
            {
                if (c.card is ICardData data)
                {
                    for (int i = 0; i < c.count; i++)
                        yield return data;
                }
            }
        }
    }
}
