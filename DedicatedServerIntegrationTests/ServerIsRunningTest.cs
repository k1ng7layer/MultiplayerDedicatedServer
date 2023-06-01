using Autofac;
using MultiplayerDedicatedServer.Builders.ServiceInstaller.Impls;
using MultiplayerDedicatedServer.Core.Server;
using MultiplayerDedicatedServer.Core.Server.Impls;
using NUnit.Framework;
using ServerTests.Utils;

namespace ServerTests
{
    public class ServerIsRunningTest : ServerIntegrationTestWithIocBase<DedicatedServer, ServerServiceInstaller>
    {
        [Test]
        public void ServerIsRunning()
        {
            var multiplayerServer = Container.Resolve<IDedicatedServer>();
            
            multiplayerServer.RunServerAsync();
            
            Assert.True(multiplayerServer.Running);
        }
    }
}