using System;

namespace iPem.Core {
    /// <summary>
    /// 区域信息
    /// </summary>
    [Serializable]
    public partial class Area {
        /// <summary>
        /// 区域编码
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 外部编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 区域名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 区域类型
        /// </summary>
        public KV<int, string> Type { get; set; }

        /// <summary>
        /// 父级区域编码
        /// </summary>
        public string ParentId { get; set; }

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
