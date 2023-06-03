using System.Net;

namespace PBMultiplayerServer.Transport.UDP.Impls
{
    public class UdpConnection : Connection
    {
        public UdpConnection(IPEndPoint remoteEndPoint) : base(remoteEndPoint)
        {
            
        }
    }
}