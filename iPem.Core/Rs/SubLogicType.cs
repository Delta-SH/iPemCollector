using System;

namespace iPem.Core {
    /// <summary>
    /// 逻辑子类信息
    /// </summary>
    [Serializable]
    public partial class SubLogicType {
        /// <summary>
        /// 逻辑子类编码
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 逻辑子类名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 逻辑分类编码
        /// </summary>
        public string LogicTypeId { get; set; }
    }
}
