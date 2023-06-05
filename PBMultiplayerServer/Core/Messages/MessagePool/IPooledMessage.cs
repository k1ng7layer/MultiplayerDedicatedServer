namespace PBMultiplayerServer.Core.Messages.MessagePool
{
    public interface IPooledMessage
    {
        void OnRetrieved();
        void Reset();
    }
}