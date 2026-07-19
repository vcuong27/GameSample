using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using Newtonsoft.Json;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Crypto.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Numerics;
using static DevelopersHub.RealtimeNetworking.Server.Packet;

namespace DevelopersHub.RealtimeNetworking.Server
{

    class Terminal
    {

        static List<int> connectedPlayerIDs = new List<int>();

        #region Update
        public const int updatesPerSecond = 30;
        public static void Start()
        {
            Console.WriteLine("Server Started.");
        }

        public static void Update()
        {

        }
        #endregion

        #region Connection
        public const int maxPlayers = 100000;
        public static int onlinePlayers = 0;
        public const int port = 8888;
        public const int websocketPort = 9999;
        // Use "localhost" for local Unity editor testing without Administrator permission.
        // Use "+" to accept LAN/mobile clients, then register URL ACL as described in WebSocket_USAGE.md.
        public const string websocketHost = "localhost";
        public const string websocketPath = "/ws/";

        public static void OnClientConnected(int id, string ip)
        {
            if (!connectedPlayerIDs.Contains(id))
            {
                connectedPlayerIDs.Add(id);
            }
            onlinePlayers = connectedPlayerIDs.Count;
        }

        public static void OnClientDisconnected(int id, string ip)
        {
            connectedPlayerIDs.Remove(id);
            onlinePlayers = connectedPlayerIDs.Count;
        }
        #endregion

