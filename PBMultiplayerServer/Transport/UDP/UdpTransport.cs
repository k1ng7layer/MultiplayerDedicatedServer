using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using PBMultiplayerServer.Core.Factories;

namespace PBMultiplayerServer.Transport.UDP.Impls
{
    public class UdpTransport : NetworkTransport
    {
        private readonly ISocketProxy _socket;
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly Dictionary<IPEndPoint, Connection> _activeClients = new();
        private bool _running;
        
        public UdpTransport(ISocketProxy socket, EndPoint ipEndPoint)
        {
            _socket = socket;
            _socket.Bind(ipEndPoint);
        }

        public override async Task UpdateAsync()
        {
            var cancellationToken = _cancellationTokenSource.Token;
            
            try
            {
                var data = new byte[1024];
            
                while (_running && !cancellationToken.IsCancellationRequested)
                {
                    var iEndpoint = new IPEndPoint(IPAddress.Any, 0);
                    //todo: прочитать про SocketFlags;
                    var receiveFromResult = await _socket.ReceiveFromAsync(data, SocketFlags.None, iEndpoint);
                    
                    if (receiveFromResult.ReceivedBytes > 0)
                    {
                        OnMessageReceived(data, receiveFromResult.ReceivedBytes, iEndpoint);
                    }
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public override void Start()
        {
            _running = true;
        }

        public override void Update()
        {
            // var data = new byte[1024];
            // var result = _socket.ReceiveFrom()
        }

        public override void Stop()  
        {
            _cancellationTokenSource.Cancel();
            _running = false;
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
    }
}