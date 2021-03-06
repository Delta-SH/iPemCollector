﻿using System;

namespace iPem.Core {
    /// <summary>
    /// 能耗公式信息
    /// </summary>
    [Serializable]
    public partial class Formula {
        /// <summary>
        /// 站点/机房/设备编码
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 类型(站点、机房、设备)
        /// </summary>
        public EnmSSH Type { get; set; }

        /// <summary>
        /// 公式类型
        /// </summary>
        public EnmFormula FormulaType { get; set; }

        /// <summary>
        /// 运算方式
        /// </summary>
        public EnmCompute ComputeType { get; set; }

        /// <summary>
        /// 公式信息
        /// </summary>
        public string FormulaText { get; set; }

        /// <summary>
        /// 公式信息
        /// </summary>
        public string FormulaValue { get; set; }

        /// <summary>
        /// 公式描述
        /// </summary>
        public string Comment { get; set; }
    }
}
