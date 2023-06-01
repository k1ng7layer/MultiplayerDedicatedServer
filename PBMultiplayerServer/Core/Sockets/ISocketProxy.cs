using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace PBMultiplayerServer.Core.Factories
{
    public interface ISocketProxy
    {
        void Bind(EndPoint localEP);
        Task<ISocketProxy> AcceptAsync();
        Task<int> ReceiveAsync(ArraySegment<byte> buffer, SocketFlags socketFlags);
    }
}