using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace PBMultiplayerServer.Core.Factories
{
    public interface ISocketProxy : IDisposable
    {
        int Available { get; }
        Socket Socket { get; }
        IPEndPoint RemoteEndpoint { get; }
        void Bind(EndPoint localEP);
        void Listen(int backlog);
        Task<ISocketProxy> AcceptAsync();
        ISocketProxy Accept();
        Task<int> ReceiveAsync(ArraySegment<byte> buffer, SocketFlags socketFlags);
        Task<SocketReceiveMessageFromResult> ReceiveMessageFromAsync(ArraySegment<byte> buffer, SocketFlags socketFlags, IPEndPoint remoteEndpoint);
        Task<SocketReceiveFromResult> ReceiveFromAsync(ArraySegment<byte> buffer, SocketFlags socketFlags, IPEndPoint endPoint);
        int ReceiveFrom(byte[] buffer, SocketFlags socketFlags, ref EndPoint remoteEP);
        int Receive(byte[] receiveBuffer, int receiveSize, int startIndex);
        Task SendAsync(byte[] data);
        void Send(byte[] data);
        bool Poll(int microSeconds, SelectMode mode);
        void Close();
    }
}