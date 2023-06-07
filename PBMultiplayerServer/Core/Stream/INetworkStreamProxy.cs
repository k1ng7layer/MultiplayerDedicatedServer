using System;
using System.Threading.Tasks;

namespace PBMultiplayerServer.Core.Stream
{
    public interface INetworkStreamProxy : IDisposable
    {
        Task<int> ReadAsync(byte[] buffer, int offset, int count);
        int Read(byte[] buffer, int offset, int count);
        void Close();
    }
}