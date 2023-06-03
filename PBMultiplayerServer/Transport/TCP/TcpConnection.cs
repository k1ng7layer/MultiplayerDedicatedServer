using System.Net;
using PBMultiplayerServer.Core.Factories;

namespace PBMultiplayerServer.Transport.TCP
{
    public class TcpConnection : Connection
    {
        private readonly ISocketProxy _socketProxy;

        public TcpConnection(IPEndPoint remoteEndpoint, 
            ISocketProxy socketProxy) : base(remoteEndpoint)
        {
            _socketProxy = socketProxy;
        }
    }
}