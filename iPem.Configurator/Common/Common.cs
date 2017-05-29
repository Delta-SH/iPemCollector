using System;
using System.Collections.Generic;

namespace iPem.Configurator {
    public partial class Common {
        public static List<object> GetPlanStore() {
            var data = new List<object>();
            foreach (PlanType type in Enum.GetValues(typeof(PlanType))) {
                data.Add(new { Id = (int)type, Name = Common.GetPlanTypeName(type) });
            }
            return data;
        }

        public static List<object> GetDbStore() {
            var data = new List<object>();
            foreach (DatabaseType dbType in Enum.GetValues(typeof(DatabaseType))) {
                data.Add(new { Id = (int)dbType, Name = dbType.ToString() });
            }
            return data;
        }

        public static string GetPlanTypeName(PlanType type) {
            switch (type) {
                case PlanType.Hour:
                    return "每小时";
                case PlanType.Day:
                    return "每天";
                case PlanType.Month:
                    return "每月";
                default:
                    return "未定义";
            }
        }

        public static string ToDateString(DateTime current) {
            if (current == default(DateTime)) return string.Empty;

            return current.ToString("yyyy-MM-dd");
        }

        public static string ToTimeString(DateTime current) {
            if (current == default(DateTime)) return string.Empty;

            return current.ToString("HH:mm:ss");
        }

        public static string ToDateTimeString(DateTime current) {
            if (current == default(DateTime)) return string.Empty;

            return current.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
