using Microsoft.Extensions.Logging;

namespace Indra.Discord
{
    internal static class LogEvent
    {
        public static EventId Receive { get; } = new EventId(201, "Receive");

        public static EventId Heartbeat { get; } = new EventId(202, "Heartbeat");

        public static EventId InternalMessageReceived { get; } = new EventId(203, "InternalMessageReceived");

        public static EventId MessageReceived { get; } = new EventId(204, "MessageReceived");

        public static EventId Ready { get; } = new EventId(204, "DiscordReady");

        public static EventId Disconnected { get; } = new EventId(205, "Disconnected");
    }
}
