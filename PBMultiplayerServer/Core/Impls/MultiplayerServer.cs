using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using MultiplayerDedicatedServer.Configuration;
using PBMultiplayerServer.Core.Factories;
using PBMultiplayerServer.Core.Messages;
using PBMultiplayerServer.Core.Messages.Factory.Impl;
using PBMultiplayerServer.Core.Messages.Impl;
using PBMultiplayerServer.Core.Messages.MessagePool;
using PBMultiplayerServer.Core.Messages.MessagePool.Impl;
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
        private readonly IConfiguration _serverConfiguration;
        private readonly ISocketProxyFactory _socketProxyFactory;
        private readonly EProtocolType _protocolType;
        private NetworkTransport _tcpTransport;
        private NetworkTransport _udpTransport;
        private CancellationTokenSource _cancellationTokenSource;
        private Dictionary<IPEndPoint, Connection> _connections = new();
        private readonly List<Action<DataReceivedEventArgs>> _dataReceivedListeners = new();
        private List<Client> _connectedClients;
        private Predicate<byte[]> _connectionApprovalHandler;
        private IMessagePool<IncomeMessage> _incomeMessagePool;
        private readonly IMessageProvider _messageProvider;
        private readonly Queue<IncomeMessage> _incomeMessageQueue = new();

        public MultiplayerServer(IPAddress ipAddress, 
            int port, 
            ISocketProxyFactory socketProxyFactory, 
            EProtocolType protocolType, 
            IConfiguration serverConfiguration)
        {
            _ipAddress = ipAddress;
            _port = port;
            _socketProxyFactory = socketProxyFactory;
            _protocolType = protocolType;
            _serverConfiguration = serverConfiguration;
        }

        public float ServerTick { get; private set; }
        public float LastServerTick { get; private set; }

        public IEnumerable<Client> ConnectedClients => _connectedClients;
        public IDictionary<IPEndPoint, Connection> Connections => _connections;
        
        public bool IsRunning { get; private set; }

        public void Start()
        {
            var messageFactory = new IncomeMessageFactory();
            _incomeMessagePool = new IncomeMessagePool(messageFactory);
            CreateServerConnection();
            IsRunning = true;
            
            _tcpTransport.Start();
            _udpTransport.Start();
        }

        public async Task UpdateAsync()
        {
            if(!IsRunning)
                return;
            
            _cancellationTokenSource = new CancellationTokenSource();

            var connectionTasks = new List<Task>();
            
            var tcpConnection = Task.Run(async () => await _tcpTransport.UpdateAsync());
            var updConnection = Task.Run(async () => await _udpTransport.UpdateAsync());

            var handleIncomeMessagesTask = Task.Run(async () =>
            {
                var messageReadTickRate = float.Parse(_serverConfiguration["ReceiveTickRate"]);
                
                while (IsRunning)
                {
                    if (ServerTick - LastServerTick >= (1 / messageReadTickRate) || messageReadTickRate <= 0)
                    {
                        while (_incomeMessageQueue.Count > 0)
                        {
                            ReadMessageQueue();
                        }
                    }
                }
            });
            
            connectionTasks.Add(updConnection);
            connectionTasks.Add(tcpConnection);
            connectionTasks.Add(handleIncomeMessagesTask);
            
            await Task.WhenAll(connectionTasks);
        }

        public void Update()
        {
            if(!IsRunning)
                return;
            
            _tcpTransport.Update();
            _udpTransport.Update();
            
            ReadMessageQueue();
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

        public void AddConnectionApprovalHandler(Func<byte[], bool> connectionApprovalHandler)
        {
            
        }

        public void ReadMessage()
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

        private async Task OnDataReceived(DataReceivedEventArgs dataReceivedEventArgs)
        {
            var messageType = _messageProvider.GetMessageType(dataReceivedEventArgs.DataBuffer);
            var message = _incomeMessagePool.RetrieveMessage();
            message.SetHeader(messageType, dataReceivedEventArgs.DataBuffer);
            
            _incomeMessageQueue.Enqueue(message);
        }

        private void ReadMessageQueue()
        {
            if (_incomeMessageQueue.Count > 0)
            {
                var message = _incomeMessageQueue.Dequeue();
                
                HandleMessage(message);
                
                _incomeMessagePool.ReturnMessage(message);
            }
        }

        private void HandleMessage(NetworkMessage message)
        {
            
        }

        private void OnClientConnected(TcpConnection connection)
        {
            var messageLength = connection.ReadFromStream(4);
        }

        public void Dispose()
        {
            _tcpTransport.Dispose();
            _udpTransport.Dispose();
        }
    }
}