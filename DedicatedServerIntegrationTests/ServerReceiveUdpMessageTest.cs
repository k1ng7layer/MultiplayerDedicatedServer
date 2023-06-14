using System.Net;
using System.Threading.Tasks;
using Autofac;
using MultiplayerDedicatedServer.Builders.ServiceInstaller.Impls;
using MultiplayerDedicatedServer.Core.Server;
using MultiplayerDedicatedServer.Core.Server.Impls;
using NUnit.Framework;
using PBMultiplayerServer.Core.Messages;
using ServerTests.Utils;

namespace ServerTests
{
    public class ServerReceiveUdpMessageTest : ServerIntegrationTestWithIocBase<DedicatedServer, ServerServiceInstaller>
    {
        [Test]
        public async Task ServerIsRunning()
        {
            var updClient = new TestUdpClient();
            
            var multiplayerServer = Container.Resolve<IDedicatedServer>();

            Task.Run(async () => multiplayerServer.RunServerAsync());
            
            await Task.Delay(1000);
            
            Assert.True(multiplayerServer.IsRunning);

            var serverIp = IPAddress.Parse("127.0.0.1");

            var serverEndPoint = new IPEndPoint(serverIp, 8888);

            await updClient.SendMessage(EMessageType.Connect, serverEndPoint);
            
            
        }
    }
}