using System;
using Klaesh.Core;
using Klaesh.Core.Message;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    public static Bootstrap Instance { get; private set; }

    private ServiceManager _serviceLocator;
    private IMessageBus _eventBus;

    private void Awake()
    {
        if (Instance != null)
            throw new Exception("multiple Bootstrap instances!");
        Instance = this;

        Create();
    }

    private void Start()
    {
        Application.targetFrameRate = 60;

        _eventBus = _serviceLocator.GetService<IMessageBus>();


        DontDestroyOnLoad(gameObject);


        //_eventBus.Publish(new ShowLoadingScreenMessage());
    }

    private void Create()
    {
        // load config

        _serviceLocator = new ServiceManager();
        ServiceLocator.Instance = _serviceLocator;
        _serviceLocator.RegisterSingleton<IMessageBus, MessageBus>(new MessageBus());
    }
}
