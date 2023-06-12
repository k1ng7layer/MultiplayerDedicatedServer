using PBMultiplayerServer.Core.Messages.Impl;

namespace PBMultiplayerServer.Core.Messages
{
    public interface IMessageProvider
    {
        IncomeMessage CreateConnectionRejectMessage(EMessageType messageType, ERejectReason rejectReason);
        EMessageType GetMessageType(byte[] messageBytes);
    }
}