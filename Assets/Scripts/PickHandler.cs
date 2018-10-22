using System;
using UnityEngine;

namespace Klaesh
{
    public interface IPickHandler
    {
        void OnPick(GameObject go, RaycastHit hit);
    }

    public interface IPickHandler<T>
        where T : MonoBehaviour
    {
        void OnPick(T comp, RaycastHit hit);
    }

    internal class PickHandlerBridge<T> : IPickHandler
        where T : MonoBehaviour
    {
        private IPickHandler<T> _handler;

        public PickHandlerBridge(IPickHandler<T> handler)
        {
            _handler = handler;
        }

        public void OnPick(GameObject go, RaycastHit hit)
        {
            var comp = go.GetComponent<T>();
            if (comp == null)
                throw new Exception(string.Format("GameObject has no '{0}' - MonoBehaviour attached", typeof(T).Name));

            _handler.OnPick(comp, hit);
        }
    }

    internal class PickHandlerAction<T> : IPickHandler
        where T : MonoBehaviour
    {
        private Action<T, RaycastHit> _action;

        public PickHandlerAction(Action<T, RaycastHit> action)
        {
            _action = action;
        }

        public void OnPick(GameObject go, RaycastHit hit)
        {
            var comp = go.GetComponent<T>();
            if (comp == null)
                throw new Exception(string.Format("GameObject has no '{0}' - MonoBehaviour attached", typeof(T).Name));

            _action.Invoke(comp, hit);
        }
    }
}
