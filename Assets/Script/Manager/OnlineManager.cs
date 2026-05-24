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
                SC_AutenticationMessage message = JsonUtility.FromJson<SC_AutenticationMessage>(jsonValue);
                if (message.loginResult == MessageStatus.SUCCESS)
                {
                    Debug.LogFormat("Login successful");
                    isLoggedIn = true;
                }
                else
                {
                    Debug.LogFormat("Login failed: {0}", message.message);
                    isLoggedIn = false;
                }
                break;
            case MessageID.GET_PROFILE:
                break;
            default:
                break;
        }

    }


    public void LoginToServer()
    {
        CS_AutenticationMessage aut = new CS_AutenticationMessage();
        aut.username = "account_01";
        aut.password = "password_01";

        SendMessage(MessageID.AUTH, aut);
    }

    public void GetPlayerProfile()
    {
        CS_PlayerProfileMessage mes = new CS_PlayerProfileMessage();
        mes.playerID = 1;

        SendMessage(MessageID.GET_PROFILE, mes);
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
