using Klaesh.Core;
using UnityEngine.UI;

namespace Klaesh.Game.UI
{
    public class GameUI : ManagerBehaviour
    {
        private IGameManager _gameManager;

        public Button EndTurnButton;
        public Text TurnNumberLabel;

        private void Start()
        {
            _gameManager = _locator.GetService<IGameManager>();

            _bus.Subscribe<RefreshGameUIMessage>(OnRefresh);
        }

        private void OnRefresh(RefreshGameUIMessage msg)
        {
            EndTurnButton.gameObject.SetActive(_gameManager.HomeSquadActive);
            TurnNumberLabel.text = $"Turn: {_gameManager.TurnNumber}";
        }

        public void OnEndTurn()
        {
            _gameManager.EndTurn();
        }
    }
}
