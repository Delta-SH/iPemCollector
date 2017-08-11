using System;

namespace iPem.Core {
    /// <summary>
    /// 工程预约映射关系
    /// </summary>
    [Serializable]
    public partial class NodesInReservation {
        /// <summary>
        /// 工程预约编码
        /// </summary>
        public string ReservationId { get; set; }

        /// <summary>
        /// 预约节点编码
        /// </summary>
        public string NodeId { get; set; }

        /// <summary>
        /// 预约节点类型
        /// </summary>
        public EnmSSH NodeType { get; set; }
    }
}