using System.Net;

namespace PBMultiplayerServer.Transport.Interfaces
{
    public interface IDataReceivedListener
    {
        void OnDataReceived(byte[] data, int byteCount, IPEndPoint remoteEndpoint);
    }
}