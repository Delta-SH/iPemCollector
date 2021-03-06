﻿using System;

namespace iPem.Core {
    /// <summary>
    /// 工程预约信息
    /// </summary>
    [Serializable]
    public partial class Reservation {
        /// <summary>
        /// 预约编号
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 预约名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 预期开始时间
        /// </summary>
        public DateTime ExpStartTime { get; set; }

        /// <summary>
        /// 实际开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 关联的工程编码
        /// </summary>
        public string ProjectId { get; set; }

        /// <summary>
        /// 创建人员
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedTime { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// 审核状态
        /// </summary>
        public EnmResult Status { get; set; }

        /// <summary>
        /// 使能状态
        /// </summary>
        public bool Enabled { get; set; }
    }
}
