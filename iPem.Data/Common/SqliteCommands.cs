using System;

namespace iPem.Data.Common {
    /// <summary>
    /// The SqliteCommands class is intended to store sqlite statements.
    /// </summary>
    public static class SqliteCommands {
        public const string Registry_Create_Tables = @"
        CREATE TABLE IF NOT EXISTS [sys_databases] (
            [id] int PRIMARY KEY NOT NULL,
            [type] int,
            [ip] varchar(128),
            [port] int,
            [uid] varchar(50),
            [password] varchar(50),
            [name] varchar(128)
        );

        CREATE TABLE IF NOT EXISTS [sys_tasks] (
            [id] varchar(50) PRIMARY KEY NOT NULL,
            [name] varchar(128),
            [plan] int,
            [last] datetime,
            [next] datetime,
            [order] int
        );

        CREATE TABLE IF NOT EXISTS [sys_plans] (
            [id] int PRIMARY KEY NOT NULL,
            [name] varchar(128),
            [interval] integer,
            [unit] int,
            [start] datetime,
            [end] datetime
        );";

        public const string Registry_Get_Database = @"
        SELECT [id],[type],[ip],[port],[uid],[password],[name] FROM [sys_databases] WHERE [id] = @id;";

        public const string Registry_Save_Database = @"
        DELETE FROM [sys_databases] WHERE [id]=@id;
        INSERT INTO [sys_databases]([id],[type],[ip],[port],[uid],[password],[name]) VALUES(@id,@type,@ip,@port,@uid,@password,@name);";

        public const string Registry_Clean_Databases = @"
        DELETE FROM [sys_databases];";
        
        public const string Registry_Get_Tasks = @"
        SELECT [id],[name],[plan],[last],[next],[order] FROM [sys_tasks];";

        public const string Registry_Save_Task = @"
        DELETE FROM [sys_tasks] WHERE [id]=@id;
        INSERT INTO [sys_tasks]([id],[name],[plan],[last],[next],[order]) VALUES(@id,@name,@plan,@last,@next,@order);";

        public const string Registry_Update_Task = @"
        UPDATE [sys_tasks] SET [last] = @last,[next] = @next WHERE [id] = @id;";

        public const string Registry_Clean_Tasks = @"
        DELETE FROM [sys_tasks];";

        public const string Registry_Get_Plans = @"
        SELECT [id],[name],[interval],[unit],[start],[end] FROM [sys_plans];";

        public const string Registry_Save_Plan = @"
        DELETE FROM [sys_plans] WHERE [id]=@id;
        INSERT INTO [sys_plans]([id],[name],[interval],[unit],[start],[end]) VALUES(@id,@name,@interval,@unit,@start,@end);";

        public const string Registry_Clean_Plans = @"
        DELETE FROM [sys_plans];";
    }
}
