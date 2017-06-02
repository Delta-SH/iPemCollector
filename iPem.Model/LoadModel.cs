using System;

namespace iPem.Model {
    /// <summary>
    /// 开关电源带载率信息
    /// </summary>
    public partial class LoadModel {
        /// <summary>
        /// 开关电源工作状态信号编号(不同的开关电源可能对应不同的工作状态信号)
        /// </summary>
        public string[] Status { get; set; }

        /// <summary>
        /// 开关电源负载电流信号编号(不同的开关电源可能对应不同的负载电流信号)
        /// </summary>
        public string[] Loads { get; set; }
    }
}