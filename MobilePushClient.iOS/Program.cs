using System;
using System.IO;

using System.Net.Http;

using System.Threading.Tasks;

using CorePush.Apple;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace MobilePushClient.iOS {
    class Program {

        private static readonly HttpClient http = new HttpClient();

        static async Task Main (string[] args) {

            var path = args.Length > 0 ? args[0] : Path.Combine(Directory.GetCurrentDirectory(), "apple.json");
            var json = JObject.Parse(File.ReadAllText(path));

            var p8privateKey = json["p8privateKey"].ToString();
            var p8privateKeyId = json["p8privateKeyId"].ToString();
            var teamId = json["teamId"].ToString();
            var appBundleIdentifier = json["appBundleIdentifier"].ToString();
            var server = json["server"].ToString().Equals("Development") ? CorePush.Apple.ApnServerType.Development : CorePush.Apple.ApnServerType.Production;
            var deviceToken = json["deviceToken"].ToString();

            var settings = new ApnSettings
            {
                AppBundleIdentifier = appBundleIdentifier,
                P8PrivateKey = p8privateKey,
                P8PrivateKeyId = p8privateKeyId,
                TeamId = teamId,
                ServerType = server,
            };

            var notification = json["payload"].ToObject<AppleNotification>();

            var apn = new ApnSender(settings, http);
            var response = await apn.SendAsync(notification, deviceToken);
            Console.WriteLine($"Response: {JsonConvert.SerializeObject(response)}");
        }
    }

    public class AppleNotification {

        [JsonProperty ("aps")]
        public ApsPayload Aps { get; set; }

        public class ApsPayload {
            [JsonProperty ("alert")]
            public string AlertBody { get; set; }
        }

    }
}