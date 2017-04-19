﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.IO;

namespace TwoB
{
    public sealed class BotConfig
    {
        [JsonProperty("token")]
        public string Token { get; private set; }

        [JsonProperty("prefix")]
        public string Prefix { get; set; }

        [JsonProperty("developers")]
        public ulong[] Developers { get; set; }


        private static BotConfig instance;

        public BotConfig() { }

        public static BotConfig Instance
        {
            get
            {
                if (instance == null)
                    CreateInstance();
                return instance;
            }
        }

        private static void CreateInstance()
        {

            var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            path = Path.Combine(path, "Config/config.json");

            if (!File.Exists(path))
                throw new FileNotFoundException($"'{path}' not found.");

            var json = string.Empty;
            using (var fileStream = File.OpenRead(path))
            using (var streamReader = new StreamReader(fileStream))
                json = streamReader.ReadToEnd();

            instance = JsonConvert.DeserializeObject<BotConfig>(json);
        }
    }
}
