using UnityEngine;

namespace Klaesh.GameEntity.Component
{
    public delegate void WeaponStatusChangedEvent();

    public class WeaponComp : MonoBehaviour, INewTurnHandler
    {
        private int _usesLeft;

        public int range = 1;
        public int damage = 1;
        public int usesPerTurn = 1;

        public event WeaponStatusChangedEvent StatusChanged;

        public int UsesLeft { get => _usesLeft; private set { _usesLeft = value; StatusChanged?.Invoke(); } }

        public void OnNewTurn()
        {
            UsesLeft = usesPerTurn;
        }

        public void Use()
        {
            if (UsesLeft > 0)
                UsesLeft -= 1;
        }
    }
}
