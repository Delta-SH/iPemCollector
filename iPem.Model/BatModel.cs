using iPem.Core;
using iPem.Core.Rs;
using System;
using System.Collections.Generic;

namespace iPem.Model {
    /// <summary>
    /// 电池放电统计策略信息
    /// </summary>
    public partial class BatModel {
        /// <summary>
        /// 所属区域
        /// </summary>
        public string AreaId { get; set; }

        /// <summary>
        /// 所属站点
        /// </summary>
        public string StationId { get; set; }

        /// <summary>
        /// 所属机房
        /// </summary>
        public string RoomId { get; set; }

        /// <summary>
        /// 所属Fsu
        /// </summary>
        public string FsuId { get; set; }

        /// <summary>
        /// 所属设备
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// 信号信息类
        /// </summary>
        public string PointId { get; set; }

        /// <summary>
        /// 同一电池组下所有单体电池信号点
        /// </summary>
        public List<string> SubSignals { get; set; }

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
