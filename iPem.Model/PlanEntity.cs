using iPem.Core;
using System;

namespace iPem.Model {
    /// <summary>
    /// Represents a plan
    /// </summary>
    public partial class PlanEntity {
        public PlanId Id { get; set; }

        public string Name { get; set; }

        public long Interval { get; set; }

        public int Unit { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }
    }
}
