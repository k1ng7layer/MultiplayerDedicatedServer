using System.Net;

namespace PBMultiplayerServer.Transport
{
    public abstract class Connection
    {
        public IPEndPoint RemoteEndpoint { get; protected set; }

        public Connection(IPEndPoint remoteEndpoint)
        {
            RemoteEndpoint = remoteEndpoint;
        }
    }
}