using System;
using Autofac;
using MultiplayerDedicatedServer.Builders.ServiceInstaller;
using MultiplayerDedicatedServer.Core.Server;

namespace MultiplayerDedicatedServer.Builders.DedicatedServerBuilder.Impls
{
    public class DedicatedServerBuilder : IDedicatedServerBuilder
    {
        private ILifetimeScope _serverScope;
        private readonly ContainerBuilder _containerBuilder = new ();
        private IContainer _container;

        public void Dispose()
        {
            _serverScope.Dispose();
            _container.Dispose();
        }

        public IDedicatedServer BuildServer<TServer, TInstaller>() 
            where TServer : IDedicatedServer where TInstaller : IServiceInstaller
        {
            var serviceInstaller = (TInstaller)Activator.CreateInstance(typeof(TInstaller));
            serviceInstaller?.ConfigureServices(_containerBuilder);
            
            _containerBuilder.RegisterType<TServer>().As<IDedicatedServer>().SingleInstance();
            
            _container = _containerBuilder.Build();
            
            _serverScope = _container.BeginLifetimeScope();

            var server = _serverScope.Resolve<IDedicatedServer>();

            return server;
        }
    }
}