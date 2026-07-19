using Newtonsoft.Json.Linq;
using System;
using System.IO;
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
        private static bool isRunning;

        public static int Port { get; private set; }
        public static string Path { get; private set; }
        public static string Host { get; private set; }

        public static void Start(int port, string path)
        {
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("WebSocket server cannot start because HttpListener is not supported.");
                return;
            }

            Port = port;
            Path = NormalizePath(path);
            Host = string.IsNullOrWhiteSpace(Terminal.websocketHost) ? "localhost" : Terminal.websocketHost.Trim();

            listener = new HttpListener();
            listener.Prefixes.Add("http://" + Host + ":" + Port + Path);

            try
            {
                listener.Start();
                isRunning = true;
                Task result = AcceptLoopAsync();

                string displayHost = Host == "+" || Host == "*" ? "0.0.0.0" : Host;
                Console.WriteLine("WebSocket Server Started on ws://{0}:{1}{2}", displayHost, Port, Path);
            }
            catch (Exception ex)
            {
                Tools.LogError(ex.Message, ex.StackTrace, "WebSocket");
                Console.WriteLine("WebSocket Server could not start. For local test use Terminal.websocketHost = \"localhost\".");

                listener.Close();
                listener = null;
                isRunning = false;
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
                    Task result = HandleClientAsync(context);
                }
                catch
                {
                    return;
                }
            }
        }

        private static async Task HandleClientAsync(HttpListenerContext context)
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

                Console.WriteLine("WebSocket client connected. ID: {0}, IP: {1}", clientID, remoteAddress);
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
                    Console.WriteLine("WebSocket client disconnected. ID: {0}, IP: {1}", clientID, remoteAddress);
                }

                await Server.clients[clientID].webSocket.DisconnectAsync();
            }
        }

        private static int GetAvailableClientID()
        {
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                if (Server.clients[i].tcp.socket == null && !Server.clients[i].webSocket.IsConnected)
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
                using (MemoryStream stream = new MemoryStream())
                {
                    WebSocketReceiveResult result;
                    do
                    {
                        result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            return;
                        }

                        stream.Write(buffer, 0, result.Count);
                    }
                    while (!result.EndOfMessage);

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        string text = Encoding.UTF8.GetString(stream.ToArray());
                        HandleTextMessage(clientID, text);
                    }
                }
            }
        }

        private static void HandleTextMessage(int clientID, string text)
        {
            try
            {
                JObject json = JObject.Parse(text);

                MessageID messageID;
                if (!ReadMessageID(json, out messageID))
                {
                    Console.WriteLine("Invalid WebSocket message. Missing messageID. Client ID: {0}", clientID);
                    return;
                }

                string jsonValue = json["jsonValue"] != null ? json["jsonValue"].ToString() : "{}";

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

        private static bool ReadMessageID(JObject json, out MessageID messageID)
        {
            messageID = 0;

            JToken token = json["messageID"];
            if (token == null)
            {
                return false;
            }

            if (token.Type == JTokenType.Integer)
            {
                messageID = (MessageID)token.Value<int>();
                return true;
            }

            return Enum.TryParse(token.ToString(), true, out messageID);
        }
    }
}
