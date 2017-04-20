using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Commands;
using Newtonsoft.Json.Linq;
using System;

namespace TwoB
{
    class Bot
    {
        private BotConfig _botConfig { get; set; }
        private DiscordClient _client { get; set; }
        private CommandModule _commands { get; set; }

        public async Task Start()
        {

            // Init
            Initialize();
            SetUpEvents();
            InstallCommands();

            // Connect our client
            this._client.DebugLogger.Log("Connecting.");

            try
            {
                await this._client.Connect();
            }
            catch (Exception exc)
            {
                this._client.DebugLogger.Log(exc.StackTrace);
            }
            await Task.Delay(-1);
        }

        // lets Initialize the _botConfig and _client
        private void Initialize()
        {
            _botConfig = BotConfig.Instance;

            this._client = new DiscordClient(new DiscordConfig
            {
                Token = _botConfig.Token,
                TokenType = TokenType.Bot,
                LogLevel = LogLevel.Debug,
                AutoReconnect = true
            });
            this._client.DebugLogger.Log("Initialized.");
        }

        // Lets install our commands
        private void InstallCommands()
        {
            this._commands = this._client.UseCommands(new CommandConfig
            {
                Prefix = this._botConfig.Prefix,
                SelfBot = false
            });

            new NierCommands().Init(_commands);
            new AnimeCommands().Init(_commands);
            new InfoCommands().Init(_commands);
        }

        // Lets set up our events
        private void SetUpEvents()
        {
            this._client.DebugLogger.Log("Setting up events.");

            this._client.Ready += async () =>
            {
                this._client.DebugLogger.Log("Ready");
                this._client.DebugLogger.Log($"Current user is '{_client.Me.Username}' which is connected to {_client.Guilds.Count} Guild(s).");

                // Lets set the Status to something.
                await _client.UpdateStatus("Emotions are Prohibited");
            };

            /*this._client.MessageCreated += async (e) =>
            {
                // If the author is a bot return as we dont want to interact with bots.
                if (e.Message.Author.IsBot)
                    return;
            };*/
        }

    }
}
