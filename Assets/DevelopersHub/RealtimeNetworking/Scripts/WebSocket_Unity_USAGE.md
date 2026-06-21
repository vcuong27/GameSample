# Unity WebSocket client usage

This client now supports the WebSocket endpoint added to the server.

Default server endpoint:

```text
ws://<server-ip>:5556/ws/
```

## Files added or changed

- `Client.cs`
  - Added `WebSocketTransport webSocket`.
  - Added `ConnectToServer(bool useWebSocket)`.
  - Keeps the old TCP/UDP flow when WebSocket is disabled.
- `Client.WebSocket.cs`
  - New WebSocket connection, receive loop, text receive, binary receive, disconnect handling.
- `Settings.cs`
  - Added WebSocket settings: `useWebSocket`, `secureWebSocket`, `webSocketPort`, `webSocketPath`, `webSocketUrl`.
- `RealtimeNetworking.cs`
  - Added `ConnectWebSocket()` and `ConnectTCP()`.
  - Added `OnWebSocketTextReceived` and `OnWebSocketMessageReceived` events.
- `Sender.cs`
  - Added `WebSocket_Send(...)` helpers.
  - Existing `TCP_Send(...)` and `UDP_Send(...)` automatically send binary WebSocket packets when connected using WebSocket.
- `WebSocketJson.cs`
  - Small JSON helper for creating and reading the server WebSocket message format without adding external packages.
- `DemoWebSocket.cs`
  - Simple Unity example.

## Option 1: enable WebSocket in Settings asset

Open `Developers Hub > Realtime Networking > Settings` and set:

```text
IP: your server IP
Use Web Socket: true
Secure Web Socket: false
Web Socket Port: 5556
Web Socket Path: /ws/
```

Then your existing call works:

```csharp
RealtimeNetworking.Connect();
```

## Option 2: connect WebSocket from code

```csharp
RealtimeNetworking.ConnectWebSocket();
```

This ignores the `Use Web Socket` checkbox and connects with WebSocket directly.

## Send JSON message to the patched server

The patched server accepts this WebSocket JSON format:

```json
{
  "messageID": "AUTH",
  "data": {
    "username": "demo",
    "password": "123456"
  }
}
```

From Unity:

```csharp
Sender.WebSocket_Send("AUTH", "{\"username\":\"demo\",\"password\":\"123456\"}");
```

or using numeric ID:

```csharp
Sender.WebSocket_Send(1, "{\"username\":\"demo\",\"password\":\"123456\"}");
```

## Receive WebSocket response

```csharp
RealtimeNetworking.OnWebSocketMessageReceived += (messageID, messageName, jsonValue, rawJson) =>
{
    Debug.Log("Message ID: " + messageID);
    Debug.Log("Message Name: " + messageName);
    Debug.Log("JSON Data: " + jsonValue);
};
```

The raw JSON can also be received:

```csharp
RealtimeNetworking.OnWebSocketTextReceived += rawJson =>
{
    Debug.Log(rawJson);
};
```

## Binary compatibility

If you connect using WebSocket, existing calls like this are routed as WebSocket binary packets:

```csharp
Sender.TCP_Send(packet);
Sender.TCP_Send(123, "hello");
```

The patched server accepts WebSocket binary packets with the old TCP packet framing.

## Unity notes

- This implementation uses `System.Net.WebSockets.ClientWebSocket` and is intended for Unity builds that support .NET 4.x / .NET Standard networking APIs.
- For Android/iOS devices, do not use `127.0.0.1` unless the server runs on the same device. Use the LAN IP of the server machine.
- For WebGL builds, Unity does not support normal .NET sockets in the same way. WebGL usually needs a browser WebSocket wrapper/plugin.
