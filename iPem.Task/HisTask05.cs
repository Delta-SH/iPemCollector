using iPem.Model;
using System;
using System.Collections.Generic;

namespace iPem.Task {
    public partial class HisTask05 : ITask {
        public string Id {
            get { return "hisTask05"; }
        }

        public string Name {
            get { return "电池曲线处理任务"; }
        }

        public long Seconds { get; set; }

        public DateTime Time { get; set; }

        public DateTime Last { get; set; }

        public DateTime Next { get; set; }

        public List<Event> Events { get; set; }

        public int Order {
            get { return 5; }
        }

        public void Execute() {
        }
    }
}
