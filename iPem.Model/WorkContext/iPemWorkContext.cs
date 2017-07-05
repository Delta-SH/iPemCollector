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

        public static List<Point> Points { get; set; }

        public static List<SubPoint> SubPoints { get; set; }

        public static List<WcProtocol> Protocols { get; set; }

        public static List<WcDevice> Devices { get; set; }

        public static List<WcFsu> Fsus { get; set; }

        public static List<WcRoom> Rooms { get; set; }

        public static List<WcStation> Stations { get; set; }

        public static List<WcArea> Areas { get; set; }
    }
}
