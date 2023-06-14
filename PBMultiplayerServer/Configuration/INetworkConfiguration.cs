namespace PBMultiplayerServer.Configuration
{
    public interface INetworkConfiguration
    {
        int MinMessageSize { get; set; }
        string IpAddress { get; set; }
        int Port { get; set; }
    }
}