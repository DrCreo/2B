using System;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Linq;
using DSharpPlus;
using System.Collections.Generic;

namespace TwoB
{
    public class AnimeCommands
    {
        // Url to the My anime list logo
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
                DiscordEmbed eb = new DiscordEmbed()
                {
                    Title = "Guess the Waifu!",
                    Color = 13411044,
                    Image = new DiscordEmbedImage()
                    {
                        Url = waifu.ImageURL
                    }
                };
                await context.Message.Respond("", false, eb);

                DiscordMessage m = await context.Client.GetInteractivityModule().WaitForMessageAsync(xm => (xm.Content.ToLower() == waifu.Name.ToLower() || 
                waifu.Aliases.Any(s => s.Equals(xm.Content, StringComparison.OrdinalIgnoreCase))), TimeSpan.FromSeconds(20));

                if (m == null)
                {
                    DiscordEmbed eb_wrong = new DiscordEmbed()
                    {
                        Title = "Guess the Waifu!",
                        Color = 13411044,
                        Description = "Noone guessed correctly.. how unpleasant.",
                        Thumbnail = new DiscordEmbedThumbnail()
                        {
                            Url = "https://s-media-cache-ak0.pinimg.com/originals/25/89/05/258905bad951ef5c45f3c716a60b1f68.gif"
                        },
                        Fields = new List<DiscordEmbedField>()
                        {
                            new DiscordEmbedField()
                            {
                                Name = "**Waifu:**",
                                Value = waifu.Name,
                                Inline = true
                            },
                            new DiscordEmbedField()
                            {
                                Name = "**Aliases:**",
                                Value = string.Join("\n",waifu.Aliases),
                                Inline = true
                            },
                            new DiscordEmbedField()
                            {
                                Name = "**Origin:**",
                                Value = string.Join("\n",waifu.Origin),
                                Inline = true
                            }
                        }
                    };
                    await context.Message.Respond("", false, eb_wrong);
                    return;
                }

                DiscordEmbed eb2 = new DiscordEmbed()
                {
                    Title = "Guess the Waifu!",
                    Color = 13411044,
                    Description = "And the Waifu was:",
                    Thumbnail = new DiscordEmbedThumbnail()
                    {
                        Url = m.Author.AvatarUrl
                    },
                    Fields = new List<DiscordEmbedField>()
                    {
                        new DiscordEmbedField()
                        {
                            Name = "**Waifu:**",
                            Value = waifu.Name,
                            Inline = true
                        },
                        new DiscordEmbedField()
                        {
                            Name = "**Winner:**",
                            Value = m.Author.Username,
                            Inline = true
                        },
                        new DiscordEmbedField()
                        {
                            Name = "**Aliases:**",
                            Value = string.Join("\n",waifu.Aliases),
                            Inline = true
                        },
                         new DiscordEmbedField()
                        {
                            Name = "**Origin:**",
                            Value = string.Join("\n",waifu.Origin),
                            Inline = true
                        }
                    }
                };

                await context.Message.Respond("", false, eb2);
            }

            /// <summary>
            /// Checks the current count of Waifus in the database
            /// </summary>
            /// <param name="context"></param>
            /// <returns></returns>
            [Command("waifu_count"), Aliases("waicount", "fucount", "wfc"), Description("Count of Waifus")]
            public async Task WaifuCount(CommandContext context)
            {
                DiscordEmbed eb = new DiscordEmbed()
                {
                    Title = "Waifu Database.",
                    Color = 13411044,
                    Fields = new List<DiscordEmbedField>()
                    {
                        new DiscordEmbedField()
                        {
                            Name = "**Waifu Count:**",
                            Value = WaifuDB.DB.Waifus.Count.ToString(),
                            Inline = true
                        }
                    }
                };

                await context.Message.Respond("", false, eb);
            }
            #endregion

            public async Task ModuleCommand(CommandContext ctx)
            {
                await ctx.RespondAsync("Test﻿");
            }    
        }
    }
}
