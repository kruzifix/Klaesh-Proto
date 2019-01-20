using UnityEngine;

namespace Klaesh.Game.Cards
{
    [CreateAssetMenu(fileName = "Terraform_", menuName = "Cards/Terraform Card")]
    public class TerraformCardData : ScriptableObject, ICardData
    {
        [Header("Card Data")]
        public string Name;
        public string Description;
        public Sprite Art;

        [Header("Terraform Data")]
        public int amount;

        string ICardData.Name => Name;
        string ICardData.Description => Description;
        public string Type => "Terraform";
        Sprite ICardData.Art => Art;
    }
}
