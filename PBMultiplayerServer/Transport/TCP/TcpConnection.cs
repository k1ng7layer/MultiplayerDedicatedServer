using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using PBMultiplayerServer.Core.Factories;
using PBMultiplayerServer.Core.Stream;

namespace PBMultiplayerServer.Transport.TCP
{
    public class TcpConnection : Connection, IDisposable
    {
        private readonly ISocketProxy _socketProxy;
        private readonly INetworkStreamProxy _networkStreamProxy;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private bool _running;
        private bool _disposed;

        public TcpConnection(IPEndPoint remoteEndpoint, 
            ISocketProxy socketProxy, 
            INetworkStreamProxy networkStreamProxy) : base(remoteEndpoint)
        {
            _socketProxy = socketProxy;
            _networkStreamProxy = networkStreamProxy;
        }

        public override void StartReceive()
        {
            _running = true;
        }

        public override async Task ReceiveAsync()
        {
            var cancellationToken = _cancellationTokenSource.Token;
            
            try
            {
                while (_running && !cancellationToken.IsCancellationRequested)
                {
                    var messageSizeBytes = await ReadFromStreamAsync(4);
                    var messageSize = BitConverter.ToInt32(messageSizeBytes, 0);
                    var message = await ReadFromStreamAsync(messageSize);
                    
                    OnDataReceived(message, message.Length, RemoteEndpoint);
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
            var cancellationToken = _cancellationTokenSource.Token;
            
            try
            {
                if (_socketProxy.Available > 0 && _running && !cancellationToken.IsCancellationRequested)
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
            _cancellationTokenSource.Cancel();
            _networkStreamProxy.Close();
            _socketProxy.Close();
        }

        protected override void Dispose(bool isDisposing)
        {
            if(_disposed)
                return;

            if (isDisposing)
            {
                _cancellationTokenSource.Dispose();
                _socketProxy?.Dispose();
                _networkStreamProxy?.Dispose();
            }

            _disposed = true;
            
            base.Dispose(isDisposing);
        }
    }
}