using System;

namespace iPem.Core {
    /// <summary>
    /// 自定义枚举信息
    /// </summary>
    [Serializable]
    public partial class EnumMethods {
        /// <summary>
        /// 编号
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 枚举类型
        /// </summary>
        public int TypeId { get; set; }

        /// <summary>
        /// 枚举索引
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Comment { get; set; }
    }
}