        #region Data
        public static void ReceivedPacket(int clientID, Packet packet)
        {
            MessageID id = (MessageID)packet.ReadInt();
            string jsonValue = packet.ReadString();
            Console.WriteLine("RECV <= MessageID:{0} jsonValue:{1} \n", id, jsonValue);

            switch (id)
            {
                #region AUTH
                case MessageID.AUTH:
                    CS_Auth message = JsonConvert.DeserializeObject<CS_Auth>(jsonValue);
                    //Console.WriteLine("username:{0} password:{1}", message.username, message.password);
                    SC_Auth authResult = Database.GetLoginResult(message.username, message.password);
                    SendAuthenticationResponse(clientID, authResult);
                    break;
                #endregion

                #region PROFILE
                case MessageID.PROFILE_GET:
                    CS_PlayerProfileGet profileGetMessage = JsonConvert.DeserializeObject<CS_PlayerProfileGet>(jsonValue);
                    SC_PlayerProfile profileResult = Database.GetPlayerProfile(profileGetMessage.playerID);
                    SendPlayerProfileResponse(clientID, profileResult);
                    break;
                case MessageID.PROFILE_UPDATE:
                    CS_PlayerProfile profileUpdateMessage = JsonConvert.DeserializeObject<CS_PlayerProfile>(jsonValue);
                    Database.UpdatePlayerProfile(profileUpdateMessage.playerID, profileUpdateMessage.playerName, profileUpdateMessage.profileVersion, profileUpdateMessage.jsonData);
                    break;
                case MessageID.PROFILE_CREATE:
                    CS_PlayerProfile profileCreateMessage = JsonConvert.DeserializeObject<CS_PlayerProfile>(jsonValue);
                    Database.CreatePlayerProfile(profileCreateMessage.playerID, profileCreateMessage.playerName, profileCreateMessage.profileVersion, profileCreateMessage.jsonData);
                    break;
                #endregion

                #region CLAN

                case MessageID.CLAN_CREATE:
                    CS_ClanCreate clanCreateMessage = JsonConvert.DeserializeObject<CS_ClanCreate>(jsonValue);
                    SC_ClanCreate clanCreateResult = Database.CreateClan(clanCreateMessage.name, clanCreateMessage.playerID, clanCreateMessage.jsonData);
                    SendClanCreateResponse(clientID, clanCreateResult);
                    break;
                case MessageID.CLAN_INFO:
                    CS_ClanInfo clanInfoMessage = JsonConvert.DeserializeObject<CS_ClanInfo>(jsonValue);
                    SC_ClanInfo clanInfoResult = Database.GetClanInfo(clanInfoMessage.clanID);
                    SendClanInfoResponse(clientID, clanInfoResult);
                    break;
                case MessageID.CLAN_LIST:
                    CS_ClanList clanListMessage = JsonConvert.DeserializeObject<CS_ClanList>(jsonValue);
                    SC_ClanList clanListResult = Database.GetClanList(clanListMessage.pageIndex, clanListMessage.pageSize);
                    SendClanListResponse(clientID, clanListResult);
                    break;
                case MessageID.CLAN_UPDATE:
                    CS_ClanUpdate clanUpdateMessage = JsonConvert.DeserializeObject<CS_ClanUpdate>(jsonValue);
                    SC_ClanUpdate clanUpdateResult = Database.UpdateClan(clanUpdateMessage.clanID, clanUpdateMessage.name, clanUpdateMessage.jsonData);
                    SendClanUpdateResponse(clientID, clanUpdateResult);
                    break;
                case MessageID.CLAN_KICK:
                    CS_ClanKick clanKickMessage = JsonConvert.DeserializeObject<CS_ClanKick>(jsonValue);
                    SC_ClanKick clanKickResult = Database.KickPlayerFromClan(clanKickMessage.clanID, clanKickMessage.playerID);
                    SendClanKickResponse(clientID, clanKickResult);
                    break;
                case MessageID.CLAN_ACCEPT:
                    CS_ClanAccept clanAcceptMessage = JsonConvert.DeserializeObject<CS_ClanAccept>(jsonValue);
                    SC_ClanAccept clanAcceptResult = Database.AcceptPlayerIntoClan(clanAcceptMessage.clanID, clanAcceptMessage.playerID);
                    SendClanAcceptResponse(clientID, clanAcceptResult);
                    break;
                case MessageID.CLAN_REQUEST:
                    CS_ClanRequest clanRequestMessage = JsonConvert.DeserializeObject<CS_ClanRequest>(jsonValue);
                    SC_ClanJoin clanRequestResult = Database.RequestToJoinClan(clanRequestMessage.clanID, clanRequestMessage.playerID);
                    SendClanRequestResponse(clientID, clanRequestResult);
                    break;
                case MessageID.CLAN_JOIN:
                    CS_ClanJoin clanJoinMessage = JsonConvert.DeserializeObject<CS_ClanJoin>(jsonValue);
                    SC_ClanJoin clanJoinResult = Database.JoinClan(clanJoinMessage.clanID, clanJoinMessage.playerID);
                    SendClanJoinResponse(clientID, clanJoinResult);
                    break;
                case MessageID.CLAN_LEAVE:
                    CS_ClanLeave clanLeaveMessage = JsonConvert.DeserializeObject<CS_ClanLeave>(jsonValue);
                    SC_ClanLeave clanLeaveResult = Database.LeaveClan(clanLeaveMessage.clanID, clanLeaveMessage.playerID);
                    SendClanLeaveResponse(clientID, clanLeaveResult);
                    break;
                case MessageID.CLAN_WAR_START:
                    CS_ClanWarStart clanWarStartMessage = JsonConvert.DeserializeObject<CS_ClanWarStart>(jsonValue);
                    SC_ClanWarStart clanWarStartResult = Database.StartClanWar(clanWarStartMessage.attackClanID, clanWarStartMessage.defendClanID);
                    SendClanWarStartResponse(clientID, clanWarStartResult);
                    break;
                case MessageID.CLAN_WAR_INFO:
                    CS_ClanWarInfo clanWarInfoMessage = JsonConvert.DeserializeObject<CS_ClanWarInfo>(jsonValue);
                    SC_ClanWarInfo clanWarInfoResult = Database.GetClanWarInfo(clanWarInfoMessage.warID);
                    SendClanWarInfoResponse(clientID, clanWarInfoResult);
                    break;
                #endregion

                #region CHAT
                case MessageID.CHAT_HISTORIES:
                    CS_ChatHistories chatHistoriesMessage = JsonConvert.DeserializeObject<CS_ChatHistories>(jsonValue);
                    SC_ChatHistories chatHistoriesResult = Database.GetChatHistories(chatHistoriesMessage.playerID, chatHistoriesMessage.clanID);
                    SendChatHistoriesResponse(clientID, chatHistoriesResult);
                    break;
                case MessageID.CHAT_MESSAGE:
                    CS_ChatMessage chatMessageMessage = JsonConvert.DeserializeObject<CS_ChatMessage>(jsonValue);
                    Database.SendChatMessage(chatMessageMessage);
                    SendChatToOtherPlayers(clientID, chatMessageMessage);
                    break;
                #endregion

                default:
                    break;
            }
        }

        private static void SendChatToOtherPlayers(int clientID, CS_ChatMessage chatMessageMessage)
        {
            foreach (var client in connectedPlayerIDs)
            {
                if (client != clientID)
                {
                    //private message to other players
                    if (chatMessageMessage.otherPlayerID != clientID)
                    {
                        if (chatMessageMessage.otherPlayerID == client)
                        {
                            SendMessage(client, MessageID.CHAT_MESSAGE, chatMessageMessage);
                        }
                    }
                    else
                    {
                        //send to all players
                        SendMessage(client, MessageID.CHAT_MESSAGE, chatMessageMessage);
                    }
                }
            }
        }

