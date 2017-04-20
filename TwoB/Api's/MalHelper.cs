using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using System;
using System.Text.RegularExpressions;

namespace TwoB
{
    public static class MalHelper
    {
        public async static Task<string[]> Anime(string search)
        {
            string[] result = { string.Empty, string.Empty };
            var doc = XDocument.Parse(await GetMalLinkAStringAsync($"https://myanimelist.net/api/anime/search.xml?q={search}"));

            var anime = doc.Descendants("anime").First();

            result[0] += "**Title:** `" + anime.Descendants("title").First().Value + "`";
            result[0] += "\n**English Title:** `" + anime.Descendants("english").First().Value + "`";
            result[0] += "\n**Synonyms:** `" + anime.Descendants("synonyms").First().Value + "`";
            result[0] += "\n**Episode count:** `" + anime.Descendants("episodes").First().Value + "`";
            result[0] += "\n**Score:** `" + anime.Descendants("score").First().Value + "`";
            result[0] += "\n**Type:** `" + anime.Descendants("type").First().Value + "`";
            result[0] += "\n**Status:** `" + anime.Descendants("status").First().Value + "`";
            result[0] += "\n**Start Date:** `" + anime.Descendants("start_date").First().Value + "`";
            result[0] += "\n**End Date:** `" + anime.Descendants("end_date").First().Value + "`";
            result[0] += "\n\n**Description:** \n" + StripHTML(anime.Descendants("synopsis").First().Value) + "\n";
            result[1] += anime.Descendants("image").First().Value;

            return result;
        }

        public async static Task<string[]> Manga(string search)
        {
            string[] result = { string.Empty, string.Empty };
            var doc = XDocument.Parse(await GetMalLinkAStringAsync($"https://myanimelist.net/api/manga/search.xml?q={search}"));

            var manga = doc.Descendants("manga").First();

            result[0] += "**Title:** `" + manga.Descendants("title").First().Value + "`";
            result[0] += "\n**English Title:** `" + manga.Descendants("english").First().Value + "`";
            result[0] += "\n**Synonyms:** `" + manga.Descendants("synonyms").First().Value + "`";
            result[0] += "\n**Chapter count:** `" + manga.Descendants("chapters").First().Value + "`";
            result[0] += "\n**Volume count:** `" + manga.Descendants("volumes").First().Value + "`";
            result[0] += "\n**Score:** `" + manga.Descendants("score").First().Value + "`";
            result[0] += "\n**Type:** `" + manga.Descendants("type").First().Value + "`";
            result[0] += "\n**Status:** `" + manga.Descendants("status").First().Value + "`";
            result[0] += "\n**Start Date:** `" + manga.Descendants("start_date").First().Value + "`";
            result[0] += "\n**End Date:** `" + manga.Descendants("end_date").First().Value + "`";
            result[0] += "\n\n**Description:** \n" + StripHTML(manga.Descendants("synopsis").First().Value) + "\n";
            result[1] += manga.Descendants("image").First().Value;

            return result;
        }

        private static async Task<string> GetMalLinkAStringAsync(string link)
        {
            var httpClient = new HttpClient();
            var byteArray = Encoding.ASCII.GetBytes(BotConfig.Instance.Mal.credentials);
            var header = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            httpClient.DefaultRequestHeaders.Authorization = header;
            var response = await httpClient.GetAsync(link);
            var content = response.Content;
            return await content.ReadAsStringAsync();
        }

        public static string StripHTML(string input)
        {
            return Regex.Replace(input, "(?:<style.+?>.+?</style>|<script.+?>.+?</script>|<(?:!|/?[a-‌​zA-Z]+).*?/?>)", String.Empty).Replace("[i]","").Replace("[/i]","");
        }
    }
}