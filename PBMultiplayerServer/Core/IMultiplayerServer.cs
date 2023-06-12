using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using PBMultiplayerServer.Core.Messages;
using PBMultiplayerServer.Data;
using PBMultiplayerServer.Transport;

namespace PBMultiplayerServer.Core
{
    public interface IMultiplayerServer : IDisposable
    {
        float ServerTick { get;}
        IEnumerable<Client> ConnectedClients { get; }
        IDictionary<IPEndPoint, Connection> Connections { get; }
        bool IsRunning { get; }
        void Start();
        Task UpdateAsync();
        void Update();
        void Stop();
        void OnClientConnectedCallback(Action<Client> clientConnectedCallback, NetworkMessage message);
        void AddConnectionApprovalHandler(Func<byte[],bool> connectionApprovalHandler);
    }
}