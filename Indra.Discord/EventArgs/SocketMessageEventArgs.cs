using Indra.Discord.Entities;

namespace Indra.Discord.EventArgs
{
    public class SocketMessageEventArgs
    {
        public string Content { get; set; }
        public GatewayPayload Payload { get; set; }
    }
}
