using System;

namespace iPem.Configurator {
    /// <summary>
    /// The SqliteCommands class is intended to store sqlite statements.
    /// </summary>
    public static class SqliteCommands {
        public const string Registry_Create_Tables = @"
        --创建数据表
        CREATE TABLE IF NOT EXISTS [params] (
            [id] varchar(50) PRIMARY KEY NOT NULL,
            [name] varchar(200),
            [json] text,
            [time] datetime
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
            [last] datetime,
            [next] datetime,
            [index] int
        );

        --创建默认常规参数
        INSERT OR IGNORE INTO [params]([id],[name],[json],[time]) VALUES('P001','常规参数',NULL,NULL);
        
        --创建默认数据库信息
        INSERT OR IGNORE INTO [databases]([id],[name],[type],[ip],[port],[uid],[password],[db]) VALUES('D001','资源数据库',1,NULL,1433,NULL,NULL,NULL);
        INSERT OR IGNORE INTO [databases]([id],[name],[type],[ip],[port],[uid],[password],[db]) VALUES('D002','应用数据库',1,NULL,1433,NULL,NULL,NULL);
        INSERT OR IGNORE INTO [databases]([id],[name],[type],[ip],[port],[uid],[password],[db]) VALUES('D003','历史数据库',1,NULL,1433,NULL,NULL,NULL);
        
        --创建默认计划任务信息
        INSERT OR IGNORE INTO [tasks]([id],[name],[json],[last],[next],[index]) VALUES('T001','能耗处理任务',NULL,NULL,NULL,1);
        INSERT OR IGNORE INTO [tasks]([id],[name],[json],[last],[next],[index]) VALUES('T002','后备时长处理任务',NULL,NULL,NULL,2);
        INSERT OR IGNORE INTO [tasks]([id],[name],[json],[last],[next],[index]) VALUES('T003','带载合格率处理任务',NULL,NULL,NULL,3);
        INSERT OR IGNORE INTO [tasks]([id],[name],[json],[last],[next],[index]) VALUES('T004','接口数据处理任务',NULL,NULL,NULL,4);
        INSERT OR IGNORE INTO [tasks]([id],[name],[json],[last],[next],[index]) VALUES('T005','电池放电处理任务',NULL,NULL,NULL,5);
        INSERT OR IGNORE INTO [tasks]([id],[name],[json],[last],[next],[index]) VALUES('T006','测值统计处理任务',NULL,NULL,NULL,6);";

        public const string Registry_Get_Database = @"
        SELECT [id],[name],[type],[ip],[port],[uid],[password],[db] FROM [databases];";

        public const string Registry_Save_Database = @"
        DELETE FROM [databases] WHERE [id]=@id;
        INSERT INTO [databases]([id],[name],[type],[ip],[port],[uid],[password],[db]) VALUES(@id,@name,@type,@ip,@port,@uid,@password,@db);";

        public const string Registry_Clean_Databases = @"
        DELETE FROM [databases];";
        
        public const string Registry_Get_Tasks = @"
        SELECT [id],[name],[json],[last],[next],[index] FROM [tasks];";

        public const string Registry_Save_Task = @"
        DELETE FROM [tasks] WHERE [id]=@id;
        INSERT INTO [tasks]([id],[name],[json],[last],[next],[index]) VALUES(@id,@name,@json,@last,@next,@index);";

        public const string Registry_Clean_Tasks = @"
        DELETE FROM [tasks];";

        public const string Registry_Get_Params = @"
        SELECT [id],[name],[json],[time] FROM [params];";

        public const string Registry_Save_Params = @"
        DELETE FROM [params] WHERE [id]=@id;
        INSERT INTO [params]([id],[name],[json],[time]) VALUES(@id,@name,@json,@time);";

        public const string Registry_Clean_Params = @"
        DELETE FROM [params];";
    }
}
