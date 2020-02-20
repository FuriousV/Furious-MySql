using System;
using System.Data.Common;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using GTANetworkAPI;

namespace Furious_V.MySQL
{
    /// <summary>
    /// The connection and primary tables are made from this class.
    /// </summary>
    class Database : Script
    {
        /// <summary>
        /// The connection with which queries are to be executed is stored in this.
        /// </summary>
        public static MySqlConnection DB_Connection { get; set; } = null;

        /// <summary>
        /// The host of the database.
        /// </summary>
        private static String DB_Host { get; set; } = "localhost";
        /// <summary>
        /// The user of the database.
        /// </summary>
        private static String DB_User { get; set; } = "root";
        /// <summary>
        /// The password of the database.
        /// </summary>
        private static String DB_Password { get; set; } = "";
        /// <summary>
        /// The database which needs to be targeted.
        /// </summary>
        private static String DB_Database { get; set; } = "Furious_V";

        /// <summary>
        /// The connection string which is used to set up the connection every time a query is to be executed.
        /// </summary>
        public static String DB_ConnectionString { get; set; } = $"SERVER={DB_Host};UID{DB_User};PASSWORD={DB_Password};DATABASE={DB_Database};";

        /// <summary>
        /// States whether the connection is established or not by checking the connection once with the <see cref="DB_ConnectionString"/>
        /// </summary>
        public static Boolean ConnectionEstablished { get; set; } = false;

        /// <summary>
        /// Creates the default tables for the database and checks if the <see cref="DB_ConnectionString"/> is correct or not.
        /// </summary>
        /// <param name="host">The host name of the database.</param>
        /// <param name="user">The user of the database.</param>
        /// <param name="password">The password of the database.</param>
        /// <param name="database">The database itself which needs to be connected with.</param>
        /// <returns></returns>
        public static async Task<Boolean> InitConnection(String host, String user, String password, String database)
        {
            DB_Host = host;
            DB_User = user;
            DB_Password = password;
            DB_Database = database;

            DB_ConnectionString = $"SERVER={host};UID={user};PASSWORD={password};DATABASE={database};";
            
            using (DB_Connection = new MySqlConnection(DB_ConnectionString))
            {
                await DB_Connection.OpenAsync();
                
                String query =  $"CREATE TABLE IF NOT EXISTS `accounts` " +
                                $"(SQLID INT(11) NOT NULL AUTO_INCREMENT," +
                                $"Name VARCHAR(32) NOT NULL DEFAULT \"\"," +
                                $"Password VARCHAR(128) NOT NULL DEFAULT \"\"," +
                                $"Banned BOOLEAN NOT NULL DEFAULT FALSE," +
                                $"PRIMARY KEY(SQLID));";

                MySqlCommand command = new MySqlCommand(query, DB_Connection);
                
                using (command)
                    await command.ExecuteNonQueryAsync();
                command.CommandText =   $"CREATE TABLE IF NOT EXISTS `players` " +
                                        $"(SQLID INT(11) NOT NULL AUTO_INCREMENT," +
                                        $"Name VARCHAR(32) NOT NULL DEFAULT \"\"," +
                                        $"Password VARCHAR(32) NOT NULL DEFAULT \"\"," +
                                        $"PrisonTime INT(11) NOT NULL DEFAULT 0," +
                                        $"Banned BOOLEAN NOT NULL DEFAULT FALSE," +
                                        $"Cash INT(11) NOT NULL DEFAULT 0," +
                                        $"Admin INT(3) NOT NULL DEFAULT 0," +
                                        $"PRIMARY KEY(SQLID), UNIQUE(Name));";

                using (command)
                    await command.ExecuteNonQueryAsync();
                
                ConnectionEstablished = true;
                Utils.Log("Connection established!", Utils.Log_Status.Log_Success);
                await DB_Connection.CloseAsync();
            }

            return ConnectionEstablished;
        }
    }
}
