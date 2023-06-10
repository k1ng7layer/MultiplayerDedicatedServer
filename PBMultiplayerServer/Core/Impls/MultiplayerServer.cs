using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using PBMultiplayerServer.Core.Factories;
using PBMultiplayerServer.Core.Messages;
using PBMultiplayerServer.Data;
using PBMultiplayerServer.Transport;
using PBMultiplayerServer.Transport.TCP;
using PBMultiplayerServer.Transport.UDP.Impls;

namespace PBMultiplayerServer.Core.Impls
{
    public class MultiplayerServer : IMultiplayerServer
    {
        private readonly IPAddress _ipAddress;
        private readonly int _port;
        private readonly object _locker = new ();
        private readonly ISocketProxyFactory _socketProxyFactory;
        private readonly EProtocolType _protocolType;
        private NetworkTransport _tcpTransport;
        private NetworkTransport _udpTransport;
        private CancellationTokenSource _cancellationTokenSource;
        private Dictionary<IPEndPoint, Connection> _connections = new();
        private readonly List<Action<DataReceivedEventArgs>> _dataReceivedListeners = new();
        private List<Client> _connectedClients;
        private Predicate<bool> _connectionApprovalHandler;

        public MultiplayerServer(IPAddress ipAddress, 
            int port, 
            ISocketProxyFactory socketProxyFactory, 
            EProtocolType protocolType)
        {
            _ipAddress = ipAddress;
            _port = port;
            _socketProxyFactory = socketProxyFactory;
            _protocolType = protocolType;
        }

        public IEnumerable<Client> ConnectedClients => _connectedClients;
        public IDictionary<IPEndPoint, Connection> Connections => _connections;
        
        public bool IsRunning { get; private set; }

        public void Start()
        {
            CreateServerConnection();
            IsRunning = true;
            
            _tcpTransport.Start();
            _udpTransport.Start();
        }

        public async Task RunAsync()
        {
            if(!IsRunning)
                return;
            
            _cancellationTokenSource = new CancellationTokenSource();

            var connectionTasks = new List<Task>();
            
            if (_protocolType == EProtocolType.TCP)
            {
                var tcpConnection = Task.Run(async () => await _tcpTransport.UpdateAsync());
                connectionTasks.Add(tcpConnection);
            }
            else if(_protocolType == EProtocolType.UDP)
            {
                var updConnection = Task.Run(async () => await _udpTransport.UpdateAsync());
                connectionTasks.Add(updConnection);
            }
            else
            {
                var tcpConnection = Task.Run(async () => await _tcpTransport.UpdateAsync());
                var updConnection = Task.Run(async () => await _udpTransport.UpdateAsync());
                
                connectionTasks.Add(updConnection);
                connectionTasks.Add(tcpConnection);
            }
            
            await Task.WhenAll(connectionTasks);
        }

        public void Run()
        {
            if(!IsRunning)
                return;
            
            _tcpTransport.Update();
            _udpTransport.Update();
        }
        
        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            
            IsRunning = false;
            
            _tcpTransport.Stop();
            _udpTransport.Stop();
        }

        public void OnClientConnectedCallback(Action<Client> clientConnectedCallback, NetworkMessage message)
        {
            
        }

        public void AddConnectionApprovalHandler(Func<bool> connectionApprovalHandler)
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
            
            _tcpTransport.AddMessageReceivedListener(OnDataReceived);
            _udpTransport.AddMessageReceivedListener(OnDataReceived);
        }

        private void OnDataReceived(DataReceivedEventArgs dataReceivedEventArgs)
        {
            Console.WriteLine($"received data amount {dataReceivedEventArgs.Amount}");
        }

        private void OnClientConnected(Connection connection)
        {
            lock (_locker)
            {
                if(!_connections.ContainsKey(connection.RemoteEndpoint))
                    _connections.Add(connection.RemoteEndpoint, connection);
            }
        }
        
        public void Dispose()
        {
            _tcpTransport.Dispose();
            _udpTransport.Dispose();
        }
    }
}