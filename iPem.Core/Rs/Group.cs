using System;

namespace iPem.Core {
    /// <summary>
    /// SC采集组信息表
    /// </summary>
    [Serializable]
    public partial class Group {
        /// <summary>
        /// SC组编号
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// SC组名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// SC组类型编号
        /// </summary>
        public int TypeId { get; set; }

        /// <summary>
        /// SC组类型名称
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// SC组IP
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// SC组端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// SC组状态
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// 注册时间
        /// </summary>
        public DateTime ChangeTime { get; set; }

        /// <summary>
        /// 离线时间
        /// </summary>
        public DateTime LastTime { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Comment { get; set; }
    }
}
