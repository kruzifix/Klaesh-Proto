using UnityEngine;

namespace Klaesh.Game.Cards
{
    [CreateAssetMenu(fileName = "Terraform_", menuName = "Cards/Terraform Card")]
    public class TerraformCardData : ScriptableObject, ICardData
    {
        public string Name;
        public string Description;
        public Sprite Art;

        string ICardData.Name => Name;
        string ICardData.Description => Description;
        public string Type => "Terraform";
        Sprite ICardData.Art => Art;
    }
}
