using System;

namespace iPem.Core {
    /// <summary>
    /// 虚拟信号
    /// </summary>
    [Serializable]
    public partial class VSignal {
        /// <summary>
        /// 区域编码(第三级区域)
        /// </summary>
        public string AreaId { get; set; }

        /// <summary>
        /// 站点编码
        /// </summary>
        public string StationId { get; set; }

        /// <summary>
        /// 机房编码
        /// </summary>
        public string RoomId { get; set; }

        /// <summary>
        /// FSU编码
        /// </summary>
        public string FsuId { get; set; }

        /// <summary>
        /// 信号编码
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// 信号编码
        /// </summary>
        public string PointId { get; set; }

        /// <summary>
        /// 信号名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 信号类型
        /// </summary>
        public EnmPoint Type { get; set; }

        /// <summary>
        /// 名称公式
        /// </summary>
        public string FormulaText { get; set; }

        /// <summary>
        /// 编号公式
        /// </summary>
        public string FormulaValue { get; set; }

        /// <summary>
        /// 单位/描述
        /// </summary>
        public string UnitState { get; set; }

        /// <summary>
        /// 存储周期(秒)
        /// </summary>
        public int SavedPeriod { get; set; }

        /// <summary>
        /// 统计周期（分钟）
        /// </summary>
        public int StaticPeriod { get; set; }

        /// <summary>
        /// 信号分类
        /// </summary>
        public EnmVSignalCategory Category { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 存储时间
        /// </summary>
        public DateTime SavedTime { get; set; }
    }
}
