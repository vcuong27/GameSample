namespace DevelopersHub.RealtimeNetworking.Client
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.WebSockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using UnityEngine;

    public partial class Client : MonoBehaviour
    {
        public class WebSocketTransport
        {
            private ClientWebSocket socket;
            private CancellationTokenSource cancellation;
            private readonly SemaphoreSlim sendLock = new SemaphoreSlim(1, 1);
            private byte[] receiveBuffer;

            public bool isConnected
            {
                get
                {
                    return socket != null && socket.State == WebSocketState.Open;
                }
            }

            public async void Connect()
            {
                List<string> errors = new List<string>();
                Uri[] uris = GetConnectionUris();

                for (int i = 0; i < uris.Length; i++)
                {
                    Uri uri = uris[i];

                    try
                    {
                        DisposeCurrentSocket();

                        cancellation = new CancellationTokenSource();
                        socket = new ClientWebSocket();
                        receiveBuffer = new byte[dataBufferSize];

                        Debug.Log("WebSocket connecting to: " + uri.AbsoluteUri);

                        Task connectTask = socket.ConnectAsync(uri, cancellation.Token);
                        Task timeoutTask = Task.Delay(connectTimeout);
                        Task completedTask = await Task.WhenAny(connectTask, timeoutTask);

                        if (completedTask != connectTask)
                        {
                            errors.Add(uri.AbsoluteUri + " => timeout");
                            try { cancellation.Cancel(); } catch { }
                            continue;
                        }

                        await connectTask;

                        Debug.Log("WebSocket connected to: " + uri.AbsoluteUri);
                        ReceiveLoop();
                        return;
                    }
                    catch (Exception ex)
                    {
                        errors.Add(uri.AbsoluteUri + " => " + ex.Message);
                    }
                }

                DisposeCurrentSocket();
                FailConnection("WebSocket connection failed. Tried: " + string.Join(" | ", errors.ToArray()));
            }

            public async void SendText(string text)
            {
                if (string.IsNullOrEmpty(text) || !isConnected)
                {
                    return;
                }

                byte[] data = Encoding.UTF8.GetBytes(text);

                try
                {
                    await sendLock.WaitAsync();

                    if (isConnected && cancellation != null)
                    {
                        await socket.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Text, true, cancellation.Token);
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log("Error sending WebSocket text: " + ex.Message);
                }
                finally
                {
                    sendLock.Release();
                }
            }

            public void SendData(Packet packet)
            {
                Debug.LogWarning("Simple WebSocket mode does not send binary Packet data. Use Sender.WebSocket_Send(messageID, jsonValue) or Sender.TCP_Send(messageID, jsonValue).");
            }

            private async Task ReceiveLoop()
            {
                try
                {
                    while (isConnected)
                    {
                        using (MemoryStream messageStream = new MemoryStream())
                        {
                            WebSocketReceiveResult result;

                            do
                            {
                                result = await socket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), cancellation.Token);

                                if (result.MessageType == WebSocketMessageType.Close)
                                {
                                    DisconnectFromReceiveLoop();
                                    return;
                                }

                                messageStream.Write(receiveBuffer, 0, result.Count);
                            }
                            while (!result.EndOfMessage);

                            if (result.MessageType != WebSocketMessageType.Text)
                            {
                                Debug.LogWarning("Simple WebSocket mode ignores non-text messages.");
                                continue;
                            }

                            string text = Encoding.UTF8.GetString(messageStream.ToArray());
                            Threading.ExecuteOnMainThread(() => instance.HandleWebSocketText(text));
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception ex)
                {
                    Debug.Log("WebSocket receive error: " + ex.Message);
                }

                DisconnectFromReceiveLoop();
            }

            private Uri[] GetConnectionUris()
            {
                List<Uri> uris = new List<Uri>();
                HashSet<string> seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                string configuredUrl = instance.settings != null ? instance.settings.webSocketUrl : "ws://localhost:5556/ws/";
                AddUri(uris, seen, configuredUrl);

                Uri configuredUri;
                if (Uri.TryCreate(configuredUrl, UriKind.Absolute, out configuredUri) && IsLoopbackHost(configuredUri.Host))
                {
                    AddUri(uris, seen, ReplaceHost(configuredUri, "localhost"));
                    AddUri(uris, seen, ReplaceHost(configuredUri, "127.0.0.1"));
                }

                return uris.ToArray();
            }

            private void AddUri(List<Uri> uris, HashSet<string> seen, string url)
            {
                Uri uri;
                if (Uri.TryCreate(url, UriKind.Absolute, out uri))
                {
                    if (!seen.Contains(uri.AbsoluteUri))
                    {
                        seen.Add(uri.AbsoluteUri);
                        uris.Add(uri);
                    }
                }
            }

            private string ReplaceHost(Uri uri, string host)
            {
                UriBuilder builder = new UriBuilder(uri);
                builder.Host = host;
                return builder.Uri.AbsoluteUri;
            }

            private bool IsLoopbackHost(string host)
            {
                return string.Equals(host, "localhost", StringComparison.OrdinalIgnoreCase) ||
                       string.Equals(host, "127.0.0.1", StringComparison.OrdinalIgnoreCase) ||
                       string.Equals(host, "::1", StringComparison.OrdinalIgnoreCase);
            }

            private void DisconnectFromReceiveLoop()
            {
                if (instance._isConnected)
                {
                    Threading.ExecuteOnMainThread(() => instance.Disconnect());
                }
            }

            public async void Disconnect()
            {
                ClientWebSocket currentSocket = socket;
                socket = null;

                try
                {
                    if (cancellation != null)
                    {
                        cancellation.Cancel();
                    }
                }
                catch { }

                if (currentSocket != null)
                {
                    try
                    {
                        if (currentSocket.State == WebSocketState.Open || currentSocket.State == WebSocketState.CloseReceived)
                        {
                            await currentSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Disconnected", CancellationToken.None);
                        }
                    }
                    catch { }
                    finally
                    {
                        currentSocket.Dispose();
                    }
                }

                if (cancellation != null)
                {
                    cancellation.Dispose();
                    cancellation = null;
                }

                receiveBuffer = null;
            }

            private void DisposeCurrentSocket()
            {
                ClientWebSocket currentSocket = socket;
                CancellationTokenSource currentCancellation = cancellation;
                socket = null;
                cancellation = null;

                try
                {
                    if (currentCancellation != null)
                    {
                        currentCancellation.Cancel();
                    }
                }
                catch { }

                try
                {
                    if (currentSocket != null)
                    {
                        currentSocket.Dispose();
                    }
                }
                catch { }

                try
                {
                    if (currentCancellation != null)
                    {
                        currentCancellation.Dispose();
                    }
                }
                catch { }
            }

            private void FailConnection(string message)
            {
                Debug.Log(message);
                Threading.ExecuteOnMainThread(() => instance.WebSocketConnectionFailed());
            }
        }

        private void HandleWebSocketText(string text)
        {
            RealtimeNetworking.instance._ReceiveWebSocketText(text);

            int clientID;
            string token;
            if (WebSocketJson.TryParseInitialization(text, out clientID, out token))
            {
                _id = clientID;
                _receiveToken = token;
                _sendToken = Tools.GenerateToken();
                _connecting = false;
                _isConnected = true;
                RealtimeNetworking.instance._Connection(true);
                return;
            }

            int messageID;
            string messageName;
            string jsonValue;
            if (WebSocketJson.TryParseServerMessage(text, out messageID, out messageName, out jsonValue))
            {
                RealtimeNetworking.instance._ReceiveWebSocketMessage(messageID, messageName, jsonValue, text);

                if (messageID >= 0)
                {
                    RealtimeNetworking.instance._ReceiveString(messageID, jsonValue);
                }
            }
        }

        private void WebSocketConnectionFailed()
        {
            _connecting = false;
            _isConnected = false;
            RealtimeNetworking.instance._Connection(false);
        }
    }
}
