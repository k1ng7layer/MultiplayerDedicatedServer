using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using PBMultiplayerServer.Core.Messages;

namespace ServerTests.Utils
{
    public class TestUdpClient : IDisposable
    {
        private Socket _socket;
        
        public async Task SendMessageAsync(EMessageType messageType, IPEndPoint destinationEndpoint)
        {
            using Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            
            var data = BitConverter.GetBytes((int)messageType);
    
            await sender.SendToAsync(data, SocketFlags.None, destinationEndpoint);
        }

        public void Dispose()
        {
            _socket?.Dispose();
        }
    }
}