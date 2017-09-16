using DSharpPlus.Entities;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

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

            var eb = new DiscordEmbedBuilder()
            {
                ThumbnailUrl = malThumbnail,
                Color = new DiscordColor(9545092),
                ImageUrl = anime.Descendants("image").First().Value
            }
            .AddField(
                "**" + anime.Descendants("title").First().Value + ", " + anime.Descendants("english").First().Value + "**",
                "" + anime.Descendants("synonyms").First().Value)
            .AddField(
                "**Episode count:**",
                "" + anime.Descendants("episodes").First().Value,
                true)
            .AddField(
                "**Status:**",
                "" + anime.Descendants("status").First().Value,
                true)
            .AddField(
                "**Description**",
                "" + (StripHTML(anime.Descendants("synopsis").First().Value.Count() > 2000 ? anime.Descendants("synopsis").First().Value.Remove(1500) + "..." : anime.Descendants("synopsis").First().Value)))
            .AddField(
                "**Date:**",
                "" + anime.Descendants("start_date").First().Value + "-" + anime.Descendants("end_date").First().Value,
                true)
            .AddField(
                "**Type:**",
                "" + anime.Descendants("type").First().Value,
                true)
            .AddField(
                "**Score:**",
                "" + anime.Descendants("score").First().Value,
                true);
            return eb;
        }

        public async static Task<DiscordEmbed> Manga(string search)
        {
            var doc = XDocument.Parse(await GetMalLinkAStringAsync($"https://myanimelist.net/api/manga/search.xml?q={search}"));

            var manga = doc.Descendants("manga").First();

            var eb = new DiscordEmbedBuilder()
            {
                ThumbnailUrl = malThumbnail,
                Color = new DiscordColor(9545092),
                ImageUrl = manga.Descendants("image").First().Value
            }
            .AddField(
                "**" + manga.Descendants("title").First().Value + ", " + manga.Descendants("english").First().Value + "**",
                "" + manga.Descendants("synonyms").First().Value == ("") ? "N/A" : manga.Descendants("synonyms").First().Value)
            .AddField(
                "**Chapter | Volume count:**",
                "" + manga.Descendants("chapters").First().Value + " | " + manga.Descendants("volumes").First().Value,
                true)
            .AddField(
                "**Status:**",
                "" + manga.Descendants("status").First().Value,
                true)
            .AddField(
                "**Description**",
                "" + (StripHTML(manga.Descendants("synopsis").First().Value.Count() > 2000 ? manga.Descendants("synopsis").First().Value.Remove(1500) + "..." : manga.Descendants("synopsis").First().Value)))
            .AddField(
                "**Date:**",
                "" + manga.Descendants("start_date").First().Value + "-" + manga.Descendants("end_date").First().Value,
                true)
            .AddField(
                "**Type:**",
                "" + manga.Descendants("type").First().Value,
                true)
            .AddField(
                "**Score:**",
                "" + manga.Descendants("score").First().Value,
                true);
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