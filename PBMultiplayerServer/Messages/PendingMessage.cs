using PBMultiplayerServer.Core.Messages.Impl;
using PBMultiplayerServer.Transport;

namespace PBMultiplayerServer.Core.Messages
{
    public readonly struct PendingMessage
    {
        public readonly IncomeMessage Message;
        public readonly Connection Sender;

        public PendingMessage(IncomeMessage message, Connection sender)
        {
            Message = message;
            Sender = sender;
        }
    }
}