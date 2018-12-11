using Klaesh.Core;
using UnityEngine.UI;

namespace Klaesh.Game.UI
{
    public class GameUI : ManagerBehaviour
    {
        private IGameManager _gameManager;

        public Button EndTurnButton;
        public Text TurnNumberLabel;
        public Text DebugInfoText;

        private void Start()
        {
            _gameManager = _locator.GetService<IGameManager>();

            _bus.Subscribe<RefreshGameUIMessage>(OnRefresh);
        }

        private void OnRefresh(RefreshGameUIMessage msg)
        {
            EndTurnButton.gameObject.SetActive(!_gameManager.TurnEnded && _gameManager.HomeSquadActive);
            TurnNumberLabel.text = $"Turn: {_gameManager.TurnNumber}";

            var config = _gameManager.CurrentConfig;

            DebugInfoText.text = $@"GameId: {config.ServerId}
HomeSquadId: {config.HomeSquadId}
Player1: {config.Squads[0].ServerId}
Player2: {config.Squads[1].ServerId}
RandomSeed: {config.RandomSeed}

TurnEnded: {_gameManager.TurnEnded}
ActiveSquadIndex: {_gameManager.ActiveSquadIndex}
HomeSquadActive: {_gameManager.HomeSquadActive}
";
        }

        public void OnEndTurn()
        {
            _gameManager.EndTurn();
            OnRefresh(null);
        }
    }
}
