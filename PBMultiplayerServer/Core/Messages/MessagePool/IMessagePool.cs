namespace PBMultiplayerServer.Core.Messages.MessagePool
{
    public interface IMessagePool<T> where T : INetworkMessage
    {
        T RetrieveMessage();
        void ReturnMessage(T message);
    }
}