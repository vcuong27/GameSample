using System;


public enum MessageID
{
    AUTH = 1,

    PROFILE_GET = 10,
    PROFILE_UPDATE,
    PROFILE_CREATE,

    CLAN_CREATE = 20,
    CLAN_UPDATE,
    CLAN_KICK,
    CLAN_ACCEPT,
    CLAN_REQUEST,
    CLAN_JOIN,
    CLAN_LEAVE,
    CLAN_INFO,

    CLAN_WAR_START = 30,
    CLAN_WAR_START_BATTLE,
    CLAN_WAR_END_BATTLE,
    CLAN_WAR_INFO,
}

public enum MessageStatus
{
    NONE,
    SUCCESS,
    ERROR,
}

public enum ErrorType
{
    NO_ERROR,
    ERR_NOT_SUPPORTED,
    ERR_INVALID_DATA,
    ERR_NO_DATA,
}


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
public class CS_PlayerProfile : IBaseMessage
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

[Serializable]
public class CS_ClanCreate : IBaseMessage
{
    public string name;
    public string playerID;
    public int Score;
    public string jsonData;
}

[Serializable]
public class SC_ClanCreate : IBaseMessage
{
    public MessageStatus createResult;
    public string clanID;
}

[Serializable]
public class CS_ClanInfor : IBaseMessage
{
    public string clanID;
}

[Serializable]
public class SC_ClanInfo : IBaseMessage
{
    public MessageStatus getInfoResult;
    public string clanID;
    public string playerOwner;
    public string name;
    public int score;
    public string jsonData;
}