using iPem.Model;
using System;
using System.Collections.Generic;

namespace iPem.Task {
    public partial class HisTask02 : IHisTask {
        public string Id {
            get { return "hisTask02"; }
        }

        public string Name {
            get { return "后备时长处理任务"; }
        }

        public long Seconds { get; set; }

        public DateTime Time { get; set; }

        public DateTime Last { get; set; }

        public DateTime Next { get; set; }

        public List<Event> Events { get; set; }

        public int Order {
            get { return 2; }
        }

        public void Execute() {
            //TODO: 蓄电池电压、蓄电池电流、蓄电池工作状态(均充、浮充、放电)
            //TODO: 蓄电池后备时长 = (蓄电池单组容量 * 单组蓄电池个数) / 蓄电池放电状态下的放电电流
        }
    }
}
