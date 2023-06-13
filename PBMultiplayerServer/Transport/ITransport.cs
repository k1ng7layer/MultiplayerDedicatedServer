using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace PBMultiplayerServer.Transport
{
    public interface ITransport : IDisposable
    {
        Task ProcessAsync(CancellationToken cancellationToken);
        void AddMessageReceivedListener(Action<byte[], int, IPEndPoint> clientConnectedCallback);
    }
}