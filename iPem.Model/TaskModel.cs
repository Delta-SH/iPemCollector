using iPem.Core;
using System;

namespace iPem.Model {
    /// <summary>
    /// 计划任务策略信息
    /// </summary>
    public partial class TaskModel {
        /// <summary>
        /// 计划类型(小时、天、月)
        /// </summary>
        public PlanType Type { get; set; }

        /// <summary>
        /// 计划开始日期
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 计划结束日期
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 计划执行间隔时间（单位由计划类型决定）
        /// </summary>
        public int Interval { get; set; }

        /// <summary>
        /// 执行开始时间/时段
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 结束时段
        /// </summary>
        public DateTime EndTime { get; set; }
    }
}
