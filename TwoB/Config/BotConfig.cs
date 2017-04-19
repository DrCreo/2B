using Newtonsoft.Json;

namespace TwoB
{
    public sealed class BotConfig
    {
        [JsonProperty("token")]
        public string Token { get; private set; }

        [JsonProperty("prefix")]
        public string Prefix { get; set; }
    }
}
