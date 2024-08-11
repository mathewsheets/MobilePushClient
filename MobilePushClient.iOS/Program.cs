using CorePush.Apple;
using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MobilePushClient.iOS
{
    class Program
    {
        static async Task Main (string[] args)
        {
            var path = args.Length > 0 ? args[0] : Path.Combine(Directory.GetCurrentDirectory(), "apple.json");
            var json = await File.ReadAllTextAsync(path);
            var config = JsonSerializer.Deserialize<AppleConfig>(json);
            var settings = GetSettings(config);
            var response = await new ApnSender(settings, new HttpClient()).SendAsync(config.Payload, config.DeviceToken);
            Console.WriteLine($"Response: {JsonSerializer.Serialize(response)}");
        }

        private static ApnSettings GetSettings(AppleConfig config)
        {
            return new ApnSettings
            {
                AppBundleIdentifier = config.AppBundleIdentifier,
                P8PrivateKey = config.P8privateKey,
                P8PrivateKeyId = config.P8privateKeyId,
                TeamId = config.TeamId,
                ServerType = config.IsDevelopment ? ApnServerType.Development : ApnServerType.Production,
            };
        }
    }

    internal class AppleConfig
    {
        [JsonPropertyName("p8privateKey")]
        public string P8privateKey { get; set; }
        
        [JsonPropertyName("p8privateKeyId")]
        public string P8privateKeyId { get; set; }

        [JsonPropertyName("teamId")]
        public string TeamId { get; set; }

        [JsonPropertyName("appBundleIdentifier")]
        public string AppBundleIdentifier { get; set; }

        [JsonPropertyName("server")]
        public string Server { get; set; }

        public bool IsDevelopment { get { return Server.Equals("Development") ? true : false; }}

        [JsonPropertyName("deviceToken")]
        public string DeviceToken { get; set; }

        [JsonPropertyName("payload")]
        public ApsPayload Payload { get; set; }
    }

    internal class ApsPayload
    {
        [JsonPropertyName("aps")]
        public ApsMessage Aps { get; set; }
    }

    internal class ApsMessage
    {
        [JsonPropertyName("alert")]
        public string AlertBody { get; set; }
    }
}