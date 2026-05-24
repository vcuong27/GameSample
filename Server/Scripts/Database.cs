using MySql.Data.MySqlClient;
using System;
using System.Data;

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
                            authResult.playerID = reader["playerID"].ToString();
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

        public static SC_PlayerProfile GetPlayerProfile(string playerID)
        {
            SC_PlayerProfile profileResult = new SC_PlayerProfile();
            string query = String.Format("SELECT playerName, profileVersion, jsonData FROM PlayerProfile WHERE playerID = '{0}';", playerID);
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

        public static void CreatePlayerProfile(string playerID, string playerName, int profileVersion, string jsonData)
        {
            string query = String.Format("INSERT INTO PlayerProfile (playerID, playerName, profileVersion, jsonData) VALUES ('{0}', '{1}', {2}, '{3}');", playerID, playerName, profileVersion, jsonData);
            using (MySqlCommand command = new MySqlCommand(query, mysqlConnection))
            {
                command.ExecuteNonQuery();
            }
        }

        public static void UpdatePlayerProfile(string playerID, string playerName, int profileVersion, string jsonData)
        {
            string query = String.Format("UPDATE PlayerProfile SET playerName = '{0}', profileVersion = {1}, jsonData = '{2}' WHERE playerID = '{3}';", playerName, profileVersion, jsonData, playerID);
            using (MySqlCommand command = new MySqlCommand(query, mysqlConnection))
            {
                command.ExecuteNonQuery();
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