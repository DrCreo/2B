using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;

namespace TwoB
{
    public class InfoCommands
    {

        [Command("ping"), Description("Returns the Socket latency.")]
        public async Task ping(CommandContext _context)
        {
            await _context.Channel.SendMessageAsync($"**Socket latency:** {_context.Client.Ping}ms");
        }

        /*
        [Command("help") , Description("Sends help.")]
        public async Task Help(CommandContext _Context)
        {
            await _Context.Message.Respond("No help for you.");
        }
        */
    }
}
