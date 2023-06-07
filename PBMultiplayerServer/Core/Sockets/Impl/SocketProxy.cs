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

        public int Available => _socket.Available;
        
        public Socket Socket => _socket;
        
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

        public ISocketProxy Accept()
        {
            var socket = _socket.Accept();
            var socketProxy = new SocketProxy(socket);

            return socketProxy;
        }

        public async Task<int> ReceiveAsync(ArraySegment<byte> buffer, SocketFlags socketFlags)
        {
            var data = await _socket.ReceiveAsync(buffer, socketFlags);

            return data;
        }

        public async Task<SocketReceiveMessageFromResult> ReceiveMessageFromAsync(ArraySegment<byte> buffer, SocketFlags socketFlags, IPEndPoint remoteEndpoint)
        {
            var data = await _socket.ReceiveMessageFromAsync(buffer, socketFlags, remoteEndpoint);

            return data;
        }

        public async Task<SocketReceiveFromResult> ReceiveFromAsync(ArraySegment<byte> buffer, SocketFlags socketFlags, IPEndPoint endPoint)
        {
            var iEndpoint = new IPEndPoint(IPAddress.Any, 0);
            
            var receiveFromResult = await _socket.ReceiveFromAsync(buffer, socketFlags, iEndpoint);

            return receiveFromResult;
        }

        public int ReceiveFrom(byte[] buffer, SocketFlags socketFlags, ref EndPoint remoteEP)
        {
            return _socket.ReceiveFrom(buffer, socketFlags, ref remoteEP);
        }

        public int Receive(byte[] receiveBuffer, int receiveSize, int startIndex)
        {
            var data = _socket.Receive(receiveBuffer, receiveSize, SocketFlags.None);

            return data;
        }

        public bool Poll(int microSeconds, SelectMode mode)
        {
            return _socket.Poll(microSeconds, mode);
        }

        public void Close()
        {
            _socket.Close();
        }

        public void Dispose()
        {
            _socket?.Dispose();
        }
    }
}