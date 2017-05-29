using System;

namespace iPem.Core {
    /// <summary>
    /// 蓄电池组
    /// </summary>
    [Serializable]
    public partial class BattGroup : Device {
        /// <summary>
        /// 单组容量
        /// </summary>
        public double SingGroupCap { get; set; }

        /// <summary>
        /// 单体电压等级
        /// </summary>
        public int SingVoltGrade { get; set; }

        /// <summary>
        /// 单组蓄电池个数
        /// </summary>
        public int SingGroupBattNumber { get; set; }
    }
}
