using System.Net.Sockets;

namespace PBMultiplayerServer.Core.Factories.Impl
{
    public class SocketProxyFactory : ISocketProxyFactory
    {
        public ISocketProxy CreateSocketProxy(AddressFamily addressFamily, 
            SocketType socketType, ProtocolType protocolType)
        {
            return new SocketProxy(addressFamily, socketType, protocolType);
        }
    }
}