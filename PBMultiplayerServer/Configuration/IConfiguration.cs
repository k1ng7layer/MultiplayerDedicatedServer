namespace MultiplayerDedicatedServer.Configuration
{
    public interface IConfiguration
    {
        void AddConfiguration(string key, string value);
        void AddJsonFile(string file);
        string GetConfiguration(string key);
        string this [string key] { get; }
    }
}