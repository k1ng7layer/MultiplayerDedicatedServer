using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace PBMultiplayerServer.Core.Factories.Impl
{
    public class SocketProxy : ISocketProxy
    {
        private readonly Socket _socket;
        
        public SocketProxy(AddressFamily addressFamily, 
            SocketType socketType, 
            ProtocolType protocolType)
        {
            _socket = new Socket(addressFamily, socketType, protocolType);
        }

        public SocketProxy(Socket socket)
        {
            _socket = socket;
        }

        public void Bind(EndPoint localEP)
        {
            _socket.Bind(localEP);
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
    }
}