namespace PBMultiplayerServer.Core.Messages.MessagePool
{
    public interface IPooledMessage
    {
        void OnPooled();
        void Reset();
    }
}