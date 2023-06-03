using System;
using System.Threading;
using System.Threading.Tasks;

namespace PBMultiplayerServer.Transport
{
    public interface ITransport : IDisposable
    {
        Task ProcessAsync(CancellationToken cancellationToken);
        void AddClientConnectedListener(Action<Connection> clientConnectedCallback);
    }
}