﻿using System;

namespace iPem.Configurator {
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