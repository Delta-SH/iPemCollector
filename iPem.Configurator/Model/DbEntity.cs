using System;

namespace iPem.Configurator {
    public partial class DbEntity {
        public string Id { get; set; }

        public string Name { get; set; }

        public DatabaseType Type { get; set; }

        public string IP { get; set; }

        public int Port { get; set; }

        public string Uid { get; set; }

        public string Password { get; set; }

        public string Db { get; set; }
    }
}
