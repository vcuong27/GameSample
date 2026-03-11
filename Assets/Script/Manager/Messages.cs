using System;
using System.Collections.Generic;
using System.Text;

public enum MessageStatus
{
    NONE,
    SUCCESS,
    ERROR,
}

namespace Assets.Script.Manager
{
    [Serializable]
    public class IBaseMessage
    {
        
    }

    [Serializable]
    public class CS_AutenticationMessage : IBaseMessage
    {
        public string username;
        public string password;
    }

    [Serializable]
    public class CS_PlayerProfileMessage : IBaseMessage
    {
        public long playerID;
    }

    [Serializable]
    public class CS_CollectFarmMessage : IBaseMessage
    {
        public long playerID;
        public long buildingID;
    }

    [Serializable]
    public class SC_CollectFarmMessage : IBaseMessage
    {
        public MessageStatus status;
        public string message;
        public FarmData farmData;
    }

}
