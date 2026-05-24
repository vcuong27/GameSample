using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using Newtonsoft.Json;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Crypto.Utilities;
using System;
using System.Data;
using System.Numerics;
using static DevelopersHub.RealtimeNetworking.Server.Packet;

namespace DevelopersHub.RealtimeNetworking.Server
{

    class Terminal
    {

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
        public const int port = 5555;

        public static void OnClientConnected(int id, string ip)
        {
            onlinePlayers++;
        }

        public static void OnClientDisconnected(int id, string ip)
        {
            onlinePlayers--;
        }
        #endregion

        #region Data
        public static void ReceivedPacket(int clientID, Packet packet)
        {
            MessageID id = (MessageID)packet.ReadInt();
            string jsonValue = packet.ReadString();
            Console.WriteLine("MessageID:{0} jsonValue:{1}", id, jsonValue);

            switch (id)
            {
                case MessageID.AUTH:
                    CS_Auth message = JsonConvert.DeserializeObject<CS_Auth>(jsonValue);
                    Console.WriteLine("username:{0} password:{1}", message.username, message.password);
                    SC_Auth authResult = Database.GetLoginResult(message.username, message.password);
                    SendAuthenticationResponse(clientID, authResult);
                    break;
                case MessageID.PROFILE_GET:
                    CS_PlayerProfileGet profileGetMessage = JsonConvert.DeserializeObject<CS_PlayerProfileGet>(jsonValue);
                    SC_PlayerProfile profileResult = Database.GetPlayerProfile(profileGetMessage.playerID);
                    SendPlayerProfileResponse(clientID, profileResult);
                    break;
                case MessageID.PROFILE_UPDATE:
                    CS_PlayerProfileUpdate profileUpdateMessage = JsonConvert.DeserializeObject<CS_PlayerProfileUpdate>(jsonValue);
                    Database.UpdatePlayerProfile(profileUpdateMessage.playerID, profileUpdateMessage.playerName, profileUpdateMessage.profileVersion, profileUpdateMessage.jsonData);
                    break;
                default:
                    break;
            }
        }

        private static void SendPlayerProfileResponse(int clientID, SC_PlayerProfile profileResult)
        {
            SendMessage(clientID, MessageID.PROFILE_GET, profileResult);
        }

        private static void SendAuthenticationResponse(int clientID, SC_Auth authResult)
        {
            SendMessage(clientID, MessageID.AUTH, authResult);
        }


        private static void SendMessage(int clientID, MessageID id, IBaseMessage baseMessage)
        {
            Packet _packet = new Packet();
            _packet.Write((int)id);
            _packet.Write(JsonConvert.SerializeObject(baseMessage));
            Sender.TCP_Send(clientID, _packet);
            Console.WriteLine("Client[{0}] Sent MessageID:{1} jsonValue:{2}", clientID, id, JsonConvert.SerializeObject(baseMessage));
        }
















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

    }
}