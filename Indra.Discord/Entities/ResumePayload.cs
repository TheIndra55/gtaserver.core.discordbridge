using Newtonsoft.Json;

namespace Indra.Discord.Entities
{
    class ResumePayload
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("session_id")]
        public string SessionId { get; set; }

        [JsonProperty("seq")]
        public int SequenceNumber { get; set; }
    }
}
