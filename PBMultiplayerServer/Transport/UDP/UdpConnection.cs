using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using PBMultiplayerServer.Core.Factories;

namespace PBMultiplayerServer.Transport.UDP.Impls
{
    public class UdpConnection : Connection
    {
        private readonly ISocketProxy _socketProxy;
        private EndPoint _remoteEndPoint;
        private bool _running;
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private CancellationToken _cancellationToken;
        private bool _disposedValue;

        public UdpConnection(IPEndPoint remoteEndPoint, 
            ISocketProxy socketProxy) : base(remoteEndPoint)
        {
            _socketProxy = socketProxy;
        }

        public override void StartReceive()
        {
            if(_running)
                CloseConnection();
            
            _running = true;
            _cancellationToken = _cancellationTokenSource.Token;
        }

        public override async Task ReceiveAsync()
        {
            _cancellationToken = _cancellationTokenSource.Token;
            
            var data = new byte[1024];
            var iEndpoint = new IPEndPoint(IPAddress.Any, 0);
            try
            {
                while (!_cancellationToken.IsCancellationRequested)
                {
                    var receiveFromResult = await _socketProxy.ReceiveFromAsync(data, SocketFlags.None, iEndpoint);
                    OnDataReceived(data, receiveFromResult.ReceivedBytes, (IPEndPoint)receiveFromResult.RemoteEndPoint);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }

        public override void Receive()
        {
            if(!_running)
                return;
            
            try
            {
                var canReceive = true;
                
                var buffer = new byte[1024];
                var byteReceived = 0;
                
                while (canReceive && !_cancellationToken.IsCancellationRequested)
                {
                    if (_socketProxy.Available > 0 && _socketProxy.Poll(0, SelectMode.SelectRead))
                    {
                        byteReceived = _socketProxy.ReceiveFrom(buffer, SocketFlags.None, ref _remoteEndPoint);
                    }
                    else
                    {
                        canReceive = false;
                    }
                    
                    if(byteReceived > 0)
                        OnDataReceived(buffer, byteReceived, (IPEndPoint)_remoteEndPoint);
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
            _running = false;
            _socketProxy.Close();
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _socketProxy.Dispose();
                    _cancellationTokenSource.Dispose();
                }

                _disposedValue = true;
            }

            // Call base class implementation.
            base.Dispose(disposing);
        }
    }
}