namespace PBMultiplayerServer.Core.Messages.Impl
{
    public class OutcomeMessage : NetworkMessage
    {
        protected int DataSize { get; }
        public int MessageSize => HEADERS_LENGTH + DataSize;

        public void SetMessage(EMessageType messageType, 
            EMessageSendMode messageSendMode)
        {
            AddInt((int)messageType);
        }
    }
}