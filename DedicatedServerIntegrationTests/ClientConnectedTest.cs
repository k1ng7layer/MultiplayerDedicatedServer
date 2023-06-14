using System.Threading.Tasks;
using NUnit.Framework;
using PBMultiplayerServer.Configuration.Impl;
using PBMultiplayerServer.Core.Impls;
using ServerTests.Helpers;

namespace ServerTests
{
    public class ClientConnectedTest
    {
        [Test]
        public async Task TestClientConnected()
        {
            var config = new DefaultNetworkConfiguration();
            
            var socketFactoryMock = new SocketProxyFactoryMock();
            var server = new MultiplayerServer(socketFactoryMock, config);
            
            server.UpdateConnectionsAsync();
            
            Assert.True(server.IsRunning);
            
            await Task.Delay(1000);
            
            Assert.True(server.Connections.Count > 0);
        }
    }
}