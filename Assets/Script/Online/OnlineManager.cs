using DevelopersHub.RealtimeNetworking.Client;
using System;
using System.Collections.Generic;
using UnityEngine;

public class OnlineManager : Singleton<OnlineManager>
{

    private bool isConnected = false;
    private bool isLoggedIn = false;
    private int playerID;

    public OnlineManager()
    {
        RealtimeNetworking.OnDisconnectedFromServer += Disconnected;
        RealtimeNetworking.OnConnectingToServerResult += ConnectResult;
        RealtimeNetworking.OnPacketReceived += PacketReceived;
        RealtimeNetworking.OnWebSocketMessageReceived += WebSocketMessageReceived;
    }

    ~OnlineManager()
    {
        RealtimeNetworking.OnDisconnectedFromServer -= Disconnected;
        RealtimeNetworking.OnConnectingToServerResult -= ConnectResult;
        RealtimeNetworking.OnPacketReceived -= PacketReceived;
        RealtimeNetworking.OnWebSocketMessageReceived -= WebSocketMessageReceived;
    }

    public void ConnectToServer()
    {
        if (true)
        {
            RealtimeNetworking.ConnectWebSocket();
        }
        else
        {
            RealtimeNetworking.ConnectTCP();
        }
    }

    private void ConnectResult(bool successful)
    {
        if (successful)
        {
            Debug.Log("Connected to server successfully.");
            isConnected = true;
        }
        else
        {
            Debug.Log("Failed to connect the server.");
            isConnected = false;
        }
    }

    private void Disconnected()
    {
        Debug.Log("Disconnected from server.");
        isConnected = false;
        isLoggedIn = false;
    }

    public bool IsConnected()
    {
        return isConnected;
    }

    public bool IsLoggedIn()
    {
        return isLoggedIn;
    }

    public int GetPlayerID()
    {
        return playerID;
    }

    private void PacketReceived(Packet packet)
    {
        MessageID id = (MessageID)packet.ReadInt();
        string jsonValue = packet.ReadString();
        ReceiveServerMessage(id, jsonValue);
    }

    private void WebSocketMessageReceived(int messageID, string messageName, string jsonValue, string rawJson)
    {
        MessageID id = (MessageID)messageID;
        ReceiveServerMessage(id, jsonValue);
    }

    private void ReceiveServerMessage(MessageID id, string jsonValue)
    {
        Debug.LogFormat("MessageID:{0} jsonValue:{1}", id, jsonValue);
        switch (id)
        {
            case MessageID.AUTH:
                SC_Auth message = JsonUtility.FromJson<SC_Auth>(jsonValue);
                if (message.loginResult == MessageStatus.SUCCESS)
                {
                    Debug.LogFormat("Login successful");
                    playerID = message.playerID;
                    isLoggedIn = true;
                }
                else
                {
                    Debug.LogFormat("Login failed: {0}", message.loginResult);
                    isLoggedIn = false;
                }
                break;
            case MessageID.PROFILE_GET:
                SC_PlayerProfile profileMessage = JsonUtility.FromJson<SC_PlayerProfile>(jsonValue);
                if (profileMessage.getProfileResult == MessageStatus.SUCCESS)
                {
                    PlayerProfile.Instance.Initialize(profileMessage.jsonData);
                    Debug.LogFormat("Profile retrieved successfully");
                }
                else
                {
                    PlayerProfile.Instance.Initialize("");
                    Debug.LogFormat("Failed to retrieve profile: {0}", profileMessage.getProfileResult);
                }
                break;

            case MessageID.CLAN_CREATE:
                SC_ClanCreate clanCreateMessage = JsonUtility.FromJson<SC_ClanCreate>(jsonValue);
                if (clanCreateMessage.createResult == MessageStatus.SUCCESS)
                {
                    Debug.LogFormat("Clan created successfully");
                    ClanManager.Instance.OnClanCreated(clanCreateMessage);
                }
                else
                {
                    Debug.LogFormat("Failed to create clan: {0}", clanCreateMessage.createResult);
                }
                break;

            case MessageID.CLAN_INFO:
                SC_ClanInfo clanInfoMessage = JsonUtility.FromJson<SC_ClanInfo>(jsonValue);
                if (clanInfoMessage.getInfoResult == MessageStatus.SUCCESS)
                {
                    Debug.LogFormat("Clan info received successfully");
                    ClanManager.Instance.OnClanInfoReceived(clanInfoMessage);
                }
                else
                {
                    Debug.LogFormat("Failed to get clan info: {0}", clanInfoMessage.getInfoResult);
                }
                break;
            case MessageID.CLAN_LIST:
                SC_ClanList clanListMessage = JsonUtility.FromJson<SC_ClanList>(jsonValue);
                if (clanListMessage.getListResult == MessageStatus.SUCCESS)
                {
                    Debug.LogFormat("Clan list received successfully");
                    ClanManager.Instance.OnListClanReceived(new List<ClanListInfo>(clanListMessage.clans));
                    ClanManager.OnCLanListReceived?.Invoke();
                }
                else
                {
                    Debug.LogFormat("Failed to get clan list: {0}", clanListMessage.getListResult);
                }
                break;
            case MessageID.CLAN_WAR_START:
                SC_ClanWarStart warStartMessage = JsonUtility.FromJson<SC_ClanWarStart>(jsonValue);
                if (warStartMessage.startResult == MessageStatus.SUCCESS)
                {
                    Debug.LogFormat("Clan war started successfully");
                    ClanManager.Instance.ClanWarStarted(warStartMessage);
                    ClanManager.OnClanWarStarted?.Invoke();
                }
                else
                {
                    Debug.LogFormat("Failed to start clan war: {0}", warStartMessage.startResult);
                }
                break;

            case MessageID.CLAN_WAR_INFO:
                SC_ClanWarInfo warInfoMessage = JsonUtility.FromJson<SC_ClanWarInfo>(jsonValue);
                if (warInfoMessage.getInfoResult == MessageStatus.SUCCESS)
                {
                    Debug.LogFormat("Clan war info received successfully");
                    ClanManager.Instance.ClanWarInfoReceived(warInfoMessage);
                    ClanManager.OnClanWarInfoReceived?.Invoke();
                }
                else
                {
                    Debug.LogFormat("Failed to get clan war info: {0}", warInfoMessage.getInfoResult);
                }
                break;

            case MessageID.CHAT_HISTORIES:
                SC_ChatHistories chatHistoriesMessage = JsonUtility.FromJson<SC_ChatHistories>(jsonValue);
                if (chatHistoriesMessage.getHistoriesResult == MessageStatus.SUCCESS)
                {
                    Debug.LogFormat("Chat histories received successfully");
                    ChatManager.Instance.ReceiveChatHistories(chatHistoriesMessage);
                    ChatManager.OnChatHistoriesReceived?.Invoke();
                }
                else
                {
                    Debug.LogFormat("Failed to get chat histories: {0}", chatHistoriesMessage.getHistoriesResult);
                }
                break;
            case MessageID.CHAT_MESSAGE:
                SC_ChatMessage chatMessage = JsonUtility.FromJson<SC_ChatMessage>(jsonValue);
                Debug.LogFormat("Chat message sent successfully");
                ChatManager.Instance.ReceiveChatMessage(chatMessage);
                ChatManager.OnChatMessageReceived?.Invoke();

                break;

            default:
                break;
        }

    }


