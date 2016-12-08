using iPem.Core;
using System;

namespace iPem.Model {
    /// <summary>
    /// Represents a task
    /// </summary>
    public partial class TaskEntity {
        public string Id { get; set; }

        public string Name { get; set; }

        public PlanId Plan { get; set; }

        public DateTime Last { get; set; }

        public DateTime Next { get; set; }

        public int Order { get; set; }
    }
}
