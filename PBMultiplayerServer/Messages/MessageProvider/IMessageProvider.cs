using PBMultiplayerServer.Core.Messages.Impl;

namespace PBMultiplayerServer.Core.Messages
{
    public interface IMessageProvider
    {
        OutcomeMessage CreateConnectionRejectMessage(EMessageType messageType, ERejectReason rejectReason);
        EMessageType GetMessageType(byte[] messageBytes);
    }
}