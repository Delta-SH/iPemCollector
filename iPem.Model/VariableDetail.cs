using iPem.Core;
using iPem.Core.Rs;
using System;

namespace iPem.Model {
    public partial class VariableDetail {
        /// <summary>
        /// 所属设备
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// 信号
        /// </summary>
        public string PointId { get; set; }

        /// <summary>
        /// 变量
        /// </summary>
        public string Variable { get; set; }
    }
}
