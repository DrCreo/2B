using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace TwoB
{
    /*
    public class NierCommands
    {
        public void Init(CommandModule _commands)
        {
            _commands.AddCommand("nier_top", async e => await GetTopArticle(e));
            _commands.AddCommand("nier_search", async e => await Search(e));
        }

        private async Task Search(CommandEventArgs e)
        {
            Console.WriteLine(string.Join("+", e.Arguments));
            var jo = JObject.Parse(await WikiaHelper.SearchWikia(WikiaHelper.WikiaType.Nier, string.Join("+", e.Arguments)));
            Console.WriteLine(jo.ToString());
            await e.Message.Respond(jo["items"][0]["url"].ToString());
        }

        private async Task GetTopArticle(CommandEventArgs e)
        {
            var jo = JObject.Parse(await WikiaHelper.GetTopArticles(WikiaHelper.WikiaType.Nier));
            await e.Message.Respond(jo["basepath"].ToString() + jo["items"][0]["url"].ToString());
        }
    }
    */
}
