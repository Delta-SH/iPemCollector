using iPem.Core;
using System;
using System.Collections.Generic;

namespace iPem.Model {
    /// <summary>
    /// 电池放电统计策略信息
    /// </summary>
    public partial class BatModel {
        /// <summary>
        /// 设备信息类
        /// </summary>
        public Device Device { get; set; }

        /// <summary>
        /// 信号信息类
        /// </summary>
        public Point Point { get; set; }

        /// <summary>
        /// 同一电池组下所有单体电池信号点
        /// </summary>
        public List<Point> SubPoints { get; set; }

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
