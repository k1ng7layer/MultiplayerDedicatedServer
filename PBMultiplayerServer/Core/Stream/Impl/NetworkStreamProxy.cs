using System.Net.Sockets;
using System.Threading.Tasks;
using PBMultiplayerServer.Core.Factories;

namespace PBMultiplayerServer.Core.Stream.Impl
{
    public class NetworkStreamProxy : INetworkStreamProxy
    {
        private readonly ISocketProxy _socketProxy;
        protected NetworkStream _networkStream;

        public NetworkStreamProxy(ISocketProxy socketProxy)
        {
            _socketProxy = socketProxy;
            
            _networkStream = new NetworkStream(_socketProxy.Socket);
        }

        public async Task<int> ReadAsync(byte[] buffer, int offset, int count)
        {
            var data = await _networkStream.ReadAsync(buffer, offset, count);

            return data;
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            var data = _networkStream.Read(buffer, offset, count);

            return data;
        }

        public void Close()
        {
            _networkStream.Close();
        }

        public async Task WriteAsync(byte[] size)
        {
            await _networkStream.WriteAsync(size);
        }

        public async Task FlushAsync()
        {
            await _networkStream.FlushAsync();
        }

        public void Dispose()
        {
            _socketProxy?.Dispose();
            _networkStream?.Dispose();
        }
    }
}