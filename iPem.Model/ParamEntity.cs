using System;

namespace iPem.Model {
    /// <summary>
    /// 基本参数信息
    /// </summary>
    public partial class ParamEntity {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 参数名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// JSON格式的详细配置信息
        /// </summary>
        public ParamModel Json { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime Time { get; set; }
    }
}
