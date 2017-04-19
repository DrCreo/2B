using System.Threading.Tasks;

namespace TwoB
{
    /// <summary>
    /// This is the Wikia module which handles interaction with the https://wikia.com API.
    /// </summary>
    public static class WikiaHelper
    {
        internal enum WikiaType
        {
            Nier = 1,
            Animanga = 2
        }

        /// <summary>
        /// Gets the Top Article from the selected Wikia.
        /// </summary>
        /// <param name="wikia"></param>
        /// <returns></returns>
        internal static Task<string> GetTopArticles(WikiaType wikia)
        {
            return BuildLink(wikia, "Articles/Top");
        }

        /// <summary>
        /// Gets a list of Articles from the selected Wikia.
        /// </summary>
        /// <param name="wikia"></param>
        /// <returns></returns>
        internal static Task<string> GetArticleList(WikiaType wikia, int listAmount = 25)
        {
            return BuildLink(wikia, $"Articles/List/?limit={listAmount}");
        }

        /// <summary>
        /// Gets the details for a Wikia Article.
        /// </summary>
        /// <param name="wikia"></param>
        /// <param name="articleID"></param>
        /// <returns></returns>
        internal static Task<string> GetArticleDetails(WikiaType wikia, int articleID)
        {
            return BuildLink(wikia, $"Articles/Details/?ids={articleID}&abstract=100&width=200&height=200");
        }

        /// <summary>
        /// Gets the last Updated Article on a selected Wikia.
        /// </summary>
        /// <param name="wikia"></param>
        /// <param name="amount"></param>
        /// <param name="allowDuplicates"></param>
        /// <returns></returns>
        internal static Task<string> GetLastUpdatedArticle(WikiaType wikia, int amount = 10, bool allowDuplicates = true)
        {
            return BuildLink(wikia, $"Activity/LatestActivity/?limit={amount}&namespaces=0&allowDuplicates={allowDuplicates.ToStrLow()}");
        }

        /// <summary>
        /// Gets a number of related Articles.
        /// </summary>
        /// <param name="wikia"></param>
        /// <param name="articleID"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        internal static Task<string> GetRelatedArticles(WikiaType wikia, int articleID, int amount = 3)
        {
            return BuildLink(wikia, $"RelatedPages/List/?ids={articleID}&limit={amount}");
        }

        /// <summary>
        /// Returns a list of articles based on a Search.
        /// </summary>
        /// <param name="wikia"></param>
        /// <param name="args"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        internal static Task<string> SearchWikia(WikiaType wikia, string args, int amount = 25)
        {
            return BuildLink(wikia, $"Search/List/?query={args}&limit={amount}&namespaces=0%2C14");
        }

        /// <summary>
        /// Builds a link and returns the json string.
        /// </summary>
        /// <param name="wikia"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private static async Task<string> BuildLink(WikiaType wikia, string args)
        {
            string link = string.Empty;
            switch (wikia)
            {
                case WikiaType.Nier:
                    link = $"http://nier.wikia.com/api/v1/{args}";
                    break;
                case WikiaType.Animanga:
                    link = $"http://animanga.wikia.com/api/v1/{args}";
                    break;
            }

            return await OSHttpClient.GetLinkAsStringAsync(link);
        }
    }
}
