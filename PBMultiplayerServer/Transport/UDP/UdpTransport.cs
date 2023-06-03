using System;
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
                    //todo: прочитать про SocketFlags;
                    var connection = await _socket.ReceiveAsync(data, SocketFlags.None);
                    if (connection > 0)
                    {
                        await Console.Out.WriteAsync($"user connected via UDP, received bytes = {connection} \n");
                    }
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void Dispose()
        {
            _socket?.Dispose();
        }
    }
}