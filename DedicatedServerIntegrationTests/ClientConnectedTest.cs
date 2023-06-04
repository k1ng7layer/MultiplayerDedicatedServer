﻿using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using PBMultiplayerServer.Core.Impls;
using PBMultiplayerServer.Transport;
using ServerTests.Helpers;

namespace ServerTests
{
    public class ClientConnectedTest
    {
        [Test]
        public async Task TestClientConnected()
        {
            var socketFactoryMock = new SocketProxyFactoryMock();
            var server = new MultiplayerServer(IPAddress.Any, 8888, socketFactoryMock, EProtocolType.TCP);
            
            server.RunAsync();
            
            Assert.True(server.IsRunning);
            
            await Task.Delay(1000);
            
            Assert.True(server.Connections.Count > 0);
        }
    }
}