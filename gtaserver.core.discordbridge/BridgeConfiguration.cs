using System;
using System.Collections.Generic;
using System.Text;

namespace gtaserver.core.discordbridge
{
    public class BridgeConfiguration
    {
        public string WebhookUrl { get; set; }
        public string Token { get; set; }
        public ulong ChannelId { get; set; }

        public ulong WebhookId
        {
            get
            {
                return ulong.Parse(WebhookUrl.Split('/')?[5]);
            }
        }
    }
}
