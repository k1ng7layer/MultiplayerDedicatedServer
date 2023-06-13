namespace PBMultiplayerServer.Utils
{
    public class ByteBuffer
    {
        private const int  INITIAL_SIZE = 4;
        
        private byte[] _buffer;

        public ByteBuffer()
        {
            _buffer = new byte[INITIAL_SIZE];
        }

        public ByteBuffer(int size)
        {
            _buffer = new byte[size];
        }
        
        
    }
}