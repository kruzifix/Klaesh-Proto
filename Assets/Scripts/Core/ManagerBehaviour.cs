using System.Collections;
using System.Collections.Generic;
using Klaesh.Core.Message;
using UnityEngine;

namespace Klaesh.Core
{
    public class ManagerBehaviour : MonoBehaviour
    {
        protected static IServiceLocator _locator;
        protected static IMessageBus _bus;

        private void Awake()
        {
            if (_locator == null)
                _locator = ServiceLocator.Instance;
            if (_bus == null)
                _bus = _locator.GetService<IMessageBus>();

            OnAwake();
        }

        protected virtual void OnAwake() { }
    }
}
