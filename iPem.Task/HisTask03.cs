using iPem.Core;
using iPem.Data;
using iPem.Model;
using System;
using System.Collections.Generic;

namespace iPem.Task {
    public partial class HisTask03 : IHisTask {
        public string Id {
            get { return "hisTask03"; }
        }

        public string Name {
            get { return "带载合格率处理任务"; }
        }

        public long Seconds { get; set; }

        public DateTime Time { get; set; }

        public DateTime Last { get; set; }

        public DateTime Next { get; set; }

        public List<Event> Events { get; set; }

        public int Order {
            get { return 3; }
        }

        public void Execute() {
            //TODO: 开关电源负载电流、工作状态(均充、浮充、放电)
            //TODO: 开关电源带载合格率 = 浮充工作状态下最大负载电流 / 模块数量 * 模块额定电流
        }
    }
}
