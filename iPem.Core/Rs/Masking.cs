using System;

namespace iPem.Core {
    /// <summary>
    /// 告警屏蔽表
    /// </summary>
    [Serializable]
    public partial class Masking {
        /// <summary>
        /// 节点编号(当节点类型为信号时，Id格式: 设备编号;信号编号)
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 节点类型
        /// </summary>
        public EnmMaskType Type { get; set; }
    }
}
