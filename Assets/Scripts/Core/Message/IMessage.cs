using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klaesh.Core.Message
{
    public interface IMessage
    {
        object Sender { get; }
    }
}
