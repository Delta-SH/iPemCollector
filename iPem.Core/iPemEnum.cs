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
        SyncAlarm,
        SyncReservation,
        SyncFormula,
        SyncMasking,
        SyncFsu,
        ExTask001,
        ExTask002,
        ExTask003,
        ExTask004,
        ExTask005,
        ExTask006,
        ExTask007
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
        GZZT,
        /// <summary>
        /// 实时能耗统计时段
        /// </summary>
        SSNH,
        /// <summary>
        /// 实时告警接口
        /// </summary>
        GJJK
    }

    public enum PeriodType {
        Day,
        Month
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

    public enum EnmSSH {
        /// <summary>
        /// 全部
        /// </summary>
        Root = -1,
        /// <summary>
        /// 区域
        /// </summary>
        Area,
        /// <summary>
        /// 站点
        /// </summary>
        Station,
        /// <summary>
        /// 机房
        /// </summary>
        Room,
        /// <summary>
        /// Fsu
        /// </summary>
        Fsu,
        /// <summary>
        /// 设备
        /// </summary>
        Device,
        /// <summary>
        /// 信号
        /// </summary>
        Point
    }

    public enum EnmPoint {
        /// <summary>
        /// 遥信
        /// </summary>
        DI = 4,
        /// <summary>
        /// 遥测
        /// </summary>
        AI = 3,
        /// <summary>
        /// 遥控
        /// </summary>
        DO = 1,
        /// <summary>
        /// 遥调
        /// </summary>
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

    /// <summary>
    /// 能耗公式类型
    /// </summary>
    public enum EnmFormula {
        /// <summary>
        /// 空调
        /// </summary>
        KT = 1001,
        /// <summary>
        /// 照明
        /// </summary>
        ZM,
        /// <summary>
        /// 办公
        /// </summary>
        BG,
        /// <summary>
        /// IT设备
        /// </summary>
        SB,
        /// <summary>
        /// 开关电源
        /// </summary>
        KGDY,
        /// <summary>
        /// UPS
        /// </summary>
        UPS,
        /// <summary>
        /// 其他
        /// </summary>
        QT,
        /// <summary>
        /// 总量
        /// </summary>
        ZL
    }

    /// <summary>
    /// 公式运算方式
    /// </summary>
    public enum EnmCompute {
        /// <summary>
        /// 电表电度
        /// </summary>
        Diff,
        /// <summary>
        /// 电压电流
        /// </summary>
        Avg
    }

    public enum EnmMethodType {
        /// <summary>
        /// 区域
        /// </summary>
        Area = 1,
        /// <summary>
        /// 站点
        /// </summary>
        Station = 2,
        /// <summary>
        /// 机房
        /// </summary>
        Room = 3,
        /// <summary>
        /// FSU
        /// </summary>
        Fsu = 4,
        /// <summary>
        /// 设备
        /// </summary>
        Device = 13,
        /// <summary>
        /// 信号
        /// </summary>
        Point = 14
    }

    public enum EnmCutType {
        /// <summary>
        /// 断站
        /// </summary>
        Off,
        /// <summary>
        /// 停电
        /// </summary>
        Cut,
        /// <summary>
        /// 发电
        /// </summary>
        Power
    }

    public enum EnmMaskType {
        /// <summary>
        /// 区域
        /// </summary>
        Area,
        /// <summary>
        /// 站点
        /// </summary>
        Station,
        /// <summary>
        /// 机房
        /// </summary>
        Room,
        /// <summary>
        /// FSU
        /// </summary>
        Fsu,
        /// <summary>
        /// 设备
        /// </summary>
        Device,
        /// <summary>
        /// 信号
        /// </summary>
        Point
    }
}