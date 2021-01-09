using Newtonsoft.Json;

namespace gtaserver.core.discordbridge.Webhook
{
    public class WebhookMessage
    {
        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("avatar_url")]
        public string AvatarUrl { get; set; }
    }
}
