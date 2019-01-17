using Klaesh.Game.Cards;
using TMPro;
using UnityEngine.UI;

namespace Klaesh.UI.Cards
{
    public class CardViewModel : ViewModelBehaviour
    {
        public TextMeshProUGUI Name;
        public TextMeshProUGUI Description;
        public TextMeshProUGUI Type;

        public Image Art;

        private Card _data;

        public Card Data { get => _data; set { _data = value; Refresh(); } }

        private void Refresh()
        {
            Name.text = _data.Data.Name;
            Description.text = _data.Data.Description;
            Type.text = _data.Data.Type.ToString();

            Art.sprite = _data.Data.Art;
        }

        public void OnCardClicked()
        {
            SendMessageUpwards("OnHandCardClicked", this);
        }
    }
}
