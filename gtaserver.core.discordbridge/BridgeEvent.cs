using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace gtaserver.core.discordbridge
{
    static class BridgeEvent
    {
        public static EventId OnEnable { get; } = new EventId(101, "OnEnable");

        public static EventId Chat { get; } = new EventId(102, "Discord");
    }
}
