using Klaesh.Core;
using Klaesh.Game;
using Klaesh.Game.Input;
using Klaesh.Game.Message;
using Klaesh.GameEntity.Component;
using Klaesh.UI;
using Klaesh.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace Klaesh.GameEntity.Widget
{
    public class EntityWidget : ViewModelBehaviour
    {
        private static IGameManager _gm;

        private RectTransform _rt;
        private HexMovementComp _moveComp;
        private VitalityComp _vitalityComp;
        private WeaponComp _weaponComp;

        public Entity Target { get; private set; }

        public Text nameText;

        [Header("Movement")]
        public GameObject movementGroup;
        public Text movementText;

        [Header("Vitality")]
        public GameObject vitalityGroup;
        public Text vitalityText;

        [Header("Weapon")]
        public GameObject weaponGroup;
        public Text weaponInfoText;

        protected override void OnAwake()
        {
            _rt = GetComponent<RectTransform>();

            if (_gm == null)
                _gm = ServiceLocator.Instance.GetService<IGameManager>();
        }

        protected override void Init()
        {
            AddSubscription(_bus.Subscribe<TurnBoundaryMessage>(OnTurnBoundary));
        }

        private void OnTurnBoundary(TurnBoundaryMessage msg)
        {
            UpdateWeapon();
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
                OnMovementChanged();
            }

            _vitalityComp = Target.GetComponent<VitalityComp>();
            vitalityGroup.SetActive(_vitalityComp != null);
            if (_vitalityComp != null)
            {
                _vitalityComp.HealthChanged += OnHealthChanged;
                OnHealthChanged();
            }

            _weaponComp = Target.GetComponent<WeaponComp>();
            weaponGroup.SetActive(false);
            if (_weaponComp != null)
            {
                _weaponComp.StatusChanged += OnWeaponStatusChanged;
                UpdateWeapon();
            }

            nameText.text = Target.name;
        }

        private void OnMovementChanged()
        {
            movementText.text = $"{_moveComp.MovementLeft}/{_moveComp.maxDistance}";
            UpdateWeapon();
        }

        private void OnHealthChanged()
        {
            vitalityText.text =  _vitalityComp.Health > 0 ? $"{_vitalityComp.Health}/{_vitalityComp.maxHealth}" : "dead";
        }

        private void OnWeaponStatusChanged()
        {
            UpdateWeapon();
        }

        private void UpdateWeapon()
        {
            bool showButton = _gm.HomeSquadActive && _gm.IsPartOfActiveSquad(Target) && Target.CanAttack();

            weaponGroup.SetActive(showButton);
            if (showButton)
            {
                weaponInfoText.text = $"U: {_weaponComp.UsesLeft} DMG: {_weaponComp.damage}";
            }
        }

        public void OnWeaponButtonClicked()
        {
            _gm.ProcessInput(InputCode.AttackMode, Target);
        }

        public void Update()
        {
            // move to entity
            var screen = Camera.main.WorldToScreenPoint(Target.transform.position);

            _rt.position = screen;

            float s = Mathk.Map(_rt.position.z, 5f, 20f, 1f, 0.4f);
            _rt.localScale = new Vector3(s, s, s);
        }
    }
}
