using iPem.Core;
using System;
using System.Collections.Generic;

namespace iPem.Model {
    /// <summary>
    /// 电池放电详细信息
    /// </summary>
    public partial class BatDetail {
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
        /// 电池信号
        /// </summary>
        public string PointId { get; set; }

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
        public List<KV<DateTime,double>> Values { get; set; }
    }
}
