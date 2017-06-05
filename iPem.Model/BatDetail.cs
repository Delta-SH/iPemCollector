using iPem.Core;
using System;
using System.Collections.Generic;

namespace iPem.Model {
    /// <summary>
    /// 电池放电详细信息
    /// </summary>
    public partial class BatDetail {
        /// <summary>
        /// 设备信息类
        /// </summary>
        public Device Device { get; set; }

        /// <summary>
        /// 信号信息类
        /// </summary>
        public Point Point { get; set; }

        /// <summary>
        /// 电池组号
        /// </summary>
        public int PackId { get; set; }

        /// <summary>
        /// 放电开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 放电结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 放电开始值
        /// </summary>
        public double StartValue { get; set; }

        /// <summary>
        /// 放电结束值
        /// </summary>
        public double EndValue { get; set; }

        /// <summary>
        /// 详细放电测值
        /// </summary>
        public List<IdValuePair<DateTime,double>> Values { get; set; }
    }
}
