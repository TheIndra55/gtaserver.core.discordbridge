using Indra.Discord.Entities;
using Indra.Discord.EventArgs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Indra.Discord
{
    public class GatewayConnection
    {
        public ClientWebSocket ClientWebSocket 
        { 
            get { 
                return _socket; 
            } 
        }

        public int LastSeqNumber
        {
            get
            {
                return _lastSeqNum;
            }
        }

        public delegate Task SocketMessageReceivedHandler(GatewayConnection conn, SocketMessageEventArgs e);
        public event SocketMessageReceivedHandler SocketMessageReceived;

        public delegate Task DisconnectedHandler(GatewayConnection conn);
        public event DisconnectedHandler Disconnected;

        private ClientWebSocket _socket;
        private string _endpoint;
        private int _heartbeatInterval;
        private int _lastSeqNum;
        private ILogger _logger;

        public GatewayConnection(string endpoint, ILogger logger = null)
        {
            _socket = new ClientWebSocket();
            _endpoint = endpoint;
            _logger = logger;

            SocketMessageReceived += InternalSocketMessageReceived;
        }

        public async Task ConnectAsync()
        {
            await _socket.ConnectAsync(new Uri(_endpoint), CancellationToken.None);

            await ReceiveAsync();
        }

        public async Task ReceiveAsync()
        {
            var receiveBuffer = new byte[4096];

            while (_socket.State == WebSocketState.Open)
            {
                _logger.LogTrace(_socket.State.ToString());
                var message = new byte[4096];
                var offset = 0;

                WebSocketReceiveResult receive;

                do
                {
                    receive = await _socket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
                    receiveBuffer.CopyTo(message, offset);

                    offset += receive.Count;
                    receiveBuffer = new byte[4096];
                } while (!receive.EndOfMessage);

                if(receive.MessageType == WebSocketMessageType.Close)
                {
                    // connection closed, reconnect
                    break;
                }

                if(receive.MessageType == WebSocketMessageType.Binary)
                {
                    _logger.LogDebug(LogEvent.Receive, "Binary message received, ignoring");
                }

                if(receive.MessageType == WebSocketMessageType.Text)
                {
                    var text = Encoding.UTF8.GetString(message);
                    var payload = JsonConvert.DeserializeObject<GatewayPayload>(text);

                    SocketMessageReceived?.Invoke(this, new SocketMessageEventArgs { Content = text, Payload = payload });
                }
            }

            Disconnected?.Invoke(this);
        }

        public async Task SendAsync(int opcode, object data, string t = null)
        {
            var payload = new GatewayPayload
            {
                Opcode = opcode,
                Data = JToken.FromObject(data)
            };

            if (t != null)
            {
                payload.Event = t;
            }

            await SendAsync(payload);
        }

        public async Task SendAsync(GatewayPayload payload)
        {
            var text = JsonConvert.SerializeObject(payload);

            await _socket.SendAsync(
                new ArraySegment<byte>(Encoding.UTF8.GetBytes(text)), WebSocketMessageType.Text, true, CancellationToken.None).ConfigureAwait(false);
        }

        private async Task SendHeartbeat()
        {
            _logger.LogTrace(LogEvent.Heartbeat, "Sending heartbeat {}", _lastSeqNum);
            await SendAsync(1, _lastSeqNum++).ConfigureAwait(false);
        }

        private async Task InternalSocketMessageReceived(GatewayConnection conn, SocketMessageEventArgs e)
        {
            if(e.Payload.Opcode == 10)
            {
                _heartbeatInterval = JObject.FromObject(e.Payload.Data).Value<int>("heartbeat_interval");
                _logger.LogTrace(LogEvent.InternalMessageReceived, "heartbeat_interval = {}", _heartbeatInterval);

                new Thread(new ThreadStart(HeartbeatLoop)).Start();
            }

            if (e.Payload.Opcode == 1)
            {
                _lastSeqNum = (int)e.Payload.Data;
                await SendHeartbeat();
            }
        }

        private async void HeartbeatLoop()
        {
            while (_socket.State == WebSocketState.Open)
            {
                await SendHeartbeat().ConfigureAwait(false);
                await Task.Delay(_heartbeatInterval).ConfigureAwait(false);
            }

            _logger.LogDebug("Exiting heartbeat loop");
        }
    }
}
