using DSharpPlus;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;

namespace TwoB
{
    public sealed class BotConfig
    {
        [JsonProperty("token")]
        public string Token { get; private set; }

        [JsonProperty("prefix")]
        public string Prefix { get; set; }

        [JsonProperty("developers")]
        public ulong[] Developers { get; set; }

        [JsonProperty("mal")]
        public MalConfig Mal { get; set; }

        [JsonProperty("music-channel-ids")]
        public ulong[] MusicChannelIds { get; set; }

        public DiscordClient Client { get; set; }

        private static BotConfig instance;

        public BotConfig() { }

        public static BotConfig Instance
        {
            get
            {
                if (instance == null)
                    CreateInstance();
                return instance;
            }
        }

        private static void CreateInstance()
        {

            var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            path = Path.Combine(path, "Config/config.json");

            if (!File.Exists(path))
                throw new FileNotFoundException($"'{path}' not found.");

            var json = string.Empty;
            using (var fileStream = File.OpenRead(path))
            using (var streamReader = new StreamReader(fileStream))
                json = streamReader.ReadToEnd();

            instance = JsonConvert.DeserializeObject<BotConfig>(json);
        }

        public class MalConfig
        {
            [JsonProperty("username")]
            public string UserName { get; set; }
            [JsonProperty("pass")]
            public string Pass { get; set; }

            public string Credentials
            {
                get
                {
                    return $"{UserName}:{Pass}";
                }
            }
        }
    }
}
