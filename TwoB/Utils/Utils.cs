using DSharpPlus;
using DSharpPlus.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TwoB
{
    public static class Utils
    {
        /// <summary>
        /// Simple Logging.
        /// </summary>
        /// <param name="aString"></param>
        public static void Log(this DebugLogger debugLogger, string aString)
        {
            Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] {aString}");
        }

        /// <summary>
        /// Checks if the user is a bot developer.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool IsDeveloper(this DiscordUser user)
        {
            if (BotConfig.Instance.Developers.Contains(user.Id))
                return true;
            return false;
        }
        /// <summary>
        /// Returns a "true" or "false" lower case string depending on the bools state.
        /// </summary>
        /// <param name="_bool"></param>
        /// <returns></returns>
        public static string ToStrLow(this bool _bool)
        {
            return _bool.ToString().ToLower();
        }

        
    }

    static class Helper
    {
        public static T[] ToArray<T>(this ICollection collection)
        {
            var items = new T[collection.Count];
            collection.CopyTo(items, 0);

            return items;
        }
    }
}
