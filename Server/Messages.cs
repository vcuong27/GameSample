using System;
using System.Collections.Generic;
using System.Text;


public enum MessageID
{
    AUTH = 1,
    GET_PROFILE = 2,
}

public enum MessageStatus
{
    NONE,
    SUCCESS,
    ERROR,
}


namespace DevelopersHub.RealtimeNetworking.Server
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
    public class CS_PlayerProfileMessage : IBaseMessage
    {
        public long playerID;
    }

}
