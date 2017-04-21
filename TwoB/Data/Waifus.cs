using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Reflection;
using System.IO;

namespace TwoB
{
    public class Waifu
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("aliases")]
        public List<string> Aliases { get; set; }
        [JsonProperty("origin")]
        public List<string> Origin { get; set; }
        [JsonProperty("image")]
        public string ImageURL { get; set; }
    }

    public class WaifuDB
    {
        [JsonProperty("waifus")]
        public List<Waifu> Waifus { get; set; }

        private static WaifuDB Instance { get; set; }

        public static WaifuDB DB
        {
            get
            {
                if (Instance == null)
                    CreateInstance();
                return Instance;
            }
        }

        private static void CreateInstance()
        {
            try
            {
                var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                path = Path.Combine(path, "Data/waifudb.json");

                if (!File.Exists(path))
                    throw new FileNotFoundException($"'{path}' not found.");

                var json = string.Empty;
                using (var fileStream = File.OpenRead(path))
                using (var streamReader = new StreamReader(fileStream))
                    json = streamReader.ReadToEnd();

                Instance = JsonConvert.DeserializeObject<WaifuDB>(json);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
        } 
    }
}
