using System;

namespace iPem.Core {
    public partial class DbIds {
        /// <summary>
        /// 资源数据库
        /// </summary>
        public static string Rs {
            get { return "D001"; }
        }

        /// <summary>
        /// 应用数据库
        /// </summary>
        public static string Sc {
            get { return "D002"; }
        }

        /// <summary>
        /// 历史数据库
        /// </summary>
        public static string Cs {
            get { return "D003"; }
        }
    }

    public enum OrderId {
        Null,
        Restart,
        Reload,
        SyncConfig,
        SyncAlarm
    }

    public enum ParamId {
        Null,
        /// <summary>
        /// SC通信中断告警编码
        /// </summary>
        ScOff,
        /// <summary>
        /// FSU通信中断告警编码
        /// </summary>
        FsuOff,
        /// <summary>
        /// 开关电源负载电流信号编码
        /// </summary>
        FZDL,
        /// <summary>
        /// 开关电源工作状态信号编码
        /// </summary>
        GZZT
    }

    public enum DatabaseType {
        SQLServer = 1,
        Oracle
    }

    public enum PlanType {
        Hour = 1,
        Day = 24,
        Month = 720
    }

    public enum RunStatus {
        Default,
        Init,
        Running,
        Pause,
        Stop
    }

    public enum EventType {
        Error = 1,
        Warning,
        Info,
        Other
    }

    public enum EnmFsuEvent {
        Undefined,
        FTP,
        FSU
    }

    public enum EnmOrganization {
        Area,
        Station,
        Room,
        Device,
        Point
    }

    /// <remarks>
    /// 4-遥信信号（DI）
    /// 3-遥测信号（AI）
    /// 1-遥控信号（DO）
    /// 2-遥调信号（AO）
    /// </remarks>
    public enum EnmPoint {
        DI = 4,
        AI = 3,
        DO = 1,
        AO = 2
    }

    public enum EnmState {
        Normal,
        Invalid
    }

    public enum EnmAlarm {
        NoAlarm,
        Level1,
        Level2,
        Level3,
        Level4
    }

    public enum EnmConfirm {
        Unconfirmed,
        Confirmed
    }

    public enum EnmFlag {
        Begin,
        End
    }

    /// <remarks>
    /// KT: 空调
    /// ZM: 照明
    /// BG: 办公
    /// SB: 设备
    /// KGDY: 开关电源
    /// UPS: UPS
    /// QT: 其他
    /// ZL: 总量
    /// </remarks>
    public enum EnmFormula {
        KT,
        ZM,
        BG,
        SB,
        KGDY,
        UPS,
        QT,
        ZL,
        PUE
    }

    /// <remarks>
    /// { Id: 1, Name: "区域" }
    /// { Id: 2, Name: "站点" }
    /// { Id: 3, Name: "机房" }
    /// { Id: 4, Name: "FSU" }
    /// { Id: 5, Name: "UPS" }
    /// { Id: 6, Name: "变压器" }
    /// { Id: 7, Name: "发电机组" }
    /// { Id: 8, Name: "风能设备" }
    /// { Id: 9, Name: "开关熔丝" }
    /// { Id: 10, Name: "移动发电机" }
    /// { Id: 11, Name: "中央空调主机系统" }
    /// { Id: 12, Name: "自动电源切换柜" }
    /// { Id: 13, Name: "设备" }
    /// { Id: 14, Name: "信号" }
    /// { Id: 15, Name: "员工"}
    /// </remarks>
    public enum EnmMethodType {
        Area = 1,
        Station = 2,
        Room = 3,
        Fsu = 4,
        Device = 13,
        Point = 14
    }

    /// <summary>
    /// 断站-0
    /// 停电-1
    /// 发电-2
    /// </summary>
    public enum EnmCutType {
        Off,
        Cut,
        Power
    }

    public enum EnmMaskType {
        Area,
        Station,
        Room,
        Fsu,
        Device,
        Point
    }
}