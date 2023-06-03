using System;
using System.Net;

namespace PBMultiplayerServer.Core.Data
{
    public class ReceivedMessage
    {
        public IPEndPoint EndPoint;
        public ArraySegment<int> Data;
    }
}