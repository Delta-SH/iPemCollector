using System;

namespace iPem.Configurator {
    public partial class TaskEntity {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Json { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public DateTime Next { get; set; }

        public int Index { get; set; }
    }
}
