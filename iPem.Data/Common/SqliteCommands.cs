using System;

namespace iPem.Data.Common {
    /// <summary>
    /// The SqliteCommands class is intended to store sqlite statements.
    /// </summary>
    public static class SqliteCommands {
        public const string Registry_Get_Databases = @"
        SELECT [id],[name],[type],[ip],[port],[uid],[password],[db] FROM [databases];";

        public const string Registry_Get_Database = @"
        SELECT [id],[name],[type],[ip],[port],[uid],[password],[db] FROM [databases] where [id] = @id;";

        public const string Registry_Get_Tasks = @"
        SELECT [id],[name],[json],[last],[next],[index] FROM [tasks];";

        public const string Registry_Get_Task = @"
        SELECT [id],[name],[json],[last],[next],[index] FROM [tasks] where [id] = @id;";
        
        public const string Registry_Update_Task = @"
        UPDATE [tasks] SET [last] = @last,[next] = @next WHERE [id] = @id;";

        public const string Registry_Get_Params = @"
        SELECT [id],[name],[json],[time] FROM [params];";

        public const string Registry_Get_Param = @"
        SELECT [id],[name],[json],[time] FROM [params] where [id] = @id;";
    }
}
