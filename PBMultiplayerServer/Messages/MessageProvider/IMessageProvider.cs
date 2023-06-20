using PBMultiplayerServer.Core.Messages.Impl;

namespace PBMultiplayerServer.Core.Messages
{
    public interface IMessageProvider
    {
        internal OutcomeMessage CreateConnectionRejectMessage(EMessageType messageType, string rejectReason);
        internal EMessageType GetMessageType(byte[] messageBytes);
    }
}