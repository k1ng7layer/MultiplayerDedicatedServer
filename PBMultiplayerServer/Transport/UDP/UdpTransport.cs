using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using PBMultiplayerServer.Core.Factories;

namespace PBMultiplayerServer.Transport.UDP.Impls
{
    public class UdpTransport : ITransport
    {
        private readonly ISocketProxy _socket;
        private CancellationToken _cancellationToken;
        private readonly List<Action<Connection>> clientConnectedListeners = new();

        public UdpTransport(ISocketProxy socket, IPEndPoint ipEndPoint)
        {
            _socket = socket;
            _socket.Bind(ipEndPoint);
        }

        public async Task ProcessAsync(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            
            try
            {
                var data = new byte[2];
            
                while (!_cancellationToken.IsCancellationRequested)
                {
                    var iEndpoint = new IPEndPoint(IPAddress.Any, 0);
                    //todo: прочитать про SocketFlags;
                    var receiveFromResult = await _socket.ReceiveFromAsync(data, SocketFlags.None, iEndpoint);
                    if (receiveFromResult.ReceivedBytes > 0)
                    {
                        OnClientConnected(new UdpConnection(iEndpoint));
                        await Console.Out.WriteAsync($"user connected via UDP, received bytes = {receiveFromResult.ReceivedBytes} \n");
                    }
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void AddClientConnectedListener(Action<Connection> clientConnectedCallback)
        {
            
        }

        private void OnClientConnected(Connection socketProxy)
        {
            foreach (var listener in clientConnectedListeners)
            {
                listener?.Invoke(socketProxy);
            }
        }

        public void Dispose()
        {
            _socket?.Dispose();
        }
    }
}