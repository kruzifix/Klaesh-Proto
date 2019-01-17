using System;
using System.Collections;
using System.Collections.Generic;
using Klaesh.Game;
using Klaesh.Game.Cards;
using Klaesh.Game.Message;
using UnityEngine;

namespace Klaesh.UI.Cards
{
    public class HandCardsViewModel : ViewModelBehaviour
    {
        private IGameManager _gameManager;
        private List<CardViewModel> _cards;

        public CardViewModel CardPrefab;

        protected override void Init()
        {
            _gameManager = _locator.GetService<IGameManager>();

            _cards = new List<CardViewModel>();

            AddSubscription(_bus.Subscribe<CardDrawnMessage>(OnCardDrawn));
            AddSubscription(_bus.Subscribe<CardUsedMessage>(OnCardUsed));
            AddSubscription(_bus.Subscribe<GameAbortedMessage>(OnGameAborted));
        }

        private void OnGameAborted(GameAbortedMessage msg)
        {
            _cards.ForEach(c => Destroy(c.gameObject));
            _cards.Clear();
        }

        private void OnCardDrawn(CardDrawnMessage msg)
        {
            // spawn card
            // TODO: do something with caching here? so gameobject aren't spawned and destroyed all the time

            var deck = msg.Sender as ICardDeck;
            if (deck.Owner == _gameManager.HomeSquad)
            {
                Debug.Log($"[HandCardsViewModel] drew card {msg.Value.Id} {msg.Value.Data.Name}");
                var vm = Instantiate(CardPrefab, transform);
                vm.Data = msg.Value;

                _cards.Add(vm);
            }
        }

        private void OnCardUsed(CardUsedMessage msg)
        {
            var deck = msg.Sender as ICardDeck;
            if (deck.Owner == _gameManager.HomeSquad)
            {
                foreach (var vm in _cards)
                {
                    if (vm.Data.Id == msg.Value.Id)
                    {
                        _cards.Remove(vm);
                        Destroy(vm.gameObject);
                        break;
                    }
                }
            }
        }

        public void OnHandCardClicked(CardViewModel cvm)
        {
            Debug.Log($"[HandCardsViewModel] On Card clicked! {cvm.Data.Id} {cvm.Data.Data.Name}");
        }
    }
}
