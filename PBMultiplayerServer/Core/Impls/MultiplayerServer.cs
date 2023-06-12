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
        private IMessageProvider _messageProvider;
        private readonly Queue<IncomeMessage> _incomeMessageQueue = new();
        private Action _serverUpdateCallBack;
        private Action<EMessageType, byte[]> _messageReceivedHandler;

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

        public TimeSpan ServerTimeSpan { get; private set; }
        public TimeSpan ServerTickDeltaTimeSpan { get; private set; }
        public int ServerTickCount { get; private set; }
        public TimeSpan LastServerTickSpan { get; private set; }
        public IEnumerable<Client> ConnectedClients => _connectedClients;
        public IDictionary<IPEndPoint, Connection> Connections => _connections;
        public bool IsRunning { get; private set; }

        public void Start()
        {
            var messageFactory = new IncomeMessageFactory();
            _incomeMessagePool = new IncomeMessagePool(messageFactory);
            _messageProvider = new MessageProvider(_incomeMessagePool);
            CreateServerConnection();
            IsRunning = true;
            
            _tcpTransport.Start();
            _udpTransport.Start();
        }

        public Task UpdateAsync()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            var serverMainTasks = new List<Task>();

            var updateTickRate = int.Parse(_serverConfiguration["ServerUpdateTickRate"]);
            
            var serverTickTask = Task.Run(async () =>
            {
                while (IsRunning && !_cancellationTokenSource.IsCancellationRequested)
                {
                    var tickTimeMilliseconds = 1000 / updateTickRate;

                    await Task.Delay(tickTimeMilliseconds);
                    
                    var tickTimeSpan = TimeSpan.FromMilliseconds(tickTimeMilliseconds);
                    
                    ServerTimeSpan += tickTimeSpan;
                    ServerTickDeltaTimeSpan = ServerTimeSpan - LastServerTickSpan;
                    LastServerTickSpan = ServerTimeSpan;
                    ServerTickCount++;

                    if (_serverUpdateCallBack != null) _serverUpdateCallBack();
                }
            });

            var handleIncomeMessagesTask = Task.Run(async () =>
            {
                while (IsRunning && !_cancellationTokenSource.IsCancellationRequested)
                {
                    await Task.Delay(ServerTickDeltaTimeSpan.Milliseconds);
                    
                    ReadMessageQueue();
                }
            });
            
            var tcpConnectionTask = Task.Run(async () => await _tcpTransport.UpdateAsync());
            var updConnectionTask = Task.Run(async () =>await _udpTransport.UpdateAsync());
            
            serverMainTasks.Add(updConnectionTask);
            serverMainTasks.Add(tcpConnectionTask);
            serverMainTasks.Add(handleIncomeMessagesTask);
            serverMainTasks.Add(serverTickTask);
            
            return Task.WhenAll(serverMainTasks);
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

        public void AddServerTickHandler(Action tickHandler)
        {
            _serverUpdateCallBack = tickHandler;
        }

        public void AddMessageReceiveHandler(Action<EMessageType, byte[]> message)
        {
            _messageReceivedHandler = message;
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
            Console.WriteLine($"OnDataReceived, type = {messageType}");
            var message = _incomeMessagePool.RetrieveMessage();
            message.SetHeader(messageType, dataReceivedEventArgs.DataBuffer);
            
            _incomeMessageQueue.Enqueue(message);
        }

        private void ReadMessageQueue()
        {
            //Console.WriteLine("ReadMessageQueue");
            if (_incomeMessageQueue.Count > 0)
            {
                var message = _incomeMessageQueue.Dequeue();
                
                HandleIncomeMessage(message);
                
                _incomeMessagePool.ReturnMessage(message);
            }
        }

        private void HandleIncomeMessage(IncomeMessage message)
        {
            switch (message.MessageType)
            {
                case EMessageType.Connect:
                    break;
                case EMessageType.Reject:
                    break;
                case EMessageType.StartSession:
                    break;
                case EMessageType.JoinSession:
                    break;
            }
        }

        public void Dispose()
        {
            _tcpTransport.Dispose();
            _udpTransport.Dispose();
        }
    }
}