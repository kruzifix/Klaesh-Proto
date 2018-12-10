using System;
using System.Collections;
using System.Collections.Generic;
using Klaesh.Core;
using Newtonsoft.Json;
using UnityEngine;

namespace Klaesh.Network
{
    public enum EventCode
    {
        GameStart,

        // Ack?
    }

    public delegate void DataReceivedCallback(EventCode eventCode, string data);

    public interface INetworker
    {
        //bool Connected { get; }
        event DataReceivedCallback DataReceived;

        void ConnectTo(string url);

        void SendData(EventCode eventCode, string data);
    }

    public class Networker : ManagerBehaviour, INetworker
    {
        private WebSocket _socket;
        private Coroutine _connectionRoutine;

        //public bool Connected => _socket != null && _socket.isConnected;

        public event DataReceivedCallback DataReceived;

        private IEnumerator DoConnection(string url)
        {
            _socket = new WebSocket(new Uri(url));

            Debug.Log($"[Networker] connecting to server at {url}");

            yield return StartCoroutine(_socket.Connect());

            while (true)
            {
                var received = _socket.RecvString();
                if (received != null)
                {
                    var msgData = JsonConvert.DeserializeObject<MsgData>(received);
                    DataReceived?.Invoke(msgData.code, msgData.data);
                }

                if (_socket.Error != null)
                {
                    Debug.LogError($"[Networker] Error: {_socket.Error}");
                    break;
                }

                yield return null;
            }

            _socket.Close();
            _connectionRoutine = null;
        }

        public void ConnectTo(string url)
        {
            if (_connectionRoutine != null)
                throw new Exception("a connection is already running!");

            _connectionRoutine = StartCoroutine(DoConnection(url));
        }

        public void SendData(EventCode eventCode, string data)
        {
            var msgData = new MsgData();
            msgData.code = eventCode;
            msgData.data = data;

            _socket.SendString(JsonConvert.SerializeObject(msgData));
        }

        protected override void OnAwake()
        {
            _locator.RegisterSingleton<INetworker>(this);
        }

        private class MsgData
        {
            public EventCode code;
            public string data;
        }
    }
}
