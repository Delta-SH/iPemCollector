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
        SyncBase,
        SyncData,
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
        /// SC通信中断信号
        /// </summary>
        ScOff,
        /// <summary>
        /// FSU通信中断信号
        /// </summary>
        FsuOff,
        /// <summary>
        /// 开关电源负载电流信号
        /// </summary>
        FZDL,
        /// <summary>
        /// 开关电源工作状态信号
        /// </summary>
        GZZT,
        /// <summary>
        /// 电池总电压信号
        /// </summary>
        DCZDY,
        /// <summary>
        /// 电池总电流信号
        /// </summary>
        DCZDL,
        /// <summary>
        /// 电池电压信号
        /// </summary>
        DCDY,
        /// <summary>
        /// 电池温度信号
        /// </summary>
        DCWD,
        /// <summary>
        /// 实时能耗统计周期
        /// </summary>
        NHZQ,
        /// <summary>
        /// 实时告警接口
        /// </summary>
        GJJK,
        /// <summary>
        /// 电池处理功能
        /// </summary>
        DCSJ,
        /// <summary>
        /// 短信告警功能
        /// </summary>
        DXGJ,
        /// <summary>
        /// 语音告警功能
        /// </summary>
        YYGJ,
        /// <summary>
        /// 虚拟信号处理频率
        /// </summary>
        XXPL
    }

    public enum PeriodType {
        Hour,
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
        AO = 2,
        /// <summary>
        /// 告警
        /// </summary>
        AL = 0
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
        /// 开关电源
        /// </summary>
        DY,
        /// <summary>
        /// UPS
        /// </summary>
        UPS,
        /// <summary>
        /// IT设备
        /// </summary>
        IT,
        /// <summary>
        /// 其他
        /// </summary>
        QT,
        /// <summary>
        /// 总计
        /// </summary>
        TT,
        /// <summary>
        /// PUE
        /// </summary>
        PUE,
        /// <summary>
        /// 停电标识
        /// </summary>
        TD,
        /// <summary>
        /// 温度标识
        /// </summary>
        WD,
        /// <summary>
        /// 湿度标识
        /// </summary>
        SD,
        /// <summary>
        /// 发电标识
        /// </summary>
        FD,
        /// <summary>
        /// 发电量
        /// </summary>
        FDL,
        /// <summary>
        /// 变压器
        /// </summary>
        BY,
        /// <summary>
        /// 线损
        /// </summary>
        XS
    }

    /// <summary>
    /// 公式运算方式
    /// </summary>
    public enum EnmCompute {
        /// <summary>
        /// 电度
        /// </summary>
        Kwh,
        /// <summary>
        /// 功率
        /// </summary>
        Power
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

    /// <summary>
    /// 电池充放电状态
    /// </summary>
    public enum EnmBatType {
        /// <summary>
        /// 放电
        /// </summary>
        Discharge,
        /// <summary>
        /// 充电
        /// </summary>
        Charge
    }

    public enum EnmBatPoint {
        /// <summary>
        /// 电池总电压信号
        /// </summary>
        DCZDY,
        /// <summary>
        /// 电池总电流信号
        /// </summary>
        DCZDL,
        /// <summary>
        /// 电池电压信号
        /// </summary>
        DCDY,
        /// <summary>
        /// 电池温度信号
        /// </summary>
        DCWD
    }

    /// <summary>
    /// 结果类型
    /// </summary>
    public enum EnmResult {
        /// <summary>
        /// 未定义
        /// </summary>
        Undefine = -1,
        /// <summary>
        /// 失败
        /// </summary>
        Failure,
        /// <summary>
        /// 成功
        /// </summary>
        Success
    }

    /// <summary>
    /// 虚拟信号分类
    /// </summary>
    public enum EnmVSignalCategory {
        /// <summary>
        /// 普通虚拟信号
        /// </summary>
        Category01 = 1001,
        /// <summary>
        /// 能耗虚拟信号
        /// </summary>
        Category02 = 1002,
        /// <summary>
        /// 列头柜分路功率
        /// </summary>
        Category03 = 1003,
        /// <summary>
        /// 列头柜分路电流
        /// </summary>
        Category04 = 1004,
        /// <summary>
        /// 列头柜分路电量
        /// </summary>
        Category05 = 1005
    }
}