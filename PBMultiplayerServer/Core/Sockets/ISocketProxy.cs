using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace PBMultiplayerServer.Core.Factories
{
    public interface ISocketProxy : IDisposable
    {
        void Bind(EndPoint localEP);
        void Listen(int backlog);
        Task<ISocketProxy> AcceptAsync();
        Task<int> ReceiveAsync(ArraySegment<byte> buffer, SocketFlags socketFlags);
    }
}