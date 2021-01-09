using Indra.Discord.Entities;

namespace Indra.Discord.EventArgs
{
    public class MessageReceivedEventArgs
    {
        public DiscordMessage Message { get; set; }
        public DiscordUser User { get; set; }
    }
}
