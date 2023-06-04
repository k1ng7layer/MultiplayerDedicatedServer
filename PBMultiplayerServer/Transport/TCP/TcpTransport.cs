using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using PBMultiplayerServer.Core.Factories;

namespace PBMultiplayerServer.Transport.TCP
{
    public class TcpTransport : ITransport
    {
        private readonly List<Action<Connection>> clientConnectedListeners = new();
        private readonly ISocketProxy _socket;
        
        public TcpTransport(ISocketProxy socketProxy, IPEndPoint ipEndPoint)
        {
            _socket = socketProxy;
            _socket.Bind(ipEndPoint);
        }

        public async Task ProcessAsync(CancellationToken cancellationToken)
        {
            _socket.Listen(1000);
            
            while (true)
            {
                var clientSocket = await _socket.AcceptAsync();
                
                OnClientConnected(new TcpConnection(clientSocket.RemoteEndpoint, clientSocket));
    
                Console.WriteLine("user connected via TCP");
            }
        }

        public void AddClientConnectedListener(Action<Connection> clientConnectedCallback)
        {
            clientConnectedListeners.Add(clientConnectedCallback);
        }

        private void OnClientConnected(Connection socketProxy)
        {
            foreach (var listener in clientConnectedListeners)
            {
                listener.Invoke(socketProxy);
            }
        }

        public void Dispose()
        {
            _socket?.Dispose();
        }
    }
}