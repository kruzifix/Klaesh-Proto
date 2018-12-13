using Klaesh.Game;
using Klaesh.Network;
using UnityEngine;
using UnityEngine.UI;

namespace Klaesh.UI.Window
{
    public class MainWindow : WindowBase
    {
        private INetworker _networker;

        public Button ConnectButton;
        public Text StatusLabel;
        public Text WaitingLabel;

        protected override void Init()
        {
            _networker = _locator.GetService<INetworker>();
            _networker.Connected += OnConnection;

            ConnectButton.gameObject.SetActive(true);
            StatusLabel.gameObject.SetActive(false);
            WaitingLabel.gameObject.SetActive(false);

            AddSubscription(_bus.Subscribe<GameStartedMessage>(OnGameStarted));
        }

        protected override void DeInit()
        {
            _networker.Connected -= OnConnection;
        }

        private void OnGameStarted(GameStartedMessage msg)
        {
            _bus.Publish(new Navigate(this, typeof(GameUIWindow)));
        }

        private void OnConnection()
        {
            StatusLabel.text = "Connected!";
            StatusLabel.color = Color.green;
            WaitingLabel.gameObject.SetActive(true);
        }

        public void OnStartGame()
        {
            // TODO: get from current webgl context/provider?
            // or from config file!
            string url = "ws://localhost:3000";

            _networker.ConnectTo(url);

            ConnectButton.gameObject.SetActive(false);
            StatusLabel.gameObject.SetActive(true);
            WaitingLabel.gameObject.SetActive(false);

            StatusLabel.text = "Connecting to Server";
            StatusLabel.color = Color.white;
        }
    }
}
