using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indra.Discord.Entities
{
    public class GatewayPayload
    {
        [JsonProperty("op")]
        public int Opcode { get; set; }

        [JsonProperty("d")]
        public object Data { get; set; }

        [JsonProperty("t", NullValueHandling = NullValueHandling.Ignore)]
        public string Event { get; set; }
    }
}
