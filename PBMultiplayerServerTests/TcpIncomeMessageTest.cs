using System;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
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

            var server = new MultiplayerServer(config);
            
            server.AddIncomeMessageListeners(HandleIncomeMessage);
            server.Start();
            
            Assert.True(server.IsRunning);
            
            Task.Run(async () => server.UpdateConnectionsAsync());
            Task.Run(async () => server.UpdateEventsAsync(1000/30));

            await Task.Delay(2000);

            var tcpClient = new TestTcpClient();
            
            await tcpClient.ConnectAsync(serverEndPoint);
            
            var messageTypeBytes = BitConverter.GetBytes((int)EMessageType.Connect);
            var messageLenghtBytes = BitConverter.GetBytes(sizeof(int));
            await tcpClient.SendMessageAsync(messageLenghtBytes);
            await tcpClient.SendMessageAsync(messageTypeBytes);
            
            await Task.Delay(1000);
            
            Assert.True(messageReceiveCount > 0);
        }
        
        private void HandleIncomeMessage(IncomeMessage message)
        {
            Assert.True(message.MessageType == EMessageType.Connect);
            messageReceiveCount++;
        }
    }
}