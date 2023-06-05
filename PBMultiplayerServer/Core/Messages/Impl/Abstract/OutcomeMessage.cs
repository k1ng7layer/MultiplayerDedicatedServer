namespace PBMultiplayerServer.Core.Messages.Impl
{
    public abstract class OutcomeMessage : NetworkMessage
    {
        protected abstract int DataSize { get; }
        public int MessageSize => HEADERS_LENGTH + DataSize;

        public void SetMessage(EMessageType messageType, 
            EMessageSendMode messageSendMode)
        {
            AddInt((int)messageType);
        }
    }
}