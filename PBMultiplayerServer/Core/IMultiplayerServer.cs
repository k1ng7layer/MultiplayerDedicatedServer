using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using PBMultiplayerServer.Transport;

namespace PBMultiplayerServer.Core
{
    public interface IMultiplayerServer : IDisposable
    {
        IDictionary<IPEndPoint, Connection> Connections { get; }
        bool IsRunning { get; }
        Task RunAsync();
        void Run();
        void Stop();
        void OnDataReceivedCallback(Action<byte[]> callback);
    }
}