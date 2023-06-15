using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using PBMultiplayerServer.Core.Factories;
using PBMultiplayerServer.Core.Messages;
using PBMultiplayerServer.Data;
using PBMultiplayerServer.Transport;

namespace PBMultiplayerServer.Core
{
    public interface IMultiplayerServer : IDisposable
    {
        ISocketProxyFactory SocketProxyFactory { get; set; }
        IMessageProvider MessageProvider { get; set; }
        IEnumerable<Client> ConnectedClients { get; }
        IDictionary<IPEndPoint, Connection> Connections { get; }
        bool IsRunning { get; }
        void Start();
        Task UpdateConnectionsAsync();
        void UpdateConnections();
        Task UpdateEventsAsync(int updateTickRate);
        void UpdateEvents();
        void Stop();
    }
}