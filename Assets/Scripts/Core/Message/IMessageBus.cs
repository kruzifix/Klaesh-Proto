using System;

namespace Klaesh.Core.Message
{
    public interface IMessageBus
    {
        SubscriberToken Subscribe<TMessage>(Action<TMessage> deliveryAction) where TMessage : class, IMessage;
        void Unsubscribe(SubscriberToken token);

        void Publish<TMessage>(TMessage message) where TMessage : class, IMessage;

        // publish in late update?
        void PublishLate();
    }
}
