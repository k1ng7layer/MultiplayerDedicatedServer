using System;
using MultiplayerDedicatedServer.Builders.ServiceInstaller;
using MultiplayerDedicatedServer.Core.Server;

namespace MultiplayerDedicatedServer.Builders.DedicatedServerBuilder
{
    public interface IDedicatedServerBuilder : IDisposable
    {
        IDedicatedServer BuildServer<TServer, TInstaller>() where TServer : IDedicatedServer where TInstaller : IServiceInstaller;
    }
}