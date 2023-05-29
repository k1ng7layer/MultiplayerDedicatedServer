using System.Threading.Tasks;
using PBMultiplayerServer.Core;

namespace MultiplayerDedicatedServer.Core.Server.Impls
{
    public class DedicatedServer : IDedicatedServer
    {
        private IMultiplayerServer _multiplayerServer;
        
        public void Dispose()
        {
            _multiplayerServer.Dispose();
        }

        public async Task RunServerAsync()
        {
            await _multiplayerServer.RunAsync();
        }
    }
}