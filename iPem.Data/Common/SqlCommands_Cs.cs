using System;

namespace iPem.Data.Common {
    public static class SqlCommands_Cs {
        //ActAlm Repository
        public const string Sql_ActAlm_Repository_GetEntities1 = @"SELECT * FROM [dbo].[A_Alarm] WHERE [AlarmTime] BETWEEN @Start AND @End ORDER BY [AlarmTime] DESC;";
        public const string Sql_ActAlm_Repository_GetEntities2 = @"SELECT * FROM [dbo].[A_Alarm] ORDER BY [AlarmTime] DESC;";
        //HisAlm Repository
        public const string Sql_HisAlm_Repository_GetEntities = @"
        DECLARE @tpDate DATETIME, 
                @tbName NVARCHAR(255),
                @tableCnt INT = 0,
                @SQL NVARCHAR(MAX) = N'';

        SET @tpDate = @Start;
        WHILE(DATEDIFF(MM,@tpDate,@End)>=0)
        BEGIN
            SET @tbName = N'[dbo].[A_HAlarm'+CONVERT(VARCHAR(6),@tpDate,112)+ N']';
            IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(@tbName) AND type in (N'U'))
            BEGIN
                IF(@tableCnt>0)
                BEGIN
                SET @SQL += N' 
                UNION ALL 
                ';
                END
        			
