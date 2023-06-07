using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using PBMultiplayerServer.Transport.Interfaces;

namespace PBMultiplayerServer.Transport
{
    public abstract class Connection : IDisposable
    {
        private readonly List<IDataReceivedListener> _dataReceivedListeners = new();
        
        public Connection(IPEndPoint remoteEndpoint)
        {
            RemoteEndpoint = remoteEndpoint;
        }
        
        public IPEndPoint RemoteEndpoint { get; }

        public abstract void StartReceive();
        public abstract Task ReceiveAsync();
        public abstract void Receive();
        public abstract void CloseConnection();
        
        public void AddDataReceivedListener(IDataReceivedListener dataReceivedListener)
        {
            _dataReceivedListeners.Add(dataReceivedListener);
        }

        protected virtual void OnDataReceived(byte[] buffer, int byteCount, IPEndPoint ipEndPoint)
        {
            foreach (var dataReceivedListener in _dataReceivedListeners)
            {
                dataReceivedListener.OnDataReceived(buffer, byteCount, ipEndPoint);
            }
        }

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