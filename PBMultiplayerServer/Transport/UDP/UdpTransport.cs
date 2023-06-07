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
        private readonly List<Action<byte[], int, IPEndPoint>> _clientConnectedListeners = new();

        public UdpTransport(ISocketProxy socket, EndPoint ipEndPoint)
        {
            _socket = socket;
            _socket.Bind(ipEndPoint);
        }

        public async Task ProcessAsync(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            
            try
            {
                var data = new byte[1024];
            
                while (!_cancellationToken.IsCancellationRequested)
                {
                    var iEndpoint = new IPEndPoint(IPAddress.Any, 0);
                    //todo: прочитать про SocketFlags;
                    var receiveFromResult = await _socket.ReceiveFromAsync(data, SocketFlags.None, iEndpoint);
                    
                    if (receiveFromResult.ReceivedBytes > 0)
                    {
                        OnMessageReceived(data, receiveFromResult.ReceivedBytes, iEndpoint);
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

        public void AddMessageReceivedListener(Action<byte[], int, IPEndPoint> clientConnectedCallback)
        {
            throw new NotImplementedException();
        }

        public void AddClientConnectedListener(Action<byte[], int, IPEndPoint> clientConnectedCallback)
        {
            _clientConnectedListeners.Add(clientConnectedCallback);
        }

        private void OnMessageReceived(byte[] data, int amount, IPEndPoint ipEndPoint)
        {
            foreach (var listener in _clientConnectedListeners)
            {
                listener?.Invoke(data, amount, ipEndPoint);
            }
        }

        public void Dispose()
        {
            _socket?.Dispose();
        }
    }
}