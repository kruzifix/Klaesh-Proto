using System.Collections.Generic;
using Klaesh.Core;
using Klaesh.Core.Message;
using UnityEngine;

namespace Klaesh.UI
{
    public class ViewModelBehaviour : MonoBehaviour
    {
        protected static IServiceLocator _locator;
        protected static IMessageBus _bus;

        private List<SubscriberToken> _tokens;

        private void Awake()
        {
            if (_locator == null)
                _locator = ServiceLocator.Instance;
            if (_bus == null)
                _bus = _locator.GetService<IMessageBus>();

            OnAwake();
        }

        private void OnEnable()
        {
            Init();
        }

        private void OnDisable()
        {
            DeInit();

            if (_tokens != null)
            {
                foreach (var t in _tokens)
                {
                    _bus.Unsubscribe(t);
                    Debug.Log($"[ViewModel] cleaning up subscription {t}");
                }
                _tokens.Clear();
            }
        }

        /// <summary>
        /// Is called in Awake().
        /// </summary>
        protected virtual void OnAwake() { }

        /// <summary>
        /// Is called in OnEnable(). Subscribe to messages here!
        /// </summary>
        protected virtual void Init() { }

        /// <summary>
        /// Is called in OnDisable().
        /// </summary>
        protected virtual void DeInit() { }

        /// <summary>
        /// Subscribe to the bus this way, so that subscriptions can be cleaned up properly.
        /// </summary>
        protected void AddSubscription(SubscriberToken token)
        {
            if (_tokens == null)
                _tokens = new List<SubscriberToken>();
            _tokens.Add(token);
        }
    }
}
