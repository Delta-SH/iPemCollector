using iPem.Core;
using iPem.Core.Rs;
using System;
using System.Collections.Generic;

namespace iPem.Model {
    /// <summary>
    /// 全局参数信息
    /// </summary>
    public static class GlobalConfig1 {
        public static List<ParamEntity> Params { get; set; }

        public static List<TaskEntity> Tasks { get; set; }

        public static void SetTaskPloy(TaskEntity task) {
            var model = task.Json;
            if (model.Type == PlanType.Hour) {
                var next = DateTime.Today.AddHours(DateTime.Now.Hour + model.Interval);
                var timeRangs = next.Hour * 3600 + next.Minute * 60 + next.Second;
                var timeRangsMin = model.StartTime.Hour * 3600 + model.StartTime.Minute * 60 + model.StartTime.Second;
                var timeRangsMax = model.EndTime.Hour * 3600 + model.EndTime.Minute * 60 + model.EndTime.Second;
                if (timeRangs < timeRangsMin)
                    next = next.Date.AddSeconds(timeRangsMin);
                else if(timeRangs > timeRangsMax)
                    next = next.AddDays(1).Date.AddSeconds(timeRangsMin);

                task.Start = task.End.AddSeconds(1);
                task.End = next.Date.AddHours(next.Hour).AddSeconds(-1);
                task.Next = next;
            } else if (model.Type == PlanType.Day) {
                var next = DateTime.Today.AddDays(model.Interval).AddSeconds(model.StartTime.Hour * 3600 + model.StartTime.Minute * 60 + model.StartTime.Second);

                task.Start = task.End.AddSeconds(1);
                task.End = next.Date.AddSeconds(-1);
                task.Next = next;
            } else if (model.Type == PlanType.Month) {
                var next = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(model.Interval);

                task.Start = task.End.AddSeconds(1);
                task.End = next.AddSeconds(-1);
                task.Next = next.AddSeconds(model.StartTime.Hour * 3600 + model.StartTime.Minute * 60 + model.StartTime.Second);
            } else {
                task.Start = task.End = task.Next = new DateTime(2099, 12, 31, 23, 59, 59);
            }
        }

        public static List<ReservationModel> Reservations { get; set; }

        public static Dictionary<string, ReversalModel> Reversals { get; set; }

        public static List<Formula> Formulas { get; set; }

        public static List<StaticModel> Statics { get; set; }

        public static List<BatModel> Batteries { get; set; }

        public static HashSet<string> Maskings { get; set; }

        public static HashSet<string> Offlines { get; set; }

        public static HashSet<string> Cuttings { get; set; }

        public static HashSet<string> Powers { get; set; }

        public static List<ScHeartbeat> ScHeartbeats { get; set; }

        #region 告警相关

        public static HashSet<string> AlarmIds { get; private set; }

        public static Dictionary<string, AlarmStart> Alarms { get; private set; }

        /// <summary>
        /// 初始化告警
        /// </summary>
        public static void InitAlarm() {
            AlarmIds = new HashSet<string>();
            Alarms = new Dictionary<string, AlarmStart>();
        }

        /// <summary>
        /// 活动告警
        /// </summary>
        public static void AddAlarm(A_AAlarm alarm) {
            AddAlarm(new AlarmStart {
                Id = alarm.Id,
                AreaId = alarm.AreaId,
                StationId = alarm.StationId,
                RoomId = alarm.RoomId,
                FsuId = alarm.FsuId,
                DeviceId = alarm.DeviceId,
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
                ReversalCount = alarm.ReversalCount,
                Masked = false
            });
        }

        /// <summary>
        /// 开始告警
        /// </summary>
        public static void AddAlarm(AlarmStart alarm) {
            if (alarm == null) return;
            if (AlarmIds == null) AlarmIds = new HashSet<string>();
            if (Alarms == null) Alarms = new Dictionary<string, AlarmStart>();

            AlarmIds.Add(alarm.Id);
            Alarms[CommonHelper.JoinKeys(alarm.DeviceId, alarm.PointId)] = alarm;
        }

        /// <summary>
        /// 结束告警
        /// </summary>
        public static void RemoveAlarm(AlarmEnd alarm) {
            if (alarm == null) 
                return;

            if (AlarmIds != null && AlarmIds.Count > 0) 
                AlarmIds.Remove(alarm.Id);

            if (Alarms != null && Alarms.Count > 0) 
                Alarms.Remove(CommonHelper.JoinKeys(alarm.DeviceId, alarm.PointId));
        }

        #endregion
    }
}
