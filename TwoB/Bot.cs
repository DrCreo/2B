using System.Threading.Tasks;
using DSharpPlus;
using System.IO;
using Newtonsoft.Json;
using System.Reflection;

namespace TwoB
{
    class Bot
    {
        private BotConfig _botConfig { get; set; }
        private DiscordClient _client { get; set; }

        public async Task Start()
        {

            // Init
            Initialize();
            this._client.DebugLogger.Log("Initialized.");
            this._client.DebugLogger.Log("Setting up events.");
            SetUpEvents();

            // Connect our client
            this._client.DebugLogger.Log("Connecting.");
            await this._client.Connect();

            await Task.Delay(-1);
        }

        private void SetUpEvents()
        {
            this._client.Ready += async () =>
            {
                this._client.DebugLogger.Log("Ready");
                this._client.DebugLogger.Log($"Current user is '{_client.Me.Username}' which is connected to {_client.Guilds.Count} Guild(s).");

                // Lets set the Status to something.
                await _client.UpdateStatus("Emotions are Prohibited");
            };

            this._client.MessageCreated += async (e) =>
            {
                // If the author is a bot return as we dont want to interact with bots.
                if (e.Message.Author.IsBot)
                    return;

                // use a switch statement to handle commands for now will be switched
                // out with a command module later.
                switch (e.Message.Content)
                {
                    case "||ping":
                        await e.Channel.SendMessage($"**Socket latency:** {_client.Ping}ms");
                        break;
                    case "||help":
                        var eb = new DiscordEmbed() { Color = 4589319, Description = "**OS Chip Functionality:** \n||ping" };
                        await e.Message.Respond("", false, eb);
                        break;
                }
            };
        }

        private void Initialize()
        {
            var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            path = Path.Combine(path, "Config/config.json");

            if (!File.Exists(path))
                throw new FileNotFoundException($"'{path}' not found.");

            var json = string.Empty;
            using (var fileStream = File.OpenRead(path))
            using (var streamReader = new StreamReader(fileStream))
                json = streamReader.ReadToEnd();

            _botConfig = JsonConvert.DeserializeObject<BotConfig>(json);

            this._client = new DiscordClient(new DiscordConfig
            {
                Token = this._botConfig.Token,
                TokenType = TokenType.Bot,
                LogLevel = LogLevel.Debug,
                AutoReconnect = true
            });
        }
    }
}
