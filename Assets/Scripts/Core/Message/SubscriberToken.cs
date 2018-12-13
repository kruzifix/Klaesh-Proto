using System;

namespace Klaesh.Core.Message
{
    public class SubscriberToken
    {
        // the id is probably redundant, as the bus uses ReferenceEquals. ¯\_(ツ)_/¯
        public long Id { get; }

        public Type Type { get; }

        public SubscriberToken(long id, Type type)
        {
            Id = id;
            Type = type;
        }

        public override string ToString()
        {
            return $"{Type.Name}:{Id}";
        }
    }
}
