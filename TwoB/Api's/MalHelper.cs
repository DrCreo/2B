using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using System;
using System.Text.RegularExpressions;
using DSharpPlus;
using System.Collections.Generic;

namespace TwoB
{
    public static class MalHelper
    {
        const string malThumbnail = "http://i.imgur.com/mo4I7Ff.jpg";

        public async static Task<DiscordEmbed> Anime(string search)
        {
            string[] result = { string.Empty, string.Empty };
            var doc = XDocument.Parse(await GetMalLinkAStringAsync($"https://myanimelist.net/api/anime/search.xml?q={search}"));

            var anime = doc.Descendants("anime").First();

            var eb = new DiscordEmbed()
            {
                Thumbnail = new DiscordEmbedThumbnail() { Url = malThumbnail },
                Color = 9545092,
                Fields = new List<DiscordEmbedField>()
                {
                    new DiscordEmbedField()
                    {
                        Name = "**"+anime.Descendants("title").First().Value+", "+anime.Descendants("english").First().Value+"**",
                        Value = anime.Descendants("synonyms").First().Value
                    },
                    new DiscordEmbedField()
                    {
                        Name = "**Episode count:**",
                        Value = anime.Descendants("episodes").First().Value,
                        Inline = true
                    },
                    new DiscordEmbedField()
                    {
                        Name = "**Description**",
                        Value = StripHTML(anime.Descendants("synopsis").First().Value)
                    },
                    new DiscordEmbedField()
                    {
                        Name = "**Date:**",
                        Value = anime.Descendants("start_date").First().Value + "-" + anime.Descendants("end_date").First().Value,
                        Inline = true
                    },
                    new DiscordEmbedField()
                    {
                        Name = "**Type:**",
                        Value = anime.Descendants("type").First().Value,
                        Inline = true
                    },
                    new DiscordEmbedField()
                    {
                        Name = "**Score:**",
                        Value = anime.Descendants("score").First().Value,
                        Inline = true
                    }
                },
                Image = new DiscordEmbedImage() { Url = anime.Descendants("image").First().Value }
            };
            return eb;
        }

        public async static Task<DiscordEmbed> Manga(string search)
        {
            var doc = XDocument.Parse(await GetMalLinkAStringAsync($"https://myanimelist.net/api/manga/search.xml?q={search}"));

            var manga = doc.Descendants("manga").First();

            var eb = new DiscordEmbed()
            {
                Thumbnail = new DiscordEmbedThumbnail() { Url = malThumbnail },
                Color = 9545092,
                Fields = new List<DiscordEmbedField>()
                {
                    new DiscordEmbedField()
                    {
                        Name = "**"+manga.Descendants("title").First().Value+", "+manga.Descendants("english").First().Value+"**",
                        Value = "" + manga.Descendants("synonyms").First().Value == ("") ? "N/A" : manga.Descendants("synonyms").First().Value
                    },
                    new DiscordEmbedField()
                    {
                        Name = "**Chapter | Volume count:**",
                        Value = "" + manga.Descendants("chapters").First().Value + " | " + manga.Descendants("volumes").First().Value,
                        Inline = true
                    },
                    new DiscordEmbedField()
                    {
                        Name = "**Status:**",
                        Value = "" + manga.Descendants("status").First().Value,
                        Inline = true
                    },
                    new DiscordEmbedField()
                    {
                        Name = "**Description**",
                        Value = "" + (StripHTML(manga.Descendants("synopsis").First().Value.Count() > 2000 ? manga.Descendants("synopsis").First().Value.Remove(1500)+ "..." : manga.Descendants("synopsis").First().Value))
                    },
                    new DiscordEmbedField()
                    {
                        Name = "**Date:**",
                        Value = "" + manga.Descendants("start_date").First().Value + "-" + manga.Descendants("end_date").First().Value,
                        Inline = true
                    },
                    new DiscordEmbedField()
                    {
                        Name = "**Type:**",
                        Value = "" + manga.Descendants("type").First().Value,
                        Inline = true
                    },
                    new DiscordEmbedField()
                    {
                        Name = "**Score:**",
                        Value = "" + manga.Descendants("score").First().Value,
                        Inline = true
                    }
                },
                Image = new DiscordEmbedImage() { Url = manga.Descendants("image").First().Value }
            };
            return eb;
        }

        private static async Task<string> GetMalLinkAStringAsync(string link)
        {
            var httpClient = new HttpClient();
            var byteArray = Encoding.ASCII.GetBytes(BotConfig.Instance.Mal.Credentials);
            var header = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            httpClient.DefaultRequestHeaders.Authorization = header;
            var response = await httpClient.GetAsync(link);
            var content = response.Content;
            return await content.ReadAsStringAsync();
        }

        public static string StripHTML(string input)
        {
            return Regex.Replace(input, "(?:<style.+?>.+?</style>|<script.+?>.+?</script>|<(?:!|/?[a-‌​zA-Z]+).*?/?>)", String.Empty).Replace("[i]", "").Replace("[/i]", "");
        }
    }
}