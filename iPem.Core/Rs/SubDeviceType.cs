using System;

namespace iPem.Core {
    /// <summary>
    /// 设备子类信息
    /// </summary>
    [Serializable]
    public partial class SubDeviceType {
        /// <summary>
        /// 设备子类编码
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 设备子类名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 设备类型编码
        /// </summary>
        public string DeviceTypeId { get; set; }
    }
}
