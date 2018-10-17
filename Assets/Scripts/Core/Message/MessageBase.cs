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

    public class GenericMessage<TContent> : MessageBase
    {
        public TContent Content { get; private set; }

        public GenericMessage(object sender, TContent content)
            : base(sender)
        {
            Content = content;
        }
    }
}
