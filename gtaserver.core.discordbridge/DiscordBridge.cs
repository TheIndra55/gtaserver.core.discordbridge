using gtaserver.core.discordbridge.Exceptions;
using gtaserver.core.discordbridge.Webhook;
using GTAServer;
using GTAServer.PluginAPI;
using GTAServer.PluginAPI.Events;
using GTAServer.ProtocolMessages;
using Indra.Discord;
using Indra.Discord.Entities;
using Indra.Discord.EventArgs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace gtaserver.core.discordbridge
{
    public class DiscordBridge : IPlugin
    {
        public string Name => "Discord Bridge";
        public string Description => "Bridge the chat between discord and ingame";
        public string Author => "TheIndra";

        private GameServer _gameServer;
        private BridgeConfiguration _configuration;
        private DiscordClient _discord;
        private ILogger _logger;

        private HttpClient _httpClient;
        public HttpClient HttpClient
        {
            get
            {
                if (_httpClient == null)
                    _httpClient = new HttpClient();

                return _httpClient;
            }
        }

        public bool OnEnable(GameServer gameServer, bool isAfterServerLoad)
        {
            _gameServer = gameServer;
            _configuration = gameServer.InitConfiguration<BridgeConfiguration>(typeof(DiscordBridge));
            _logger = Util.LoggerFactory.CreateLogger<DiscordBridge>();

            _logger.LogInformation(BridgeEvent.OnEnable, "Enabling discord bridge");
            _discord = new DiscordClient(_configuration.Token, _logger);

            DiscordChannel channel;
            try
            {
                channel = GetChannel(_configuration.ChannelId);
                _logger.LogInformation(BridgeEvent.OnEnable, "Bridging with #" + channel.Name);

                _ = _discord.StartAsync();
            }
            catch(ChannelFetchFailedException e)
            {
                _logger.LogWarning(BridgeEvent.OnEnable, "Failed to fetch channel: {}, you will not be able to send messages from discord", e.StatusCode);
            }

            GameEvents.OnChatMessage.Add(OnChatMessage);
            _discord.MessageReceived += MessageReceived;

            return true;
        }

        private async Task MessageReceived(DiscordClient client, MessageReceivedEventArgs e)
        {
            if (e.Message.WebhookId == _configuration.WebhookId) return;
            if (e.Message.Channel != _configuration.ChannelId) return;

            _logger.LogInformation(BridgeEvent.Chat, $"<{e.User.Username}>: {e.Message.Content}");

            _gameServer.SendChatMessageToAll(e.User.Username + " [Spectator]", e.Message.Content);
            await Task.FromResult(0);
        }

        private PluginResponse<ChatData> OnChatMessage(Client client, ChatData chatData)
        {
            if (chatData.Message.Substring(0, 1) != "/")
            {
                ExecuteWebhook(new WebhookMessage
                {
                    Content = chatData.Message,
                    Username = client.DisplayName
                });
            }

            return new PluginResponse<ChatData>
            {
                ContinuePluginProc = true,
                ContinueServerProc = true,
                Data = chatData
            };
        }

        public DiscordChannel GetChannel(ulong id)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri("https://discord.com/api/channels/" + id)
            };
            request.Headers.Add("Authorization", "Bot " + _configuration.Token);

            var response = HttpClient.SendAsync(request).Result;
            if(!response.IsSuccessStatusCode)
            {
                throw new ChannelFetchFailedException() { StatusCode = response.StatusCode };
            }

            return JsonConvert.DeserializeObject<DiscordChannel>(response.Content.ReadAsStringAsync().Result);
        }

        public void ExecuteWebhook(WebhookMessage message)
        {
            var payload = JsonConvert.SerializeObject(message, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(_configuration.WebhookUrl),
                Method = HttpMethod.Post
            };
            request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

            HttpClient.SendAsync(request);
        }
    }
}
