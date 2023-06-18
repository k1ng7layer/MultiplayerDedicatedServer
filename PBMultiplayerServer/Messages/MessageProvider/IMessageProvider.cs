using PBMultiplayerServer.Core.Messages.Impl;

namespace PBMultiplayerServer.Core.Messages
{
    public interface IMessageProvider
    {
        internal IncomeMessage CreateConnectionRejectMessage(EMessageType messageType, ERejectReason rejectReason);
        internal EMessageType GetMessageType(byte[] messageBytes);
    }
}