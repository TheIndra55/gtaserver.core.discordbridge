using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indra.Discord.Entities
{
    public class DiscordMessage
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("channel_id")]
        public ulong Channel { get; set; }

        [JsonProperty("guild_id")]
        public ulong Guild { get; set; }

        [JsonProperty("author")]
        public DiscordUser User { get; set; }

        [JsonProperty("webhook_id")]
        public ulong WebhookId { get; set; }
    }
}
