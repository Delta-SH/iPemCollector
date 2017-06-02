using iPem.Core;
using System;

namespace iPem.Model {
    /// <summary>
    /// 信号测值统计策略信息
    /// </summary>
    public partial class StaticModel {
        /// <summary>
        /// 设备信息类
        /// </summary>
        public Device Device { get; set; }

        /// <summary>
        /// 信号信息类
        /// </summary>
        public Point Point { get; set; }

        /// <summary>
        /// 统计周期(分钟)
        /// </summary>
        public int Interval { get; set; }
    }
}
