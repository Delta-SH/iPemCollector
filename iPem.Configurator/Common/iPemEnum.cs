using System;

namespace iPem.Configurator {
    public enum OrderId {
        Null,
        Restart,
        Reload,
        SyncConfig,
        SyncAlarm
    }

    public enum ParamId {
        Null,
        Param1,
        Param2,
        Param3,
        Param4
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
        Database,
        Plan
    }

    public enum PlanType {
        Hour = 1,
        Day = 24,
        Month = 720
    }
}