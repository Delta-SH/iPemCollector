using iPem.Core;
using System;
using System.Collections.Generic;

namespace iPem.Model {
    /// <summary>
    /// 工程预约相关信息
    /// </summary>
    public partial class ReservationModel {
        /// <summary>
        /// 预约信息
        /// </summary>
        public Reservation Reservation { get; set; }

        /// <summary>
        /// 预约设备信息
        /// </summary>
        public HashSet<string> Devices { get; set; }
    }
}
