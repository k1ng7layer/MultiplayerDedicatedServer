using PBMultiplayerServer.Transport;

namespace PBMultiplayerServer.Data
{
    public class Client
    {
        public Client(Connection clientConnection)
        {
            ClientConnection = clientConnection;
        }
        public Connection ClientConnection { get;}
        public ushort ClientId { get; private set; }
    }
}