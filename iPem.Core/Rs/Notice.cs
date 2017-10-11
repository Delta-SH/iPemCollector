using System;

namespace iPem.Core.Rs {
    /// <summary>
    /// 配置同步表
    /// </summary>
    [Serializable]
    public partial class Notice {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 本系统所用标识为2
        /// </summary>
        public int SysType { get; set; }

        /// <summary>
        /// 需要同步的表名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 同步时间
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Desc { get; set; }
    }
}
