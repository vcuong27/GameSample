namespace DevelopersHub.RealtimeNetworking.Client
{
    using UnityEngine;

    public class DemoWebSocket : MonoBehaviour
    {
        private void Start()
        {
            RealtimeNetworking.OnDisconnectedFromServer += Disconnected;
            RealtimeNetworking.OnConnectingToServerResult += ConnectResult;
            RealtimeNetworking.OnWebSocketTextReceived += WebSocketTextReceived;
            RealtimeNetworking.OnWebSocketMessageReceived += WebSocketMessageReceived;

            // This connects to Settings.webSocketUrl, for example ws://127.0.0.1:5556/ws/.
            RealtimeNetworking.ConnectWebSocket();
        }

        private void OnDestroy()
        {
            RealtimeNetworking.OnDisconnectedFromServer -= Disconnected;
            RealtimeNetworking.OnConnectingToServerResult -= ConnectResult;
            RealtimeNetworking.OnWebSocketTextReceived -= WebSocketTextReceived;
            RealtimeNetworking.OnWebSocketMessageReceived -= WebSocketMessageReceived;
        }

        private void Disconnected()
        {
            Debug.Log("WebSocket disconnected from server.");
        }

        private void ConnectResult(bool successful)
        {
            if (!successful)
            {
                Debug.Log("Failed to connect WebSocket server.");
                return;
            }

            Debug.Log("Connected to WebSocket server successfully. Client ID: " + Client.instance.id);

            // Example for the patched server: MessageID.AUTH = 1.
            Sender.WebSocket_Send(1, "{\"username\":\"demo\",\"password\":\"123456\"}");

            // You can also use the message name accepted by the server.
            // Sender.WebSocket_Send("AUTH", "{\"username\":\"demo\",\"password\":\"123456\"}");
        }

        private void WebSocketTextReceived(string rawJson)
        {
            Debug.Log("WS <= " + rawJson);
        }

        private void WebSocketMessageReceived(int messageID, string messageName, string jsonValue, string rawJson)
        {
            Debug.Log("WS message <= ID: " + messageID + " Name: " + messageName + " Data: " + jsonValue);
        }
    }
}
