using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using PBMultiplayerServer.Authentication;
using PBMultiplayerServer.Configuration.Impl;
using PBMultiplayerServer.Core.Impls;
using PBMultiplayerServer.Core.Messages;
using PBMultiplayerServer.Core.Messages.Impl;
using ServerTests.Utils;

namespace ServerTests
{
    public class UdpIncomeMessageTest
    {
        private int messageReceiveCount;
        private MultiplayerServer _multiplayerServer;
        private TestUdpClient _client;

        [TearDown]
        public void TearDown()
        {
            _multiplayerServer.Stop();
            _multiplayerServer.Dispose();
            _client.Dispose();
        }
        
        [Test, MaxTime(4000)]
        public async Task TestUdpIncomeMessage()
        {
            var ipAddress = IPAddress.Parse("127.0.0.1");
            
            const int tcpPort = 8888;
            const int udpPort = 8889;
            
            var serverEndPoint = new IPEndPoint(ipAddress, udpPort);
            
            var config = new DefaultNetworkConfiguration
            {
                IpAddress = "127.0.0.1",
                Port = tcpPort,
                MinMessageSize = 4,
            };

            _multiplayerServer = new MultiplayerServer(config);
            
            _multiplayerServer.Start();
            
            _multiplayerServer.AddIncomeMessageListeners(HandleIncomeMessage);
            _multiplayerServer.AddApprovalCallback(OnClientConnected);
            
            Assert.True(_multiplayerServer.IsRunning);
            
            Task.Run(async () => _multiplayerServer.UpdateConnectionsAsync());
            Task.Run(async () => _multiplayerServer.UpdateEventsAsync(1000 / 30));

            await Task.Delay(1000);

            _client = new TestUdpClient();

            await _client.SendMessageAsync(EMessageType.Connect, serverEndPoint);
            
            await Task.Delay(2000);
            
            Assert.True(messageReceiveCount > 0);
            
        }
        
        private void HandleIncomeMessage(IncomeMessage message)
        {
            Assert.True(message.MessageType == EMessageType.Connect);
            messageReceiveCount++;
        }

        private LoginResult OnClientConnected(byte[] data)
        {
            return new LoginResult(ELoginResult.Success, string.Empty);
        }
    }
}