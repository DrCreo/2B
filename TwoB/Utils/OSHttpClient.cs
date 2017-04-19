using System.Net.Http;
using System.Threading.Tasks;

namespace TwoB
{
    public static class OSHttpClient
    {
        public static async Task<string> GetLinkAsStringAsync(string link)
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(link);
            var content = response.Content;
            return await content.ReadAsStringAsync();
        }
    }
}
