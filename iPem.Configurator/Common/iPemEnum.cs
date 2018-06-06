using System;

namespace iPem.Configurator {
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

    public enum TestStatus {
        Default,
        Testing,
        Success,
        Failure,
        Stop
    }

    public enum NodeType {
        Root,
        Parent,
        Param,
        Database,
        Plan
    }

    public enum PlanType {
        Hour = 1,
        Day = 24,
        Month = 720
    }
}