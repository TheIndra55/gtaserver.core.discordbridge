using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indra.Discord.Entities
{
    class IdentifyPayload
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("compress")]
        public bool Compress { get; set; } = false;

        [JsonProperty("properties")]
        public IdentifyProperties Properties { get; set; } = new IdentifyProperties();

        [JsonProperty("intents")]
        public int Intents { get; set; }
    }

    class IdentifyProperties
    {
        [JsonProperty("$os")]
        public string Platform { get; set; } = "windows";

        [JsonProperty("$browser")]
        public string Browser { get; set; } = "Indra.Discord";

        [JsonProperty("$device")]
        public string Device { get; set; } = "Indra.Discord";
    }
}
