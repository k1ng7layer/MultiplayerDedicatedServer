using System.Threading.Tasks;
using PBMultiplayerServer.Core;

namespace MultiplayerDedicatedServer.Core.Server.Impls
{
    public class DedicatedServer : IDedicatedServer
    {
        private readonly IMultiplayerServer _multiplayerServer;

        public DedicatedServer(IMultiplayerServer multiplayerServer)
        {
            _multiplayerServer = multiplayerServer;
        }
        
        public bool Running { get; private set; }
        
        public void Dispose()
        {
            _multiplayerServer.Dispose();
        }
        
        public async Task RunServerAsync()
        {
            Running = true;
            
            _multiplayerServer.Start();
            await _multiplayerServer.UpdateAsync();
        }

        private Task<bool> Func(byte[] arg)
        {
            return Task.FromResult(true);
        }
    }
}