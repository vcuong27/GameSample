using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
        public static string Host { get; private set; }

        public static void Start(int port, string path)
        {
            Start(port, path, Terminal.websocketHost);
        }

        public static void Start(int port, string path, string host)
        {
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("WebSocket server cannot start because HttpListener is not supported on this platform.");
                return;
            }

            Port = port;
            Path = NormalizePath(path);
            Host = NormalizeHost(host);

            string primaryPrefix = BuildPrefix(Host, Port, Path);
            string fallbackPrefix = BuildPrefix("localhost", Port, Path);

            if (!TryStartListener(new[] { primaryPrefix }))
            {
                if (!IsLocalHost(Host) && TryStartListener(new[] { fallbackPrefix }))
                {
                    Host = "localhost";
                    Console.WriteLine("WebSocket Server Started on ws://localhost:{0}{1}", Port, Path);
                    Console.WriteLine("Warning: WebSocket fallback is local-only. To accept LAN/mobile clients, run the URL ACL command in WebSocket_USAGE.md or run the server as Administrator.");
                    return;
                }

                Console.WriteLine("WebSocket Server could not start. See WebSocket_USAGE.md for Windows URL ACL instructions.");
                return;
            }

            Console.WriteLine("WebSocket Server Started on ws://{0}:{1}{2}", DisplayHost(Host), Port, Path);
        }

        private static bool TryStartListener(IEnumerable<string> prefixes)
        {
            StopListenerOnly();

            HttpListener newListener = new HttpListener();
            foreach (string prefix in prefixes)
            {
                newListener.Prefixes.Add(prefix);
            }

            try
            {
                newListener.Start();
                listener = newListener;
                isRunning = true;
                _ = AcceptLoopAsync();
                return true;
            }
            catch (HttpListenerException ex)
            {
                Tools.LogError(ex.Message, ex.StackTrace, "WebSocket");
                newListener.Close();
                return false;
            }
            catch (UnauthorizedAccessException ex)
            {
                Tools.LogError(ex.Message, ex.StackTrace, "WebSocket");
                newListener.Close();
                return false;
            }
            catch (Exception ex)
            {
                Tools.LogError(ex.Message, ex.StackTrace, "WebSocket");
                newListener.Close();
                return false;
            }
        }

        private static void StopListenerOnly()
        {
            isRunning = false;

            if (listener == null)
            {
                return;
            }

            try
            {
                listener.Stop();
            }
            catch
            {
            }

            try
            {
                listener.Close();
            }
            catch
            {
            }

            listener = null;
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

        private static string NormalizeHost(string host)
        {
            if (string.IsNullOrWhiteSpace(host))
            {
                return "localhost";
            }

            return host.Trim();
        }

        private static string BuildPrefix(string host, int port, string path)
        {
            return $"http://{host}:{port}{path}";
        }

        private static bool IsLocalHost(string host)
        {
            return string.Equals(host, "localhost", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(host, "127.0.0.1", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(host, "::1", StringComparison.OrdinalIgnoreCase);
        }

        private static string DisplayHost(string host)
        {
            if (host == "+" || host == "*")
            {
                return "0.0.0.0";
            }

            return host;
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
            messageID = default;
            JToken token = GetProperty(root, "messageID") ?? GetProperty(root, "packetID") ?? GetProperty(root, "id");
            if (token == null)
            {
                return false;
            }

            if (token.Type == JTokenType.Integer)
            {
                int value = token.Value<int>();
                if (Enum.IsDefined(typeof(MessageID), value))
                {
                    messageID = (MessageID)value;
                    return true;
                }

                return false;
            }

            string text = token.ToString();
            if (int.TryParse(text, out int numericValue) && Enum.IsDefined(typeof(MessageID), numericValue))
            {
                messageID = (MessageID)numericValue;
                return true;
            }

            return Enum.TryParse(text, true, out messageID);
        }

        private static JToken GetProperty(JObject root, string name)
        {
            JProperty property = root.Properties().FirstOrDefault(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));
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
                return token.ToString();
            }

            return token.ToString(Formatting.None);
        }

        private static void HandleBinaryMessage(int clientID, byte[] data)
        {
            try
            {
                if (data == null || data.Length == 0)
                {
                    return;
                }

                byte[] packetData = StripLengthPrefixIfPresent(data);
                Threading.ExecuteOnMainThread(() =>
                {
                    using (Packet packet = new Packet(packetData))
                    {
                        Terminal.ReceivedPacket(clientID, packet);
                    }
                });
            }
            catch (Exception ex)
            {
                Tools.LogError(ex.Message, ex.StackTrace, "WebSocket");
            }
        }

        private static byte[] StripLengthPrefixIfPresent(byte[] data)
        {
            if (data.Length >= 4)
            {
                int declaredLength = BitConverter.ToInt32(data, 0);
                if (declaredLength == data.Length - 4)
                {
                    byte[] stripped = new byte[declaredLength];
                    Buffer.BlockCopy(data, 4, stripped, 0, declaredLength);
                    return stripped;
                }
            }

            return data;
        }
    }
}
