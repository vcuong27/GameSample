using MySql.Data.MySqlClient;
using System;
using System.Data;


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

// CREATE TABLE `game`.`chatmessages` (
//      `ChatID` INT NOT NULL AUTO_INCREMENT ,
//      `PlayerID` INT NOT NULL ,
//      `Channel` CHAR(20) NOT NULL ,
//      `OtherPlayerID` INT NOT NULL ,
//      `Message` LONGTEXT NOT NULL ,
//      `SentTime` DATETIME NOT NULL ,
//      PRIMARY KEY (`ChatID`),
//      FOREIGN KEY (playerID) REFERENCES Account(playerID) ON DELETE CASCADE,
//      FOREIGN KEY (OtherPlayerID) REFERENCES Account(playerID) ON DELETE CASCADE
// );

namespace DevelopersHub.RealtimeNetworking.Server
{
    class Database
    {

        #region MySQL

        private static MySqlConnection _mysqlConnection;
        private const string _mysqlServer = "127.0.0.1";
        private const string _mysqlUsername = "root";
        private const string _mysqlPassword = "";
        private const string _mysqlDatabase = "Game";

        public static MySqlConnection mysqlConnection
        {
            get
            {
                if (_mysqlConnection == null || _mysqlConnection.State == ConnectionState.Closed)
                {
                    try
                    {
                        _mysqlConnection = new MySqlConnection("SERVER=" + _mysqlServer + "; DATABASE=" + _mysqlDatabase + "; UID=" + _mysqlUsername + "; PASSWORD=" + _mysqlPassword + ";");
                        _mysqlConnection.Open();
                        Console.WriteLine("Connection established with MySQL database.");
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Failed to connect the MySQL database.");
                    }
                }
                else if (_mysqlConnection.State == ConnectionState.Broken)
                {
                    try
                    {
                        _mysqlConnection.Close();
                        _mysqlConnection = new MySqlConnection("SERVER=" + _mysqlServer + "; DATABASE=" + _mysqlDatabase + "; UID=" + _mysqlUsername + "; PASSWORD=" + _mysqlPassword + ";");
                        _mysqlConnection.Open();
                        Console.WriteLine("Connection re-established with MySQL database.");
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Failed to connect the MySQL database.");
                    }
                }
                return _mysqlConnection;
            }
        }

