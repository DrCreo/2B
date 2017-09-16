using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace TwoB
{

    public class NierCommands
    {
        [Command("nier_search"), Description("Searches the nier Wikia."), Aliases("snier")]
        public async Task Search(CommandContext _context, [Description("Args to search.")] params string[] searchArgs)
        {
            var jo = JObject.Parse(await WikiaHelper.SearchWikia(WikiaHelper.WikiaType.Nier, string.Join("+", searchArgs)));
            await _context.Message.RespondAsync(jo["items"][0]["url"].ToString());
        }

        [Command("nier_top"), Description("Returns the top Article on the Nier Wikia."), Aliases("tnier")]
        public async Task GetTopArticle(CommandContext _context)
        {
            var jo = JObject.Parse(await WikiaHelper.GetTopArticles(WikiaHelper.WikiaType.Nier));
            await _context.Message.RespondAsync(jo["basepath"].ToString() + jo["items"][0]["url"].ToString());
        }
    }
}
