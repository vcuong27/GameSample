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

        public async void SendMessage(MessageID messageID, string jsonValue)
        {
            try
            {
                await SendJsonAsync(new
                {
                    messageID = (int)messageID,
                    messageName = messageID.ToString(),
                    jsonValue = string.IsNullOrEmpty(jsonValue) ? "{}" : jsonValue
                });
            }
            catch (Exception ex)
            {
                Tools.LogError(ex.Message, ex.StackTrace, "WebSocket");
            }
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
