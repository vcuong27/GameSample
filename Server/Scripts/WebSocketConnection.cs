using Newtonsoft.Json;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DevelopersHub.RealtimeNetworking.Server
{
    class WebSocketConnection
    {
        private readonly int id;
        private readonly SemaphoreSlim sendLock = new SemaphoreSlim(1, 1);

        public WebSocket socket { get; private set; }

        public string remoteAddress { get; private set; }

        public bool IsConnected
        {
            get { return socket != null && socket.State == WebSocketState.Open; }
        }

        public WebSocketConnection(int _id)
        {
            id = _id;
        }

        public void Initialize(WebSocket _socket, string _remoteAddress)
        {
            socket = _socket;
            remoteAddress = _remoteAddress;
        }

        public Task SendInitializationAsync(string token)
        {
            return SendJsonAsync(new
            {
                packetID = "INITIALIZATION",
                clientID = id,
                token = token
            });
        }
        private static string Escape(string value)
        {
            StringBuilder builder = new StringBuilder(value.Length + 8);

            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];

                switch (c)
                {
                    case '\\': builder.Append("\\\\"); break;
                    case '\"': builder.Append("\\\""); break;
                    case '\b': builder.Append("\\b"); break;
                    case '\f': builder.Append("\\f"); break;
                    case '\n': builder.Append("\\n"); break;
                    case '\r': builder.Append("\\r"); break;
                    case '\t': builder.Append("\\t"); break;
                    default:
                        if (c < 32)
                        {
                            builder.Append("\\u");
                            builder.Append(((int)c).ToString("x4"));
                        }
                        else
                        {
                            builder.Append(c);
                        }
                        break;
                }
            }

            return builder.ToString();
        }

        private static string Quote(string value)
        {
            return "\"" + Escape(value ?? string.Empty) + "\"";
        }

        public static string CreateMessage(int messageID, string jsonValue)
        {
            return "{\"messageID\":" + messageID + ",\"jsonValue\":" + Quote(jsonValue ?? "{}") + "}";
        }

        public async void SendMessage(MessageID messageID, string jsonValue)
        {

            string mes = CreateMessage((int)messageID, jsonValue);

            Task ts =  SendTextAsync(mes);

            //try
            //{
            //    await SendJsonAsync(new
            //    {
            //        messageID = (int)messageID,
            //        jsonValue = string.IsNullOrEmpty(jsonValue) ? "{}" : jsonValue
            //    });
            //}
            //catch (Exception ex)
            //{
            //    Tools.LogError(ex.Message, ex.StackTrace, "WebSocket");
            //}
        }

        private Task SendJsonAsync(object value)
        {
            return SendTextAsync(JsonConvert.SerializeObject(value));
        }

        private async Task SendTextAsync(string text)
        {
            if (!IsConnected)
            {
                return;
            }

            byte[] bytes = Encoding.UTF8.GetBytes(text);

            await sendLock.WaitAsync();
            try
            {
                if (IsConnected)
                {
                    await socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
            finally
            {
                sendLock.Release();
            }
        }

        public async Task DisconnectAsync()
        {
            WebSocket currentSocket = socket;
            socket = null;
            remoteAddress = null;

            if (currentSocket == null)
            {
                return;
            }

            try
            {
                if (currentSocket.State == WebSocketState.Open || currentSocket.State == WebSocketState.CloseReceived)
                {
                    await currentSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Disconnected", CancellationToken.None);
                }
            }
            catch (Exception ex)
            {
                Tools.LogError(ex.Message, ex.StackTrace, "WebSocket");
            }
            finally
            {
                currentSocket.Dispose();
            }
        }
    }
}
