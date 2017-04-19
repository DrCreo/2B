using System;
using DSharpPlus;

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
    }
}
