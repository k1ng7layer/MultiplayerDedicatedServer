using System.Net.Sockets;

namespace PBMultiplayerServer.Core.Factories
{
    public interface ISocketProxyFactory
    {
        ISocketProxy CreateSocketProxy(AddressFamily addressFamily, 
            SocketType socketType, ProtocolType protocolType);
    }
}