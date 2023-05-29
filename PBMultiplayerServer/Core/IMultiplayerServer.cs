using System;
using System.Threading.Tasks;

namespace PBMultiplayerServer.Core
{
    public interface IMultiplayerServer : IDisposable
    {
        Task RunAsync();
        void Run();
    }
}