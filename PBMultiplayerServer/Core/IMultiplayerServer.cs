using System;
using System.Threading.Tasks;

namespace PBMultiplayerServer.Core
{
    public interface IMultiplayerServer : IDisposable
    {
        bool IsRunning { get; }
        Task RunAsync();
        void Run();
        void Stop();
        void OnDataReceivedCallback(Action<byte[]> callback);
    }
}