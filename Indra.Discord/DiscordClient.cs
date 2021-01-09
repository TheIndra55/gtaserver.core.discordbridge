using Indra.Discord.Entities;
using Indra.Discord.EventArgs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Indra.Discord
{
    public class DiscordClient
    {
        private string _token;
        private GatewayConnection _gateway;
        private ILogger _logger;
        private string _gatewayUrl;

        private string _sessionId;

        public delegate Task MessageReceivedHandler(DiscordClient client, MessageReceivedEventArgs e);
        public event MessageReceivedHandler MessageReceived;

        public DiscordClient(string token, ILogger logger = null, string gatewayUrl = "wss://gateway.discord.gg")
        {
            _token = token;
            _gatewayUrl = gatewayUrl + "/?v=6&encoding=json";
            _gateway = new GatewayConnection(_gatewayUrl, logger);
            _logger = logger;
        }

        public async Task StartAsync()
        {
            _gateway.SocketMessageReceived += SocketMessageReceived;
            _gateway.Disconnected += Disconnected;

            await _gateway.ConnectAsync();
        }

        private async Task Disconnected(GatewayConnection conn)
        {
            _logger.LogInformation(LogEvent.Disconnected, "Connection to Discord lost, attempting to reconnect in 2s");
            await Task.Delay(2000);

            _logger.LogTrace(LogEvent.Disconnected, "Reconnecting now");
            _gateway = new GatewayConnection(_gatewayUrl, _logger);

            _gateway.SocketMessageReceived += SocketMessageReceived;
            _gateway.Disconnected += Disconnected;

            await _gateway.ConnectAsync();
        }

        private async Task SocketMessageReceived(GatewayConnection conn, EventArgs.SocketMessageEventArgs e)
        {
            _logger.LogTrace(LogEvent.MessageReceived, "Opcode = {}, Event = {}", e.Payload.Opcode, e.Payload.Event ?? "null");

            if(e.Payload.Opcode == 10)
            {
                // hello received, send identify or resume
                if (_sessionId == null)
                {
                    var identify = new IdentifyPayload
                    {
                        Token = _token,
                        Intents = 512
                    };

                    await _gateway.SendAsync(2, identify);
                }
                else
                {
                    _logger.LogDebug(LogEvent.MessageReceived, "Attempting to resume");
                    var resume = new ResumePayload
                    {
                        SequenceNumber = _gateway.LastSeqNumber,
                        SessionId = _sessionId,
                        Token = _token
                    };

                    await _gateway.SendAsync(6, resume);
                }
            }

            if (e.Payload.Opcode == 9)
            {
                _logger.LogDebug(LogEvent.MessageReceived, "Invalid Session received, reconnecting");
                _sessionId = null;

                await _gateway.ClientWebSocket.CloseAsync(WebSocketCloseStatus.Empty, "reconnecting", CancellationToken.None);
            }

            if (e.Payload.Opcode == 0)
            {
                if(e.Payload.Event == "MESSAGE_CREATE")
                {
                    var message = JObject.FromObject(e.Payload.Data).ToObject<DiscordMessage>();
                    MessageReceived?.Invoke(this, new MessageReceivedEventArgs { Message = message, User = message.User });
                }

                if(e.Payload.Event == "READY")
                {
                    var ready = JObject.FromObject(e.Payload.Data).ToObject<ReadyPayload>();

                    _sessionId = ready.SessionId;
                    _logger.LogInformation(LogEvent.Ready, "Connected to discord as {}#{}", ready.User.Username, ready.User.Discriminator);
                }
            }
        }
    }
}
