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
    private string playerID;

    private void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }


    private void Start()
    {

    }

    private void OnDestroy()
    {

    }

    public void ConnectToServer()
    {
        RealtimeNetworking.OnDisconnectedFromServer += Disconnected;
        RealtimeNetworking.OnConnectingToServerResult += ConnectResult;
        RealtimeNetworking.OnPacketReceived += PacketReceived;
        // Try to connect the server
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


    private void PacketReceived(Packet packet)
    {
        MessageID id = (MessageID)packet.ReadInt();
        string jsonValue = packet.ReadString();
        Debug.LogFormat("MessageID:{0} jsonValue:{1}", id, jsonValue);
        switch (id)
        {
            case MessageID.AUTH:
                SC_Auth  message = JsonUtility.FromJson<SC_Auth>(jsonValue);
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
        CS_PlayerProfileUpdate mes = new CS_PlayerProfileUpdate();
        mes.playerID = playerID;
        mes.playerName = playerName;
        mes.profileVersion = profileVersion;
        mes.jsonData = jsonData;
        SendMessage(MessageID.PROFILE_UPDATE, mes);
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
