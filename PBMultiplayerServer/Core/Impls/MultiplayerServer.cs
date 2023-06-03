using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using PBMultiplayerServer.Core.Factories;
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
        private readonly ISocketProxyFactory _socketProxyFactory;
        private readonly EProtocolType _protocolType;
        private ITransport _tcpTransport;
        private ITransport _udpTransport;
        private CancellationTokenSource _cancellationTokenSource;

        public MultiplayerServer(IPAddress ipAddress, int port, 
            ISocketProxyFactory socketProxyFactory, 
            EProtocolType protocolType)
        {
            _ipAddress = ipAddress;
            _port = port;
            _socketProxyFactory = socketProxyFactory;
            _protocolType = protocolType;
        }
        
        public bool IsRunning { get; private set; }
        
        public void Dispose()
        {
            _tcpTransport.Dispose();
            _udpTransport.Dispose();
        }
        
        public async Task RunAsync()
        {
            IsRunning = true;
            
            CreateServerConnection();
            
            _cancellationTokenSource = new CancellationTokenSource();

            var connectionTasks = new List<Task>();
            
            if (_protocolType == EProtocolType.TCP)
            {
                var tcpConnection = Task.Run(async () => await _tcpTransport.ProcessAsync(_cancellationTokenSource.Token));
                connectionTasks.Add(tcpConnection);
            }
            else if(_protocolType == EProtocolType.UDP)
            {
                var updConnection = Task.Run(async () => await _udpTransport.ProcessAsync(_cancellationTokenSource.Token));
                connectionTasks.Add(updConnection);
            }
            else
            {
                var tcpConnection = Task.Run(async () => await _tcpTransport.ProcessAsync(_cancellationTokenSource.Token));
                var updConnection = Task.Run(async () => await _udpTransport.ProcessAsync(_cancellationTokenSource.Token));
                
                connectionTasks.Add(updConnection);
                connectionTasks.Add(tcpConnection);
            }
                
            await Task.WhenAll(connectionTasks);
        }

        public void Run()
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

            var tcpSocketListener = _socketProxyFactory.CreateSocketProxy(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var udpSocketListener =
                _socketProxyFactory.CreateSocketProxy(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            
            _tcpTransport = new TcpTransport(tcpSocketListener, iEndPointTcp);
            _udpTransport = new UdpTransport(udpSocketListener, iEndPointUdp);
        }
    }
}