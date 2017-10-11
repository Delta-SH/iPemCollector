using iPem.Core;
using iPem.Core.Rs;
using System;

namespace iPem.Model {
    /// <summary>
    /// 信号测值统计策略信息
    /// </summary>
    public partial class StaticModel {
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
        /// 统计周期(分钟)
        /// </summary>
        public int Interval { get; set; }
    }
}
