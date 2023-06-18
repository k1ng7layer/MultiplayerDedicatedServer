using System;
using System.Threading;
using System.Threading.Tasks;
using MultiplayerDedicatedServer.Configuration;
using PBMultiplayerServer.Core;
using PBMultiplayerServer.Utils;

namespace MultiplayerDedicatedServer.Core.Server.Impls
{
    public class DedicatedServer : IDedicatedServer
    {
        private readonly IMultiplayerServer _multiplayerServer;
        private readonly IConfiguration _configuration;
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        public DedicatedServer(IMultiplayerServer multiplayerServer, 
            IConfiguration configuration)
        {
            _multiplayerServer = multiplayerServer;
            _configuration = configuration;
        }
        
        public TimeSpan ServerTimeSpan { get; private set; }
        public TimeSpan ServerTickDeltaTimeSpan { get; private set; }
        public int ServerTickCount { get; private set; }
        public TimeSpan LastServerTickSpan { get; private set; }
        
        public bool IsRunning { get; private set; }

        public async Task RunServerAsync()
        {
            try
            {
                IsRunning = true;
                
                _multiplayerServer.Start();

                Task.Run(async () => await _multiplayerServer.UpdateConnectionsAsync());
                
                await RunServerLoopAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private async Task RunServerLoopAsync()
        {
            var cancellationToken = _cancellationTokenSource.Token;
            var updateTickRate = int.Parse(_configuration[ConfigurationKeys.ServerUpdateTickRate]);
            
            while (IsRunning && !cancellationToken.IsCancellationRequested)
            {
                var tickTimeMilliseconds = 1000 / updateTickRate;

                await Task.Delay(tickTimeMilliseconds, cancellationToken);
                    
                var tickTimeSpan = TimeSpan.FromMilliseconds(tickTimeMilliseconds);
                    
                ServerTimeSpan += tickTimeSpan;
                ServerTickDeltaTimeSpan = ServerTimeSpan - LastServerTickSpan;
                LastServerTickSpan = ServerTimeSpan;
                ServerTickCount++;

                Tick();
            }
        }

        private void Tick()
        {
            Console.WriteLine($"server time sec = {ServerTickDeltaTimeSpan.TotalSeconds}");
        }
        
        public void Dispose()
        {
            _multiplayerServer.Dispose();
        }
    }
}