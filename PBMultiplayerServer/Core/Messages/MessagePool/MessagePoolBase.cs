using System.Collections.Generic;
using PBMultiplayerServer.Core.Messages;
using PBMultiplayerServer.Core.Messages.Factory;

namespace MultiplayerDedicatedServer.PBMultiplayerServer.Core.Messages.MessagePool
{
    public abstract class MessagePoolBase<T> where T : NetworkMessage
    {
        private readonly IMessageFactory<T> _messageFactory;
        private readonly Queue<T> _networkMessagesQueue = new();

        protected MessagePoolBase(IMessageFactory<T> messageFactory)
        {
            _messageFactory = messageFactory;
        }

        public T GetMessage()
        {
            T result;

            if (_networkMessagesQueue.Count != 0)
            {
                result = _networkMessagesQueue.Dequeue();
            }
            else
            {
                result = _messageFactory.Create();
            }

            OnCreated(result);
            
            return result;
        }

        protected abstract void OnCreated(T message);
    }
}