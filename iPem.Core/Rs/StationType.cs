using System;

namespace iPem.Core {
    /// <summary>
    /// 站点类型信息
    /// </summary>
    [Serializable]
    public partial class StationType {
        /// <summary>
        /// 站点类型编码
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 站点类型名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Comment { get; set; }
    }
}
