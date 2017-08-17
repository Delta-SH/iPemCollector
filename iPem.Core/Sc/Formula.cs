﻿using System;

namespace iPem.Core {
    /// <summary>
    /// 能耗公式信息
    /// </summary>
    [Serializable]
    public partial class Formula {
        /// <summary>
        /// 站点/机房编码
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 类型(机房、站点)
        /// </summary>
        public EnmSSH Type { get; set; }

        /// <summary>
        /// 公式类型(空调、照明、办公、设备、开关电源、UPS、其他、总量)
        /// </summary>
        public EnmFormula FormulaType { get; set; }

        /// <summary>
        /// 运算方式(差值运算、均值运算)
        /// </summary>
        public EnmCompute ComputeType { get; set; }

        /// <summary>
        /// 公式信息
        /// </summary>
        public string FormulaText { get; set; }

        /// <summary>
        /// 公式描述
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// 入库时间
        /// </summary>
        public DateTime CreatedTime { get; set; }
    }
}