                SET @SQL += N'SELECT * FROM ' + @tbName + N' WHERE [AlarmTime] BETWEEN ''' + CONVERT(NVARCHAR,@Start,120) + N''' AND ''' + CONVERT(NVARCHAR,@End,120) + N'''';
                SET @tableCnt += 1;
            END
            SET @tpDate = DATEADD(MM,1,@tpDate);
        END

        IF(@tableCnt>0)
        BEGIN
	        SET @SQL = N';WITH HisAlm AS
		        (
			        ' + @SQL + N'
		        )
		        SELECT * FROM HisAlm;'
        END

        EXECUTE sp_executesql @SQL;";
        //HisBat Repository
        public const string Sql_HisBat_Repository_GetEntities = @"
        DECLARE @tpDate DATETIME, 
                @tbName NVARCHAR(255),
                @tableCnt INT = 0,
                @SQL NVARCHAR(MAX) = N'';

        SET @tpDate = @Start;
        WHILE(DATEDIFF(MM,@tpDate,@End)>=0)
        BEGIN
            SET @tbName = N'[dbo].[V_Bat'+CONVERT(VARCHAR(6),@tpDate,112)+ N']';
            IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(@tbName) AND type in (N'U'))
            BEGIN
                IF(@tableCnt>0)
                BEGIN
                SET @SQL += N' 
                UNION ALL 
                ';
                END
        			
                SET @SQL += N'SELECT * FROM ' + @tbName + N' WHERE [ValueTime] BETWEEN ''' + CONVERT(NVARCHAR,@Start,120) + N''' AND ''' + CONVERT(NVARCHAR,@End,120) + N'''';
                SET @tableCnt += 1;
            END
            SET @tpDate = DATEADD(MM,1,@tpDate);
        END

        IF(@tableCnt>0)
        BEGIN
	        SET @SQL = N';WITH HisBat AS
		        (
			        ' + @SQL + N'
		        )
		        SELECT * FROM HisBat ORDER BY [ValueTime];'
        END

        EXECUTE sp_executesql @SQL;";
        //HisBatTime Repository
        public const string Sql_HisBatTime_Repository_SaveEntities = @"
        IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'TT_BatTime{0}') AND type in (N'U'))
        BEGIN
            CREATE TABLE [dbo].[TT_BatTime{0}](
	            [AreaId] [varchar](100) NOT NULL,
	            [StationId] [varchar](100) NOT NULL,
	            [RoomId] [varchar](100) NOT NULL,
	            [DeviceId] [varchar](100) NOT NULL,
	            [StartTime] [datetime] NOT NULL,
	            [EndTime] [datetime] NOT NULL,
	            [StartValue] [float] NOT NULL,
	            [EndValue] [float] NOT NULL,
	            [CreatedTime] [datetime] NOT NULL,
             CONSTRAINT [PK_TT_BatTime{0}] PRIMARY KEY CLUSTERED 
            (
	            [AreaId] ASC,
	            [StationId] ASC,
	            [RoomId] ASC,
	            [DeviceId] ASC,
	            [StartTime] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
            ) ON [PRIMARY]
        END
        UPDATE [dbo].[TT_BatTime{0}] SET [EndTime] = @EndTime,[StartValue] = @StartValue,[EndValue] = @EndValue,[CreatedTime] = @CreatedTime WHERE [AreaId] = @AreaId AND [StationId] = @StationId AND [RoomId] = @RoomId AND [DeviceId] = @DeviceId AND [StartTime] = @StartTime;
        IF(@@ROWCOUNT = 0)
        BEGIN
            INSERT INTO [dbo].[TT_BatTime{0}]([AreaId],[StationId],[RoomId],[DeviceId],[StartTime],[EndTime],[StartValue],[EndValue],[CreatedTime]) VALUES(@AreaId,@StationId,@RoomId,@DeviceId,@StartTime,@EndTime,@StartValue,@EndValue,@CreatedTime);
        END";
        public const string Sql_HisBatTime_Repository_DeleteEntities = @"
        DECLARE @tpDate DATETIME, 
                @tbName NVARCHAR(255),
                @SQL NVARCHAR(MAX) = N'';

        SET @tpDate = @Start;
        WHILE(DATEDIFF(MM,@tpDate,@End)>=0)
        BEGIN
            SET @tbName = N'[dbo].[TT_BatTime'+CONVERT(VARCHAR(6),@tpDate,112)+ N']';
            IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(@tbName) AND type in (N'U'))
            BEGIN
                SET @SQL += N'
		        DELETE FROM ' + @tbName + N' WHERE [StartTime] BETWEEN ''' + CONVERT(NVARCHAR,@Start,120) + N''' AND ''' + CONVERT(NVARCHAR,@End,120) + N''';';
            END
            SET @tpDate = DATEADD(MM,1,@tpDate);
        END
        EXECUTE sp_executesql @SQL;";
        //HisElec Repository
        public const string Sql_HisElec_Repository_SaveEntities = @"
        IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'TT_Elec{0}') AND type in (N'U'))
        BEGIN
        CREATE TABLE [dbo].[TT_Elec{0}](
	        [Id] [varchar](100) NOT NULL,
	        [Type] [int] NOT NULL,
	        [FormulaType] [int] NOT NULL,
	        [Period] [datetime] NOT NULL,
	        [Value] [float] NOT NULL,
         CONSTRAINT [PK_TT_Elec{0}] PRIMARY KEY CLUSTERED 
        (
	        [Id] ASC,
	        [Type] ASC,
	        [FormulaType] ASC,
	        [Period] ASC
        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
        ) ON [PRIMARY]
        END
        UPDATE [dbo].[TT_Elec{0}] SET [Value] = @Value WHERE [Id] = @Id AND [Type] = @Type AND [FormulaType] = @FormulaType AND [Period] = @Period;
        IF(@@ROWCOUNT = 0)
        BEGIN
	        INSERT INTO [dbo].[TT_Elec{0}]([Id],[Type],[FormulaType],[Period],[Value]) VALUES(@Id,@Type,@FormulaType,@Period,@Value);
        END";
        public const string Sql_HisElec_Repository_DeleteEntities = @"
        DECLARE @tpDate DATETIME, 
                @tbName NVARCHAR(255),
                @SQL NVARCHAR(MAX) = N'';

        SET @tpDate = @Start;
        WHILE(DATEDIFF(MM,@tpDate,@End)>=0)
        BEGIN
            SET @tbName = N'[dbo].[TT_Elec'+CONVERT(VARCHAR(6),@tpDate,112)+ N']';
            IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(@tbName) AND type in (N'U'))
            BEGIN
                SET @SQL += N'
		        DELETE FROM ' + @tbName + N' WHERE [Period] BETWEEN ''' + CONVERT(NVARCHAR,@Start,120) + N''' AND ''' + CONVERT(NVARCHAR,@End,120) + N''';';
            END
            SET @tpDate = DATEADD(MM,1,@tpDate);
        END
        EXECUTE sp_executesql @SQL;";
        //HisLoadRate Repository
        public const string Sql_HisLoadRate_Repository_SaveEntities = @"
        IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'TT_LoadRate{0}') AND type in (N'U'))
        BEGIN
        CREATE TABLE [dbo].[TT_LoadRate{0}](
	        [AreaId] [varchar](100) NOT NULL,
	        [StationId] [varchar](100) NOT NULL,
	        [RoomId] [varchar](100) NOT NULL,
	        [DeviceId] [varchar](100) NOT NULL,
	        [StartTime] [datetime] NOT NULL,
	        [EndTime] [datetime] NOT NULL,
	        [Value] [float] NOT NULL,
	        [CreatedTime] [datetime] NOT NULL,
         CONSTRAINT [PK_TT_LoadRate{0}] PRIMARY KEY CLUSTERED 
        (
	        [AreaId] ASC,
	        [StationId] ASC,
	        [RoomId] ASC,
	        [DeviceId] ASC,
	        [StartTime] ASC
        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
        ) ON [PRIMARY]
        END
        UPDATE [dbo].[TT_LoadRate{0}] SET [EndTime] = @EndTime,[Value] = @Value,[CreatedTime] = @CreatedTime WHERE [AreaId] = @AreaId AND [StationId] = @StationId AND [RoomId] = @RoomId AND [DeviceId] = @DeviceId AND [StartTime] = @StartTime;
        IF(@@ROWCOUNT = 0)
        BEGIN
            INSERT INTO [dbo].[TT_LoadRate{0}]([AreaId],[StationId],[RoomId],[DeviceId],[StartTime],[EndTime],[Value],[CreatedTime]) VALUES(@AreaId,@StationId,@RoomId,@DeviceId,@StartTime,@EndTime,@Value,@CreatedTime);
        END";
        public const string Sql_HisLoadRate_Repository_DeleteEntities = @"
        DECLARE @tpDate DATETIME, 
                @tbName NVARCHAR(255),
                @SQL NVARCHAR(MAX) = N'';

        SET @tpDate = @Start;
        WHILE(DATEDIFF(MM,@tpDate,@End)>=0)
        BEGIN
            SET @tbName = N'[dbo].[TT_LoadRate'+CONVERT(VARCHAR(6),@tpDate,112)+ N']';
            IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(@tbName) AND type in (N'U'))
            BEGIN
                SET @SQL += N'
		        DELETE FROM ' + @tbName + N' WHERE [StartTime] BETWEEN ''' + CONVERT(NVARCHAR,@Start,120) + N''' AND ''' + CONVERT(NVARCHAR,@End,120) + N''';';
            END
            SET @tpDate = DATEADD(MM,1,@tpDate);
        END
        EXECUTE sp_executesql @SQL;";
        //HisStatic Repository
        public const string Sql_HisStatic_Repository_GetEntities = @"
        DECLARE @tpDate DATETIME, 
                @tbName NVARCHAR(255),
                @tableCnt INT = 0,
                @SQL NVARCHAR(MAX) = N'';

        SET @tpDate = @Start;
        WHILE(DATEDIFF(MM,@tpDate,@End)>=0)
        BEGIN
            SET @tbName = N'[dbo].[V_Static'+CONVERT(VARCHAR(6),@tpDate,112)+ N']';
            IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(@tbName) AND type in (N'U'))
            BEGIN
                IF(@tableCnt>0)
                BEGIN
                SET @SQL += N' 
                UNION ALL 
                ';
                END
        			
                SET @SQL += N'SELECT * FROM ' + @tbName + N' WHERE [BeginTime] BETWEEN ''' + CONVERT(NVARCHAR,@Start,120) + N''' AND ''' + CONVERT(NVARCHAR,@End,120) + N'''';
                SET @tableCnt += 1;
            END
            SET @tpDate = DATEADD(MM,1,@tpDate);
        END

        IF(@tableCnt>0)
        BEGIN
	        SET @SQL = N';WITH HisStatic AS
		        (
			        ' + @SQL + N'
		        )
		        SELECT * FROM HisStatic ORDER BY [BeginTime];'
        END

        EXECUTE sp_executesql @SQL;";
        //HisValue Repository
        public const string Sql_HisValue_Repository_GetEntities = @"
        DECLARE @tpDate DATETIME, 
                @tbName NVARCHAR(255),
                @tableCnt INT = 0,
                @SQL NVARCHAR(MAX) = N'';

        SET @tpDate = @Start;
        WHILE(DATEDIFF(MM,@tpDate,@End)>=0)
        BEGIN
            SET @tbName = N'[dbo].[V_HMeasure'+CONVERT(VARCHAR(6),@tpDate,112)+ N']';
            IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(@tbName) AND type in (N'U'))
            BEGIN
                IF(@tableCnt>0)
                BEGIN
                SET @SQL += N' 
                UNION ALL 
                ';
                END
        			
                SET @SQL += N'SELECT * FROM ' + @tbName + N' WHERE [UpdateTime] BETWEEN ''' + CONVERT(NVARCHAR,@Start,120) + N''' AND ''' + CONVERT(NVARCHAR,@End,120) + N'''';
                SET @tableCnt += 1;
            END
            SET @tpDate = DATEADD(MM,1,@tpDate);
        END

        IF(@tableCnt>0)
        BEGIN
	        SET @SQL = N';WITH HisValue AS
		        (
			        ' + @SQL + N'
		        )
		        SELECT * FROM HisValue ORDER BY [UpdateTime];'
        END

        EXECUTE sp_executesql @SQL;";
        public const string Sql_HisValue_Repository_GetEntitiesByPoint = @"
        DECLARE @tpDate DATETIME, 
                @tbName NVARCHAR(255),
                @tableCnt INT = 0,
                @SQL NVARCHAR(MAX) = N'';

        SET @tpDate = @Start;
        WHILE(DATEDIFF(MM,@tpDate,@End)>=0)
        BEGIN
            SET @tbName = N'[dbo].[V_HMeasure'+CONVERT(VARCHAR(6),@tpDate,112)+ N']';
            IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(@tbName) AND type in (N'U'))
            BEGIN
                IF(@tableCnt>0)
                BEGIN
                SET @SQL += N' 
                UNION ALL 
                ';
                END
        			
                SET @SQL += N'SELECT * FROM ' + @tbName + N' WHERE [DeviceId] = ' + @DeviceId + N' AND [PointId] = ' + @PointId + N' AND [UpdateTime] BETWEEN ''' + CONVERT(NVARCHAR,@Start,120) + N''' AND ''' + CONVERT(NVARCHAR,@End,120) + N'''';
                SET @tableCnt += 1;
            END
            SET @tpDate = DATEADD(MM,1,@tpDate);
        END

        IF(@tableCnt>0)
        BEGIN
	        SET @SQL = N';WITH HisValue AS
		        (
			        ' + @SQL + N'
		        )
		        SELECT * FROM HisValue ORDER BY [UpdateTime];'
        END

        EXECUTE sp_executesql @SQL;";
        public const string Sql_HisValue_Repository_GetProcedure = @"
        DECLARE @tpDate DATETIME, 
                @tbName NVARCHAR(255),
                @tableCnt INT = 0,
                @SQL NVARCHAR(MAX) = N'';

        SET @tpDate = @Start;
        WHILE(DATEDIFF(MM,@tpDate,@End)>=0)
        BEGIN
            SET @tbName = N'[dbo].[V_HMeasure'+CONVERT(VARCHAR(6),@tpDate,112)+ N']';
            IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(@tbName) AND type in (N'U'))
            BEGIN
                IF(@tableCnt>0)
                BEGIN
                SET @SQL += N' 
                UNION ALL 
                ';
                END
        			
                SET @SQL += N'SELECT * FROM ' + @tbName + N' WHERE [DeviceId] = ' + @DeviceId + N' AND [PointId] = ' + @PointId + N' AND [UpdateTime] BETWEEN ''' + CONVERT(NVARCHAR,@Start,120) + N''' AND ''' + CONVERT(NVARCHAR,@End,120) + N'''';
                SET @tableCnt += 1;
            END
            SET @tpDate = DATEADD(MM,1,@tpDate);
        END

        IF(@tableCnt>0)
        BEGIN
	        SET @SQL = N';WITH HisValue AS
		        (
			        ' + @SQL + N'
		        )
		        SELECT MAX([Value]) AS [MaxValue], MIN([Value]) AS [MinValue] FROM HisValue;'
        END

        EXECUTE sp_executesql @SQL;";
    }
}
