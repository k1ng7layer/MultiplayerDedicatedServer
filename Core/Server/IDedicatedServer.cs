using System;
using System.Threading.Tasks;

namespace MultiplayerDedicatedServer.Core.Server
{
    public interface IDedicatedServer : IDisposable
    {
        bool Running { get; }
        Task RunServerAsync();
    }
}