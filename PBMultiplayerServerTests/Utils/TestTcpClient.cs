using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ServerTests.Utils
{
    public class TestTcpClient : IDisposable
    {
        private Socket _socket;
        private NetworkStream _networkStream;
        
        public async Task ConnectAsync(IPEndPoint serverEndPoint)
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
         
            await _socket.ConnectAsync(serverEndPoint);
            
            _networkStream = new NetworkStream(_socket);
        }

        private async Task SendMessage(byte[] data)
        {
            var array = new ArraySegment<byte>(data);

            await _networkStream.WriteAsync(data);
            await _networkStream.FlushAsync();
        }

        public async Task SendMessageAsync(byte[] message)
        {
            await SendMessage(message);
        }
        
        public async Task SendMessageFromConsole()
        {
            while (true)
            {
                string? message = Console.ReadLine();
                var messageType = int.Parse(message);
                var bytes = BitConverter.GetBytes(messageType);
                await SendMessage(bytes);
            }
        }

        public void Dispose()
        {
            _socket?.Dispose();
            _networkStream?.Dispose();
        }
    }
}