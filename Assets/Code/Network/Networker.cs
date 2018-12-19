using System;
using System.Collections;
using System.Collections.Generic;
using Klaesh.Core;
using Klaesh.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace Klaesh.Network
{
    //[JsonConverter(typeof(StringEnumConverter))]
    public enum EventCode
    {
        //HeartBeat,
        GameStart,
        GameAbort,

        StartTurn,
        EndTurn,

        DoJob

        // Ack?
    }

    public delegate void DataReceivedCallback(EventCode eventCode, string data);
    public delegate void ConnectCallback();

    public interface INetworker
    {
        //bool Connected { get; }
        event DataReceivedCallback DataReceived;
        event ConnectCallback Connected;

        void ConnectTo(string url);
        void Disconnect();

        void SendData(EventCode eventCode, object data);
    }

    public class Networker : ManagerBehaviour, INetworker
    {
        private IJsonConverter _converter;

        private bool _keepAlive;
        private WebSocket _socket;
        private Coroutine _connectionRoutine;

        //public bool Connected => _socket != null && _socket.isConnected;

        public event DataReceivedCallback DataReceived;
        public event ConnectCallback Connected;

        private IEnumerator DoConnection(string url)
        {
            _socket = new WebSocket(new Uri(url));

            Debug.Log($"[Networker] connecting to server at {url}");

            yield return StartCoroutine(_socket.Connect());

            if (_socket.Error != null)
            {
                Debug.LogError($"[Networker] Error: {_socket.Error}");

                _socket.Close();
                _socket = null;
                _connectionRoutine = null;
                yield break;
            }

            Connected?.Invoke();

            _keepAlive = true;
            while (_keepAlive)
            {
                var received = _socket.RecvString();
                if (received != null)
                {
                    Debug.Log($"[Networker] received data: {received}");
                    var msgData = JsonConvert.DeserializeObject<MsgData>(received);
                    Debug.Log($"[Networker] parsed to: {msgData.code} {msgData.data}");
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
            _socket = null;
            _connectionRoutine = null;
        }

        public void ConnectTo(string url)
        {
            if (_connectionRoutine != null)
                throw new Exception("a connection is already running!");

            _connectionRoutine = StartCoroutine(DoConnection(url));
        }

        public void Disconnect()
        {
            if (_connectionRoutine != null)
            {
                _keepAlive = false;
            }
        }

        public void SendData(EventCode eventCode, object data)
        {
            var msgData = new MsgData();
            msgData.code = eventCode;
            msgData.data = _converter.SerializeObject(data);

            var str = _converter.SerializeObject(msgData);
            Debug.Log($"[Networker] sending data {str}");
            _socket.SendString(str);
        }

        protected override void OnAwake()
        {
            _locator.RegisterSingleton<INetworker>(this);
        }

        private void Start()
        {
            _converter = _locator.GetService<IJsonConverter>();
        }

        private void OnDestroy()
        {
            if (_connectionRoutine != null)
            {
                StopCoroutine(_connectionRoutine);
                _connectionRoutine = null;

                if (_socket != null)
                {
                    _socket.Close();
                    _socket = null;
                }
            }
        }

        private class MsgData
        {
            public EventCode code;
            public string data;
        }
    }
}
