using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using PBMultiplayerServer.Authentication;
using PBMultiplayerServer.Configuration;
using PBMultiplayerServer.Core.Abstractions;
using PBMultiplayerServer.Core.Factories;
using PBMultiplayerServer.Core.Messages;
using PBMultiplayerServer.Core.Messages.Impl;
using PBMultiplayerServer.Data;
using PBMultiplayerServer.Transport;
using PBMultiplayerServer.Transport.TCP;
using PBMultiplayerServer.Transport.UDP.Impls;

namespace PBMultiplayerServer.Core.Impls
{
    public class MultiplayerServer : MultiplayerServerBase,  
        IMultiplayerServer
    {
        private readonly INetworkConfiguration _networkConfiguration;
        private readonly Queue<PendingMessage> _incomeMessageQueue = new();
        private readonly List<Client> _connectedClients = new();
        private IMessageProvider _messageProvider;
        private ISocketProxyFactory _socketProxyFactory;
        private NetworkTransport _tcpTransport;
        private NetworkTransport _udpTransport;
        private CancellationTokenSource _cancellationTokenSource;
        private Dictionary<IPEndPoint, Connection> _connections = new();
        private Action<IncomeMessage> _incomeMessageCallback;
        private Func<byte[], LoginResult> _connectionApproveHandler;
        private Func<Task<byte[]>, LoginResult> _connectionApproveTaskHandler;

        public MultiplayerServer(INetworkConfiguration networkConfiguration)
        {
            _networkConfiguration = networkConfiguration;
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

        public Task UpdateConnectionsAsync()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            
            if(!IsRunning)
                return Task.FromCanceled(_cancellationTokenSource.Token);
            
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

        public void AddIncomeMessageListeners(Action<IncomeMessage> incomeMessageCallback)
        {
            _incomeMessageCallback = incomeMessageCallback;
        }
        
        public void AddApprovalCallback(Func<byte[], LoginResult> callback)
        {
            _connectionApproveHandler = callback;
        }
        
        public void AddApprovalCallback(Func<Task<byte[]>, LoginResult> callback)
        {
            _connectionApproveTaskHandler = callback;
        }
        
        private void CreateServerConnection()
        {
            var ipAddress = IPAddress.Parse(_networkConfiguration.IpAddress);
            var port = _networkConfiguration.Port;
            
            var updPort = port + 1;
            var iEndPointTcp = new IPEndPoint(ipAddress, port);
            var iEndPointUdp = new IPEndPoint(ipAddress, updPort);

            var tcpSocketListener = SocketProxyFactory.CreateSocketProxy(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var udpSocketListener =
                SocketProxyFactory.CreateSocketProxy(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            
            _tcpTransport = new TcpTransport(tcpSocketListener, iEndPointTcp, _networkConfiguration);
            _udpTransport = new UdpTransport(udpSocketListener, iEndPointUdp);
            
            _tcpTransport.AddMessageReceivedListener(OnDataReceived);
            _udpTransport.AddMessageReceivedListener(OnDataReceived);
        }

        private void OnDataReceived(DataReceivedEventArgs dataReceivedEventArgs)
        {
            var messageType = MessageProvider.GetMessageType(dataReceivedEventArgs.DataBuffer);
            Console.WriteLine($"OnDataReceived, type = {messageType}");
            var message = IncomeMessagePool.RetrieveMessage();
            message.SetHeader(messageType, dataReceivedEventArgs.DataBuffer);
            
            if(dataReceivedEventArgs.Sender.Approved || messageType is EMessageType.Connect or EMessageType.JoinSession)
                _incomeMessageQueue.Enqueue(new PendingMessage(message, dataReceivedEventArgs.Sender));
        }

        private void ReadMessageQueue()
        {
            while (_incomeMessageQueue.Count > 0)
            {
                var message = _incomeMessageQueue.Dequeue();
                
                HandleIncomeMessage(message);
                
                IncomeMessagePool.ReturnMessage(message.Message);
            }
        }

        private void HandleIncomeMessage(PendingMessage pendingMessage)
        {
            switch (pendingMessage.Message.MessageType)
            {
                case EMessageType.JoinSession:
                case EMessageType.Connect:
                    var loginResult = _connectionApproveHandler(pendingMessage.Message.Bytes);
                    if (loginResult.Result == ELoginResult.Success)
                        Approve(pendingMessage.Sender, loginResult.Message);
                    else Reject(pendingMessage.Sender, loginResult.Message);
                    _incomeMessageCallback.Invoke(pendingMessage.Message);
                    break;
                case EMessageType.StartSession:
                    break;
                    case EMessageType.Disconnect:
                        break;
                case EMessageType.Custom:
                    _incomeMessageCallback.Invoke(pendingMessage.Message);
                    break;
            }
        }

        private void Reject(Connection messageSender, string reason)
        {
            var message = _messageProvider.CreateConnectionRejectMessage(EMessageType.Reject, reason);
            
            messageSender.Send(message.Bytes);
            
            OutcomeMessagePool.ReturnMessage(message);
            
            _tcpTransport.CloseConnection(messageSender);
        }

        private void Approve(Connection messageSender, string message)
        {
            var client = new Client(messageSender);
            
            _connectedClients.Add(client);
        }

        public void Dispose()
        {
            _tcpTransport.Dispose();
            _udpTransport.Dispose();
        }
    }
}