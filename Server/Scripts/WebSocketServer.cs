using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DevelopersHub.RealtimeNetworking.Server
{
    class WebSocketServer
    {
        private static HttpListener listener;
        private static bool isRunning = false;

        public static int Port { get; private set; }
        public static string Path { get; private set; }

        public static void Start(int port, string path)
        {
            try
            {
                if (!HttpListener.IsSupported)
                {
                    Console.WriteLine("WebSocket server cannot start because HttpListener is not supported on this platform.");
                    return;
                }

                Port = port;
                Path = NormalizePath(path);

                listener = new HttpListener();
                listener.Prefixes.Add($"http://*:{Port}{Path}");
                listener.Start();
                isRunning = true;

                _ = AcceptLoopAsync();
                Console.WriteLine("WebSocket Server Started on ws://0.0.0.0:{0}{1}", Port, Path);
            }
            catch (Exception ex)
            {
                Tools.LogError(ex.Message, ex.StackTrace, "WebSocket");
            }
        }

        private static string NormalizePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return "/ws/";
            }

            if (!path.StartsWith("/"))
            {
                path = "/" + path;
            }

            if (!path.EndsWith("/"))
            {
                path += "/";
            }

            return path;
        }

        private static async Task AcceptLoopAsync()
        {
            while (isRunning)
            {
                try
                {
                    HttpListenerContext context = await listener.GetContextAsync();
                    _ = HandleContextAsync(context);
                }
                catch (ObjectDisposedException)
                {
                    return;
                }
                catch (HttpListenerException)
                {
                    return;
                }
                catch (Exception ex)
                {
                    Tools.LogError(ex.Message, ex.StackTrace, "WebSocket");
                }
            }
        }

        private static async Task HandleContextAsync(HttpListenerContext context)
        {
            if (!context.Request.IsWebSocketRequest)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.Close();
                return;
            }

            int clientID = GetAvailableClientID();
            if (clientID <= 0)
            {
                context.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                context.Response.Close();
                return;
            }

            string remoteAddress = context.Request.RemoteEndPoint != null ? context.Request.RemoteEndPoint.Address.ToString() : "unknown";
            bool connected = false;

            try
            {
                HttpListenerWebSocketContext webSocketContext = await context.AcceptWebSocketAsync(null);
                WebSocket socket = webSocketContext.WebSocket;

                Server.clients[clientID].webSocket.Initialize(socket, remoteAddress);
                Server.clients[clientID].sendToken = Tools.GenerateToken();
                Terminal.OnClientConnected(clientID, remoteAddress);
                connected = true;

                Console.WriteLine("Incoming WebSocket connection from {0}. Client ID: {1}.", remoteAddress, clientID);
                await Server.clients[clientID].webSocket.SendInitializationAsync(Server.clients[clientID].sendToken);
                await ReceiveLoopAsync(clientID, socket);
            }
            catch (Exception ex)
            {
                Tools.LogError(ex.Message, ex.StackTrace, "WebSocket");
            }
            finally
            {
                if (connected)
                {
                    Terminal.OnClientDisconnected(clientID, remoteAddress);
                    Console.WriteLine("WebSocket client with IP {0} has been disconnected.", remoteAddress);
                }

                await Server.clients[clientID].webSocket.DisconnectAsync();
            }
        }

        private static int GetAvailableClientID()
        {
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                if (Server.clients[i].tcp.socket == null && (Server.clients[i].webSocket == null || !Server.clients[i].webSocket.IsConnected))
                {
                    return i;
                }
            }

            return 0;
        }

        private static async Task ReceiveLoopAsync(int clientID, WebSocket socket)
        {
            byte[] buffer = new byte[Client.dataBufferSize];

            while (socket.State == WebSocketState.Open)
            {
                using (MemoryStream messageStream = new MemoryStream())
                {
                    WebSocketReceiveResult result;
                    do
                    {
                        result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            return;
                        }

                        messageStream.Write(buffer, 0, result.Count);
                    }
                    while (!result.EndOfMessage);

                    byte[] data = messageStream.ToArray();
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        string text = Encoding.UTF8.GetString(data);
                        HandleTextMessage(clientID, text);
                    }
                    else if (result.MessageType == WebSocketMessageType.Binary)
                    {
                        HandleBinaryMessage(clientID, data);
                    }
                }
            }
        }

        private static void HandleTextMessage(int clientID, string text)
        {
            try
            {
                JObject root = JObject.Parse(text);
                if (!TryReadMessageID(root, out MessageID messageID))
                {
                    Console.WriteLine("Invalid WebSocket message from Client[{0}]. Missing or invalid messageID.", clientID);
                    return;
                }

                JToken dataToken = GetProperty(root, "jsonValue") ?? GetProperty(root, "data") ?? GetProperty(root, "payload");
                string jsonValue = GetJsonValue(dataToken);

                using (Packet packet = new Packet())
                {
                    packet.Write((int)messageID);
                    packet.Write(jsonValue);
                    byte[] packetBytes = packet.ToArray();

                    Threading.ExecuteOnMainThread(() =>
                    {
                        using (Packet readablePacket = new Packet(packetBytes))
                        {
                            Terminal.ReceivedPacket(clientID, readablePacket);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Tools.LogError(ex.Message, ex.StackTrace, "WebSocket");
            }
        }

        private static bool TryReadMessageID(JObject root, out MessageID messageID)
        {
            messageID = default(MessageID);
            JToken token = GetProperty(root, "messageID") ?? GetProperty(root, "id") ?? GetProperty(root, "packetID");
            if (token == null)
            {
                return false;
            }

            if (token.Type == JTokenType.Integer)
            {
                messageID = (MessageID)token.Value<int>();
                return true;
            }

            string value = token.ToString();
            if (int.TryParse(value, out int intValue))
            {
                messageID = (MessageID)intValue;
                return true;
            }

            return Enum.TryParse(value, true, out messageID);
        }

        private static JToken GetProperty(JObject root, string propertyName)
        {
            JProperty property = root.Properties().FirstOrDefault(item => string.Equals(item.Name, propertyName, StringComparison.OrdinalIgnoreCase));
            return property != null ? property.Value : null;
        }

        private static string GetJsonValue(JToken token)
        {
            if (token == null || token.Type == JTokenType.Null)
            {
                return "{}";
            }

            if (token.Type == JTokenType.String)
            {
                string value = token.Value<string>();
                if (!string.IsNullOrWhiteSpace(value) && (value.TrimStart().StartsWith("{") || value.TrimStart().StartsWith("[")))
                {
                    return value;
                }

                return JsonConvert.SerializeObject(value);
            }

            return token.ToString(Formatting.None);
        }

        private static void HandleBinaryMessage(int clientID, byte[] data)
        {
            try
            {
                byte[] packetBytes = RemoveTcpLengthPrefixIfNeeded(data);

                Threading.ExecuteOnMainThread(() =>
                {
                    using (Packet packet = new Packet(packetBytes))
                    {
                        int packetID = packet.ReadInt();
                        if (Server.packetHandlers.TryGetValue(packetID, out Server.PacketHandler handler))
                        {
                            handler(clientID, packet);
                        }
                        else
                        {
                            Console.WriteLine("Invalid WebSocket binary packet from Client[{0}]. Packet ID: {1}.", clientID, packetID);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Tools.LogError(ex.Message, ex.StackTrace, "WebSocket");
            }
        }

        private static byte[] RemoveTcpLengthPrefixIfNeeded(byte[] data)
        {
            if (data.Length < 8)
            {
                return data;
            }

            int length = BitConverter.ToInt32(data, 0);
            if (length == data.Length - 4)
            {
                byte[] packetBytes = new byte[length];
                Array.Copy(data, 4, packetBytes, 0, length);
                return packetBytes;
            }

            return data;
        }
    }
}
