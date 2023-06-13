using System.Collections.Generic;
using PBMultiplayerServer.Core.Messages;
using PBMultiplayerServer.Core.Messages.Factory;
using PBMultiplayerServer.Core.Messages.MessagePool;

namespace MultiplayerDedicatedServer.PBMultiplayerServer.Core.Messages.MessagePool
{
    public abstract class MessagePoolBase<T> : IMessagePool<T> where T : NetworkMessage
    {
        private readonly IMessageFactory<T> _messageFactory;
        private readonly Queue<T> _networkMessagesQueue = new();

        protected MessagePoolBase(IMessageFactory<T> messageFactory)
        {
            _messageFactory = messageFactory;
        }

        public T RetrieveMessage()
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

        public void ReturnMessage(T message)
        {
            
        }

        protected abstract void OnCreated(T message);
    }
}