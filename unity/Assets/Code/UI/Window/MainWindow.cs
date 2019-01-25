using Klaesh.Game.Message;
using Klaesh.Network;
using UnityEngine;
using UnityEngine.UI;

namespace Klaesh.UI.Window
{
    public class MainWindow : WindowBase
    {
        private INetworker _networker;

        [Header("Connect")]
        public GameObject ConnectGroup;
        public InputField IpInput;
        public Button ConnectButton;

        [Header("Status")]
        public GameObject StatusGroup;
        public Text StatusLabel;
        public Text WaitingLabel;

        protected override void Init()
        {
            _networker = _locator.GetService<INetworker>();
            _networker.Connected += OnConnection;

            ConnectGroup.SetActive(true);
            StatusGroup.SetActive(false);

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

        public void OnStartGame(bool ki)
        {
            // TODO: get from current webgl context/provider?
            // or from config file!
            string ip = IpInput.text;
            if (string.IsNullOrWhiteSpace(ip))
                ip = "localhost";
            string url = $"ws://{ip.Trim()}:3000" + (ki ? "?ki" : "");

            _networker.ConnectTo(url);

            ConnectGroup.SetActive(false);
            StatusGroup.SetActive(true);
            WaitingLabel.gameObject.SetActive(false);

            StatusLabel.text = $"Connecting to {url}";
            StatusLabel.color = Color.white;
        }

        public void OnMapGenClick()
        {
            _bus.Publish(new Navigate(this, typeof(MapGenWindow)));
        }
    }
}
