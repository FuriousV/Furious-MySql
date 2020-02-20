using System;
using System.Collections.Generic;

namespace Furious_V.Admin
{
    /// <summary>
    /// Contains all relevant attributes and methods for admins.
    /// </summary>
    class Data
    {
        /// <summary>
        /// The different types of admin.
        /// </summary>
        public enum E_ADMIN_LEVEL
        {
            ADMIN_NONE,
            ADMIN_PROBIE,
            ADMIN_GENERAL,
            ADMIN_HEAD,
            ADMIN_EXECUTIVE,
            ADMIN_OWNER
        }
    }
}
