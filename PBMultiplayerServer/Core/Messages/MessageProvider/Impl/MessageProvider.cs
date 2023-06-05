using PBMultiplayerServer.Core.Messages.Impl;
using PBMultiplayerServer.Core.Messages.MessagePool;

namespace PBMultiplayerServer.Core.Messages
{
    public class MessageProvider : IMessageProvider
    {
        private readonly IMessagePool<OutcomeMessage> _messagePool;

        public MessageProvider(IMessagePool<OutcomeMessage> messagePool)
        {
            _messagePool = messagePool;
        }

        public OutcomeMessage CreateConnectionRejectMessage(EMessageType messageType, ERejectReason rejectReason)
        {
            var message = _messagePool.RetrieveMessage();
          
            message.AddInt((int)messageType);
            message.AddInt((int)rejectReason);
            
            return message;
        }
    }
}