using System;

namespace iPem.Core {
    /// <summary>
    /// 结束告警信息
    /// </summary>
    [Serializable]
    public partial class EndAlarm {
        /// <summary>
        /// 告警唯一标识，与开始告警ID一致
        /// </summary>
        public string Id { get; set; }

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
        /// 告警开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 告警结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

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
        public double StartValue { get; set; }

        /// <summary>
        /// 告警结束值
        /// </summary>
        public double EndValue { get; set; }

        /// <summary>
        /// 告警描述
        /// </summary>
        public string AlarmDesc { get; set; }

        /// <summary>
        /// 告警预留字段
        /// </summary>
        public string AlarmRemark { get; set; }
    }
}