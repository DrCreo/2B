using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using System;
using System.Threading.Tasks;

namespace TwoB
{
    class Bot
    {
        private DiscordClient _client { get; set; }
        private CommandsNextModule _commands { get; set; }
        private MusicModule _musMod { get; set; }

        /// <summary>
        /// Starts the Bot.
        /// </summary>
        /// <returns></returns>
        public async Task Start()
        {

            // Init
            Initialize();
            SetUpEvents();
            InstallCommands();
            _client.UseInteractivity();

            // Connect our client
            this._client.DebugLogger.Log("Connecting.");


            await this._client.ConnectAsync();

            await Task.Delay(-1);
        }

        /// <summary>
        /// Initialized the DiscordClient
        /// </summary>
        private void Initialize()
        {
            this._client = new DiscordClient(new DiscordConfiguration
            {
                Token = BotConfig.Instance.Token,
                TokenType = TokenType.Bot,
                LogLevel = LogLevel.Debug,
                AutoReconnect = true
            });
            this._client.DebugLogger.Log("Initialized.");

            // Pass a refrence to the DiscordClient to the BotConfig singleton
            BotConfig.Instance.Client = this._client;

        }

        /// <summary>
        /// Installs Commands.
        /// </summary>
        private void InstallCommands()
        {
            var cncfg = new CommandsNextConfiguration
            {
                StringPrefix = BotConfig.Instance.Prefix,
                EnableDms = true,
                EnableMentionPrefix = true,
                EnableDefaultHelp = true
            };

            this._commands = this._client.UseCommandsNext(cncfg);
            this._commands.RegisterCommands<AnimeCommands>();
            this._commands.RegisterCommands<InfoCommands>();
            this._commands.RegisterCommands<NierCommands>();
        }


        /// <summary>
        /// Sets up Discord Events.
        /// </summary>
        private void SetUpEvents()
        {
            this._client.DebugLogger.Log("Setting up events.");

            this._client.Heartbeated += _client_HeartBeated;

            this._client.SocketOpened += _client_SocketOpened;


            this._client.Ready += async (e) =>
            {
                this._client.DebugLogger.Log("Ready");
                this._client.DebugLogger.Log($"Current user is '{_client.CurrentUser.Username}' which is connected to {_client.Guilds.Count} Guild(s).");

                SetupMusicModule();

                // Lets set the Status to something.
                await _client.UpdateStatusAsync(new Game("Emotions are Prohibited"));
            };


            this._client.MessageCreated += async (e) =>
            {
                if (e.Message.Content.ToLower() == $"{BotConfig.Instance.Prefix}restart mm" && e.Author.IsDeveloper())
                {
                    await e.Message.RespondAsync("Restarting Music Module now.");

                    _musMod.EmergencyDisconnect();
                    await Task.Delay(5000);
                    _musMod = null;
                    _musMod = new MusicModule();
                    _musMod.StartMusicModule();
                    await e.Message.RespondAsync("Restart Complete.");
                }
            };
        }


        /// <summary>
        /// Setup the MusicModule
        /// </summary>
        private void SetupMusicModule()
        {
            _musMod = new MusicModule();
            _musMod.StartMusicModule();
        }

        /// <summary>
        /// DiscordClient Socket Opened Event. 
        /// </summary>
        /// <returns></returns>
        private Task _client_SocketOpened()
        {
            _client.DebugLogger.Log("Socket opened");
            return Task.Delay(0);
        }

        /// <summary>
        /// DiscordClient Heart Beated Event.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private Task _client_HeartBeated(HeartbeatEventArgs e)
        {
            _client.DebugLogger.Log($"Heart Beat: {e.Ping}ms");
            return Task.Delay(0);
        }

        /// <summary>
        /// DiscordClient Socket Closed Event.
        /// </summary>
        /// <returns></returns>
        private Task _client_SocketClosed()
        {
            _client.DebugLogger.Log("Socket closed");
            return Task.Delay(0);
        }
    }
}
