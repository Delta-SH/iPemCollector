using System;

namespace iPem.Configurator {
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