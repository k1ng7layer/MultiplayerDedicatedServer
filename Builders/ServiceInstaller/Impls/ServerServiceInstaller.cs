using System.Net;
using Autofac;
using MultiplayerDedicatedServer.Configuration;
using MultiplayerDedicatedServer.Configuration.Impl;
using PBMultiplayerServer.Core;
using PBMultiplayerServer.Core.Factories;
using PBMultiplayerServer.Core.Factories.Impl;
using PBMultiplayerServer.Core.Impls;
using PBMultiplayerServer.Transport;

namespace MultiplayerDedicatedServer.Builders.ServiceInstaller.Impls
{
    public class ServerServiceInstaller : IServiceInstaller
    {
        public void ConfigureServices(ContainerBuilder builder)
        {
            var configuration = new DefaultConfiguration();
            
            configuration.AddConfiguration("ReceiveTickRate", "30");
            configuration.AddConfiguration("SendTickRate", "30");
            configuration.AddConfiguration("ServerUpdateTickRate", "35");

            builder.RegisterInstance<IConfiguration>(configuration);
            
            builder.RegisterType<MultiplayerServer>().As<IMultiplayerServer>().SingleInstance().WithParameters(new []
            {
                new TypedParameter(typeof(IPAddress), IPAddress.Parse("127.0.0.1")),
                new TypedParameter(typeof(int), 8888),
                new TypedParameter(typeof(ISocketProxyFactory), new SocketProxyFactory()),
                new TypedParameter(typeof(EProtocolType), EProtocolType.UDP_TCP),
                new TypedParameter(typeof(IConfiguration), configuration)
            });
        }
    }
}