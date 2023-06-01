using Autofac;
using MultiplayerDedicatedServer.Builders.ServiceInstaller;
using MultiplayerDedicatedServer.Core.Server;
using NUnit.Framework;

namespace ServerTests.Utils
{
    [TestFixture]
    public class ServerIntegrationTestWithIocBase<TServer, TInstaller> 
        where TInstaller : IServiceInstaller, new() where TServer : IDedicatedServer
    {
        private ILifetimeScope _lifetimeScope;
        protected IContainer Container { get; private set; }
        
        [SetUp]
        public void SetUp()
        {
            var containerBuilder = new ContainerBuilder();
            var serviceInstaller = new TInstaller();
            
            serviceInstaller.ConfigureServices(containerBuilder);
            
            containerBuilder.RegisterType<TServer>().As<IDedicatedServer>().SingleInstance();

            Configure(containerBuilder);

            Container = containerBuilder.Build();
            
            _lifetimeScope = Container.BeginLifetimeScope();
        }
        
        protected virtual void Configure(ContainerBuilder builder)
        { }
        
        [TearDown]
        public void TearDown()
        {
            _lifetimeScope.Dispose();
            Container.Dispose();
        }
    }
}