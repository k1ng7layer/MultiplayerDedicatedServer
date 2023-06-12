using System;
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
            try
            {
                Running = true;
            
                _multiplayerServer.AddServerTickHandler(Tick);
                _multiplayerServer.Start();

                await _multiplayerServer.UpdateAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
      
        }

        private void Tick()
        {
            //Console.WriteLine($"server time sec = {_multiplayerServer.ServerTickDeltaTimeSpan.TotalSeconds}");
        }
    }
}