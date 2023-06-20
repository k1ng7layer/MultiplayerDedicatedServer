using System;
using System.Numerics;

namespace PBMultiplayerServer.Core.Messages.MessageHelper
{
    public static class MessageReader
    {
         public static ushort GetUshort(this NetworkMessage message, int startIndex)
        {
            var intSpan = new Span<byte>(message.Bytes, startIndex, sizeof(ushort));
            var result = BitConverter.ToUInt16(intSpan);

            return result;
        }
        
        public static ushort GetUshort(this NetworkMessage message)
        {
            var intSpan = new Span<byte>(message.Bytes, message.WritePos, sizeof(ushort));
            var result = BitConverter.ToUInt16(intSpan);
            
            message.WritePos += sizeof(ushort);

            return result;
        }
    
        public static int GetInt(this NetworkMessage message)
        {
            var intSpan = new Span<byte>(message.Bytes, message.WritePos, sizeof(int));
            var result = BitConverter.ToInt32(intSpan);
            
            message.WritePos += sizeof(int);
            
            return result;
        }
        
        public static int GetInt(this NetworkMessage message, byte[] data, int startIndex)
        {
            var intSpan = new Span<byte>(data, startIndex, sizeof(int));
            var result = BitConverter.ToInt32(intSpan);

            return result;
        }
        
        public static float GetFloat(this NetworkMessage message)
        {
            var intSpan = new Span<byte>(message.Bytes, message.WritePos, sizeof(float));
            var result = BitConverter.ToSingle(intSpan);
            
            message.WritePos += sizeof(float);
            
            return result;
        }

        public static string GetString(this NetworkMessage message, byte[] data, int startIndex, int length)
        {
            var intSpan = new Span<byte>(data, startIndex, length);
            var value = intSpan.ToArray();
            var str = System.Text.Encoding.Default.GetString(value);
            var result = BitConverter.ToString(value);

            return str;
        }

        public static bool GetBool(this NetworkMessage message)
        {
            var span = new Span<byte>(message.Bytes, message.WritePos, 1);
            var value = BitConverter.ToBoolean(span);
            
            message.WritePos++;
            
            return value;
        }
        
        public static bool GetBool(this NetworkMessage message, byte[] data, int startIndex)
        {
            var span = new Span<byte>(data, startIndex, 1);
            var value = BitConverter.ToBoolean(span);

            message.WritePos++;
            
            return value;
        }
        
        public static Vector3 GetVector3(this NetworkMessage message, int startIndex)
        {
            var length = sizeof(float);
            var xSpan = new Span<byte>(message.Bytes, startIndex, length);
            var ySpan = new Span<byte>(message.Bytes, startIndex + length, length);
            var zSpan = new Span<byte>(message.Bytes, startIndex + length * 2, length);
            
            var x = BitConverter.ToSingle(xSpan);
            var y = BitConverter.ToSingle(ySpan);
            var z = BitConverter.ToSingle(zSpan);

            message.WritePos += 12;
            
            return new Vector3(x, y, z);
        }
        
        public static Vector3 GetVector3(this NetworkMessage message)
        {
            var x = GetFloat(message);
            var y = GetFloat(message);
            var z = GetFloat(message);
            
            return new Vector3(x, y, z);
        }
        
        public static Quaternion GetQuaternion(this NetworkMessage message)
        {
            var x = GetFloat(message);
            var y = GetFloat(message);
            var z = GetFloat(message);
            var w = GetFloat(message);

            return new Quaternion(x, y, z, w);
        }
    }
}