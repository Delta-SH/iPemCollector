using iPem.Core;
using System;

namespace iPem.Model {
    /// <summary>
    /// Represents a database
    /// </summary>
    public partial class DbEntity {
        /// <summary>
        /// Database Id
        /// </summary>
        public DatabaseId Id { get; set; }

        /// <summary>
        /// Database Type
        /// </summary>
        public DatabaseType Type { get; set; }

        /// <summary>
        /// Database IP
        /// </summary>
        public String IP { get; set; }

        /// <summary>
        /// Database Port
        /// </summary>
        public Int32 Port { get; set; }

        /// <summary>
        /// Database Uid
        /// </summary>
        public String Uid { get; set; }

        /// <summary>
        /// Database Password
        /// </summary>
        public String Password { get; set; }

        /// <summary>
        /// Database Name
        /// </summary>
        public String Name { get; set; }
    }
}
