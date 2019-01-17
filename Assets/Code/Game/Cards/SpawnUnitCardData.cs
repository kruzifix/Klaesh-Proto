using UnityEngine;

namespace Klaesh.Game.Cards
{
    [CreateAssetMenu(fileName = "SpawnUnit_", menuName = "Cards/SpawnUnit Card")]
    public class SpawnUnitCardData : ScriptableObject, ICardData
    {
        public string Name;
        public string Description;
        public Sprite Art;

        string ICardData.Name => Name;
        string ICardData.Description => Description;
        public string Type => "Unit";
        Sprite ICardData.Art => Art;
    }
}
