using System;

namespace iPem.Core {

    public enum DatabaseId {
        Rs = 1,
        Cs,
        Sc
    }

    public enum DatabaseType {
        SQLServer = 1,
        Oracle
    }

    public enum PlanId {
        Act = 1,
        His
    }

    public enum EventType {
        Error = 1,
        Warning,
        Info,
        Other
    }

    public enum TestStatus {
        Default,
        Testing,
        Success,
        Failure,
        Stop
    }

    public enum RunStatus {
        Default,
        Init,
        Running,
        Pause,
        Stop
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

    /// <summary>
    /// Represents the point state enumeration
    /// </summary>
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
        Station,
        Room,
        Fsu,
        Ups,
        BianYaQi,
        FaDianJiZu,
        FengNengSheBei,
        KaiGuanRongSi,
        YiDongFaDianJi,
        ZhongYangKongTiaoZhuJiXiTong,
        ZiDongDianYuanQieHuanGui,
        Device,
        Point,
        Employee
    }
}