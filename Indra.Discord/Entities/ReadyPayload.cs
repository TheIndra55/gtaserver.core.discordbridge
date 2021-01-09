using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indra.Discord.Entities
{
    class ReadyPayload
    {
        [JsonProperty("v")]
        public short Version { get; set; }

        [JsonProperty("user")]
        public DiscordUser User { get; set; }

        [JsonProperty("session_id")]
        public string SessionId { get; set; }
    }
}
