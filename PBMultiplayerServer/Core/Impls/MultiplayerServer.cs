using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using PBMultiplayerServer.Configuration;
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
        private readonly object _locker = new ();
        private readonly INetworkConfiguration _networkConfiguration;
        private readonly ISocketProxyFactory _socketProxyFactory;
        private readonly Queue<IncomeMessage> _incomeMessageQueue = new();
        private NetworkTransport _tcpTransport;
        private NetworkTransport _udpTransport;
        private CancellationTokenSource _cancellationTokenSource;
        private Dictionary<IPEndPoint, Connection> _connections = new();
        private List<Client> _connectedClients;
        private IMessagePool<IncomeMessage> _incomeMessagePool;
        private IMessageProvider _messageProvider;
        
        public MultiplayerServer(ISocketProxyFactory socketProxyFactory,
            INetworkConfiguration networkConfiguration)
        {
            _socketProxyFactory = socketProxyFactory;
            _networkConfiguration = networkConfiguration;
        }
        
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

        public Task UpdateConnectionsAsync()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            var serverMainTasks = new List<Task>();
            
            var tcpConnectionTask = Task.Run(async () => await _tcpTransport.UpdateAsync());
            var updConnectionTask = Task.Run(async () => await _udpTransport.UpdateAsync());
            
            serverMainTasks.Add(updConnectionTask);
            serverMainTasks.Add(tcpConnectionTask);

            return Task.WhenAll(serverMainTasks);
        }

        public void UpdateConnections()
        {
            if(!IsRunning)
                return;
            
            _tcpTransport.Update();
            _udpTransport.Update();
        }

        public async Task UpdateEventsAsync(int updateTickRateMilliseconds)
        {
            while (IsRunning && !_cancellationTokenSource.IsCancellationRequested)
            {
                ReadMessageQueue();
                await Task.Delay(updateTickRateMilliseconds);
            }
        }

        public void UpdateEvents()
        {
            if(!IsRunning)
                return;
            
            ReadMessageQueue();
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            
            IsRunning = false;
            
            _tcpTransport.Stop();
            _udpTransport.Stop();
        }

        private void CreateServerConnection()
        {
            var ipAddress = IPAddress.Parse(_networkConfiguration.IpAddress);
            var port = _networkConfiguration.Port;
            
            var updPort = port + 1;
            var iEndPointTcp = new IPEndPoint(ipAddress, port);
            var iEndPointUdp = new IPEndPoint(ipAddress, updPort);

            var tcpSocketListener = _socketProxyFactory.CreateSocketProxy(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var udpSocketListener =
                _socketProxyFactory.CreateSocketProxy(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            
            _tcpTransport = new TcpTransport(tcpSocketListener, iEndPointTcp, _networkConfiguration);
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
            while (_incomeMessageQueue.Count > 0)
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