using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using Newtonsoft.Json.Linq;
using System;

namespace TwoB
{
    class Bot
    {
        private BotConfig _botConfig { get; set; }
        private DiscordClient _client { get; set; }
        private CommandsNextModule _commands { get; set; }

        public async Task Start()
        {

            // Init
            Initialize();
            SetUpEvents();
            InstallCommands();
            _client.UseInteractivity();

            

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
            var cncfg = new CommandsNextConfiguration
            {
                Prefix = _botConfig.Prefix,
                EnableDms = true,
                EnableMentionPrefix = true,
                EnableDefaultHelp = true
            };

            this._commands = this._client.UseCommandsNext(cncfg);
            this._commands.CommandErrored += _commands_CommandErrored;
            this._commands.RegisterCommands<AnimeCommands>();
            this._commands.RegisterCommands<InfoCommands>();
            this._commands.RegisterCommands<NierCommands>();
        }


        private Task _commands_CommandErrored(CommandErrorEventArgs e)
        {
            this._client.DebugLogger.Log($"CommandsNext Exception:  { e.Exception.GetType()}: { e.Exception.Message} " + DateTime.Now);
            return Task.Delay(0);
        }

        // Lets set up our events
        private void SetUpEvents()
        {
            this._client.DebugLogger.Log("Setting up events.");

            this._client.SocketClosed += _client_SocketClosed;

            this._client.Ready += async () =>
            {
                this._client.DebugLogger.Log("Ready");
                this._client.DebugLogger.Log($"Current user is '{_client.Me.Username}' which is connected to {_client.Guilds.Count} Guild(s).");
                

                // Lets set the Status to something.
                await _client.UpdateStatus("Emotions are Prohibited");
            };
        }

        private Task _client_SocketClosed()
        {
            _client.DebugLogger.Log("Socket closed");
            return Task.Delay(0);
        }
    }
}
