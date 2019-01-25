using System;
using Klaesh.Core;
using Klaesh.Core.Message;
using Klaesh.UI;
using Klaesh.UI.Window;
using Klaesh.Utility;
using UnityEngine;

namespace Klaesh
{
    public class Bootstrap : MonoBehaviour, ICoroutineStarter
    {
        public static Bootstrap Instance { get; private set; }

        private ServiceManager _serviceLocator;
        private IMessageBus _bus;

        private void Awake()
        {
            if (Instance != null)
                throw new Exception("multiple Bootstrap instances!");
            Instance = this;
            Create();
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            Application.targetFrameRate = 60;

            _bus.Publish(new Navigate(this, typeof(MainWindow)));
        }

        private void LateUpdate()
        {
            _bus.DoLatePublish();
        }

        private void Create()
        {
            _serviceLocator = new ServiceManager();
            ServiceLocator.Instance = _serviceLocator;
            _serviceLocator.RegisterSingleton<ICoroutineStarter>(this);
            _bus = new MessageBus();
            _serviceLocator.RegisterSingleton<IMessageBus>(_bus);
            _serviceLocator.RegisterSingleton<IJsonConverter>(new CustomJsonConverter());
        }
    }
}
