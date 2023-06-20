using System;
using System.Numerics;
using System.Text;

namespace PBMultiplayerServer.Core.Messages
{
    public abstract class NetworkMessage : INetworkMessage
    {
        protected const int HEADERS_LENGTH = 8;
        protected const int INITIAL_SIZE = 1024;

        public int WritePos { get; internal set; }
        public byte[] Bytes { get; protected set; }
        
        public virtual void OnRetrieved()
        {
            
        }

        public virtual void Reset()
        {
            
        }

        public void AddInt(int value)
        {
            var bytes = BitConverter.GetBytes(value);

            Bytes[WritePos] = bytes[0];
            Bytes[WritePos + 1] = bytes[1];
            Bytes[WritePos + 2] = bytes[2];
            Bytes[WritePos + 3] = bytes[3];

            WritePos += sizeof(int);
        }
        
        public void AddFloat(float value)
        {
            var bytes = BitConverter.GetBytes(value);
            
            Bytes[WritePos] = bytes[0];
            Bytes[WritePos + 1] = bytes[1];
            Bytes[WritePos + 2] = bytes[2];
            Bytes[WritePos + 3] = bytes[3];
            
            WritePos += sizeof(float);
        }
        
        public void AddString(string value)
        {
            var bytes = Encoding.ASCII.GetBytes(value);
            
            for (int i = 0; i < bytes.Length; i++)
            {
                Bytes[WritePos + i] = bytes[i];
            }
            
            WritePos += bytes.Length;
        }
        
        public void AddBytes(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                Bytes[WritePos + i] = bytes[i];
            }
            
            WritePos += bytes.Length;
        }
        
        public void AddUshort(ushort value)
        {
            var bytes = BitConverter.GetBytes(value);

            Bytes[WritePos] = bytes[0];
            Bytes[WritePos + 1] = bytes[1];

            WritePos += sizeof(ushort);
        }
        
        protected void AddBool(bool value)
        {
            var bytes = BitConverter.GetBytes(value);

            Bytes[WritePos] = bytes[0];

            WritePos += 1;
        }
        
        public void AddVector3(Vector3 value)
        {
            AddFloat(value.X);
            AddFloat(value.Y);
            AddFloat(value.Z);
        }
        
        public void AddQuaternion(Quaternion value)
        {
            AddFloat(value.X);
            AddFloat(value.Y);
            AddFloat(value.Z);
            AddFloat(value.W);
        }
    }
}