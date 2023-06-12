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
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly int _minMessageSize;
        private bool _running;
        private bool _disposed;

        public TcpConnection(IPEndPoint remoteEndpoint, 
            ISocketProxy socketProxy, 
            INetworkStreamProxy networkStreamProxy, 
            int minMessageSize) : base(remoteEndpoint)
        {
            _socketProxy = socketProxy;
            _networkStreamProxy = networkStreamProxy;
            _minMessageSize = minMessageSize;
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
                    var messageSizeBytes = await ReadFromStreamAsync(sizeof(int));
                    
                    var messageSize = BitConverter.ToInt32(messageSizeBytes, 0);
                    
                    if(messageSize < _minMessageSize)
                        continue;

                    var messageBytes = await ReadFromStreamAsync(messageSize);

                    OnDataReceived(messageBytes, messageBytes.Length, RemoteEndpoint);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// read without stream
        /// </summary>
        // public override async Task ReceiveAsync()
        // {
        //     var cancellationToken = _cancellationTokenSource.Token;
        //
        //     try
        //     {
        //         while (_running && !cancellationToken.IsCancellationRequested)
        //         {
        //             var array = new ArraySegment<byte>(new byte[sizeof(int)]);
        //             
        //             await _socketProxy.ReceiveAsync(array, SocketFlags.None);
        //             
        //             var messageSize = BitConverter.ToInt32(array.Array, 0);
        //             
        //             if(messageSize < _minMessageSize)
        //                 continue;
        //             
        //             var messageArray = new byte[messageSize];
        //             var messageBytes = await _socketProxy.ReceiveAsync(messageArray, SocketFlags.None);
        //
        //             OnDataReceived(messageArray, messageBytes, RemoteEndpoint);
        //         }
        //     }
        //     catch (Exception e)
        //     {
        //         Console.WriteLine(e);
        //         throw;
        //     }
        // }

        public override void Receive()
        {
            var cancellationToken = _cancellationTokenSource.Token;
            
            try
            {
                if (_socketProxy.Available > 0 && _running && !cancellationToken.IsCancellationRequested)
                {
                    var messageSizeArray = ReadFromStream(sizeof(int));
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

        public override void Send(byte[] data)
        {
            
        }

        public override async Task SendAsync(byte[] data)
        {
            var size = BitConverter.GetBytes(data.Length);
            await _networkStreamProxy.WriteAsync(size);
            await _networkStreamProxy.FlushAsync();
            await _networkStreamProxy.WriteAsync(data);
            await _networkStreamProxy.FlushAsync();
        }
        
        private async Task<byte[]> ReadFromStreamAsync(int nBytes)
        {
            var buffer = new byte[nBytes];
            var readPosition = 0;
        
            while (readPosition < nBytes)
            {
                readPosition += await _networkStreamProxy.ReadAsync(buffer, readPosition, nBytes - readPosition);
            }

            return buffer;
        }
        
        
        private byte[] ReadFromStream(int nBytes)
        {
            var buffer = new byte[nBytes];
            var readPosition = 0;
        
            while (readPosition < nBytes)
            {
                readPosition += _networkStreamProxy.Read(buffer, readPosition, nBytes - readPosition);
            }

            return buffer;
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