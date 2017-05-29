using System;

namespace iPem.Core {
    /// <summary>
    /// 模版信息
    /// </summary>
    [Serializable]
    public partial class Protocol {
        /// <summary>
        /// 模版编码
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 模版名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 所属设备类型
        /// </summary>
        public DeviceType DeviceType { get; set; }

        /// <summary>
        /// 所属设备子类
        /// </summary>
        public SubDeviceType SubDeviceType { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool Enabled { get; set; }
    }
}
