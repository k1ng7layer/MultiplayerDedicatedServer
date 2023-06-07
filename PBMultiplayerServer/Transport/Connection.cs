using System.Net;
using System.Threading.Tasks;

namespace PBMultiplayerServer.Transport
{
    public abstract class Connection
    {
        public IPEndPoint RemoteEndpoint { get; }

        public Connection(IPEndPoint remoteEndpoint)
        {
            RemoteEndpoint = remoteEndpoint;
        }

        public abstract Task ReceiveAsync();
        public abstract void Receive();
        public abstract void CloseConnection();

    }
}