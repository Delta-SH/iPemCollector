using iPem.Core;
using System;
using System.Collections.Generic;

namespace iPem.Model {
    public static class iPemWorkContext {
        public static List<LogicType> LogicTypes { get; set; }

        public static List<SubLogicType> SubLogicTypes { get; set; }

        public static List<DeviceType> DeviceTypes { get; set; }

        public static List<SubDeviceType> SubDeviceTypes { get; set; }

        public static List<RoomType> RoomTypes { get; set; }

        public static List<StationType> StationTypes { get; set; }

        public static List<EnumMethods> AreaTypes { get; set; }

        /// <summary>
        /// Key = PointId
        /// </summary>
        public static Dictionary<string, Point> Points { get; set; }

        /// <summary>
        /// Key = PointId + StationTypeId
        /// </summary>
        public static Dictionary<string, SubPoint> SubPoints { get; set; }

        public static List<WcDevice> Devices { get; set; }

        /// <summary>
        /// Key = DeviceId
        /// </summary>
        public static Dictionary<string, WcDevice> DeviceSet1 { get; set; }

        /// <summary>
        /// Key = FSUCode + DeviceCode
        /// </summary>
        public static Dictionary<string, WcDevice> DeviceSet2 { get; set; }

        public static List<Fsu> Fsus { get; set; }

        public static List<Room> Rooms { get; set; }

        public static List<Station> Stations { get; set; }

        public static List<WcArea> Areas { get; set; }
    }
}
