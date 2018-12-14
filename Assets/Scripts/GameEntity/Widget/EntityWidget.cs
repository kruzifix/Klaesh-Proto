using System;
using Klaesh.GameEntity.Component;
using Klaesh.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Klaesh.GameEntity.Widget
{
    public class EntityWidget : ViewModelBehaviour
    {
        private RectTransform _rt;
        private HexMovementComp _moveComp;

        public Entity Target { get; private set; }

        public Text nameText;

        [Header("Movement")]
        public GameObject movementGroup;
        public Text movementText;

        protected override void OnAwake()
        {
            _rt = GetComponent<RectTransform>();
        }

        public void SetTarget(Entity target)
        {
            if (Target != null)
                return;

            Target = target;
            _moveComp = Target.GetComponent<HexMovementComp>();
            movementGroup.SetActive(_moveComp != null);
            if (_moveComp != null)
            {
                _moveComp.MovementChanged += OnMovementChanged;
                movementText.text = $"{_moveComp.MovementLeft}/{_moveComp.maxDistance}";
            }

            Refresh();
        }

        public void Refresh()
        {
            nameText.text = Target.name;
        }

        private void OnMovementChanged()
        {
            movementText.text = $"{_moveComp.MovementLeft}/{_moveComp.maxDistance}";
        }

        public void Update()
        {
            // move to entity
            var screen = Camera.main.WorldToScreenPoint(Target.transform.position);

            _rt.position = screen;
        }
    }
}