    public void LoginToServer(string username, string password)
    {
        CS_Auth aut = new CS_Auth();
        aut.username = username;
        aut.password = password;

        SendToServer(MessageID.AUTH, aut);
    }

    public void GetPlayerProfile()
    {
        CS_PlayerProfileGet mes = new CS_PlayerProfileGet();
        mes.playerID = playerID;

        SendToServer(MessageID.PROFILE_GET, mes);
    }

    public void UpdatePlayerProfile(string playerName, int profileVersion, string jsonData)
    {
        CS_PlayerProfile mes = new CS_PlayerProfile();
        mes.playerID = playerID;
        mes.playerName = playerName;
        mes.profileVersion = profileVersion;
        mes.jsonData = jsonData;
        SendToServer(MessageID.PROFILE_UPDATE, mes);
    }

    public void CreatePlayerProfile(int playerID, string playerName, int profileVersion, string jsonData)
    {
        CS_PlayerProfile mes = new CS_PlayerProfile();
        mes.playerID = playerID;
        mes.playerName = playerName;
        mes.profileVersion = profileVersion;
        mes.jsonData = jsonData;
        SendToServer(MessageID.PROFILE_CREATE, mes);
    }

    public void CreateClan(string clanName, string jsonData)
    {
        CS_ClanCreate mes = new CS_ClanCreate();
        mes.name = clanName;
        mes.playerID = playerID;
        mes.jsonData = jsonData;
        SendToServer(MessageID.CLAN_CREATE, mes);
    }

    public void GetClanInfo(int clanID)
    {
        CS_ClanInfo mes = new CS_ClanInfo();
        mes.clanID = clanID;
        SendToServer(MessageID.CLAN_INFO, mes);
    }

    public void GetListClan(int index)
    {
        CS_ClanList mes = new CS_ClanList();
        mes.pageIndex = index;
        mes.pageSize = 10;
        SendToServer(MessageID.CLAN_LIST, mes);
    }


    public void AttackClan(int clanID, int otherClanID)
    {
        CS_ClanWarStart mes = new CS_ClanWarStart();
        mes.attackClanID = clanID;
        mes.defendClanID = otherClanID;
        SendToServer(MessageID.CLAN_WAR_START, mes);
    }

    public void GetClanWarInfo(int warID)
    {
        CS_ClanWarInfo mes = new CS_ClanWarInfo();
        mes.warID = warID;
        SendToServer(MessageID.CLAN_WAR_INFO, mes);
    }

    public void GetChatHistory()
    {
        CS_ChatHistories mes = new CS_ChatHistories();
        mes.playerID = playerID;
        mes.clanID = PlayerProfile.Instance.getClanID();
        SendToServer(MessageID.CHAT_HISTORIES, mes);
    }

    public void SendChatMessage(string message)
    {
        CS_ChatMessage mes = new CS_ChatMessage();
        mes.playerID = playerID;
        mes.otherPlayerID = playerID;
        mes.clanID = PlayerProfile.Instance.getClanID();
        mes.message = message;
        mes.sentTime = DateTimeOffset.UtcNow.DateTime;
        SendToServer(MessageID.CHAT_MESSAGE, mes);
    }

    void SendToServer(MessageID id, IBaseMessage baseMessage)
    {
        string jsonValue = JsonUtility.ToJson(baseMessage);

        if (Client.instance.usingWebSocket)
        {
            Sender.WebSocket_Send((int)id, jsonValue);
            Debug.LogFormat("WS SEND => MessageID:{0} jsonValue:{1}", id, jsonValue);

        }
        else
        {
            Packet _packet = new Packet();
            _packet.Write((int)id);
            _packet.Write(jsonValue);
            Sender.TCP_Send(_packet);
            Debug.LogFormat("TCP SEND => MessageID:{0} jsonValue:{1}", id, jsonValue);
        }
    }
}
