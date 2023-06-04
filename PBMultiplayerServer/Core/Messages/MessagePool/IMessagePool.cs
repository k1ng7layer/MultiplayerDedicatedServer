namespace PBMultiplayerServer.Core.Messages.MessagePool
{
    public interface IMessagePool<T> where T : INetworkMessage
    {
        T GetMessage();
        void ReturnMessage(T message);
    }
}