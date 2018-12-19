using UnityEngine;

namespace Klaesh.GameEntity.Component
{
    public delegate void HealthChangedEvent();

    public class VitalityComp : MonoBehaviour
    {
        public int maxHealth;

        public event HealthChangedEvent HealthChanged;

        public int Health { get; private set; }

        private void Start()
        {
            Health = maxHealth;

            HealthChanged?.Invoke();
        }

        public void Damage(int dmg)
        {
            Health -= dmg;

            HealthChanged?.Invoke();
        }
    }
}
