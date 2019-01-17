using UnityEngine;

namespace Klaesh.Game.Cards
{
    [CreateAssetMenu(fileName = "SpawnUnit_", menuName = "Cards/SpawnUnit Card")]
    public class SpawnUnitCardData : ScriptableObject, ICardData
    {
        [Header("Card Data")]
        public string Name;
        public string Description;
        public Sprite Art;

        [Header("Unit Data")]
        public string EntityId;

        string ICardData.Name => Name;
        string ICardData.Description => Description;
        public string Type => "Unit";
        Sprite ICardData.Art => Art;
    }
}
