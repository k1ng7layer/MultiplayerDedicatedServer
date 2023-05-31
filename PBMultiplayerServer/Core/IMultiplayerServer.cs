using System;
using System.Threading.Tasks;

namespace PBMultiplayerServer.Core
{
    public interface IMultiplayerServer : IDisposable
    {
        bool IsRunning { get; }
        Task RunUdpAsync();
        Task RunTcpAsync();
        void RunTcp();
        void RunUdp();
        void Stop();
        void OnDataReceivedCallback(Action<byte[]> callback);
    }
}