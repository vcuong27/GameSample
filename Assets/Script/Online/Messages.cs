using System;
using System.Collections.Generic;


//--1.Bảng Tài khoản người chơi
//CREATE TABLE Account (
//    playerID INT NOT NULL,
//    userName VARCHAR(255) NOT NULL,
//    password VARCHAR(255) NOT NULL,
//    PRIMARY KEY (playerID)
//);

//--2.Bảng Hồ sơ hiển thị của người chơi
//CREATE TABLE PlayerProfile (
//    playerID INT NOT NULL,
//    playerName VARCHAR(100) NOT NULL,
//    profileVersion INT DEFAULT 1,
//    jsonData JSON,
//    PRIMARY KEY (playerID),
//    FOREIGN KEY (playerID) REFERENCES Account(playerID) ON DELETE CASCADE
//);

//--3.Bảng Thông tin Clan
//CREATE TABLE Clan (
//    clanID INT NOT NULL,
//    name VARCHAR(100) NOT NULL,
//    owner VARCHAR(100) NOT NULL,
//    Score INT DEFAULT 0,
//    memberCount INT DEFAULT 1,
//    jsonData JSON,
//    PRIMARY KEY (clanID)
//);

//--4.Bảng Thành viên trong Clan (Dữ liệu cá nhân đối với Clan)
//CREATE TABLE ClanData (
//    clanID INT NOT NULL,
//    playerID INT NOT NULL,
//    jsonData JSON, -- Chứa: điểm cống hiến, số trận clan war, vị trí...
//    PRIMARY KEY (clanID, playerID), -- Khóa chính kết hợp để một người không thể ở 2 vị trí trong 1 clan
//    FOREIGN KEY (clanID) REFERENCES Clan(clanID) ON DELETE CASCADE,
//    FOREIGN KEY (playerID) REFERENCES Account(playerID) ON DELETE CASCADE
//);

//--5.Bảng Xếp hạng người chơi nội bộ Clan
//CREATE TABLE Leaderboard (
//    clanID INT NOT NULL,
//    playerID INT NOT NULL,
//    Score INT DEFAULT 0, -- Điểm số dùng để xếp hạng cá nhân trong Clan
//    PRIMARY KEY (clanID, playerID),
//    FOREIGN KEY (clanID) REFERENCES Clan(clanID) ON DELETE CASCADE,
//    FOREIGN KEY (playerID) REFERENCES Account(playerID) ON DELETE CASCADE
//);

//--6.Bảng Quản lý các trận đấu Clan War
//CREATE TABLE ClanWar (
//    id INT AUTO_INCREMENT NOT NULL, -- ID tự động tăng cho từng trận đấu
//    attackClanID INT NOT NULL,
//    defendClanID INT NOT NULL,
//    StartTime DATETIME NOT NULL,
//    EndTime DATETIME NOT NULL,
//    jsonData JSON, -- Chứa: detail data, kết quả các trận đánh nhỏ, phần thưởng...
//    PRIMARY KEY (id),
//    FOREIGN KEY (attackClanID) REFERENCES Clan(clanID) ON DELETE CASCADE,
//    FOREIGN KEY (defendClanID) REFERENCES Clan(clanID) ON DELETE CASCADE
//);

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
    CLAN_LIST,

    CLAN_WAR_START = 30,
    CLAN_WAR_START_BATTLE,
    CLAN_WAR_END_BATTLE,
    CLAN_WAR_INFO,

    CHAT_HISTORIES = 60,
    CHAT_MESSAGE,
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
    public int playerID;
}

[Serializable]
public class CS_PlayerProfileGet : IBaseMessage
{
    public int playerID;
}

[Serializable]
public class CS_PlayerProfile : IBaseMessage
{
    public int playerID;
    public string playerName;
    public int profileVersion;
    public string jsonData;
}

[Serializable]
public class SC_PlayerProfile : IBaseMessage
{
    public MessageStatus getProfileResult;
    public int playerID;
    public string playerName;
    public int profileVersion;
    public string jsonData;
}

[Serializable]
public class CS_ClanCreate : IBaseMessage
{
    public string name;
    public int playerID;
    public int Score;
    public string jsonData;
}

[Serializable]
public class SC_ClanCreate : IBaseMessage
{
    public MessageStatus createResult;
    public int clanID;
}

[Serializable]
public class CS_ClanInfo : IBaseMessage
{
    public int clanID;
}

