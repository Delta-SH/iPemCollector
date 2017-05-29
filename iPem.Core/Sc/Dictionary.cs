using System;

namespace iPem.Core {
    /// <summary>
    /// 系统参数字典
    /// </summary>
    [Serializable]
    public partial class Dictionary {
        /// <summary>
        /// 参数编码
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 参数名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// JSON格式参数
        /// </summary>
        public string ValuesJson { get; set; }

        /// <summary>
        /// Binary格式参数(预留)
        /// </summary>
        public byte[] ValuesBinary { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime LastUpdatedDate { get; set; }
    }
}
