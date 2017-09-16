using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TwoB
{
    public class AnimeCommands
    {
        // Url to the My anime list logo
        const string malThumnail = "http://i.imgur.com/mo4I7Ff.jpg";

        [Group("anime"), Aliases("a"), Description("Anime and Manga Commands.")]
        public class AnimeMangaGroup
        {
            
            [Command("mal_anime"), Aliases("mala"), Description("Searches mal for anime.")]
            public async Task SearchMalAnime(CommandContext context, [Description("Args to search")] params string[] searchargs)
            {
                var result = await MalHelper.Anime(string.Join("+", searchargs));
                await context.Message.RespondAsync("", false, result);
            }

            [Command("mal_manga"), Aliases("malm"), Description("Searches mal for manga.")]
            public async Task SearchMalManga(CommandContext context, [Description("Args to search")] params string[] searchargs)
            {
                var result = await MalHelper.Manga(string.Join("+", searchargs));
                await context.Message.RespondAsync("", false, result);
            }

            [Command("animanga_top"), Aliases("amtop"), Description("Returns the top article on the Animanga Wikia.")]
            public async Task GetTopArticle(CommandContext context)
            {
                var jo = JObject.Parse(await WikiaHelper.GetTopArticles(WikiaHelper.WikiaType.Animanga));
                await context.Message.RespondAsync(jo["basepath"].ToString() + jo["items"][0]["url"].ToString());
            }

            [Command("animanga_search"), Aliases("asearch", "amsearch"), Description("Searches the Animanga wikia.")]
            public async Task Search(CommandContext context, [Description("Args to search.")] params string[] searchArgs)
            {
                var jo = JObject.Parse(await WikiaHelper.SearchWikia(WikiaHelper.WikiaType.Animanga, string.Join("+", searchArgs)));
                await context.Message.RespondAsync(jo["items"][0]["url"].ToString());
            }

            #region Waifu DB stuff

            /// <summary>
            /// Guess that waifu mini games
            /// </summary>
            /// <param name="context"></param>
            /// <returns></returns>
            [Command("waifu"), Aliases("wai", "fu"), Description("Guess that waifu")]
            public async Task Waifu(CommandContext context)
            {
                Random rng = new Random();
                var waifu = WaifuDB.DB.Waifus[rng.Next(WaifuDB.DB.Waifus.Count)];
                var eb = new DiscordEmbedBuilder()
                {
                    Title = "Guess the Waifu!",
                    Color = new DiscordColor(13411044),
                    ImageUrl = waifu.ImageURL
                    
                };
                await context.Message.RespondAsync("", false, eb);

                var m = await context.Client.GetInteractivityModule().WaitForMessageAsync(xm => (xm.Content.ToLower() == waifu.Name.ToLower() || 
                waifu.Aliases.Any(s => s.Equals(xm.Content, StringComparison.OrdinalIgnoreCase))), TimeSpan.FromSeconds(20));

                if (m == null)
                {
                    var eb_wrong = new DiscordEmbedBuilder()
                    {
                        Title = "Guess the Waifu!",
                        Color = new DiscordColor(13411044),
                        Description = "Noone guessed correctly.. how unpleasant.",
                        ThumbnailUrl =  "https://s-media-cache-ak0.pinimg.com/originals/25/89/05/258905bad951ef5c45f3c716a60b1f68.gif",
                    }.AddField(
                        "**Waifu:**",
                        waifu.Name,
                        true)
                    .AddField(
                        "**Aliases:**",
                        string.Join("\n", waifu.Aliases),
                        true)
                    .AddField(
                        "**Origin:**",
                        string.Join("\n", waifu.Origin),
                        true);
                    await context.Message.RespondAsync("", false, eb_wrong);
                    return;
                }

                var eb2 = new DiscordEmbedBuilder()
                {
                    Title = "Guess the Waifu!",
                    Color = new DiscordColor(13411044),
                    Description = "And the Waifu was:",
                    ThumbnailUrl = m.User.AvatarUrl,
                }
                .AddField(
                     "**Waifu:**",
                     waifu.Name,
                     true)
                .AddField(
                    "**Winner:**",
                    m.User.Username,
                    true)
                .AddField(
                    "**Aliases:**",
                    string.Join("\n", waifu.Aliases),
                    true)
                    .AddField(
                    "**Origin:**",
                    string.Join("\n", waifu.Origin),
                    true);

                await context.Message.RespondAsync("", false, eb2);
            }

            /// <summary>
            /// Checks the current count of Waifus in the database
            /// </summary>
            /// <param name="context"></param>
            /// <returns></returns>
            [Command("waifu_count"), Aliases("waicount", "fucount", "wfc"), Description("Count of Waifus")]
            public async Task WaifuCount(CommandContext context)
            {
                var eb = new DiscordEmbedBuilder()
                {
                    Title = "Waifu Database.",
                    Color = new DiscordColor(13411044),
                }
                .AddField(
                    "**Waifu Count:**",
                    WaifuDB.DB.Waifus.Count.ToString(),
                    true
                    );

                await context.Message.RespondAsync("", false, eb);
            }
            #endregion

            public async Task ModuleCommand(CommandContext ctx)
            {
                await ctx.RespondAsync("Test﻿");
            }    
        }
    }
}
