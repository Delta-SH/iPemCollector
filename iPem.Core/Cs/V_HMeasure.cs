﻿using System;

namespace iPem.Core {
    /// <summary>
    /// 历史性能数据表
    /// </summary>
    [Serializable]
    public partial class V_HMeasure {
        /// <summary>
        /// 区域编码(第三级区域)
        /// </summary>
        public string AreaId { get; set; }

        /// <summary>
        /// 站点编码
        /// </summary>
        public string StationId { get; set; }

        /// <summary>
        /// 机房编码
        /// </summary>
        public string RoomId { get; set; }

        /// <summary>
        /// FSU编码
        /// </summary>
        public string FsuId { get; set; }

        /// <summary>
        /// 设备编码
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// 信号编码
        /// </summary>
        public string PointId { get; set; }

        /// <summary>
        /// 信号描述
        /// </summary>
        public string SignalDesc { get; set; }

        /// <summary>
        /// 测值类型(绝对阈值，百分比阈值)
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 监测值
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// 测值时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
    }
}
