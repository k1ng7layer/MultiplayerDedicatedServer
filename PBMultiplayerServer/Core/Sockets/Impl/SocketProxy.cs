using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace PBMultiplayerServer.Core.Factories.Impl
{
    public class SocketProxy : ISocketProxy
    {
        private readonly Socket _socket;
        private IPEndPoint _remoteEndpoint;
        
        public SocketProxy(AddressFamily addressFamily, 
            SocketType socketType, 
            ProtocolType protocolType)
        {
            _socket = new Socket(addressFamily, socketType, protocolType);
        }

        public SocketProxy(Socket socket)
        {
            _socket = socket;
            _remoteEndpoint = (IPEndPoint)_socket.RemoteEndPoint;
        }

        public IPEndPoint RemoteEndpoint => _remoteEndpoint;

        public void Bind(EndPoint localEP)
        {
            _socket.Bind(localEP);
        }

        public void Listen(int backlog)
        {
            _socket.Listen(backlog);
        }

        public async Task<ISocketProxy> AcceptAsync()
        {
            var acceptTask = await _socket.AcceptAsync();
            
            var socketProxy = new SocketProxy(acceptTask);
            
            return socketProxy;
        }

        public async Task<int> ReceiveAsync(ArraySegment<byte> buffer, SocketFlags socketFlags)
        {
            var data = await _socket.ReceiveAsync(buffer, socketFlags);

            return data;
        }

        public async Task<SocketReceiveFromResult> ReceiveFromAsync(ArraySegment<byte> buffer, SocketFlags socketFlags, IPEndPoint endPoint)
        {
            var iEndpoint = new IPEndPoint(IPAddress.Any, 0);
            
            var receiveFromResult = await _socket.ReceiveFromAsync(buffer, socketFlags, iEndpoint);

            return receiveFromResult;
        }

        public void Dispose()
        {
            _socket?.Dispose();
        }
    }
}