[Serializable]
public class SC_ClanInfo : IBaseMessage
{
    public MessageStatus getInfoResult;
    public int clanID;
    public int ownerID;
    public string playerOwner;
    public string name;
    public int score;
    public int memberCount;
    public string jsonData;
}

[Serializable]
public class CS_ClanList : IBaseMessage
{
    public int pageIndex;
    public int pageSize;
}

[Serializable]
public class ClanListInfo
{
    public int clanID;
    public string name;
    public int memberCount;
}

[Serializable]
public class ClanInfo
{
    public int memberCount;
    public string message;
    public string flagName;
    public int[] listPlayerRequestID; // danh sách người chơi đang chờ phê duyệt vào clan (chỉ có leader mới có)
}

[Serializable]
public class SC_ClanList : IBaseMessage
{
    public MessageStatus getListResult;
    public int totalClans;
    public ClanListInfo[] clans;
}

[Serializable]
public class CS_ClanUpdate : IBaseMessage
{
    public int clanID;
    public string name;
    public int Score;
    public string jsonData;
}

[Serializable]
public class SC_ClanUpdate : IBaseMessage
{
    public MessageStatus updateResult;
    public int clanID;
}

[Serializable]
public class CS_ClanKick : IBaseMessage
{
    public int clanID;
    public int playerID;
}

[Serializable]
public class SC_ClanKick : IBaseMessage
{
    public MessageStatus kickResult;
    public int clanID;
    public int playerID;
}

[Serializable]
public class CS_ClanAccept : IBaseMessage
{
    public int clanID;
    public int playerID;
}

[Serializable]
public class SC_ClanAccept : IBaseMessage
{
    public MessageStatus acceptResult;
    public int clanID;
    public int playerID;
}

[Serializable]
public class CS_ClanRequest : IBaseMessage
{
    public int clanID;
    public int playerID;
}

[Serializable]
public class SC_ClanRequest : IBaseMessage
{
    public MessageStatus requestResult;
    public int clanID;
    public int playerID;
}

[Serializable]
public class CS_ClanJoin : IBaseMessage
{
    public int clanID;
    public int playerID;
}

[Serializable]
public class SC_ClanJoin : IBaseMessage
{
    public MessageStatus joinResult;
    public int clanID;
    public int playerID;
}

[Serializable]
public class CS_ClanLeave : IBaseMessage
{
    public int clanID;
    public int playerID;
}

[Serializable]
public class SC_ClanLeave : IBaseMessage
{
    public MessageStatus leaveResult;
    public int clanID;
    public int playerID;
}

[Serializable]
public class CS_ClanWarStart : IBaseMessage
{
    public int attackClanID;
    public int defendClanID;
}

[Serializable]
public class SC_ClanWarStart : IBaseMessage
{
    public MessageStatus startResult;
    public int warID;
}


[Serializable]
public class BattleInfo
{
    public int battleID;
    public int attackerID;
    public int defenderID;
    public int attackerScore;
    public int defenderScore;
    public string jsonData;
}

[Serializable]
public class ClanWarInfo
{
    public int warID;
    public int attackClanID;
    public int defendClanID;
    public DateTime startTime;
    public DateTime endTime;
    public string jsonData;
    public BattleInfo[] battles;
}

[Serializable]
public class CS_ClanWarInfo : IBaseMessage
{
    public int warID;
}

[Serializable]
public class SC_ClanWarInfo : IBaseMessage
{
    public MessageStatus getInfoResult;
    public int warID;
    public int attackClanID;
    public int defendClanID;
    public DateTime startTime;
    public DateTime endTime;
    public string jsonData;
    public BattleInfo[] battles;
}


[Serializable]
public class ChatMessageItem : IBaseMessage
{
    public int chatID;
    public int playerID;
    public int otherPlayerID;
    public string channel;
    public string message;
    public DateTime sentTime;
}

[Serializable]
public class CS_ChatHistories : IBaseMessage
{
    public int playerID;
    public int clanID;
}

[Serializable]
public class SC_ChatHistories : IBaseMessage
{
    public MessageStatus getHistoriesResult;
    public List<ChatMessageItem> messages;
}

[Serializable]
public class CS_ChatMessage : IBaseMessage
{
    public int playerID;
    public int otherPlayerID;
    public int clanID;
    public string channel;
    public string message;
    public DateTime sentTime;
}

[Serializable]
public class SC_ChatMessage : IBaseMessage
{
    public ChatMessageItem messages;
}
