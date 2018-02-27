using iPem.Core;
using iPem.Core.Rs;
using System;
using System.Collections.Generic;

namespace iPem.Model {
    /// <summary>
    /// 全局参数信息
    /// </summary>
    public static class GlobalConfig {
        public static List<ParamEntity> CurParams { get; set; }

        public static List<TaskEntity> CurTasks { get; set; }

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

        public static HashSet<string> Maskings { get; set; }

        public static List<ReservationModel> Reservations { get; set; }

        public static Dictionary<string, ReversalModel> ReversalKeys { get; set; }

        public static List<Formula> FormulaModels { get; set; }

        public static List<StaticModel> StaticModels { get; set; }

        public static List<BatModel> BatModels { get; set; }

        public static HashSet<string> Offs { get; set; }

        public static HashSet<string> Cuttings { get; set; }

        public static HashSet<string> Powers { get; set; }

        public static List<ScHeartbeat> ScHeartbeats { get; set; }

        #region 告警相关

        /// <summary>
        /// 当前的活动告警
        /// </summary>
        public static List<AlarmStart> Alarms { get; private set; }

        /// <summary>
        /// Key: 告警唯一标识
        /// </summary>
        public static Dictionary<string, AlarmStart> AlarmKeys1 { get; private set; }

        /// <summary>
        /// Key: 设备编码+信号编码
        /// </summary>
        public static Dictionary<string, AlarmStart> AlarmKeys2 { get; private set; }

        /// <summary>
        /// 清空告警
        /// </summary>
        public static void InitAlarm() {
            Alarms = new List<AlarmStart>();
            AlarmKeys1 = new Dictionary<string, AlarmStart>();
            AlarmKeys2 = new Dictionary<string, AlarmStart>();
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
            if (Alarms == null) Alarms = new List<AlarmStart>();
            if (AlarmKeys1 == null) AlarmKeys1 = new Dictionary<string, AlarmStart>();
            if (AlarmKeys2 == null) AlarmKeys1 = new Dictionary<string, AlarmStart>();

            Alarms.Add(alarm);
            AlarmKeys1[alarm.Id] = alarm;
            AlarmKeys2[CommonHelper.JoinKeys(alarm.DeviceId, alarm.PointId)] = alarm;
        }

        /// <summary>
        /// 结束告警
        /// </summary>
        public static void RemoveAlarm(AlarmEnd alarm) {
            if (alarm == null) return;
            if (Alarms != null) Alarms.RemoveAll(a => a.Id == alarm.Id);
            if (AlarmKeys1 != null) AlarmKeys1.Remove(alarm.Id);
            if (AlarmKeys2 != null) AlarmKeys2.Remove(CommonHelper.JoinKeys(alarm.DeviceId, alarm.PointId));
        }

        #endregion
    }
}
