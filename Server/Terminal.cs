using MySqlX.XDevAPI;
using Newtonsoft.Json;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Crypto.Utilities;
using System;
using System.Numerics;

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
                    CS_AutenticationMessage message = JsonConvert.DeserializeObject<CS_AutenticationMessage>(jsonValue);
                    Console.WriteLine("username:{0} password:{1}", message.username, message.password);
                    SendAuthenticationResponse(clientID, true, "Authentication successful.");
                    break;
                case MessageID.GET_PROFILE:
                    break;
                default:
                    break;
            }

        }

        private static void SendAuthenticationResponse(int clientID, bool success, string message)
        {
            SC_AutenticationMessage responseMessage = new SC_AutenticationMessage();
            responseMessage.loginResult = success ? MessageStatus.SUCCESS : MessageStatus.ERROR;
            responseMessage.message = message;
            SendMessage(clientID, MessageID.AUTH, responseMessage);
        }


        private static void SendMessage(int clientID, MessageID id, IBaseMessage baseMessage)
        {

            Packet _packet = new Packet();
            _packet.Write((int)id);
            _packet.Write(JsonConvert.SerializeObject(baseMessage));
            _packet.SetID((int)Packet.ID.CUSTOM);
            _packet.WriteLength();
            Sender.TCP_Send(clientID, id, _packet);

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