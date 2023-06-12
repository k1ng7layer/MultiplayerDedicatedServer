using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using PBMultiplayerServer.Core.Factories;
using PBMultiplayerServer.Transport.Interfaces;

namespace PBMultiplayerServer.Transport.UDP.Impls
{
    public class UdpTransport : NetworkTransport, IDataReceivedListener
    {
        private readonly ISocketProxy _socket;
        private readonly UdpConnection _serverConnection;
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly Dictionary<IPEndPoint, Connection> _activeClients = new();
        private bool _running;
        
        public UdpTransport(ISocketProxy socket, EndPoint ipEndPoint)
        {
            _socket = socket;
            _serverConnection = new UdpConnection((IPEndPoint)ipEndPoint, _socket);
            _socket.Bind(ipEndPoint);
        }

        public override async Task UpdateAsync()
        {
            if(!_running)
                return;

            await _serverConnection.ReceiveAsync();
        }

        public override void Start()
        {
            _running = true;
            _serverConnection.AddDataReceivedListener(this);
        }

        public override void Update()
        {
            if(_running)
                _serverConnection.Receive();
        }

        public override void Stop()  
        {
            _running = false;
            _cancellationTokenSource.Cancel();
            _serverConnection.CloseConnection();
        }

        private void OnMessageReceived(byte[] data, int amount, IPEndPoint ipEndPoint)
        {
            var hasConnection = _activeClients.ContainsKey(ipEndPoint);
            
            if(!hasConnection)
                return;
            
            var connection = _activeClients[ipEndPoint];
            
            foreach (var listener in _clientConnectedListeners)
            {
                listener?.Invoke(new DataReceivedEventArgs(data, amount, connection));
            }
        }

        public void Dispose()
        {
            _socket?.Dispose();
            _cancellationTokenSource.Dispose();
        }

        public void OnDataReceived(byte[] data, int byteCount, IPEndPoint remoteEndpoint)
        {
            Console.WriteLine($"OnDataReceived");
        }
    }
}