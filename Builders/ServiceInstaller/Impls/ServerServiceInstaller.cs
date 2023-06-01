using System.Net;
using Autofac;
using PBMultiplayerServer.Core;
using PBMultiplayerServer.Core.Impls;

namespace MultiplayerDedicatedServer.Builders.ServiceInstaller.Impls
{
    public class ServerServiceInstaller : IServiceInstaller
    {
        public void ConfigureServices(ContainerBuilder builder)
        {
            builder.RegisterType<MultiplayerServer>().As<IMultiplayerServer>().SingleInstance().WithParameters(new []
            {
                new TypedParameter(typeof(IPAddress), IPAddress.Parse("127.0.0.1")),
                new TypedParameter(typeof(int), 8888)
            });
            
            // builder.BindFromSubstitute<MultiplayerServer, IMultiplayerServer>().As<IMultiplayerServer>().SingleInstance().WithParameters(new []
            // {
            //     new TypedParameter(typeof(IPAddress), IPAddress.Parse("127.0.0.1")),
            //     new TypedParameter(typeof(int), 8888)
            // });
        }
    }
}