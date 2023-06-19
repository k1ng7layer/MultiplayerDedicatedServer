using System;
using PBMultiplayerServer.Core.Messages.Impl;
using PBMultiplayerServer.Core.Messages.MessagePool;

namespace PBMultiplayerServer.Core.Messages
{
    public class MessageProvider : IMessageProvider
    {
        private readonly IMessagePool<IncomeMessage> _incomeMessagePool;
        private readonly IMessagePool<OutcomeMessage> _outcomeMessagePool;

        public MessageProvider(IMessagePool<IncomeMessage> incomeMessagePool, 
            IMessagePool<OutcomeMessage> outcomeMessagePool)
        {
            _incomeMessagePool = incomeMessagePool;
            _outcomeMessagePool = outcomeMessagePool;
        }

        OutcomeMessage IMessageProvider.CreateConnectionRejectMessage(EMessageType messageType, string rejectReason)
        {
            var message = _outcomeMessagePool.RetrieveMessage();
          
            message.AddInt((int)messageType);
            message.AddString(rejectReason);
            
            return message;
        }

        EMessageType IMessageProvider.GetMessageType(byte[] messageBytes)
        {
            var messageTypeSpan = new Span<byte>(messageBytes, 0, sizeof(int));
            var messageType = (EMessageType)BitConverter.ToInt32(messageTypeSpan);

            return messageType;
        }
        
    }
}