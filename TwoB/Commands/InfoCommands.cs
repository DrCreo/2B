using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Linq;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace TwoB
{
    public class InfoCommands
    {

        [Command("ping"), Description("Returns the Socket latency.")]
        public async Task ping(CommandContext _context)
        {
            await _context.Channel.SendMessage($"**Socket latency:** {_context.Client.Ping}ms");
        }

        [Command("help") , Description("Sends help.")]
        public async Task Help(CommandContext _Context)
        {
            await _Context.Message.Respond("No help for you.");
        }
    }
}