        public static SC_Auth GetLoginResult(string username, string password)
        {
            SC_Auth authResult = new SC_Auth();
            string query = String.Format("SELECT playerID FROM Account WHERE username = '{0}' AND password = '{1}';", username, password);
            using (MySqlCommand command = new MySqlCommand(query, mysqlConnection))
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            authResult.loginResult = MessageStatus.SUCCESS;
                            authResult.playerID = int.Parse(reader["playerID"].ToString());
                        }
                    }
                    else
                    {
                        authResult.loginResult = MessageStatus.ERROR;
                    }
                }
            }
            return authResult;
        }

        public static SC_PlayerProfile GetPlayerProfile(int playerID)
        {
            SC_PlayerProfile profileResult = new SC_PlayerProfile();
            string query = String.Format("SELECT playerName, profileVersion, jsonData FROM PlayerProfile WHERE playerID = {0};", playerID);
            using (MySqlCommand command = new MySqlCommand(query, mysqlConnection))
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            profileResult.getProfileResult = MessageStatus.SUCCESS;
                            profileResult.playerName = reader["playerName"].ToString();
                            profileResult.profileVersion = int.Parse(reader["profileVersion"].ToString());
                            profileResult.jsonData = reader["jsonData"].ToString();
                        }
                    }
                    else
                    {
                        profileResult.getProfileResult = MessageStatus.ERROR;
                    }
                }
            }
            return profileResult;
        }

        public static void CreatePlayerProfile(int playerID, string playerName, int profileVersion, string jsonData)
        {
            string query = String.Format("INSERT INTO PlayerProfile (playerID, playerName, profileVersion, jsonData) VALUES ('{0}', '{1}', {2}, '{3}');", playerID, playerName, profileVersion, jsonData);
            using (MySqlCommand command = new MySqlCommand(query, mysqlConnection))
            {
                command.ExecuteNonQuery();
            }
        }

        public static void UpdatePlayerProfile(int playerID, string playerName, int profileVersion, string jsonData)
        {
            string query = String.Format("UPDATE PlayerProfile SET playerName = '{0}', profileVersion = {1}, jsonData = '{2}' WHERE playerID = '{3}';", playerName, profileVersion, jsonData, playerID);
            using (MySqlCommand command = new MySqlCommand(query, mysqlConnection))
            {
                command.ExecuteNonQuery();
            }
        }

        public static SC_ClanCreate CreateClan(string clanName, int playerID, string jsonData)
        {
            string query = String.Format("INSERT INTO Clan (name, owner, jsonData) VALUES ('{0}', {1}, '{2}');", clanName, playerID, jsonData);
            using (MySqlCommand command = new MySqlCommand(query, mysqlConnection))
            {
                command.ExecuteNonQuery();
            }

            int clanID = 0;
            string getClanIDQuery = String.Format("SELECT clanID FROM Clan WHERE name = '{0}' AND owner = {1};", clanName, playerID);
            using (MySqlCommand command = new MySqlCommand(getClanIDQuery, mysqlConnection))
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        clanID = int.Parse(reader["clanID"].ToString());
                    }
                }
            }

            return new SC_ClanCreate
            {
                createResult = MessageStatus.SUCCESS,
                clanID = clanID
            };
        }

        public static SC_ClanInfo GetClanInfo(int clanID)
        {
            SC_ClanInfo clanInfoResult = new SC_ClanInfo();
            string query = String.Format("SELECT name, owner, jsonData, score, memberCount FROM Clan WHERE clanID = {0};", clanID);
            using (MySqlCommand command = new MySqlCommand(query, mysqlConnection))
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            clanInfoResult.getInfoResult = MessageStatus.SUCCESS;
                            clanInfoResult.clanID = clanID;
                            clanInfoResult.ownerID = int.Parse(reader["owner"].ToString());
                            clanInfoResult.jsonData = reader["jsonData"].ToString();
                            clanInfoResult.score = int.Parse(reader["score"].ToString());
                            clanInfoResult.memberCount = int.Parse(reader["memberCount"].ToString());
                            clanInfoResult.name = reader["name"].ToString();
                        }
                    }
                    else
                    {
                        clanInfoResult.getInfoResult = MessageStatus.ERROR;
                    }
                }
            }

            if (clanInfoResult.getInfoResult == MessageStatus.SUCCESS)
            {
                string getPlayerNameQuery = String.Format("SELECT playerName FROM PlayerProfile WHERE playerID = {0};", clanInfoResult.ownerID);
                using (MySqlCommand command = new MySqlCommand(getPlayerNameQuery, mysqlConnection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            clanInfoResult.playerOwner = reader["playerName"].ToString();
                        }
                    }
                }
            }
            else
            {
                clanInfoResult.playerOwner = "Unknown";
            }

            return clanInfoResult;
        }

        public static SC_ClanList GetClanList(int pageIndex, int pageSize)
        {
            SC_ClanList clanListResult = new SC_ClanList();
            clanListResult.getListResult = MessageStatus.SUCCESS;
            clanListResult.totalClans = 0;
            clanListResult.clans = new ClanListInfo[0];

            string query = String.Format("SELECT clanID, name, memberCount FROM Clan ORDER BY score DESC LIMIT {0}, {1};", pageIndex * pageSize, pageSize);
            using (MySqlCommand command = new MySqlCommand(query, mysqlConnection))
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            ClanListInfo clanInfo = new ClanListInfo();
                            clanInfo.clanID = int.Parse(reader["clanID"].ToString());
                            clanInfo.name = reader["name"].ToString();
                            clanInfo.memberCount = int.Parse(reader["memberCount"].ToString());
                            Array.Resize(ref clanListResult.clans, clanListResult.clans.Length + 1);
                            clanListResult.clans[clanListResult.clans.Length - 1] = clanInfo;
                        }
                    }
                }
            }

            return clanListResult;
        }

        public static SC_ClanUpdate UpdateClan(int clanID, string name, string jsonData)
        {
            int rs = 0;
            string query = String.Format("UPDATE Clan SET name = '{0}', jsonData = '{1}' WHERE clanID = {2};", name, jsonData, clanID);
            using (MySqlCommand command = new MySqlCommand(query, mysqlConnection))
            {
                rs = command.ExecuteNonQuery();
            }
            if (rs == 0)
            {
                return new SC_ClanUpdate
                {
                    updateResult = MessageStatus.ERROR
                };
            }
            else
            {
                return new SC_ClanUpdate
                {
                    updateResult = MessageStatus.SUCCESS
                };
            }

        }

        public static SC_ClanKick KickPlayerFromClan(int clanID, int playerID)
        {
            int rs = 0;
            string query = String.Format("DELETE FROM ClanMember WHERE clanID = {0} AND playerID = {1};", clanID, playerID);
            using (MySqlCommand command = new MySqlCommand(query, mysqlConnection))
            {
                rs = command.ExecuteNonQuery();
            }
            if (rs == 0)
            {
                return new SC_ClanKick
                {
                    kickResult = MessageStatus.ERROR
                };
            }
            else
            {
                return new SC_ClanKick
                {
                    kickResult = MessageStatus.SUCCESS
                };
            }

        }

        public static SC_ClanAccept AcceptPlayerIntoClan(int clanID, int playerID)
        {
            int rs = 0;
            string query = String.Format("INSERT INTO ClanMember (clanID, playerID) VALUES ({0}, {1});", clanID, playerID);
            using (MySqlCommand command = new MySqlCommand(query, mysqlConnection))
            {
                rs = command.ExecuteNonQuery();
            }
            if (rs != 0)
            {
                string updateMemberCountQuery = String.Format("UPDATE Clan SET memberCount = memberCount + 1 WHERE clanID = {0};", clanID);
                using (MySqlCommand command = new MySqlCommand(updateMemberCountQuery, mysqlConnection))
                {
                    command.ExecuteNonQuery();
                }
                return new SC_ClanAccept
                {
                    acceptResult = MessageStatus.SUCCESS
                };
            }
            else
            {
                return new SC_ClanAccept
                {
                    acceptResult = MessageStatus.ERROR
                };
            }
        }

        public static SC_ClanJoin RequestToJoinClan(int clanID, int playerID)
        {
            int rs = 0;
            string query = String.Format("INSERT INTO ClanJoinRequest (clanID, playerID) VALUES ({0}, {1});", clanID, playerID);
            using (MySqlCommand command = new MySqlCommand(query, mysqlConnection))
            {
                rs = command.ExecuteNonQuery();
            }
            if (rs != 0)
            {
                return new SC_ClanJoin
                {
                    joinResult = MessageStatus.SUCCESS
                };
            }
            else
            {
                return new SC_ClanJoin
                {
                    joinResult = MessageStatus.ERROR
                };
            }
        }

        public static SC_ClanJoin JoinClan(int clanID, int playerID)
        {
            int rs = 0;
            string query = String.Format("INSERT INTO ClanMember (clanID, playerID) VALUES ({0}, {1});", clanID, playerID);
            using (MySqlCommand command = new MySqlCommand(query, mysqlConnection))
            {
                rs = command.ExecuteNonQuery();
            }
            if (rs != 0)
            {
                string updateMemberCountQuery = String.Format("UPDATE Clan SET memberCount = memberCount + 1 WHERE clanID = {0};", clanID);
                using (MySqlCommand command = new MySqlCommand(updateMemberCountQuery, mysqlConnection))
                {
                    command.ExecuteNonQuery();
                }
                return new SC_ClanJoin
                {
                    joinResult = MessageStatus.SUCCESS
                };
            }
            else
            {
                return new SC_ClanJoin
                {
                    joinResult = MessageStatus.ERROR
                };
            }
        }

        public static SC_ClanLeave LeaveClan(int clanID, int playerID)
        {
            int rs = 0;
            string query = String.Format("DELETE FROM ClanMember WHERE clanID = {0} AND playerID = {1};", clanID, playerID);
            using (MySqlCommand command = new MySqlCommand(query, mysqlConnection))
            {
                rs = command.ExecuteNonQuery();
            }
            if (rs != 0)
            {
                string updateMemberCountQuery = String.Format("UPDATE Clan SET memberCount = memberCount - 1 WHERE clanID = {0};", clanID);
                using (MySqlCommand command = new MySqlCommand(updateMemberCountQuery, mysqlConnection))
                {
                    command.ExecuteNonQuery();
                }
                return new SC_ClanLeave
                {
                    leaveResult = MessageStatus.SUCCESS
                };
            }
            else
            {
                return new SC_ClanLeave
                {
                    leaveResult = MessageStatus.ERROR
                };
            }
        }

        public static SC_ClanWarStart StartClanWar(int attackClanID, int defendClanID)
        {
            string query = String.Format("INSERT INTO ClanWar (attackClanID, defendClanID, startTime, endTime) VALUES ({0}, {1}, NOW(), NOW() + INTERVAL 30 DAY);", attackClanID, defendClanID);
            int rs = 0;
            using (MySqlCommand command = new MySqlCommand(query, mysqlConnection))
            {
                rs = command.ExecuteNonQuery();
            }
            if (rs != 0)
            {
                int warID = 0;
                string getWarIDQuery = String.Format("SELECT id from ClanWar WHERE attackClanID = {0} AND defendClanID = {1};", attackClanID, defendClanID);
                using (MySqlCommand command = new MySqlCommand(getWarIDQuery, mysqlConnection))
                {
                    warID = Convert.ToInt32(command.ExecuteScalar());
                }
                return new SC_ClanWarStart
                {
                    startResult = MessageStatus.SUCCESS,
                    warID = warID
                };
            }
            else
            {
                return new SC_ClanWarStart
                {
                    startResult = MessageStatus.ERROR
                };
            }
        }

        public static SC_ClanWarInfo GetClanWarInfo(int warID)
        {
            SC_ClanWarInfo info = new SC_ClanWarInfo();
            info.getInfoResult = MessageStatus.ERROR;
            string query = String.Format("SELECT attackClanID, defendClanID, startTime, endTime FROM ClanWar WHERE id = {0};", warID);
            using (MySqlCommand command = new MySqlCommand(query, mysqlConnection))
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        info.warID = warID;
                        info.attackClanID = reader.GetInt32("attackClanID");
                        info.defendClanID = reader.GetInt32("defendClanID");
                        info.startTime = reader.GetDateTime("startTime");
                        info.endTime = reader.GetDateTime("endTime");
                        info.getInfoResult = MessageStatus.SUCCESS;
                    }
                }
            }
            return info;
        }

        public static SC_ChatHistories GetChatHistories(int playerID, int clanID)
        {
            SC_ChatHistories histories = new SC_ChatHistories();
            histories.getHistoriesResult = MessageStatus.ERROR;
            string query = String.Format("SELECT ChatID, PlayerID, Channel, OtherPlayerID, Message, SentTime FROM ChatHistory WHERE clanID = {0} ORDER BY timestamp DESC LIMIT 20;", clanID);
            using (MySqlCommand command = new MySqlCommand(query, mysqlConnection))
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ChatMessageItem message = new ChatMessageItem()
                        {
                            chatID = reader.GetInt32("ChatID"),
                            playerID = reader.GetInt32("PlayerID"),
                            channel = reader.GetString("Channel"),
                            otherPlayerID = reader.GetInt32("OtherPlayerID"),
                            message = reader.GetString("Message"),
                            sentTime = reader.GetDateTime("SentTime")
                        };
                        histories.messages.Add(message);
                    }
                    histories.getHistoriesResult = MessageStatus.SUCCESS;
                }
            }
            return histories;


        }

        public static void SendChatMessage(CS_ChatMessage chatMessageMessage)
        {
            int rs = 0;
            string query = String.Format("INSERT INTO ChatMessages (PlayerID, Channel, OtherPlayerID, Message, SentTime, clanID) VALUES ({0}, '{1}', {2}, '{3}', NOW(), {4});", chatMessageMessage.playerID, chatMessageMessage.channel, chatMessageMessage.otherPlayerID, chatMessageMessage.message, chatMessageMessage.clanID);
            using (MySqlCommand command = new MySqlCommand(query, mysqlConnection))
            {
                rs = command.ExecuteNonQuery();
            }

        }



        #endregion

        #region SQL
        /*
        private static SqlConnection _sqlConnection;
        private const string _sqlServer = "server";
        private const string _sqlDatabase = "database";

        public static SqlConnection sqlConnection
        {
            get
            {
                if (_sqlConnection == null || _sqlConnection.State == ConnectionState.Closed)
                {
                    try
                    {
                        var connectionString = @"Server=localhost\" + _sqlServer + ";Database=" + _sqlDatabase + ";Initial Catalog=" + _sqlDatabase + ";Trusted_Connection=True;MultipleActiveResultSets=true";
                        _sqlConnection = new SqlConnection(connectionString);
                        _sqlConnection.Open();
                        Console.WriteLine("Connection established with SQL database.");
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Failed to connect the SQL database.");
                    }
                }
                else if (_sqlConnection.State == ConnectionState.Broken)
                {
                    try
                    {
                        _sqlConnection.Close();
                        var connectionString = @"Server=localhost\" + _sqlServer + ";Database=" + _sqlDatabase + ";Initial Catalog=" + _sqlDatabase + ";Trusted_Connection=True;MultipleActiveResultSets=true";
                        _sqlConnection = new SqlConnection(connectionString);
                        _sqlConnection.Open();
                        Console.WriteLine("Connection re-established with SQL database.");
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Failed to connect the SQL database.");
                    }
                }
                return _sqlConnection;
            }
        }

        public static void Demo_SQL_1()
        {
            string query = String.Format("UPDATE database.table SET int_column = {0}, string_column = '{1}', datetime_column = GETUTCDATE();", 123, "Hello World");
            using (SqlCommand command = new SqlCommand(query, sqlConnection))
            {
                command.ExecuteNonQuery();
            }
        }

        public static void Demo_SQL_2()
        {
            string query = String.Format("SELECT column1, column2 FROM database.table WHERE column3 = {0} ORDER BY column1 DESC;", 123);
            using (SqlCommand command = new SqlCommand(query, sqlConnection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            int column1 = int.Parse(reader["column1"].ToString());
                            string column2 = reader["column2"].ToString();
                        }
                    }
                }
            }
        }
        */
        #endregion

    }
}