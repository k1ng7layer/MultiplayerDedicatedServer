namespace PBMultiplayerServer.Core.Messages.Factory
{
    public interface IMessageFactory<T> where T : NetworkMessage
    {
        T Create(params object[] args);
    }
}