using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace MultiplayerDedicatedServer.Configuration.Impl
{
    public class DefaultConfiguration : IConfiguration
    {
        private readonly Dictionary<string, string> _inMemoryConfiguration = new();
        
        public void AddConfiguration(string key, string value)
        {
            if(!_inMemoryConfiguration.ContainsKey(key))
                _inMemoryConfiguration.Add(key, value);
            else
            {
                throw new InvalidOperationException(
                    $"[{nameof(DefaultConfiguration)}] configuration record with same key {key} already exists");
            }
        }

        public void AddJsonFile(string file)
        {
            var jsonString = File.ReadAllText(@$"D:\GITRepository\GameServer\GameServer\Server\Core\Configuration\FileConfiguration\{file}");
            var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);

            foreach (var resultKey in result.Keys)
            {
                if(!_inMemoryConfiguration.ContainsKey(resultKey))
                    _inMemoryConfiguration.Add(resultKey, result[resultKey]);
                else
                {
                    _inMemoryConfiguration[resultKey] = result[resultKey];
                }
            }
        }

        public string GetConfiguration(string key)
        {
            if (_inMemoryConfiguration.TryGetValue(key, out var value))
                return value;
            
            throw new InvalidOperationException(
                $"[{nameof(DefaultConfiguration)}] could not find configuration with key {key}");
        }

        public string this[string key]
        {
            get
            {
                if (_inMemoryConfiguration.TryGetValue(key, out var value))
                    return value;
                
                throw new InvalidOperationException(
                    $"[{nameof(DefaultConfiguration)}] could not find configuration with key {key}");
            }
        }
    }
}