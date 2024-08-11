using CorePush.Firebase;
using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MobilePushClient.Android
{
    class Program
    {
        static async Task Main (string[] args)
        {
            var path = args.Length > 0 ? args[0] : Path.Combine(Directory.GetCurrentDirectory(), "google.json");
            var json = await File.ReadAllTextAsync(path);
            var config = JsonSerializer.Deserialize<GoogleConfig>(json);
            var settings = GetSettings(config);
            var response = await new FirebaseSender(settings, new HttpClient()).SendAsync(config.Payload);
            Console.WriteLine($"Response: {JsonSerializer.Serialize(response)}");        
        }

        private static FirebaseSettings GetSettings(GoogleConfig config)
        {
            return new FirebaseSettings(config.ProjectId, config.PrivateKey, config.ClientEmail, config.TokenUri);
        }
    }

    internal class GoogleConfig
    {
        [JsonPropertyName("project_id")]
        public string ProjectId { get; set; }

        [JsonPropertyName("private_key")]
        public string PrivateKey { get; set; }

        [JsonPropertyName("client_email")]
        public string ClientEmail { get; set; }

        [JsonPropertyName("token_uri")]
        public string TokenUri { get; set; }

        [JsonPropertyName("payload")]
        public GooglePayload Payload { get; set; }
    }

    internal class GooglePayload 
    {
        [JsonPropertyName("message")]
        public GoogleMessage Mesasge { get; set; }
    }

    internal class GoogleMessage
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("notification")]
        public GoogleNotification Notification { get; set; }
    }

    internal class GoogleNotification
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("body")]
        public string Body { get; set; }
    }
}