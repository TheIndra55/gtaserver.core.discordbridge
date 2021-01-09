using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Indra.Discord.Entities
{
    public class DiscordUser
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("discriminator")]
        public string Discriminator { get; set; }

        [JsonProperty("avatar")]
        public string Avatar { get; set; }

        public string AvatarUrl
        {
            get
            {
                return $"https://cdn.discordapp.com/{Id}/{Avatar}.png?size=1024";
            }
        }
    }
}
