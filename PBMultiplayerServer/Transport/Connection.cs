using System.Net;

namespace PBMultiplayerServer.Transport
{
    public abstract class Connection
    {
        public IPEndPoint RemoteEndpoint { get; }

        public Connection(IPEndPoint remoteEndpoint)
        {
            RemoteEndpoint = remoteEndpoint;
        }
    }
}