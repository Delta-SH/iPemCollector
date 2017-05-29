using System;

namespace iPem.Core {
    /// <summary>
    /// 逻辑分类
    /// </summary>
    [Serializable]
    public partial class LogicType {
        /// <summary>
        /// 逻辑分类编码
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 逻辑分类名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 设备类型编码
        /// </summary>
        public string DeviceTypeId { get; set; }
    }
}
