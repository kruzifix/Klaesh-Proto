using System;
using System.Collections.Concurrent;
using System.Text;
using System.Collections;

#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

namespace Klaesh.Network
{
    public class WebSocket
    {
        private Uri _url;

        public WebSocket(Uri url)
        {
            _url = url;

            string protocol = _url.Scheme;
            if (!protocol.Equals("ws") && !protocol.Equals("wss"))
                throw new ArgumentException("Unsupported protocol: " + protocol);
        }

        public void SendString(string str)
        {
            Send(Encoding.UTF8.GetBytes(str));
        }

        public string RecvString()
        {
            byte[] retval = Recv();
            if (retval == null)
                return null;
            return Encoding.UTF8.GetString(retval);
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern int SocketCreate(string url);

        [DllImport("__Internal")]
        private static extern int SocketState(int socketInstance);

        [DllImport("__Internal")]
        private static extern void SocketSend(int socketInstance, byte[] ptr, int length);

        [DllImport("__Internal")]
        private static extern void SocketRecv(int socketInstance, byte[] ptr, int length);

        [DllImport("__Internal")]
        private static extern int SocketRecvLength(int socketInstance);

        [DllImport("__Internal")]
        private static extern void SocketClose(int socketInstance);

        [DllImport("__Internal")]
        private static extern int SocketError(int socketInstance, byte[] ptr, int length);

        private int _NativeRef = 0;

        public void Send(byte[] buffer)
        {
            SocketSend(_NativeRef, buffer, buffer.Length);
        }

        public byte[] Recv()
        {
            int length = SocketRecvLength(_NativeRef);
            if (length == 0)
                return null;
            byte[] buffer = new byte[length];
            SocketRecv(_NativeRef, buffer, length);
            return buffer;
        }

        public IEnumerator Connect()
        {
            _NativeRef = SocketCreate(_url.ToString());

            while (SocketState(_NativeRef) == 0)
                yield return null;
        }
 
        public void Close()
        {
            SocketClose(_NativeRef);
        }

        public string Error
        {
            get
            {
                const int bufsize = 1024;
                byte[] buffer = new byte[bufsize];
                int result = SocketError(_NativeRef, buffer, bufsize);

                if (result == 0)
                    return null;

                return Encoding.UTF8.GetString(buffer);
            }
        }
#else
        private WebSocketSharp.WebSocket _socket;
        private ConcurrentQueue<byte[]> _messages = new ConcurrentQueue<byte[]>();
        private bool _isConnected = false;

        public string Error { get; private set; }

        public IEnumerator Connect()
        {
            _socket = new WebSocketSharp.WebSocket(_url.ToString());
            _socket.OnMessage += (sender, e) => _messages.Enqueue(e.RawData);
            _socket.OnOpen += (sender, e) => _isConnected = true;
            _socket.OnError += (sender, e) => Error = e.Message;
            _socket.ConnectAsync();

            while (!_isConnected && Error == null)
                yield return null;
        }

        public void Send(byte[] buffer)
        {
            _socket.Send(buffer);
        }

        public byte[] Recv()
        {
            byte[] data;
            if (_messages.TryDequeue(out data))
                return data;
            return null;
        }

        public void Close()
        {
            _socket.Close();
        }
#endif
    }
}
