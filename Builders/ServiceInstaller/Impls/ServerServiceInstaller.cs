using Autofac;
using MultiplayerDedicatedServer.Configuration;
using PBMultiplayerServer.Configuration;
using PBMultiplayerServer.Configuration.Impl;
using PBMultiplayerServer.Core;
using PBMultiplayerServer.Core.Impls;
using PBMultiplayerServer.Utils;

namespace MultiplayerDedicatedServer.Builders.ServiceInstaller.Impls
{
    public class ServerServiceInstaller : IServiceInstaller
    {
        public void ConfigureServices(ContainerBuilder builder)
        {
            var configuration = new DefaultConfiguration();
            
            configuration.Add(ConfigurationKeys.ReceiveTickRate, "30");
            configuration.Add(ConfigurationKeys.SendTickRate, "30");
            configuration.Add(ConfigurationKeys.ServerUpdateTickRate, "35");
            configuration.Add(ConfigurationKeys.MinMessageSize, "4");

            var networkConfig = new DefaultNetworkConfiguration()
            {
                MinMessageSize = 4,
                IpAddress = "127.0.0.1",
                Port = 8888,
            };
            
            builder.RegisterInstance<IConfiguration>(configuration);
            builder.RegisterInstance<INetworkConfiguration>(networkConfig);

            builder.RegisterType<MultiplayerServer>().As<IMultiplayerServer>().SingleInstance();
        }
        
        
    }
}