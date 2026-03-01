using Assets.Script.Manager;
using DevelopersHub.RealtimeNetworking.Client;
using NUnit.Framework.Constraints;
using System;
using UnityEditor.Sprites;
using UnityEngine;
using static DevelopersHub.RealtimeNetworking.Client.RealtimeNetworking;

public class OnlineManager : MonoBehaviour
{
    public enum MessageID
    {
        AUTH = 1,
        GET_ROOMS = 2,
        CREATE_ROOM = 3,
        JOIN_ROOM = 4,
        LEAVE_ROOM = 5,
        DELETE_ROOM = 6,
        ROOM_UPDATED = 7,
        KICK_FROM_ROOM = 8,
        STATUS_IN_ROOM = 9,
        START_ROOM = 10,
        SYNC_GAME = 11,
        SET_HOST = 12,
        DESTROY_OBJECT = 13,
        CHANGE_OWNER = 14,
        CHANGE_OWNER_CONFIRM = 15,
        CREATE_PARTY = 16,
        INVITE_PARTY = 17,
        LEAVE_PARTY = 18,
        KICK_PARTY_MEMBER = 19,
        JOIN_MATCHMAKING = 20,
        LEAVE_MATCHMAKING = 21,
        PARTY_UPDATED = 22,
        GET_FRIENDS = 23,
        ADD_FRIEND = 24,
        REMOVE_FRIEND = 25,
        ANSWER_FRIEND = 26,
        GET_PROFILE = 27,
        ANSWER_PARTY_INVITE = 28,
        MATCHMAKING_STARTED = 29,
        MATCHMAKING_STOPPED = 30,
        LEAVE_GAME = 31,
        GAME_STARTED = 32,
        NETCODE_INIT = 33,
        NETCODE_STARTED = 34,
        FRIEND_REQUESTS = 35,
        PURCHASE = 36,
        GET_CHARACTERS = 37,
        GET_EQUIPMENTS = 38,
        SET_CHARACTER_SELECTED = 39,
        CHARACTER_EQUIP = 40,
        CHARACTER_UNEQUIP = 41
    }

    private static OnlineManager _instance;
    public static OnlineManager Instance => _instance;

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

    public void SendAutentication()
    {
        AutenticationMessage aut = new AutenticationMessage();
        aut.username = "test";
        aut.password = "test";

        SendMessage(MessageID.AUTH, aut);
    }

    public void GetPlayerProfile()
    {
        GetPlayerProfileMessage mes = new GetPlayerProfileMessage();

        SendMessage(MessageID.GET_PROFILE, mes);
    }

    void SendMessage(MessageID id, IBaseMessage baseMessage)
    {

        Packet _packet = new Packet();
        _packet.Write((int)id);
        _packet.Write(JsonUtility.ToJson(baseMessage));
        _packet.SetID((int)Packet.ID.INTERNAL);
        _packet.WriteLength();
        Client.instance.tcp.SendData(_packet);

    }

    internal void ReceiveInternal(Packet packet)
    {
        MessageID id = (MessageID)packet.ReadInt();
        switch (id)
        {
            case MessageID.AUTH:
                break;
            case MessageID.GET_ROOMS:
                break;
            case MessageID.CREATE_ROOM:
                break;
            case MessageID.JOIN_ROOM:
                break;
            case MessageID.LEAVE_ROOM:
                break;
            case MessageID.DELETE_ROOM:
                break;
            case MessageID.ROOM_UPDATED:
                break;
            case MessageID.KICK_FROM_ROOM:
                break;
            case MessageID.STATUS_IN_ROOM:
                break;
            case MessageID.START_ROOM:
                break;
            case MessageID.SYNC_GAME:
                break;
            case MessageID.SET_HOST:
                break;
            case MessageID.DESTROY_OBJECT:
                break;
            case MessageID.CHANGE_OWNER:
                break;
            case MessageID.CHANGE_OWNER_CONFIRM:
                break;
            case MessageID.CREATE_PARTY:
                break;
            case MessageID.INVITE_PARTY:
                break;
            case MessageID.LEAVE_PARTY:
                break;
            case MessageID.KICK_PARTY_MEMBER:
                break;
            case MessageID.JOIN_MATCHMAKING:
                break;
            case MessageID.LEAVE_MATCHMAKING:
                break;
            case MessageID.PARTY_UPDATED:
                break;
            case MessageID.GET_FRIENDS:
                break;
            case MessageID.ADD_FRIEND:
                break;
            case MessageID.REMOVE_FRIEND:
                break;
            case MessageID.ANSWER_FRIEND:
                break;
            case MessageID.GET_PROFILE:
                PlayerProfileData profile = JsonUtility.FromJson<PlayerProfileData>(packet.ReadString());
                PlayerProfile.Instance.Initialize(profile);
                break;
            case MessageID.ANSWER_PARTY_INVITE:
                break;
            case MessageID.MATCHMAKING_STARTED:
                break;
            case MessageID.MATCHMAKING_STOPPED:
                break;
            case MessageID.LEAVE_GAME:
                break;
            case MessageID.GAME_STARTED:
                break;
            case MessageID.NETCODE_INIT:
                break;
            case MessageID.NETCODE_STARTED:
                break;
            case MessageID.FRIEND_REQUESTS:
                break;
            case MessageID.PURCHASE:
                break;
            case MessageID.GET_CHARACTERS:
                break;
            case MessageID.GET_EQUIPMENTS:
                break;
            case MessageID.SET_CHARACTER_SELECTED:
                break;
            case MessageID.CHARACTER_EQUIP:
                break;
            case MessageID.CHARACTER_UNEQUIP:
                break;
            default:
                break;
        }


    }
}
