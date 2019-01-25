using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaesh.Core.Message
{
    public class MessageBus : IMessageBus
    {
        private long _tokenId;
        private Dictionary<Type, List<ISubscription>> _subscribers;

        //private List<IMessage> _delayedMessages;

        public MessageBus()
        {
            _tokenId = 0;
            _subscribers = new Dictionary<Type, List<ISubscription>>();
            //_delayedMessages = new List<IMessage>();
        }

        public SubscriberToken Subscribe<TMessage>(Action<TMessage> deliveryAction)
            where TMessage : class, IMessage
        {
            if (deliveryAction == null)
                throw new Exception("no deliveryAction");

            lock (_subscribers)
            {
                List<ISubscription> subs;
                if (!_subscribers.TryGetValue(typeof(TMessage), out subs))
                {
                    subs = new List<ISubscription>();
                    _subscribers[typeof(TMessage)] = subs;
                }

                var token = GetToken(typeof(TMessage));

                subs.Add(new Subscription<TMessage>(token, deliveryAction));

                return token;
            }
        }

        public void Unsubscribe(SubscriberToken token)
        {
            if (token == null)
                throw new Exception("token is null");

            lock (_subscribers)
            {
                List<ISubscription> subs;
                if (!_subscribers.TryGetValue(token.Type, out subs))
                    return;

                subs.RemoveAll(sub => ReferenceEquals(sub.Token, token));
            }
        }

        public void Publish<TMessage>(TMessage message)
            where TMessage : class, IMessage
        {
            if (message == null)
                throw new Exception("message is null");

            ISubscription[] currentSubs;

            lock (_subscribers)
            {
                if (!_subscribers.ContainsKey(typeof(TMessage)))
                    return;

                currentSubs = _subscribers[typeof(TMessage)].ToArray();
            }

            foreach (var sub in currentSubs)
            {
                // should deliver ? => filter!

                try
                {
                    sub.Deliver(message);
                }
                catch (Exception e)
                {
                    Debug.LogErrorFormat("[MessageBus] Exception delivering Message: {0}", e);
                }
            }
        }

        public void PublishLate<TMessage>(TMessage message)
            where TMessage : class, IMessage
        {
            throw new NotImplementedException();

            // TODO: i think the missing type information after saving it as IMessage will be a problem!
            //_delayedMessages.Add(message);
        }

        public void DoLatePublish()
        {
            // TODO: i think the missing type information after saving it as IMessage will be a problem!
            //foreach (var msg in _delayedMessages)
            //{
            //    Publish(msg);
            //}
            //_delayedMessages.Clear();
        }

        private SubscriberToken GetToken(Type type)
        {
            var token = new SubscriberToken(_tokenId, type);
            _tokenId++;
            return token;
        }

        private interface ISubscription
        {
            SubscriberToken Token { get; }
            void Deliver(IMessage message);
        }

        private class Subscription<TMessage> : ISubscription
            where TMessage : class, IMessage
        {
            private SubscriberToken _token;
            private Action<TMessage> _deliveryAction;

            public SubscriberToken Token => _token;

            public Subscription(SubscriberToken token, Action<TMessage> deliveryAction)
            {
                _token = token;
                _deliveryAction = deliveryAction;
            }

            public void Deliver(IMessage message)
            {
                if (!(message is TMessage))
                    throw new Exception("Trying to deliver Message of wrong type!");

                _deliveryAction.Invoke(message as TMessage);
            }
        }
    }
}
