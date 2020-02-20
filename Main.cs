using System;
using System.Threading.Tasks;
using GTANetworkAPI;

namespace Furious_V
{
    public class Main : Script
    {
        /// <summary>
        /// Contains the current version of the gamemode saved inside the database.
        /// </summary>
        public static String ServerVersion { get; set; } = "0.0.1a";

        /// <summary>
        /// This callback simply passes the credentials for the database for connection.
        /// </summary>
        /// <returns>Void</returns>
        [ServerEvent(Event.ResourceStart)]
        public static async Task OnResourceStart()
        {
            await MySQL.Database.InitConnection("localhost", "root", "", "furious_v");
            return;
        }
    }
}
