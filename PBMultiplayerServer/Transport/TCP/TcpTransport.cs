using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using PBMultiplayerServer.Core.Factories;

namespace PBMultiplayerServer.Transport.TCP
{
    public class TcpTransport : ITransport
    {
        private ISocketProxy _socket;
        
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
                var connection = await _socket.AcceptAsync();
                Console.WriteLine("user connected via TCP");
            }
        }
        
        public void Dispose()
        {
            _socket?.Dispose();
        }
    }
}