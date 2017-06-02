using System;

namespace iPem.Model {
    /// <summary>
    /// 计划任务信息
    /// </summary>
    public partial class TaskEntity {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 计划任务名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// JSON格式的计划任务执行策略
        /// </summary>
        public TaskModel Json { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime Start { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime End { get; set; }

        /// <summary>
        /// 执行时间
        /// </summary>
        public DateTime Next { get; set; }

        /// <summary>
        /// 计划任务执行权重
        /// </summary>
        public int Index { get; set; }
    }
}
