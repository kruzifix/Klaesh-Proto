using System;
using System.Collections;
using System.Collections.Generic;
using Klaesh.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Klaesh
{
    public interface IGameInputComponent
    {
        void RegisterHandler(string tag, Action<GameObject> handler);
        void RegisterHandler<T>(string tag, Action<T> handler);
    }

    public class GameInputComponent : ManagerBehaviour, IGameInputComponent, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler
    {
        private Dictionary<string, List<Action<GameObject>>> _handlers;

        protected override void OnAwake()
        {
            _handlers = new Dictionary<string, List<Action<GameObject>>>();
            _locator.RegisterSingleton<IGameInputComponent>(this);
        }

        public void RegisterHandler(string tag, Action<GameObject> handler)
        {
            if (!_handlers.ContainsKey(tag))
                _handlers.Add(tag, new List<Action<GameObject>>());
            var list = _handlers[tag];

            list.Add(handler);
        }

        public void RegisterHandler<T>(string tag, Action<T> handler)
        {
            RegisterHandler(tag, obj =>
            {
                var comp = obj.GetComponent<T>();
                if (comp != null)
                {
                    handler(comp);
                }
            });
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                Debug.Log($"POINTER DOWN {eventData.pointerEnter} {eventData.pointerPress?.name}");
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                Debug.Log($"POINTER UP {eventData.pointerEnter} {eventData.pointerPress?.name}");

                if (eventData.pointerEnter != null && eventData.rawPointerPress != null && eventData.pointerEnter == eventData.rawPointerPress)
                {
                    var tag = eventData.pointerEnter.tag;

                    if (!_handlers.ContainsKey(tag))
                        return;

                    foreach (var handler in _handlers[tag])
                    {
                        handler(eventData.pointerEnter);
                    }
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                Debug.Log($"POINTER ENTER {eventData.pointerEnter} {eventData.pointerPress?.name}");
            }
        }
    }
}
