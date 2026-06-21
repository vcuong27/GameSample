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
            get
            {
                return socket != null && socket.State == WebSocketState.Open;
            }
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

        public async Task SendInitializationAsync(string token)
        {
            string json = JsonConvert.SerializeObject(new
            {
                packetID = "INITIALIZATION",
                clientID = id,
                token = token
            });

            await SendTextAsync(json);
        }

        public async void SendMessage(MessageID messageID, string jsonValue)
        {
            try
            {
                await SendMessageAsync(messageID, jsonValue);
            }
            catch (Exception ex)
            {
                Tools.LogError(ex.Message, ex.StackTrace);
            }
        }

        public async Task SendMessageAsync(MessageID messageID, string jsonValue)
        {
            object data = null;
            try
            {
                data = JsonConvert.DeserializeObject(jsonValue);
            }
            catch
            {
                data = jsonValue;
            }

            string json = JsonConvert.SerializeObject(new
            {
                messageID = (int)messageID,
                messageName = messageID.ToString(),
                jsonValue = jsonValue,
                data = data
            });

            await SendTextAsync(json);
        }

        public async Task SendTextAsync(string text)
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
            catch (Exception ex)
            {
                Tools.LogError(ex.Message, ex.StackTrace);
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
                Tools.LogError(ex.Message, ex.StackTrace);
            }
            finally
            {
                currentSocket.Dispose();
                remoteAddress = null;
            }
        }
    }
}
