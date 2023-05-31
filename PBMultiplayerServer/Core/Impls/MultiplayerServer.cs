using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using PBMultiplayerServer.Transport;
using PBMultiplayerServer.Transport.TCP;
using PBMultiplayerServer.Transport.UDP.Impls;

namespace PBMultiplayerServer.Core.Impls
{
    public class MultiplayerServer : IMultiplayerServer
    {
        private readonly IPAddress _ipAddress;
        private readonly int _port;
        private readonly Dictionary<IPEndPoint, ITransport> _connections = new();
        private ITransport _tcpTransport;
        private ITransport _udpTransport;
        private CancellationTokenSource _cancellationTokenSource;

        public MultiplayerServer(IPAddress ipAddress, int port)
        {
            _ipAddress = ipAddress;
            _port = port;
        }
        
        public bool IsRunning { get; private set; }
        
        public void Dispose()
        {
            _tcpTransport.Dispose();
            _udpTransport.Dispose();
        }
        
        public async Task RunUdpAsync()
        {
            IsRunning = true;
            
            CreateServerConnection();
            
            _cancellationTokenSource = new CancellationTokenSource();
            
            var updConnection = Task.Run(async () => await _udpTransport.ProcessAsync(_cancellationTokenSource.Token));
          
            await Task.WhenAll(updConnection);
        }

        public async Task RunTcpAsync()
        {
            IsRunning = true;
            
            var tcpConnection = Task.Run(async () => await _tcpTransport.ProcessAsync(_cancellationTokenSource.Token));
            
            await Task.WhenAll(tcpConnection);
        }

        public void RunTcp()
        {
            
        }

        public void RunUdp()
        {
            
        }
        
        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            
            IsRunning = false;
        }

        public void OnDataReceivedCallback(Action<byte[]> callback)
        {
            
        }

        private void CreateServerConnection()
        {
            var updPort = _port + 1;
            var iEndPointTcp = new IPEndPoint(_ipAddress, _port);
            var iEndPointUdp = new IPEndPoint(_ipAddress, updPort);
            
            _tcpTransport = new TcpTransport(iEndPointTcp);
            _udpTransport = new UdpTransport(iEndPointUdp);
        }
    }
}