using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indra.Discord.Entities
{
    public class DiscordChannel
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("guild_id")]
        public ulong Guild { get; set; }
    }
}
