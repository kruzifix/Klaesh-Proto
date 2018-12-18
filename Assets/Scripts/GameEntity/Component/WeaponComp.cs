using UnityEngine;

namespace Klaesh.GameEntity.Component
{
    public class WeaponComp : MonoBehaviour, INewTurnHandler
    {
        public int range = 1;
        public int damage = 1;
        public int usesPerTurn = 1;

        public int UsesLeft { get; private set; }

        public void OnNewTurn()
        {
            UsesLeft = usesPerTurn;
        }

        public void Use()
        {
            UsesLeft -= 1;
        }
    }
}
