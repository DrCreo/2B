using System;
using DSharpPlus.Commands;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Linq;
using DSharpPlus;

namespace TwoB
{
    public class AnimeCommands
    {
        const string malThumnail = "http://i.imgur.com/mo4I7Ff.jpg";

        public void Init(CommandModule _commands)
        {
            _commands.AddCommand("animanga_top", async e => await GetTopArticle(e));
            _commands.AddCommand("animanga_search", async e => await Search(e));
            _commands.AddCommand("anime", async e => await SearchMalAnime(e));
            _commands.AddCommand("manga", async e => await SearchMalManga(e));
        }

        private async Task Search(CommandEventArgs e)
        {
            Console.WriteLine(string.Join("+", e.Arguments));
            var jo = JObject.Parse(await WikiaHelper.SearchWikia(WikiaHelper.WikiaType.Animanga, string.Join("+", e.Arguments)));
            Console.WriteLine(jo.ToString());
            await e.Message.Respond(jo["items"][0]["url"].ToString());
        }

        private async Task SearchMalAnime(CommandEventArgs e)
        {
            var result = await MalHelper.Anime(string.Join("+", e.Arguments));
            await e.Message.Respond("", false, result);
        }

        private async Task SearchMalManga(CommandEventArgs e)
        {
            var result = await MalHelper.Manga(string.Join("+", e.Arguments));
            await e.Message.Respond("", false, result);
        }

        private async Task GetTopArticle(CommandEventArgs e)
        {
            var jo = JObject.Parse(await WikiaHelper.GetTopArticles(WikiaHelper.WikiaType.Animanga));
            await e.Message.Respond(jo["basepath"].ToString() + jo["items"][0]["url"].ToString());
        }
    }
}
