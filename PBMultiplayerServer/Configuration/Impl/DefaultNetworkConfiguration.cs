namespace PBMultiplayerServer.Configuration.Impl
{
    public class DefaultNetworkConfiguration : INetworkConfiguration
    {
        public int MinMessageSize { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
    }
}