using Klaesh.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Klaesh.Input
{
    public class GameInputComponent : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        private static IGameInputManager _manager;

        private void Start()
        {
            if (_manager == null)
                _manager = ServiceLocator.Instance.GetService<IGameInputManager>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _manager.OnEnter(gameObject);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _manager.OnExit(gameObject);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            _manager.OnDown(gameObject);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            // OBACHT! OnPointerUp is triggered on the pointerDown GameObject!
            // use the GameObject from pointerEnter to get drag target
            _manager.OnUp(gameObject);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            _manager.OnClick(gameObject);
        }
    }
}
