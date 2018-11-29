using System;
using System.Collections.Generic;
using System.Linq;
using Klaesh.Core;
using UnityEngine;

using HandlerDict = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<Klaesh.IPickHandler>>;

namespace Klaesh
{
    public interface IObjectPicker
    {
        IDictionary<KeyCode, HandlerDict> Handlers { get; }

        bool Enabled { get; set; }

        void RegisterHandler(KeyCode key, string tag, IPickHandler handler);
        void RegisterHandler<T>(KeyCode key, string tag, IPickHandler<T> handler) where T : MonoBehaviour;
        void RegisterHandler<T>(KeyCode key, string tag, Action<T, RaycastHit> handler) where T : MonoBehaviour;
    }

    public class ObjectPicker : MonoBehaviour, IObjectPicker
    {
        private Dictionary<KeyCode, HandlerDict> _handlers;

        public IDictionary<KeyCode, HandlerDict> Handlers => _handlers;

        public bool Enabled { get; set; } = true;

        private void Awake()
        {
            _handlers = new Dictionary<KeyCode, HandlerDict>();
            ServiceLocator.Instance.RegisterSingleton<IObjectPicker, ObjectPicker>(this);
        }

        private void Update()
        {
            if (!Enabled)
                return;
            var pressedKeys = _handlers.Keys.Where(k => Input.GetKeyDown(k));
            if (pressedKeys.Count() > 0)
            {
                // TODO: check that mousePosition is inside screen/limits!
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    string tag = hit.transform.tag;

                    var handlers = pressedKeys.Select(k => _handlers[k]);
                    foreach (var dict in handlers)
                    {
                        if (dict.ContainsKey(tag))
                        {
                            dict[tag].ForEach(h => {
                                if (h.Enabled)
                                    h.OnPick(hit.transform.gameObject, hit);
                                });
                        }
                    }
                }
            }
        }

        private void OnDestroy()
        {
            ServiceLocator.Instance.DeregisterSingleton<IObjectPicker>();
        }

        public void RegisterHandler(KeyCode key, string tag, IPickHandler handler)
        {
            if (!_handlers.ContainsKey(key))
                _handlers.Add(key, new HandlerDict());

            var dict = _handlers[key];
            if (!dict.ContainsKey(tag))
                dict.Add(tag, new List<IPickHandler>());

            var list = dict[tag];
            list.Add(handler);

            handler.Enabled = true;
        }

        public void RegisterHandler<T>(KeyCode key, string tag, IPickHandler<T> handler)
            where T : MonoBehaviour
        {
            RegisterHandler(key, tag, new PickHandlerBridge<T>(handler));
        }

        public void RegisterHandler<T>(KeyCode key, string tag, Action<T, RaycastHit> handler)
            where T : MonoBehaviour
        {
            RegisterHandler(key, tag, new PickHandlerAction<T>(handler));
        }
    }
}
