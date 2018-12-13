using System;

namespace Klaesh.Core.Message
{
    public abstract class MessageBase : IMessage
    {
        private WeakReference _sender;

        public object Sender => _sender == null ? null : _sender.Target;

        protected MessageBase(object sender)
        {
            _sender = new WeakReference(sender);
        }
    }

    public class MessageBase<TContent> : MessageBase
    {
        public TContent Value { get; private set; }

        public MessageBase(object sender, TContent value)
            : base(sender)
        {
            Value = value;
        }
    }
}
