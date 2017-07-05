using System;

namespace iPem.Model {
    /// <summary>
    /// 电池放电统计参数信息
    /// </summary>
    public partial class BatParam {
        /// <summary>
        /// 设备编号
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// 信号编号
        /// </summary>
        public string PointId { get; set; }

        /// <summary>
        /// 电池组号
        /// </summary>
        public int PackId { get; set; }

        /// <summary>
        /// 电池组总电压标准电压值
        /// </summary>
        public double Voltage { get; set; }
    }
}
