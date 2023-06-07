using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using PBMultiplayerServer.Core.Factories;

namespace ServerTests.Helpers
{
    public class SocketProxyMock : ISocketProxy
    {
        private IPEndPoint _remoteEndpoint;

        public SocketProxyMock()
        {
            _remoteEndpoint = new IPEndPoint(IPAddress.Any, 0);
        }
        
        public void Dispose()
        {
            
        }

        public int Available { get; }
        public Socket Socket { get; }
        public IPEndPoint RemoteEndpoint => _remoteEndpoint;
        
        public void Bind(EndPoint localEP)
        {
            
        }

        public void Listen(int backlog)
        {
            
        }

        public Task<ISocketProxy> AcceptAsync()
        {
            ISocketProxy socketProxy = new SocketProxyMock();
            
            return Task.FromResult(socketProxy);
        }

        public ISocketProxy Accept()
        {
            throw new NotImplementedException();
        }

        public Task<int> ReceiveAsync(ArraySegment<byte> buffer, SocketFlags socketFlags)
        {
            return Task.FromResult(new int());
        }

        public Task<SocketReceiveMessageFromResult> ReceiveMessageFromAsync(ArraySegment<byte> buffer, SocketFlags socketFlags, IPEndPoint remoteEndpoint)
        {
            throw new NotImplementedException();
        }

        public Task<SocketReceiveFromResult> ReceiveFromAsync(ArraySegment<byte> buffer, SocketFlags socketFlags, IPEndPoint endPoint)
        {
            throw new NotImplementedException();
        }

        public int ReceiveFrom(byte[] buffer, SocketFlags socketFlags, ref EndPoint remoteEP)
        {
            throw new NotImplementedException();
        }

        public int Receive(byte[] receiveBuffer, int receiveSize, int startIndex)
        {
            throw new NotImplementedException();
        }

        public bool Poll(int microSeconds, SelectMode mode)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }
    }
}