using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PBMultiplayerServer.Transport
{
    public abstract class NetworkTransport : IDisposable
    {
        protected readonly List<Func<DataReceivedEventArgs, Task>> _clientConnectedListeners = new();

        public void AddMessageReceivedListener(Func<DataReceivedEventArgs, Task> dataReceivedEventArgs)
        {
            _clientConnectedListeners.Add(dataReceivedEventArgs);
        }
        
        public abstract Task UpdateAsync();
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