using System;
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

        public EMessageType GetMessageType(byte[] messageBytes)
        {
            var messageTypeSpan = new Span<byte>(messageBytes, 4, sizeof(int));
            var messageType = (EMessageType)BitConverter.ToInt32(messageTypeSpan);

            return messageType;
        }
        
    }
}