using System;

namespace iPem.Core {
    /// <summary>
    /// 机房信息
    /// </summary>
    [Serializable]
    public partial class Room {
        /// <summary>
        /// 机房编码
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 外部编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 机房名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 机房类型
        /// </summary>
        public RoomType Type { get; set; }

        /// <summary>
        /// 区域编码
        /// </summary>
        public string AreaId { get; set; }

        /// <summary>
        /// 站点编码
        /// </summary>
        public string StationId { get; set; }

        /// <summary>
        /// 站点名称
        /// </summary>
        public string StationName { get; set; }

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
