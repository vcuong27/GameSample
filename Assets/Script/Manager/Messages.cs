using System;
using System.Collections.Generic;
using System.Text;


public enum MessageID
{
    AUTH = 1,
    PROFILE_GET = 2,
    PROFILE_UPDATE = 3,
}


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
    public class CS_Auth : IBaseMessage
    {
        public string username;
        public string password;
    }

    [Serializable]
    public class SC_Auth : IBaseMessage
    {
        public MessageStatus loginResult;
        public string playerID;
    }


    [Serializable]
    public class CS_PlayerProfileGet : IBaseMessage
    {
        public string playerID;
    }

    [Serializable]
    public class CS_PlayerProfileUpdate : IBaseMessage
    {
        public string playerID;
        public string playerName;
        public int profileVersion;
        public string jsonData;
    }

    [Serializable]
    public class SC_PlayerProfile : IBaseMessage
    {
        public MessageStatus getProfileResult;
        public string playerID;
        public string playerName;
        public int profileVersion;
        public string jsonData;
    }



}
