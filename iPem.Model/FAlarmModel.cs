using iPem.Core;
using System;

namespace iPem.Model {
    /// <summary>
    /// 告警同步信息
    /// </summary>
    public partial class FAlarmModel {
        /// <summary>
        /// 关联的设备信息
        /// </summary>
        public Device Device { get; set; }

        /// <summary>
        /// 关联的信号信息
        /// </summary>
        public Point Point { get; set; }

        /// <summary>
        /// 告警流水信息
        /// </summary>
        public A_FAlarm Alarm { get; set; }
    }
}