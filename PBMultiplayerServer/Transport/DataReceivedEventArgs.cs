namespace PBMultiplayerServer.Transport
{
    public class DataReceivedEventArgs
    {
        public DataReceivedEventArgs(byte[] dataBuffer, int amount, Connection sender)
        {
            DataBuffer = dataBuffer;
            Amount = amount;
            Sender = sender;
        }

        public byte[] DataBuffer { get; private set; }
        public int Amount { get; private set; }
        public Connection Sender { get; private set; }
    }
}