using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using PBMultiplayerServer.Core.Factories;

namespace PBMultiplayerServer.Transport.UDP.Impls
{
    public class UdpConnection : Connection
    {
        private readonly ISocketProxy _socketProxy;

        public UdpConnection(IPEndPoint remoteEndPoint, 
            ISocketProxy socketProxy) : base(remoteEndPoint)
        {
            _socketProxy = socketProxy;
        }

        public override async Task ReceiveAsync()
        {
            var data = new byte[1024];
            var iEndpoint = new IPEndPoint(IPAddress.Any, 0);
            var receiveFromResult = await _socketProxy.ReceiveFromAsync(data, SocketFlags.None, iEndpoint);
        }

        public override void Receive()
        {
            var buffer = new byte[1024];
            var remoteEndPoint = (EndPoint)new IPEndPoint(IPAddress.Any, 0);

            if (_socketProxy.Available > 0 && _socketProxy.Poll(0, SelectMode.SelectRead))
            {
                var byteCount = _socketProxy.ReceiveFrom(buffer, SocketFlags.None, ref remoteEndPoint);
            }
              
        }

        public override void CloseConnection()
        {
            
        }
    }
}