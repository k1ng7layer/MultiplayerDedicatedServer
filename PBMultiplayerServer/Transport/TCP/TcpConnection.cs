using System;
using System.Net;
using System.Threading.Tasks;
using PBMultiplayerServer.Core.Factories;
using PBMultiplayerServer.Core.Stream;

namespace PBMultiplayerServer.Transport.TCP
{
    public class TcpConnection : Connection, IDisposable
    {
        private readonly ISocketProxy _socketProxy;
        private readonly INetworkStreamProxy _networkStreamProxy;

        public TcpConnection(IPEndPoint remoteEndpoint, 
            ISocketProxy socketProxy, 
            INetworkStreamProxy networkStreamProxy) : base(remoteEndpoint)
        {
            _socketProxy = socketProxy;
            _networkStreamProxy = networkStreamProxy;
        }

        public override async Task ReceiveAsync()
        {
            try
            {
                while (true)
                {
                    var messageSizeBytes = await ReadFromStreamAsync(4);
                    var messageSize = BitConverter.ToInt32(messageSizeBytes, 0);
                    var message = await ReadFromStreamAsync(messageSize);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        public async Task<byte[]> ReadFromStreamAsync(int nBytes)
        {
            var buffer = new byte[nBytes];
            var readPosition = 0;
        
            while (readPosition < nBytes)
            {
                readPosition += await _networkStreamProxy.ReadAsync(buffer, readPosition, nBytes - readPosition);
            }

            return buffer;
        }
        
        public byte[] ReadFromStream(int nBytes)
        {
            var buffer = new byte[nBytes];
            var readPosition = 0;
        
            while (readPosition < nBytes)
            {
                readPosition += _networkStreamProxy.Read(buffer, readPosition, nBytes - readPosition);
            }

            return buffer;
        }

        public override void Receive()
        {
            try
            {
                if (_socketProxy.Available > 0)
                {
                    var messageSizeArray = ReadFromStream(4);
                    var size = BitConverter.ToInt32(messageSizeArray);
                    var message = ReadFromStream(size);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public override void CloseConnection()
        {
            _networkStreamProxy.Close();
            _socketProxy.Close();
        }

        public void Dispose()
        {
            _socketProxy?.Dispose();
            _networkStreamProxy?.Dispose();
            
            GC.SuppressFinalize(this);
        }
    }
}