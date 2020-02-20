using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using GTANetworkAPI;
using MySql.Data.MySqlClient;

using Furious_V;
using Furious_V.Admin;
using Furious_V.MySQL;

namespace Furious_V.Player
{
    /// <summary>
    /// The entire player related data and functionalities are present in this class.
    /// </summary>
    class Data : Script
    {
        /// <summary>
        /// The readonly string which defines the key for saving the object created from <see cref="Data"/> in a <see cref="Client"/>
        /// </summary>
        private static readonly String Identifier = "Custom_Player_Data";

        /// <summary>
        /// The primary key <see cref="Sqlid"/> which is stored in the database, unique for every player.
        /// </summary>
        private int Sqlid { get; set; }
        /// <summary>
        /// The name of the player as stored in <see cref="Client.Name"/>
        /// </summary>
        /// <remarks> A copy is stored in <see cref="Data"/> for accessibility and reducing complexities. </remarks>
        public string Name { get; set; }
        /// <summary>
        /// The type of admin. The type is as seen in <see cref="Admin.Data.E_ADMIN_LEVEL"/>
        /// <list type="bullet">
        /// <item><description><see cref="Admin.Data.E_ADMIN_LEVEL.ADMIN_NONE"/></description></item>
        /// <item><description><see cref="Admin.Data.E_ADMIN_LEVEL.ADMIN_PROBIE"/></description></item>
        /// <item><description><see cref="Admin.Data.E_ADMIN_LEVEL.ADMIN_GENERAL"/></description></item>
        /// <item><description><see cref="Admin.Data.E_ADMIN_LEVEL.ADMIN_HEAD"/></description></item>
        /// <item><description><see cref="Admin.Data.E_ADMIN_LEVEL.ADMIN_EXECUTIVE"/></description></item>
        /// <item><description><see cref="Admin.Data.E_ADMIN_LEVEL.ADMIN_OWNER"/></description></item>
        /// </list>
        /// </summary>
        public Admin.Data.E_ADMIN_LEVEL Admin { get; set; }
        /// <summary>
        /// Contains the cash in hand of the player.
        /// </summary>
        public int Cash { get; set; }
        /// <summary>
        /// Determines whether the player is logged in or not.
        /// </summary>
        public Boolean LoggedIn { get; set; }
        /// <summary>
        /// The prison time of the player.
        /// </summary>
        public int PrisonTime { get; set; }
        /// <summary>
        /// Determines if the player is banned from the server or not.
        /// </summary>
        public Boolean Banned { get; set; }

        /// <summary>
        /// Assigns all the default values to the object.
        /// </summary>
        /// <param name="playername">The <see cref="Client.Name"/> of the player.</param>
        Data(string playername)
        {
            this.Sqlid = 0;
            this.Name = playername;
            this.Admin = Furious_V.Admin.Data.E_ADMIN_LEVEL.ADMIN_NONE;
            this.Cash = 0;
            this.LoggedIn = false;
            this.PrisonTime = 0;
            this.Banned = false;
        }
        Data() : this(null) { }

        /// <summary>
        /// This returns the <see cref="Data"/> type stored within the <see cref="Client"/> with the key <see cref="Identifier"/>.
        /// </summary>
        /// <param name="player">The <see cref="Client"/> passed which will return the <see cref="Data"/>.</param>
        /// <returns><see cref="Data"/></returns>
        public static Data GetPlayerData(Client player)
        {
            if (player.HasData(Identifier))
            {
                return player.GetData(Identifier);
            }
            else
            {
                Utils.Log($"{player.Name} does not have player data set and thus were kicked.", Utils.Log_Status.Log_Warning);
                player.SendChatMessage("Error: There was an error with the player data.");
                player.Kick();
                return null;
            }
        }

        /// <summary>
        /// Checks whether or not the player's data already exists in the database or not.
        /// </summary>
        /// <param name="playername">The <see cref="Client.Name"/> is used to check the existence of data in the database.</param>
        /// <returns><see cref="Boolean"/>. true is the data exists. false is it does not exists.</returns>
        private static async Task<Boolean> AccountExists(String playername)
        {
            String query = "SELECT * FROM `players` WHERE `Name`=@playerName LIMIT 1;";
            using (Database.DB_Connection = new MySqlConnection(Database.DB_ConnectionString))
            {
                Boolean found = false;

                await Database.DB_Connection.OpenAsync();

                using (MySqlCommand command = new MySqlCommand(query, Database.DB_Connection))
                {
                    command.Parameters.AddWithValue("@playerName", playername);
                    var reader = await command.ExecuteReaderAsync();
                    
                    using (reader)
                    {
                        if(await reader.ReadAsync())
                            found = true;
                    }
                }

                await Database.DB_Connection.CloseAsync();

                return found;
            }
        }
        
