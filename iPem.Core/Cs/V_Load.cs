using System;

namespace iPem.Core {
    /// <summary>
    /// 开关电源带载率表
    /// </summary>
    [Serializable]
    public partial class V_Load {
        /// <summary>
        /// 区域编码(第三级区域)
        /// </summary>
        public string AreaId { get; set; }

        /// <summary>
        /// 站点编码
        /// </summary>
        public string StationId { get; set; }

        /// <summary>
        /// 机房编码
        /// </summary>
        public string RoomId { get; set; }

        /// <summary>
        /// 设备编码
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 汇聚值
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// 入库时间
        /// </summary>
        public DateTime CreatedTime { get; set; }
    }
}
