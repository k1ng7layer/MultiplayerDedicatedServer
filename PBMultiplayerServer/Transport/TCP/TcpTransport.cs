﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using PBMultiplayerServer.Core.Factories;
using PBMultiplayerServer.Core.Stream.Impl;

namespace PBMultiplayerServer.Transport.TCP
{
    public class TcpTransport : NetworkTransport
    {
        private readonly List<Action<Connection>> clientConnectedListeners = new();
        private readonly List<Task> _runningTasks = new();
        private readonly ISocketProxy _socket;
        private readonly EndPoint _transportIpEndPoint;
        private readonly List<Connection> _activeConnections = new ();
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private bool _running;
        private bool _disposedValue;

        public TcpTransport(ISocketProxy socketProxy, 
            EndPoint transportIpEndPoint)
        {
            _socket = socketProxy;
            _transportIpEndPoint = transportIpEndPoint;
        }
    
        public override async Task UpdateAsync()
        {
            if(_running)
                return;
            
            var cancellationToken = _cancellationTokenSource.Token;
            _socket.Bind(_transportIpEndPoint);
            _socket.Listen(1);
            
            _running = true;
            
            while (_running && !cancellationToken.IsCancellationRequested)
            {
                var clientSocket = await _socket.AcceptAsync();
                
                HandleNewConnection(clientSocket);
            }
        }

        public override void Start()
        {
            if(_running)
                return;
            
            _running = true;
            _socket.Bind(_transportIpEndPoint);
            _socket.Listen(10);
        }

        public override void Update()
        {
            try
            {
                Accept();
                
                foreach (var connection in _activeConnections)
                    connection.Receive();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public override void Stop()
        {
            _running = false;
            CloseConnections();
            _socket.Close();
        }

        private void Accept()
        {
            if (_socket.Poll(0, SelectMode.SelectRead))
            {
                var socketProxy = _socket.Accept();
                var tcpStream = new NetworkStreamProxy(socketProxy);
                var tcpConnection = new TcpConnection(socketProxy.RemoteEndpoint, socketProxy, tcpStream);
              
                _activeConnections.Add(tcpConnection);
                
                OnClientConnected(tcpConnection);
            }
        }

        private void CloseConnections()
        {
            foreach (var active in _activeConnections)
            {
                active.CloseConnection();
            }
            
            _activeConnections.Clear();
        }

        private async Task HandleNewConnection(ISocketProxy socketProxy)
        {
            var tcpStream = new NetworkStreamProxy(socketProxy);
            
            using (var tcpConnection = new TcpConnection(socketProxy.RemoteEndpoint, socketProxy, tcpStream))
            {
                try
                {
                    var task = tcpConnection.ReceiveAsync();
                    
                    _runningTasks.Add(task);
                    _activeConnections.Add(tcpConnection);
                    
                    OnClientConnected(tcpConnection);
                    
                    await task.ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        private void OnClientConnected(Connection socketProxy)
        {
            foreach (var listener in clientConnectedListeners)
            {
                listener.Invoke(socketProxy);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _socket.Dispose();
                    _cancellationTokenSource.Dispose();
                }

                _disposedValue = true;
            }
            
            base.Dispose(disposing);
        }
    }
}