        /// <summary>
        /// Loads the player data from the database to the custom object <see cref="Data"/>
        /// </summary>
        /// <returns>void</returns>
        private async Task LoadPlayerData()
        {
            String query = $"SELECT * FROM `players` WHERE `Name`=@playername LIMIT 1;";
            using (Database.DB_Connection = new MySqlConnection(Database.DB_ConnectionString))
            {
                await Database.DB_Connection.OpenAsync();
                using (MySqlCommand command = new MySqlCommand(query, Database.DB_Connection))
                {
                    command.Parameters.AddWithValue("@playername", this.Name);
                    var reader = await command.ExecuteReaderAsync();

                    using (reader)
                    {
                        while (await reader.ReadAsync())
                        {
                            this.Sqlid = Convert.ToInt32( reader["SQLID"] );
                            switch (Convert.ToInt32( reader["Admin"] ))
                            {
                                case 1:
                                    {
                                        this.Admin = Furious_V.Admin.Data.E_ADMIN_LEVEL.ADMIN_PROBIE;
                                        break;
                                    }
                                case 2:
                                    {
                                        this.Admin = Furious_V.Admin.Data.E_ADMIN_LEVEL.ADMIN_GENERAL;
                                        break;
                                    }
                                case 3:
                                    {
                                        this.Admin = Furious_V.Admin.Data.E_ADMIN_LEVEL.ADMIN_HEAD;
                                        break;
                                    }
                                case 4:
                                    {
                                        this.Admin = Furious_V.Admin.Data.E_ADMIN_LEVEL.ADMIN_EXECUTIVE;
                                        break;
                                    }
                                case 5:
                                    {
                                        this.Admin = Furious_V.Admin.Data.E_ADMIN_LEVEL.ADMIN_OWNER;
                                        break;
                                    }
                                default:
                                    {
                                        this.Admin = Furious_V.Admin.Data.E_ADMIN_LEVEL.ADMIN_NONE;
                                        break;
                                    }
                            }
                            this.Cash = Convert.ToInt32(reader["Cash"]);
                            this.PrisonTime = Convert.ToInt32(reader["PrisonTime"]);
                            this.Banned = Convert.ToBoolean(reader["Banned"]);
                            this.LoggedIn = true;
                        }
                    }
                }
                await Database.DB_Connection.CloseAsync();
            }

            return;
        }
        /// <summary>
        /// Inserts default data into the database and assigns values to the <see cref="Data"/> for the <see cref="Client"/>.
        /// </summary>
        /// <returns>void</returns>
        private async Task RegisterPlayerData()
        {
            String query = $"INSERT INTO `players` (`Name`) VALUES (@playername);";
            using (Database.DB_Connection = new MySqlConnection(Database.DB_ConnectionString))
            {
                await Database.DB_Connection.OpenAsync();
                using (MySqlCommand command = new MySqlCommand(query, Database.DB_Connection))
                {
                    command.Parameters.AddWithValue("@playername", this.Name);
                    await command.ExecuteNonQueryAsync();
                    this.Sqlid = Convert.ToInt32(command.LastInsertedId);
                }
                await Database.DB_Connection.CloseAsync();
            }
            this.Admin = Furious_V.Admin.Data.E_ADMIN_LEVEL.ADMIN_NONE;
            this.Cash = 0;
            this.Banned = false;
            this.PrisonTime = 0;
            this.LoggedIn = true;
            return;
        }
        /// <summary>
        /// Updates the player data in the database with respect to the <see cref="Data.Name"/>.
        /// </summary>
        /// <returns>void</returns>
        public async Task SavePlayerData()
        {
            String query;
            query = $"UPDATE `players` SET Admin=@adminlevel, Cash=@cash, PrisonTime=@prisontime, Banned=@banned WHERE Name=@playername;";
            
            using (Database.DB_Connection = new MySqlConnection(Database.DB_ConnectionString))
            {
                await Database.DB_Connection.OpenAsync();

                using (MySqlCommand command = new MySqlCommand(query, Database.DB_Connection))
                {
                    command.Parameters.AddWithValue("@adminlevel", (int)this.Admin);
                    command.Parameters.AddWithValue("@cash", this.Cash);
                    command.Parameters.AddWithValue("@prisontime", this.PrisonTime);
                    command.Parameters.AddWithValue("@banned", this.Banned);
                    command.Parameters.AddWithValue("@playername", this.Name);

                    await command.ExecuteNonQueryAsync();
                }
                await Database.DB_Connection.CloseAsync();
            }
            return;
        }
        /// <summary>
        /// <para>Creates an instance of <see cref="Data"/> and then stores in inside <see cref="Client"/> with the key <see cref="Identifier"/>.</para>
        /// <para>It then either loads player data or registers new data depending if the account already exists or not.</para>
        /// </summary>
        /// <param name="player"><see cref="Client"/> which is passed in the function for the <see cref="OnPlayerConnect(Client)"/> callback.</param>
        [ServerEvent(Event.PlayerConnected)]
        private async static void OnPlayerConnect(Client player)
        {
            Utils.Log($"{player.Name} connected.", Utils.Log_Status.Log_Debug);
            Data temp = new Data(player.Name);
            player.SetData(Identifier, temp);

            if (await AccountExists(player.Name))
            {
                Utils.Log($"Loading player data for {player.Name}", Utils.Log_Status.Log_Debug);
                await temp.LoadPlayerData();
                Utils.Log($"Loaded player data for {player.Name}", Utils.Log_Status.Log_Success);
            }
            else
            {
                Utils.Log($"Registering player data for {player.Name}", Utils.Log_Status.Log_Debug);
                await temp.RegisterPlayerData();
                Utils.Log($"Registered player data for {player.Name}", Utils.Log_Status.Log_Success);
            }
            return;
        }
        /// <summary>
        /// The <see cref="Data"/> is updated when the player disconnects.
        /// </summary>
        /// <param name="player">The <see cref="Client"/> which is diconnecting.</param>
        /// <param name="type">The type of disconnect.</param>
        /// <param name="reason">The reason of disconnect.</param>
        [ServerEvent(Event.PlayerDisconnected)]
        private async static void OnPlayerDisconnect(Client player, DisconnectionType type, string reason)
        {
            try
            {
                await GetPlayerData(player).SavePlayerData();
                Utils.Log($"Saved {player.Name}'s data.", Utils.Log_Status.Log_Debug);
            }
            catch (Exception)
            {
                return;
            }
            return;
        }
    }
}
