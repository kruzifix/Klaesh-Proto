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

        public Button RecruitButton;
        public Button AbortButton;

        private void Start()
        {
            _gameManager = _locator.GetService<IGameManager>();

            _bus.Subscribe<RefreshGameUIMessage>(OnRefresh);
        }

        private void OnRefresh(RefreshGameUIMessage msg)
        {
            bool active = !_gameManager.TurnEnded && _gameManager.HomeSquadActive;
            EndTurnButton.gameObject.SetActive(active);
            RecruitButton.gameObject.SetActive(active);
            AbortButton.gameObject.SetActive(active);

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

        public void OnAbortInput()
        {
            _gameManager.ActivateInput("abort");
        }

        public void OnRecruitUnit()
        {
            // TODO: pass unit type here!
            _gameManager.ActivateInput("recruitUnit");
        }
    }
}
