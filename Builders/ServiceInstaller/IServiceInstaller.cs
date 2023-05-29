using Autofac;

namespace MultiplayerDedicatedServer.Builders.ServiceInstaller
{
    public interface IServiceInstaller
    {
        void ConfigureServices(ContainerBuilder builder);
    }
}