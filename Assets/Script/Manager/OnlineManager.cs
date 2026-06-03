using Assets.Script.Manager;
using DevelopersHub.RealtimeNetworking.Client;
using System;
using UnityEngine;

public class OnlineManager : MonoBehaviour
{



    private static OnlineManager _instance;
    public static OnlineManager Instance => _instance;


    private bool isConnected = false;
    private bool isLoggedIn = false;
    private int playerID;

    private void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }


    private void Start()
    {
        RealtimeNetworking.OnDisconnectedFromServer += Disconnected;
        RealtimeNetworking.OnConnectingToServerResult += ConnectResult;
        RealtimeNetworking.OnPacketReceived += PacketReceived;
    }

    private void OnDestroy()
    {
        RealtimeNetworking.OnDisconnectedFromServer -= Disconnected;
        RealtimeNetworking.OnConnectingToServerResult -= ConnectResult;
        RealtimeNetworking.OnPacketReceived -= PacketReceived;
    }

    public void ConnectToServer()
    {
        RealtimeNetworking.Connect();
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
            default:
                break;
        }

    }


    public void LoginToServer()
    {
        CS_Auth aut = new CS_Auth();
        aut.username = "player01";
        aut.password = "123456";

        SendMessage(MessageID.AUTH, aut);
    }

    public void GetPlayerProfile()
    {
        CS_PlayerProfileGet mes = new CS_PlayerProfileGet();
        mes.playerID = playerID;

        SendMessage(MessageID.PROFILE_GET, mes);
    }

    public void UpdatePlayerProfile(string playerName, int profileVersion, string jsonData)
    {
        CS_PlayerProfile mes = new CS_PlayerProfile();
        mes.playerID = playerID;
        mes.playerName = playerName;
        mes.profileVersion = profileVersion;
        mes.jsonData = jsonData;
        SendMessage(MessageID.PROFILE_UPDATE, mes);
    }

    public void CreatePlayerProfile(int playerID, string playerName, int profileVersion, string jsonData)
    {
        CS_PlayerProfile mes = new CS_PlayerProfile();
        mes.playerID = playerID;
        mes.playerName = playerName;
        mes.profileVersion = profileVersion;
        mes.jsonData = jsonData;
        SendMessage(MessageID.PROFILE_CREATE, mes);
    }

    public void CreateClan(string clanName, string jsonData)
    {
        CS_ClanCreate mes = new CS_ClanCreate();
        mes.name = clanName;
        mes.playerID = playerID;
        mes.jsonData = jsonData;
        SendMessage(MessageID.CLAN_CREATE, mes);
    }

    public void GetClanInfo(int clanID)
    {
        CS_ClanInfo mes = new CS_ClanInfo();
        mes.clanID = clanID;
        SendMessage(MessageID.CLAN_INFO, mes);
    }



















    void SendMessage(MessageID id, IBaseMessage baseMessage)
    {

        Packet _packet = new Packet();
        _packet.Write((int)id);
        _packet.Write(JsonUtility.ToJson(baseMessage));
        _packet.SetID((int)Packet.ID.CUSTOM);
        _packet.WriteLength();
        Client.instance.tcp.SendData(_packet);
    }

}
