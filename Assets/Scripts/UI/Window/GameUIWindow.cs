using Klaesh.Game;
using Klaesh.Game.Input;
using Klaesh.Game.Message;
using UnityEngine.UI;

namespace Klaesh.UI.Window
{
    public class GameUIWindow : WindowBase
    {
        private IGameManager _gameManager;

        public Button EndTurnButton;
        public Text TurnNumberLabel;
        public Text DebugInfoText;

        public Button RecruitButton;

        protected override void Init()
        {
            _gameManager = _locator.GetService<IGameManager>();

            Refresh(false);

            AddSubscription(_bus.Subscribe<GameAbortedMessage>(OnGameAborted));
            AddSubscription(_bus.Subscribe<TurnBoundaryMessage>(OnTurnBoundary));
        }

        private void OnGameAborted(GameAbortedMessage msg)
        {
            Close();
        }

        private void OnTurnBoundary(TurnBoundaryMessage msg)
        {
            bool active = !_gameManager.TurnEnded && _gameManager.HomeSquadActive;
            Refresh(active);
        }

        public void Refresh(bool active)
        {
            EndTurnButton.gameObject.SetActive(active);
            RecruitButton.gameObject.SetActive(active);

            TurnNumberLabel.text = $"Turn: {_gameManager.TurnNumber}";

            bool showDebug = _gameManager.GameConfig != null;
            DebugInfoText.gameObject.SetActive(showDebug);
            if (showDebug)
            {
                var config = _gameManager.GameConfig;

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
        }

        public void OnEndTurn()
        {
            _gameManager.EndTurn();
        }

        public void OnRecruitUnit()
        {
            // TODO: pass unit type here!
            _gameManager.ProcessInput(InputCode.RecruitUnit, null);
        }
    }
}
