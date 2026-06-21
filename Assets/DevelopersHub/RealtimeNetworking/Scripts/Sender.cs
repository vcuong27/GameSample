namespace DevelopersHub.RealtimeNetworking.Client
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Sender : MonoBehaviour
    {

        #region Core
        private static void SendTCPData(Packet _packet)
        {
            _packet.WriteLength();
            if (Client.instance.usingWebSocket)
            {
                Client.instance.webSocket.SendData(_packet);
                return;
            }
            Client.instance.tcp.SendData(_packet);
        }

        private static void SendUDPData(Packet _packet)
        {
            _packet.WriteLength();
            if (Client.instance.usingWebSocket)
            {
                Client.instance.webSocket.SendData(_packet);
                return;
            }
            Client.instance.udp.SendData(_packet);
        }
        public static void WebSocket_SendRaw(string json)
        {
            if (!string.IsNullOrEmpty(json) && Client.instance.webSocket != null)
            {
                Client.instance.webSocket.SendText(json);
            }
        }

        public static void WebSocket_Send(int messageID, string jsonValue)
        {
            WebSocket_SendRaw(WebSocketJson.CreateMessage(messageID, jsonValue));
        }

        public static void WebSocket_Send(string messageID, string jsonValue)
        {
            WebSocket_SendRaw(WebSocketJson.CreateMessage(messageID, jsonValue));
        }

        public static void WebSocket_SendBinary(Packet packet)
        {
            if (packet != null && Client.instance.webSocket != null)
            {
                packet.WriteLength();
                Client.instance.webSocket.SendData(packet);
            }
        }
        #endregion

        #region TCP
        public static void TCP_Send(int packetID)
        {
            using (Packet packet = new Packet((int)Packet.ID.NULL))
            {
                packet.Write(packetID);
                SendTCPData(packet);
            }
        }

        public static void TCP_Send(Packet packet)
        {
            if(packet != null)
            {
                packet.SetID((int)Packet.ID.CUSTOM);
                SendTCPData(packet);
            }
        }

        public static void TCP_Send(int packetID, string data)
        {
            if (data != null)
            {
                using (Packet packet = new Packet((int)Packet.ID.STRING))
                {
                    packet.Write(packetID);
                    packet.Write(data);
                    SendTCPData(packet);
                }
            }
        }

        public static void TCP_Send(int packetID, byte[] data)
        {
            if (data != null)
            {
                using (Packet packet = new Packet((int)Packet.ID.BYTES))
                {
                    packet.Write(packetID);
                    packet.Write(data.Length);
                    packet.Write(data);
                    SendTCPData(packet);
                }
            }
        }

        public static void TCP_Send(int packetID, byte data)
        {
            using (Packet packet = new Packet((int)Packet.ID.BYTE))
            {
                packet.Write(packetID);
                packet.Write(data);
                SendTCPData(packet);
            }
        }

        public static void TCP_Send(int packetID, int data)
        {
            using (Packet packet = new Packet((int)Packet.ID.INTEGER))
            {
                packet.Write(packetID);
                packet.Write(data);
                SendTCPData(packet);
            }
        }

        public static void TCP_Send(int packetID, bool data)
        {
            using (Packet packet = new Packet((int)Packet.ID.BOOLEAN))
            {
                packet.Write(packetID);
                packet.Write(data);
                SendTCPData(packet);
            }
        }

        public static void TCP_Send(int packetID, float data)
        {
            using (Packet packet = new Packet((int)Packet.ID.FLOAT))
            {
                packet.Write(packetID);
                packet.Write(data);
                SendTCPData(packet);
            }
        }

        public static void TCP_Send(int packetID, short data)
        {
            using (Packet packet = new Packet((int)Packet.ID.SHORT))
            {
                packet.Write(packetID);
                packet.Write(data);
                SendTCPData(packet);
            }
        }

        public static void TCP_Send(int packetID, long data)
        {
            using (Packet packet = new Packet((int)Packet.ID.LONG))
            {
                packet.Write(packetID);
                packet.Write(data);
                SendTCPData(packet);
            }
        }

        public static void TCP_Send(int packetID, Vector3 data)
        {
            using (Packet packet = new Packet((int)Packet.ID.VECTOR3))
            {
                packet.Write(packetID);
                packet.Write(new System.Numerics.Vector3(data.x, data.y, data.z));
                SendTCPData(packet);
            }
        }

        public static void TCP_Send(int packetID, Quaternion data)
        {
            using (Packet packet = new Packet((int)Packet.ID.QUATERNION))
            {
                packet.Write(packetID);
                packet.Write(new System.Numerics.Quaternion(data.x, data.y, data.z, data.w));
                SendTCPData(packet);
            }
        }
        #endregion
        
        #region UDP
        public static void UDP_Send(int packetID)
        {
            using (Packet packet = new Packet((int)Packet.ID.NULL))
            {
                packet.Write(packetID);
                SendUDPData(packet);
            }
        }

        public static void UDP_Send(Packet packet)
        {
            if (packet != null)
            {
                packet.SetID((int)Packet.ID.CUSTOM);
                SendUDPData(packet);
            }
        }

        public static void UDP_Send(int packetID, string data)
        {
            if (data != null)
            {
                using (Packet packet = new Packet((int)Packet.ID.STRING))
                {
                    packet.Write(packetID);
                    packet.Write(data);
                    SendUDPData(packet);
                }
            }
        }

        public static void UDP_Send(int packetID, byte[] data)
        {
            if (data != null)
            {
                using (Packet packet = new Packet((int)Packet.ID.BYTES))
                {
                    packet.Write(packetID);
                    packet.Write(data.Length);
                    packet.Write(data);
                    SendUDPData(packet);
                }
            }
        }

        public static void UDP_Send(int packetID, byte data)
        {
            using (Packet packet = new Packet((int)Packet.ID.BYTE))
            {
                packet.Write(packetID);
                packet.Write(data);
                SendUDPData(packet);
            }
        }

        public static void UDP_Send(int packetID, int data)
        {
            using (Packet packet = new Packet((int)Packet.ID.INTEGER))
            {
                packet.Write(packetID);
                packet.Write(data);
                SendUDPData(packet);
            }
        }

        public static void UDP_Send(int packetID, bool data)
        {
            using (Packet packet = new Packet((int)Packet.ID.BOOLEAN))
            {
                packet.Write(packetID);
                packet.Write(data);
                SendUDPData(packet);
            }
        }

        public static void UDP_Send(int packetID, float data)
        {
            using (Packet packet = new Packet((int)Packet.ID.FLOAT))
            {
                packet.Write(packetID);
                packet.Write(data);
                SendUDPData(packet);
            }
        }

        public static void UDP_Send(int packetID, short data)
        {
            using (Packet packet = new Packet((int)Packet.ID.SHORT))
            {
                packet.Write(packetID);
                packet.Write(data);
                SendUDPData(packet);
            }
        }

        public static void UDP_Send(int packetID, long data)
        {
            using (Packet packet = new Packet((int)Packet.ID.LONG))
            {
                packet.Write(packetID);
                packet.Write(data);
                SendUDPData(packet);
            }
        }

        public static void UDP_Send(int packetID, Vector3 data)
        {
            using (Packet packet = new Packet((int)Packet.ID.VECTOR3))
            {
                packet.Write(packetID);
                packet.Write(new System.Numerics.Vector3(data.x, data.y, data.z));
                SendUDPData(packet);
            }
        }

        public static void UDP_Send(int packetID, Quaternion data)
        {
            using (Packet packet = new Packet((int)Packet.ID.QUATERNION))
            {
                packet.Write(packetID);
                packet.Write(new System.Numerics.Quaternion(data.x, data.y, data.z, data.w));
                SendUDPData(packet);
            }
        }
        #endregion

    }
}