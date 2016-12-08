using iPem.Model;
using System;
using System.Collections.Generic;

namespace iPem.Task {
    public partial class ActTask03 : IActTask {
        public string Id {
            get { return "actTask03"; }
        }

        public string Name {
            get { return "实时任务1"; }
        }

        public long Seconds { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public DateTime Last { get; set; }

        public DateTime Next { get; set; }

        public List<Event> Events { get; set; }

        public int Order {
            get { return 3; }
        }

        public void Execute() {
            
        }
    }
}