        private static void SendChatHistoriesResponse(int clientID, SC_ChatHistories chatHistoriesResult)
        {
            SendMessage(clientID, MessageID.CHAT_HISTORIES, chatHistoriesResult);
        }

        private static void SendClanWarInfoResponse(int clientID, SC_ClanWarInfo clanWarInfoResult)
        {
            SendMessage(clientID, MessageID.CLAN_WAR_INFO, clanWarInfoResult);
        }

        private static void SendClanWarStartResponse(int clientID, SC_ClanWarStart clanWarStartResult)
        {
            SendMessage(clientID, MessageID.CLAN_WAR_START, clanWarStartResult);
        }

        #region CLAN

        private static void SendClanLeaveResponse(int clientID, SC_ClanLeave clanLeaveResult)
        {
            SendMessage(clientID, MessageID.CLAN_LEAVE, clanLeaveResult);
        }

        private static void SendClanJoinResponse(int clientID, SC_ClanJoin clanJoinResult)
        {
            SendMessage(clientID, MessageID.CLAN_JOIN, clanJoinResult);
        }

        private static void SendClanRequestResponse(int clientID, SC_ClanJoin clanRequestResult)
        {
            SendMessage(clientID, MessageID.CLAN_REQUEST, clanRequestResult);
        }

        private static void SendClanAcceptResponse(int clientID, SC_ClanAccept clanAcceptResult)
        {
            SendMessage(clientID, MessageID.CLAN_ACCEPT, clanAcceptResult);
        }

        private static void SendClanKickResponse(int clientID, SC_ClanKick clanKickResult)
        {
            SendMessage(clientID, MessageID.CLAN_KICK, clanKickResult);
        }

        public static void SendClanUpdateResponse(int clientID, SC_ClanUpdate clanUpdateResult)
        {
            SendMessage(clientID, MessageID.CLAN_UPDATE, clanUpdateResult);
        }

        private static void SendClanInfoResponse(int clientID, SC_ClanInfo clanInfoResult)
        {
            SendMessage(clientID, MessageID.CLAN_INFO, clanInfoResult);
        }

        private static void SendClanCreateResponse(int clientID, SC_ClanCreate clanCreateResult)
        {
            SendMessage(clientID, MessageID.CLAN_CREATE, clanCreateResult);
        }

        private static void SendClanListResponse(int clientID, SC_ClanList clanListResult)
        {
            SendMessage(clientID, MessageID.CLAN_LIST, clanListResult);
        }

        #endregion

        #region PROFILE

        private static void SendPlayerProfileResponse(int clientID, SC_PlayerProfile profileResult)
        {
            SendMessage(clientID, MessageID.PROFILE_GET, profileResult);
        }

        #endregion

        #region AUTH

        private static void SendAuthenticationResponse(int clientID, SC_Auth authResult)
        {
            SendMessage(clientID, MessageID.AUTH, authResult);
        }
        #endregion

        private static void SendMessage(int clientID, MessageID id, IBaseMessage baseMessage)
        {
            string jsonValue = JsonConvert.SerializeObject(baseMessage);

            if (Server.clients.ContainsKey(clientID) && Server.clients[clientID].webSocket != null && Server.clients[clientID].webSocket.IsConnected)
            {
                Server.clients[clientID].webSocket.SendMessage(id, jsonValue);
            }
            else
            {
                Packet _packet = new Packet();
                _packet.Write((int)id);
                _packet.Write(jsonValue);
                Sender.TCP_Send(clientID, _packet);
            }

            Console.WriteLine("SEND => Client[{0}] Sent MessageID:{1} jsonValue:{2}\n", clientID, id, jsonValue);
        }















        #region NOT USED

        public static void ReceivedBytes(int clientID, int packetID, byte[] data)
        {

        }

        public static void ReceivedString(int clientID, int packetID, string data)
        {

        }

        public static void ReceivedInteger(int clientID, int packetID, int data)
        {

        }

        public static void ReceivedFloat(int clientID, int packetID, float data)
        {

        }

        public static void ReceivedBoolean(int clientID, int packetID, bool data)
        {

        }

        public static void ReceivedVector3(int clientID, int packetID, Vector3 data)
        {

        }

        public static void ReceivedQuaternion(int clientID, int packetID, Quaternion data)
        {

        }

        public static void ReceivedLong(int clientID, int packetID, long data)
        {

        }

        public static void ReceivedShort(int clientID, int packetID, short data)
        {

        }

        public static void ReceivedByte(int clientID, int packetID, byte data)
        {

        }

        public static void ReceivedEvent(int clientID, int packetID)
        {

        }
        #endregion

        #endregion

    }
}