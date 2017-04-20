using System;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Linq;
using DSharpPlus;

namespace TwoB
{
    public class AnimeCommands
    {
        const string malThumnail = "http://i.imgur.com/mo4I7Ff.jpg";
        [Group("anime"), Aliases("a"), CanExecute, Description("Anime and Manga Commands.")]
        public class AnimeMangaGroup
        {

            [Command("mal_anime"), Aliases("mala"), Description("Searches mal for anime.")]
            public async Task SearchMalAnime(CommandContext context, [Description("Args to search")] params string[] searchargs)
            {
                var result = await MalHelper.Anime(string.Join("+", searchargs));
                await context.Message.Respond("", false, result);
            }

            [Command("mal_manga"), Aliases("malm"), Description("Searches mal for manga.")]
            public async Task SearchMalManga(CommandContext context, [Description("Args to search")] params string[] searchargs)
            {
                var result = await MalHelper.Manga(string.Join("+", searchargs));
                await context.Message.Respond("", false, result);
            }

            [Command("animanga_top"), Aliases("amtop"), Description("Returns the top article on the Animanga Wikia.")]
            public async Task GetTopArticle(CommandContext context)
            {
                var jo = JObject.Parse(await WikiaHelper.GetTopArticles(WikiaHelper.WikiaType.Animanga));
                await context.Message.Respond(jo["basepath"].ToString() + jo["items"][0]["url"].ToString());
            }

            [Command("animanga_search"), Aliases("asearch", "amsearch"), Description("Searches the Animanga wikia.")]
            public async Task Search(CommandContext context, [Description("Args to search.")] params string[] searchArgs)
            {
                var jo = JObject.Parse(await WikiaHelper.SearchWikia(WikiaHelper.WikiaType.Animanga, string.Join("+", searchArgs)));
                await context.Message.Respond(jo["items"][0]["url"].ToString());
            }

            public async Task ModuleCommand(CommandContext ctx)
            {
                await ctx.RespondAsync("Test﻿");
            }
        }
    }
}
