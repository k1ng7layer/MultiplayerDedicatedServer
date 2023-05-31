using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace PBMultiplayerServer.Transport.TCP
{
    public class TcpTransport : ITransport
    {
        private readonly IPEndPoint _ipEndPoint;
        private Socket _serverTcpConnection;
        
        public TcpTransport(IPEndPoint ipEndPoint)
        {
            _ipEndPoint = ipEndPoint;
            _serverTcpConnection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _serverTcpConnection.Bind(_ipEndPoint);
        }
        
        public async Task ProcessAsync(CancellationToken cancellationToken)
        {
            _serverTcpConnection.Listen(1000);
            
            while (true)
            {
                //var connection = await _serverTcpConnection.ReceiveAsync(bytesRead, SocketFlags.None, cancellationToken );
                var connection = await _serverTcpConnection.AcceptAsync();
                Console.WriteLine("user connected via TCP");
            }
        }
        
        public void Dispose()
        {
            _serverTcpConnection?.Dispose();
        }
    }
}