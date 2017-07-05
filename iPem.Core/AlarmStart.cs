using System;

namespace iPem.Core {
    /// <summary>
    /// 开始告警信息
    /// </summary>
    [Serializable]
    public partial class AlarmStart {
        /// <summary>
        /// 告警唯一标识
        /// </summary>
        public string Id { get; set; }

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
        /// Fsu编码
        /// </summary>
        public string FsuId { get; set; }

        /// <summary>
        /// Fsu外部编码
        /// </summary>
        public string FsuCode { get; set; }

        /// <summary>
        /// 设备编码
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// 设备外部编码
        /// </summary>
        public string DeviceCode { get; set; }

        /// <summary>
        /// 信号编码
        /// </summary>
        public string PointId { get; set; }

        /// <summary>
        /// FSU内告警唯一标识
        /// </summary>
        public string SerialNo { get; set; }

        /// <summary>
        /// 网管告警编码
        /// </summary>
        public string NMAlarmId { get; set; }

        /// <summary>
        /// 告警时间
        /// </summary>
        public DateTime AlarmTime { get; set; }

        /// <summary>
        /// 告警级别
        /// </summary>
        public EnmAlarm AlarmLevel { get; set; }

        /// <summary>
        /// 告警标记
        /// </summary>
        public EnmFlag AlarmFlag { get; set; }

        /// <summary>
        /// 告警触发值
        /// </summary>
        public double AlarmValue { get; set; }

        /// <summary>
        /// 告警描述
        /// </summary>
        public string AlarmDesc { get; set; }

        /// <summary>
        /// 告警预留字段
        /// </summary>
        public string AlarmRemark { get; set; }

        /// <summary>
        /// 工程预约编号(非工程预约告警时，此值为NULL)
        /// </summary>
        public string ReservationId { get; set; }

        /// <summary>
        /// 工程预约名称(非工程预约告警时，此值为NULL)
        /// </summary>
        public string ReservationName { get; set; }

        /// <summary>
        /// 工程预约开始时间(非工程预约告警时，此值为NULL)
        /// </summary>
        public DateTime? ReservationStart { get; set; }

        /// <summary>
        /// 工程预约结束时间(非工程预约告警时，此值为NULL)
        /// </summary>
        public DateTime? ReservationEnd { get; set; }

        /// <summary>
        /// 主告警编号(当本身为主告警时，此值为NULL)
        /// </summary>
        public string PrimaryId { get; set; }

        /// <summary>
        /// 关联告警编号(当本身为主关联告警时，此值为NULL)
        /// </summary>
        public string RelatedId { get; set; }

        /// <summary>
        /// 告警过滤编号(当本身为主告警时，此值为NULL)
        /// </summary>
        public string FilterId { get; set; }

        /// <summary>
        /// 翻转告警编号(当本身为主翻转告警时，此值为NULL)
        /// </summary>
        public string ReversalId { get; set; }

        /// <summary>
        /// 翻转告警次数(当本身为主翻转告警时，此值为NULL)
        /// </summary>
        public int ReversalCount { get; set; }

        /// <summary>
        /// 是否为屏蔽告警
        /// </summary>
        public bool Masked { get; set; }
    }
}