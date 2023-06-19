using System;
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
    public class TcpIncomeMessageTest
    {
        private int messageReceiveCount;
        private MultiplayerServer _multiplayerServer;
        private TestTcpClient _client;

        [TearDown]
        public void TearDown()
        {
            _multiplayerServer.Stop();
            _multiplayerServer.Dispose();
            _client.Dispose();
        }
        
        
        [Test, MaxTime(4000)]
        public async Task TestTcpIncomeMessage()
        {
            var ipAddress = IPAddress.Parse("127.0.0.1");

            var serverEndPoint = new IPEndPoint(ipAddress, 8888);
            const int port = 8888;
            
            var config = new DefaultNetworkConfiguration
            {
                IpAddress = "127.0.0.1",
                Port = port,
                MinMessageSize = 4,
            };

            _multiplayerServer = new MultiplayerServer(config);
            
            _multiplayerServer.AddIncomeMessageListeners(HandleIncomeMessage);
            _multiplayerServer.AddApprovalCallback(OnClientConnected);
            _multiplayerServer.Start();
            
            Assert.True(_multiplayerServer.IsRunning);
            
            Task.Run(async () => _multiplayerServer.UpdateConnectionsAsync());
            Task.Run(async () => _multiplayerServer.UpdateEventsAsync(1000/30));

            await Task.Delay(2000);

            _client = new TestTcpClient();
            
            await _client.ConnectAsync(serverEndPoint);
            
            var messageTypeBytes = BitConverter.GetBytes((int)EMessageType.Connect);
            var messageLenghtBytes = BitConverter.GetBytes(sizeof(int));
            await _client.SendMessageAsync(messageLenghtBytes);
            await _client.SendMessageAsync(messageTypeBytes);
            
            await Task.Delay(1000);
            
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