using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

using CorePush.Google;

using Newtonsoft.Json;

using Newtonsoft.Json.Linq;

namespace MobilePushClient.Android {
    class Program {

        private static readonly HttpClient http = new HttpClient();

        static async Task Main (string[] args) {

            var path = args.Length > 0 ? args[0] : Path.Combine(Directory.GetCurrentDirectory(), "google.json");
            var json = JObject.Parse(File.ReadAllText(path));

            var serverKey = json["serverKey"].ToString();
            var senderId = json["senderId"].ToString();
            var deviceToken = json["deviceToken"].ToString();

            var settings = new FcmSettings
            {
                SenderId = senderId,
                ServerKey = serverKey
            };

            var notification = json["payload"].ToObject<GoogleNotification>();

            var fcm = new FcmSender(settings, http);
            var response = await fcm.SendAsync(deviceToken, notification);
            Console.WriteLine($"Response: {JsonConvert.SerializeObject(response)}");
        }
    }

    public class GoogleNotification {

        [JsonProperty ("notification")]
        public Payload Data { get; set; }

        public class Payload {

            [JsonProperty ("title")]
            public string Title { get; set; }

            [JsonProperty ("body")]
            public string Body { get; set; }
        }
    }
}