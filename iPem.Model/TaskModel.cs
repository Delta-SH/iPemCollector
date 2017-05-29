using iPem.Core;
using System;

namespace iPem.Model {
    public partial class TaskModel {
        public PlanType Type { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int Interval { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }
    }
}
