using System;

namespace iPem.Configurator {
    /// <summary>
    /// The SqliteCommands class is intended to store sqlite statements.
    /// </summary>
    public static class SqliteCommands {
        public const string Registry_Create_Tables = @"
        --创建数据表
        CREATE TABLE IF NOT EXISTS [orders] (
            [id] int PRIMARY KEY NOT NULL,
            [param] varchar(1024),
            [time] timestamp NOT NULL DEFAULT (datetime('now','localtime'))
        );

        CREATE TABLE IF NOT EXISTS [params] (
            [id] int PRIMARY KEY NOT NULL,
            [value] varchar(1024),
            [time] timestamp NOT NULL DEFAULT (datetime('now','localtime'))
        );

        CREATE TABLE IF NOT EXISTS [databases] (
            [id] varchar(50) PRIMARY KEY NOT NULL,
            [name] varchar(200),
            [type] int,
            [ip] varchar(128),
            [port] int,
            [uid] varchar(50),
            [password] varchar(50),
            [db] varchar(100)
        );

        CREATE TABLE IF NOT EXISTS [tasks] (
            [id] varchar(50) PRIMARY KEY NOT NULL,
            [name] varchar(200),
            [json] text,
            [start] datetime,
            [end] datetime,
            [next] datetime,
            [index] int
        );

        --创建默认参数信息
        INSERT OR IGNORE INTO [params]([id],[value]) VALUES(1,'076013000');
        INSERT OR IGNORE INTO [params]([id],[value]) VALUES(2,'076010000');
        INSERT OR IGNORE INTO [params]([id],[value]) VALUES(3,'006309000');
        INSERT OR IGNORE INTO [params]([id],[value]) VALUES(4,'006402000');
        
        --创建默认数据库信息
        INSERT OR IGNORE INTO [databases]([id],[name],[type],[ip],[port],[uid],[password],[db]) VALUES('D001','资源数据库',1,NULL,1433,NULL,NULL,NULL);
        INSERT OR IGNORE INTO [databases]([id],[name],[type],[ip],[port],[uid],[password],[db]) VALUES('D002','应用数据库',1,NULL,1433,NULL,NULL,NULL);
        INSERT OR IGNORE INTO [databases]([id],[name],[type],[ip],[port],[uid],[password],[db]) VALUES('D003','历史数据库',1,NULL,1433,NULL,NULL,NULL);
        
        --创建默认计划任务信息
        INSERT OR IGNORE INTO [tasks]([id],[name],[json],[start],[end],[next],[index]) VALUES('T001','能耗数据处理任务',NULL,NULL,NULL,NULL,1);
        INSERT OR IGNORE INTO [tasks]([id],[name],[json],[start],[end],[next],[index]) VALUES('T002','电池充放电处理任务',NULL,NULL,NULL,NULL,2);
        INSERT OR IGNORE INTO [tasks]([id],[name],[json],[start],[end],[next],[index]) VALUES('T003','信号测值统计任务',NULL,NULL,NULL,NULL,3);
        INSERT OR IGNORE INTO [tasks]([id],[name],[json],[start],[end],[next],[index]) VALUES('T004','开关电源带载率统计任务',NULL,NULL,NULL,NULL,4);
        INSERT OR IGNORE INTO [tasks]([id],[name],[json],[start],[end],[next],[index]) VALUES('T005','资管接口同步任务',NULL,NULL,NULL,NULL,5);
        INSERT OR IGNORE INTO [tasks]([id],[name],[json],[start],[end],[next],[index]) VALUES('T006','参数自动巡检任务',NULL,NULL,NULL,NULL,6);";

        public const string Registry_Get_Order = @"
        SELECT [id],[param],[time] FROM [orders];";

        public const string Registry_Save_Order = @"
        INSERT OR IGNORE INTO [orders]([id],[param]) VALUES(@id,@param);";

        public const string Registry_Delete_Order = @"
        DELETE FROM [orders] WHERE [id]=@id;";

        public const string Registry_Clean_Order = @"
        DELETE FROM [orders];";

        public const string Registry_Get_Param = @"
        SELECT [id],[value],[time] FROM [params];";

        public const string Registry_Save_Param = @"
        DELETE FROM [params] WHERE [id]=@id;
        INSERT INTO [params]([id],[value]) VALUES(@id,@value);";

        public const string Registry_Delete_Param = @"
        DELETE FROM [params] WHERE [id]=@id;";
        
        public const string Registry_Get_Database = @"
        SELECT [id],[name],[type],[ip],[port],[uid],[password],[db] FROM [databases];";

        public const string Registry_Save_Database = @"
        DELETE FROM [databases] WHERE [id]=@id;
        INSERT INTO [databases]([id],[name],[type],[ip],[port],[uid],[password],[db]) VALUES(@id,@name,@type,@ip,@port,@uid,@password,@db);";

        public const string Registry_Clean_Databases = @"
        DELETE FROM [databases];";
        
        public const string Registry_Get_Tasks = @"
        SELECT [id],[name],[json],[start],[end],[next],[index] FROM [tasks];";

        public const string Registry_Save_Task = @"
        DELETE FROM [tasks] WHERE [id]=@id;
        INSERT INTO [tasks]([id],[name],[json],[index]) VALUES(@id,@name,@json,@index);";

        public const string Registry_Clean_Tasks = @"
        DELETE FROM [tasks];";
    }
}
