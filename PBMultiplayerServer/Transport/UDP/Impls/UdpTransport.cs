using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace PBMultiplayerServer.Transport.UDP.Impls
{
    public class UdpTransport : ITransport
    {
        private readonly Socket _serverUdpConnection;
        private CancellationToken _cancellationToken;

        public UdpTransport(IPEndPoint ipEndPoint)
        {
            _serverUdpConnection = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _serverUdpConnection.Bind(ipEndPoint);
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
                    var connection = await _serverUdpConnection.ReceiveAsync(data, SocketFlags.None);
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
            _serverUdpConnection?.Dispose();
        }
    }
}