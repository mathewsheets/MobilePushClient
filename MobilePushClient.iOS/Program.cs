using System;
using System.IO;
using System.Threading.Tasks;

using CorePush.Apple;

using Newtonsoft.Json;
using static MobilePushClient.iOS.AppleNotification;
using Newtonsoft.Json.Linq;

namespace MobilePushClient.iOS {
    class Program {
        static async Task Main (string[] args) {

            var path = args.Length > 0 ? args[0] : Path.Combine(Directory.GetCurrentDirectory(), "apple.json");
            var json = JObject.Parse(File.ReadAllText(path));

            var p8privateKey = json["p8privateKey"].ToString();
            var p8privateKeyId = json["p8privateKeyId"].ToString();
            var teamId = json["teamId"].ToString();
            var appBundleIdentifier = json["appBundleIdentifier"].ToString();
            var server = json["server"].ToString().Equals("Development") ? CorePush.Apple.ApnServerType.Development : CorePush.Apple.ApnServerType.Production;
            var deviceToken = json["deviceToken"].ToString();

            var notification = json["payload"].ToObject<AppleNotification>();

            using (var apn = new ApnSender (p8privateKey, p8privateKeyId, teamId, appBundleIdentifier, server)) {

                var result = await apn.SendAsync (notification, deviceToken);
                Console.WriteLine($"Result: {JsonConvert.SerializeObject(result)}");
            }
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