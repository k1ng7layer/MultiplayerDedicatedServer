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
            var udpRun = _multiplayerServer.RunUdpAsync();
            var tcpRun = _multiplayerServer.RunTcpAsync();

            Running = true;
            
            await Task.WhenAll(udpRun, tcpRun);
        }
    }
}