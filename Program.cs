using System.Threading.Tasks;
using MultiplayerDedicatedServer.Builders.DedicatedServerBuilder.Impls;
using MultiplayerDedicatedServer.Builders.ServiceInstaller;
using MultiplayerDedicatedServer.Core.Server.Impls;

namespace MultiplayerDedicatedServer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using (var serverBuilder = new DedicatedServerBuilder())
            {
                var server = serverBuilder.BuildServer<DedicatedServer, IServiceInstaller>();

                await server.RunServerAsync().ConfigureAwait(false);
            }
        }
    }
}