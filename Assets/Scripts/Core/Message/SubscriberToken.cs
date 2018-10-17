using System;

namespace Klaesh.Core.Message
{
    public class SubscriberToken
    {
        private IMessageBus _bus;
        private long _id;
        private Type _type;

        public long Id => _id;
        public Type Type => _type;

        public SubscriberToken(IMessageBus bus, long id, Type type)
        {
            _bus = bus;
            _id = id;
            _type = type;
        }
    }
}
