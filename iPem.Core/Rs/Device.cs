using System;

namespace iPem.Core {
    /// <summary>
    /// 设备信息
    /// </summary>
    [Serializable]
    public partial class Device {
        /// <summary>
        /// 设备编码
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        public DeviceType Type { get; set; }

        /// <summary>
        /// 设备子类型
        /// </summary>
        public SubDeviceType SubType { get; set; }

        /// <summary>
        /// 所属区域
        /// </summary>
        public string AreaId { get; set; }

        /// <summary>
        /// 所属区域
        /// </summary>
        public string AreaName { get; set; }

        /// <summary>
        /// 所属站点
        /// </summary>
        public string StationId { get; set; }

        /// <summary>
        /// 所属站点
        /// </summary>
        public string StationName { get; set; }

        /// <summary>
        /// 所属站点类型
        /// </summary>
        public StationType StationType { get; set; }

        /// <summary>
        /// 所属机房
        /// </summary>
        public string RoomId { get; set; }

        /// <summary>
        /// 所属机房
        /// </summary>
        public string RoomName { get; set; }

        /// <summary>
        /// 所属FSU编码
        /// </summary>
        public string FsuId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Comment { get; set; }
    }
}
