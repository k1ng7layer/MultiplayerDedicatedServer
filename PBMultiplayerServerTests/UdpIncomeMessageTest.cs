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
    public class UdpIncomeMessageTest
    {
        private int messageReceiveCount;
        
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

            var server = new MultiplayerServer(config);
            
            server.Start();
            
            server.AddIncomeMessageListeners(HandleIncomeMessage);
            
            Assert.True(server.IsRunning);
            
            Task.Run(async () => server.UpdateConnectionsAsync());
            Task.Run(async () => server.UpdateEventsAsync(1000 / 30));

            await Task.Delay(1000);

            var testUdpClient = new TestUdpClient();

            await testUdpClient.SendMessageAsync(EMessageType.Connect, serverEndPoint);
            
            await Task.Delay(2000);
            
            Assert.True(messageReceiveCount > 0);
            
        }
        
        private void HandleIncomeMessage(IncomeMessage message)
        {
            Assert.True(message.MessageType == EMessageType.Connect);
            messageReceiveCount++;
        }
    }
}