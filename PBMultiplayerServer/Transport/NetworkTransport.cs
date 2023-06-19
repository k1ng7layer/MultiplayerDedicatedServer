using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PBMultiplayerServer.Transport
{
    public abstract class NetworkTransport : IDisposable
    {
        protected readonly List<Action<DataReceivedEventArgs>> _receiveDataListeners = new();

        public void AddMessageReceivedListener(Action<DataReceivedEventArgs> dataReceivedEventArgs)
        {
            _receiveDataListeners.Add(dataReceivedEventArgs);
        }
        
        public abstract Task UpdateAsync();
        public abstract void Start();
        public abstract void Update();
        public abstract void Stop();
        public abstract void CloseConnection(Connection connection);

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