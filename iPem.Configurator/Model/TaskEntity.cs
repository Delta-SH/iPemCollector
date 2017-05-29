using System;

namespace iPem.Configurator {
    public partial class TaskEntity {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Json { get; set; }

        public DateTime Last { get; set; }

        public DateTime Next { get; set; }

        public int Index { get; set; }
    }
}
