namespace PBMultiplayerServer.Core.Messages.Impl
{
    public class IncomeMessage : NetworkMessage
    {
        public EMessageType MessageType { get; private set; }
        
        public void SetHeader(EMessageType messageType, byte[] data)
        {
            MessageType = messageType;
        }
    }
}