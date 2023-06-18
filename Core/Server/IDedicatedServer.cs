using System;
using System.Threading.Tasks;

namespace MultiplayerDedicatedServer.Core.Server
{
    public interface IDedicatedServer : IDisposable
    {
        TimeSpan ServerTimeSpan { get;}
        TimeSpan ServerTickDeltaTimeSpan { get;}
        int ServerTickCount { get;}
        bool IsRunning { get; }
        Task RunServerAsync();
    }
}