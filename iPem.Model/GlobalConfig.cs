using iPem.Core;
using iPem.Core.Rs;
using System;
using System.Collections.Generic;

namespace iPem.Model {
    /// <summary>
    /// 全局参数信息
    /// </summary>
    public static class GlobalConfig {
        public static ParamEntity CurParam { get; set; }

        public static List<TaskEntity> CurTasks { get; set; }

        public static Dictionary<string, RedefinePoint> RedefinePoints { get; set; }

        public static DateTime RedefineLoadTime { get; set; }

        public static List<ReservationModel> Reservations { get; set; }

        public static DateTime ReservationLoadTime { get; set; }

        public static Dictionary<string, ReversalModel> ReversalKeys { get; set; }

        #region 告警相关

        /// <summary>
        /// 当前的活动告警
        /// </summary>
        public static List<StartAlarm> Alarms { get; private set; }

        /// <summary>
        /// Key: 告警唯一标识
        /// </summary>
        public static Dictionary<string, StartAlarm> AlarmKeys1 { get; private set; }

        /// <summary>
        /// Key: 设备编码+信号编码
        /// </summary>
        public static Dictionary<string, StartAlarm> AlarmKeys2 { get; private set; }

        /// <summary>
        /// 告警加载时间
        /// </summary>
        public static DateTime AlarmsLoadTime { get; set; }

        /// <summary>
        /// 清空告警
        /// </summary>
        public static void InitAlarm() {
            Alarms = new List<StartAlarm>();
            AlarmKeys1 = new Dictionary<string, StartAlarm>();
            AlarmKeys2 = new Dictionary<string, StartAlarm>();
        }

        /// <summary>
        /// 活动告警
        /// </summary>
        public static void AddAlarm(A_AAlarm alarm) {
            AddAlarm(new StartAlarm {
                Id = alarm.Id,
                AreaId = alarm.AreaId,
                StationId = alarm.StationId,
                RoomId = alarm.RoomId,
                FsuId = alarm.FsuId,
                FsuCode = null,
                DeviceId = alarm.DeviceId,
                DeviceCode = null,
                PointId = alarm.PointId,
                SerialNo = alarm.SerialNo,
                NMAlarmId = alarm.NMAlarmId,
                AlarmTime = alarm.AlarmTime,
                AlarmLevel = alarm.AlarmLevel,
                AlarmFlag = EnmFlag.Begin,
                AlarmValue = alarm.AlarmValue,
                AlarmDesc = alarm.AlarmDesc,
                AlarmRemark = alarm.AlarmRemark,
                ReservationId = alarm.ReservationId,
                ReservationName = null,
                ReservationStart = null,
                ReservationEnd = null,
                PrimaryId = alarm.PrimaryId,
                RelatedId = alarm.RelatedId,
                FilterId = alarm.FilterId,
                ReversalId = alarm.ReversalId,
                ReversalCount = alarm.ReversalCount
            });
        }

        /// <summary>
        /// 开始告警
        /// </summary>
        public static void AddAlarm(StartAlarm alarm) {
            if (alarm == null) return;
            if (Alarms == null) Alarms = new List<StartAlarm>();
            if (AlarmKeys1 == null) AlarmKeys1 = new Dictionary<string, StartAlarm>();
            if (AlarmKeys2 == null) AlarmKeys1 = new Dictionary<string, StartAlarm>();

            Alarms.Add(alarm);
            AlarmKeys1[alarm.Id] = alarm;
            AlarmKeys2[CommonHelper.JoinKeys(alarm.DeviceId, alarm.PointId)] = alarm;
        }

        /// <summary>
        /// 结束告警
        /// </summary>
        public static void RemoveAlarm(EndAlarm alarm) {
            if (alarm == null) return;
            if (Alarms != null) Alarms.RemoveAll(a => a.Id == alarm.Id);
            if (AlarmKeys1 != null) AlarmKeys1.Remove(alarm.Id);
            if (AlarmKeys2 != null) AlarmKeys2.Remove(CommonHelper.JoinKeys(alarm.DeviceId, alarm.PointId));
        }

        #endregion
    }
}
