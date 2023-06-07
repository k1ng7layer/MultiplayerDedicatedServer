using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace PBMultiplayerServer.Transport
{
    public abstract class NetworkTransport : IDisposable
    {
        protected readonly List<Action<byte[], int, IPEndPoint>> _clientConnectedListeners = new();

        public void AddMessageReceivedListener(Action<byte[], int, IPEndPoint> clientConnectedCallback)
        {
            _clientConnectedListeners.Add(clientConnectedCallback);
        }
        
        public abstract Task UpdateAsync(CancellationToken cancellationToken);
        public abstract void Start();
        public abstract void Update();
        public abstract void Stop();

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}