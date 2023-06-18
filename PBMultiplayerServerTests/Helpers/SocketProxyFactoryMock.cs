using System.Net.Sockets;
using PBMultiplayerServer.Core.Factories;

namespace ServerTests.Helpers
{
    public class SocketProxyFactoryMock : ISocketProxyFactory
    {
        public ISocketProxy CreateSocketProxy(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType)
        {
            return new SocketProxyMock();
        }
    }
}