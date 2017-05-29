using System;

namespace iPem.Core {
    /// <summary>
    /// 组合开关电源
    /// </summary>
    [Serializable]
    public partial class CombSwitElecSour : Device {
        /// <summary>
        /// 额定输出电压
        /// </summary>
        public double RatedOutputVolt { get; set; }

        /// <summary>
        /// 监控模块型号
        /// </summary>
        public string MoniModuleModel { get; set; }

        /// <summary>
        /// 现有整流模块总数
        /// </summary>
        public string ExisRModuleCount { get; set; }

        /// <summary>
        /// 整流模块型号
        /// </summary>
        public string RModuleModel { get; set; }

        /// <summary>
        /// 整流模块额定工作电压
        /// </summary>
        public int RModuleRatedWorkVolt { get; set; }

        /// <summary>
        /// 单个整流模块额定输出容量 单位：A 
        /// </summary>
        public string SingRModuleRatedOPCap { get; set; }

        /// <summary>
        /// 单组电池组熔丝容量
        /// </summary>
        public string SingGBattGFuseCap { get; set; }

        /// <summary>
        /// 电池组熔丝组数
        /// </summary>
        public int BattGFuseGNumber { get; set; }

        /// <summary>
        /// 能否二次下电
        /// </summary>
        public string OrCanSecoDownPower { get; set; }
    }
}
