using System;
using System.IO;
using System.Threading.Tasks;

using CorePush.Google;

using Newtonsoft.Json;

using Newtonsoft.Json.Linq;
using static MobilePushClient.Android.GoogleNotification;

namespace MobilePushClient.Android {
    class Program {
        static async Task Main (string[] args) {

            var path = args.Length > 0 ? args[0] : Path.Combine(Directory.GetCurrentDirectory(), "google.json");
            var json = JObject.Parse(File.ReadAllText(path));

            var serverKey = json["serverKey"].ToString();
            var senderId = json["senderId"].ToString();
            var deviceToken = json["deviceToken"].ToString();

            var notification = json["payload"].ToObject<GoogleNotification>();

            using (var fcm = new FcmSender (serverKey, senderId)) {

                var result = await fcm.SendAsync (deviceToken, notification);
                Console.WriteLine($"Result: {JsonConvert.SerializeObject(result)}");
            }
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