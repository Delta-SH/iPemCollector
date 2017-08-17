using System;

namespace iPem.Data.Common {
    public static class SqlCommands_Cs {
        /// <summary>
        /// Alarm Repository
        /// </summary>
        public const string Sql_Alarm_Repository_Start = @"
        INSERT INTO [dbo].[A_AAlarm]([Id],[AreaId],[StationId],[RoomId],[FsuId],[DeviceId],[PointId],[SerialNo],[NMAlarmId],[AlarmTime],[AlarmLevel],[AlarmValue],[AlarmDesc],[AlarmRemark],[ReservationId],[PrimaryId],[RelatedId],[FilterId],[ReversalId],[ReversalCount],[Masked],[CreatedTime]) VALUES(@Id,@AreaId,@StationId,@RoomId,@FsuId,@DeviceId,@PointId,@SerialNo,@NMAlarmId,@AlarmTime,@AlarmLevel,@AlarmValue,@AlarmDesc,@AlarmRemark,@ReservationId,@PrimaryId,@RelatedId,@FilterId,@ReversalId,@ReversalCount,@Masked,GETDATE());
        INSERT INTO [dbo].[A_IAlarm]([AreaId],[StationId],[RoomId],[FsuId],[DeviceId],[PointId],[SerialNo],[NMAlarmId],[AlarmTime],[AlarmLevel],[AlarmFlag],[AlarmDesc],[AlarmValue],[AlarmRemark],[ReservationId],[ReservationName],[ReservationStart],[ReservationEnd],[PrimaryId],[RelatedId],[FilterId],[ReversalId],[Masked],[CreatedTime]) VALUES(@AreaId,@StationId,@RoomId,@FsuId,@DeviceId,@PointId,@SerialNo,@NMAlarmId,@AlarmTime,@AlarmLevel,@AlarmFlag,@AlarmDesc,@AlarmValue,@AlarmRemark,@ReservationId,@ReservationName,@ReservationStart,@ReservationEnd,@PrimaryId,@RelatedId,@FilterId,@ReversalId,@Masked,GETDATE());
        DELETE FROM [dbo].[A_TAlarm] WHERE [FsuId] = @FsuCode AND [SerialNo] = @SerialNo AND [AlarmFlag] = @AlarmFlag;";
        public const string Sql_Alarm_Repository_End = @"
        IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'A_HAlarm{0}') AND type in (N'U'))
        BEGIN
	        CREATE TABLE [dbo].[A_HAlarm{0}](
	            [Id] [varchar](200) NOT NULL,
	            [AreaId] [varchar](100) NOT NULL,
	            [StationId] [varchar](100) NOT NULL,
	            [RoomId] [varchar](100) NOT NULL,
	            [FsuId] [varchar](100) NOT NULL,
	            [DeviceId] [varchar](100) NOT NULL,
	            [PointId] [varchar](100) NOT NULL,
	            [SerialNo] [varchar](100) NOT NULL,
	            [NMAlarmId] [varchar](100) NOT NULL,
	            [StartTime] [datetime] NOT NULL,
	            [EndTime] [datetime] NOT NULL,
	            [AlarmLevel] [int] NOT NULL,
	            [StartValue] [float] NOT NULL,
	            [EndValue] [float] NOT NULL,
	            [AlarmDesc] [varchar](120) NULL,
	            [AlarmRemark] [varchar](100) NULL,
	            [Confirmed] [int] NULL,
	            [Confirmer] [varchar](100) NULL,
	            [ConfirmedTime] [datetime] NULL,
	            [ReservationId] [varchar](100) NULL,
	            [PrimaryId] [varchar](200) NULL,
	            [RelatedId] [varchar](200) NULL,
	            [FilterId] [varchar](200) NULL,
	            [ReversalId] [varchar](200) NULL,
	            [ReversalCount] [int] NOT NULL,
	            [Masked] [bit] NOT NULL,
	            [CreatedTime] [datetime] NOT NULL,
             CONSTRAINT [PK_A_HAlarm_{0}] PRIMARY KEY CLUSTERED 
            (
	            [Id] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
            ) ON [PRIMARY]
        END
        INSERT INTO [dbo].[A_HAlarm{0}]([Id],[AreaId],[StationId],[RoomId],[FsuId],[DeviceId],[PointId],[SerialNo],[NMAlarmId],[StartTime],[EndTime],[AlarmLevel],[StartValue],[EndValue],[AlarmDesc],[AlarmRemark],[Confirmed],[Confirmer],[ConfirmedTime],[ReservationId],[PrimaryId],[RelatedId],[FilterId],[ReversalId],[ReversalCount],[Masked],[CreatedTime]) VALUES(@Id,@AreaId,@StationId,@RoomId,@FsuId,@DeviceId,@PointId,@SerialNo,@NMAlarmId,@StartTime,@EndTime,@AlarmLevel,@StartValue,@EndValue,@AlarmDesc,@AlarmRemark,@Confirmed,@Confirmer,@ConfirmedTime,@ReservationId,@PrimaryId,@RelatedId,@FilterId,@ReversalId,@ReversalCount,@Masked,GETDATE());
        INSERT INTO [dbo].[A_IAlarm]([AreaId],[StationId],[RoomId],[FsuId],[DeviceId],[PointId],[SerialNo],[NMAlarmId],[AlarmTime],[AlarmLevel],[AlarmFlag],[AlarmDesc],[AlarmValue],[AlarmRemark],[Confirmed],[Confirmer],[ConfirmedTime],[ReservationId],[ReservationName],[ReservationStart],[ReservationEnd],[PrimaryId],[RelatedId],[FilterId],[ReversalId],[Masked],[CreatedTime]) VALUES(@AreaId,@StationId,@RoomId,@FsuId,@DeviceId,@PointId,@SerialNo,@NMAlarmId,@EndTime,@AlarmLevel,@AlarmFlag,@AlarmDesc,@EndValue,@AlarmRemark,@Confirmed,@Confirmer,@ConfirmedTime,@ReservationId,@ReservationName,@ReservationStart,@ReservationEnd,@PrimaryId,@RelatedId,@FilterId,@ReversalId,@Masked,GETDATE());
        DELETE FROM [dbo].[A_AAlarm] WHERE [Id] = @Id;
        DELETE FROM [dbo].[A_TAlarm] WHERE [FsuId] = @FsuCode AND [SerialNo] = @SerialNo AND [AlarmFlag] = @AlarmFlag;";
        public const string Sql_Alarm_Repository_Delete = @"DELETE FROM [dbo].[A_TAlarm] WHERE [FsuId] = @FsuId AND [SerialNo] = @SerialNo AND [AlarmFlag] = @AlarmFlag;";
        /// <summary>
        /// A_AAlarm Repository
        /// </summary>
        public const string Sql_A_AAlarm_Repository_GetEntity = @"SELECT * FROM [dbo].[A_AAlarm] WHERE [Id] = @Id;";
        public const string Sql_A_AAlarm_Repository_GetEntityInPoint = @"SELECT * FROM [dbo].[A_AAlarm] WHERE [DeviceId] = @DeviceId AND [PointId]=@PointId;";
        public const string Sql_A_AAlarm_Repository_GetEntitiesInDevice = @"SELECT * FROM [dbo].[A_AAlarm] WHERE [DeviceId] = @DeviceId ORDER BY [AlarmTime];";
        public const string Sql_A_AAlarm_Repository_GetEntitiesInSpan = @"SELECT * FROM [dbo].[A_AAlarm] WHERE [AlarmTime] BETWEEN @Start AND @End ORDER BY [AlarmTime];";
        public const string Sql_A_AAlarm_Repository_GetEntities = @"SELECT * FROM [dbo].[A_AAlarm] ORDER BY [AlarmTime];";
        /// <summary>
        /// A_HAlarm Repository
        /// </summary>
        public const string Sql_A_HAlarm_Repository_GetEntities = @"
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
        			
                SET @SQL += N'SELECT * FROM ' + @tbName + N' WHERE [StartTime] BETWEEN ''' + CONVERT(NVARCHAR,@Start,120) + N''' AND ''' + CONVERT(NVARCHAR,@End,120) + N'''';
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
		    SELECT * FROM HisAlm ORDER BY [StartTime] DESC;'
        END

        EXECUTE sp_executesql @SQL;";

        /// <summary>
        /// A_TAlarm Repository
        /// </summary>
        public const string Sql_A_TAlarm_Repository_GetEntities1 = @"SELECT * FROM [dbo].[A_TAlarm] ORDER BY [AlarmTime];";        
        public const string Sql_A_TAlarm_Repository_GetEntities2 = @"SELECT * FROM [dbo].[A_TAlarm] WHERE [AlarmTime] BETWEEN @Start AND @End ORDER BY [AlarmTime];";
        public const string Sql_A_TAlarm_Repository_SaveEntities = @"
        IF NOT EXISTS( SELECT 1 FROM [dbo].[A_TAlarm] WHERE [FsuId]=@FsuId AND [SerialNo]=@SerialNo AND [AlarmFlag]=@AlarmFlag)
        BEGIN
	        INSERT INTO [dbo].[A_TAlarm]([FsuId],[DeviceId],[PointId],[SignalId],[SignalNumber],[SerialNo],[NMAlarmId],[AlarmTime],[AlarmLevel],[AlarmFlag],[AlarmDesc],[AlarmValue],[AlarmRemark]) 
	        VALUES(@FsuId,@DeviceId,@PointId,@SignalId,@SignalNumber,@SerialNo,@NMAlarmId,@AlarmTime,,@AlarmLevel,@AlarmFlag,@AlarmDesc,@AlarmValue,@AlarmRemark);
        END";

        /// <summary>
        /// A_FAlarm Repository
        /// </summary>
        public const string Sql_A_FAlarm_Repository_GetEntities1 = @"SELECT * FROM [dbo].[A_FAlarm] ORDER BY [AlarmTime];";
        public const string Sql_A_FAlarm_Repository_GetEntities2 = @"SELECT * FROM [dbo].[A_FAlarm] WHERE [AlarmTime] BETWEEN @Start AND @End ORDER BY [AlarmTime];";
        public const string Sql_A_FAlarm_Repository_Delete1 = @"DELETE FROM [dbo].[A_FAlarm] WHERE [FsuId]=@FsuId AND [SerialNo]=@SerialNo AND [AlarmFlag]=@AlarmFlag;";
        public const string Sql_A_FAlarm_Repository_Delete2 = @"DELETE FROM [dbo].[A_FAlarm] WHERE [AlarmTime] BETWEEN @Start AND @End;";

        /// <summary>
        /// H_FsuEvent Repository
        /// </summary>
        public const string Sql_H_FsuEvent_Repository_GetEntities1 = @"SELECT * FROM [dbo].[H_FsuEvent] WHERE [EventTime] BETWEEN @Start AND @End ORDER BY [EventTime] DESC;";
        public const string Sql_H_FsuEvent_Repository_GetEntities2 = @"SELECT * FROM [dbo].[H_FsuEvent] WHERE [EventTime] BETWEEN @Start AND @End AND [EventType] = @EventType ORDER BY [EventTime] DESC;";

        /// <summary>
        /// H_IArea Repository
        /// </summary>
        public const string Sql_H_IArea_Repository_SaveEntities = @"
        IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[H_IArea{0}]') AND type in (N'U'))
        BEGIN
            CREATE TABLE [dbo].[H_IArea{0}](
	            [Id] [varchar](100) NOT NULL,
	            [Name] [varchar](200) NOT NULL,
	            [TypeId] [varchar](100) NULL,
	            [TypeName] [varchar](200) NULL,
	            [ParentId] [varchar](100) NOT NULL,
             CONSTRAINT [PK_H_IArea{0}] PRIMARY KEY CLUSTERED 
            (
	            [Id] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
            ) ON [PRIMARY]
        END
        INSERT INTO [dbo].[H_IArea{0}]([Id],[Name],[TypeId],[TypeName],[ParentId]) VALUES(@Id,@Name,@TypeId,@TypeName,@ParentId);";
        public const string Sql_H_IArea_Repository_DeleteEntities = @"
        IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[H_IArea{0}]') AND type in (N'U'))
        DROP TABLE [dbo].[H_IArea{0}];";

        /// <summary>
        /// H_IDevice Repository
        /// </summary>
        public const string Sql_H_IDevice_Repository_SaveEntities = @"
        IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[H_IDevice{0}]') AND type in (N'U'))
        BEGIN
            CREATE TABLE [dbo].[H_IDevice{0}](
		        [Id] [varchar](100) NOT NULL,
		        [Name] [varchar](200) NOT NULL,
		        [TypeId] [varchar](100) NULL,
		        [TypeName] [varchar](200) NULL,
		        [StationId] [varchar](100) NOT NULL,
	         CONSTRAINT [PK_H_IDevice{0}] PRIMARY KEY CLUSTERED 
	        (
		        [Id] ASC
	        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	        ) ON [PRIMARY]
        END
        INSERT INTO [dbo].[H_IDevice{0}]([Id],[Name],[TypeId],[TypeName],[StationId]) VALUES(@Id,@Name,@TypeId,@TypeName,@StationId);";
        public const string Sql_H_IDevice_Repository_DeleteEntities = @"
        IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[H_IDevice{0}]') AND type in (N'U'))
        DROP TABLE [dbo].[H_IDevice{0}];";

        /// <summary>
        /// H_IStation Repository
        /// </summary>
        public const string Sql_H_IStation_Repository_SaveEntities = @"
        IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[H_IStation{0}]') AND type in (N'U'))
        BEGIN
            CREATE TABLE [dbo].[H_IStation{0}](
		        [Id] [varchar](100) NOT NULL,
		        [Name] [varchar](200) NOT NULL,
		        [TypeId] [varchar](100) NULL,
		        [TypeName] [varchar](200) NULL,
		        [AreaId] [varchar](100) NOT NULL,
	         CONSTRAINT [PK_H_IStation{0}] PRIMARY KEY CLUSTERED 
	        (
		        [Id] ASC
	        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	        ) ON [PRIMARY]
        END
        INSERT INTO [dbo].[H_IStation{0}]([Id],[Name],[TypeId],[TypeName],[AreaId]) VALUES(@Id,@Name,@TypeId,@TypeName,@AreaId);";
        public const string Sql_H_IStation_Repository_DeleteEntities = @"
        IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[H_IStation{0}]') AND type in (N'U'))
        DROP TABLE [dbo].[H_IStation{0}];";

        /// <summary>
        /// V_AMeasure Repository
        /// </summary>
        public const string Sql_V_AMeasure_Repository_GetEntity = @"SELECT * FROM [dbo].[V_AMeasure] WHERE [DeviceId]=@DeviceId AND [PointId]=@PointId;";
        public const string Sql_V_AMeasure_Repository_GetEntities = @"SELECT * FROM [dbo].[V_AMeasure];";
        public const string Sql_V_AMeasure_Repository_GetEntitiesInDevice = @"SELECT * FROM [dbo].[V_AMeasure] WHERE [DeviceId]=@DeviceId;";

        /// <summary>
        /// V_Bat Repository
        /// </summary>
        public const string Sql_V_Bat_Repository_SaveEntities = @"
        IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'V_Bat{0}') AND type in (N'U'))
        BEGIN
            CREATE TABLE [dbo].[V_Bat{0}](
	            [AreaId] [varchar](100) NOT NULL,
	            [StationId] [varchar](100) NOT NULL,
	            [RoomId] [varchar](100) NOT NULL,
	            [DeviceId] [varchar](100) NOT NULL,
	            [PointId] [varchar](100) NOT NULL,
	            [PackId] [int] NOT NULL,
	            [StartTime] [datetime] NOT NULL,
	            [Value] [float] NOT NULL,
	            [ValueTime] [datetime] NOT NULL
            ) ON [PRIMARY]
        END
        INSERT INTO [dbo].[V_Bat{0}]([AreaId],[StationId],[RoomId],[DeviceId],[PointId],[PackId],[StartTime],[Value],[ValueTime]) VALUES(@AreaId,@StationId,@RoomId,@DeviceId,@PointId,@PackId,@StartTime,@Value,@ValueTime);";
        public const string Sql_V_Bat_Repository_DeleteEntities = @"
        DECLARE @tpDate DATETIME, 
                @tbName NVARCHAR(255),
                @SQL NVARCHAR(MAX) = N'';

        SET @tpDate = @Start;
        WHILE(DATEDIFF(MM,@tpDate,@End)>=0)
        BEGIN
            SET @tbName = N'[dbo].[V_Bat'+CONVERT(VARCHAR(6),@tpDate,112)+ N']';
            IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(@tbName) AND type in (N'U'))
            BEGIN
                SET @SQL += N'
		        DELETE FROM ' + @tbName + N' WHERE [DeviceId]=''' + @DeviceId + N''' AND [PointId]=''' + @PointId + N''' AND [PackId]='+ CAST(@PackId AS NVARCHAR) + N' AND [ValueTime] BETWEEN ''' + CONVERT(NVARCHAR,@Start,120) + N''' AND ''' + CONVERT(NVARCHAR,@End,120) + N''';';
            END
            SET @tpDate = DATEADD(MM,1,@tpDate);
        END
        EXECUTE sp_executesql @SQL;";

        /// <summary>
        /// V_BatTime Repository
        /// </summary>
        public const string Sql_V_BatTime_Repository_SaveEntities = @"
        IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'V_BatTime{0}') AND type in (N'U'))
        BEGIN
            CREATE TABLE [dbo].[V_BatTime{0}](
	            [AreaId] [varchar](100) NOT NULL,
	            [StationId] [varchar](100) NOT NULL,
	            [RoomId] [varchar](100) NOT NULL,
	            [DeviceId] [varchar](100) NOT NULL,
	            [PointId] [varchar](100) NOT NULL,
	            [PackId] [int] NOT NULL,
	            [StartTime] [datetime] NOT NULL,
	            [EndTime] [datetime] NOT NULL,
	            [StartValue] [float] NOT NULL,
	            [EndValue] [float] NOT NULL
            ) ON [PRIMARY]
        END
        INSERT INTO [dbo].[V_BatTime{0}]([AreaId],[StationId],[RoomId],[DeviceId],[PointId],[PackId],[StartTime],[EndTime],[StartValue],[EndValue]) VALUES(@AreaId,@StationId,@RoomId,@DeviceId,@PointId,@PackId,@StartTime,@EndTime,@StartValue,@EndValue);";
        public const string Sql_V_BatTime_Repository_DeleteEntities = @"
        DECLARE @tpDate DATETIME, 
                @tbName NVARCHAR(255),
                @SQL NVARCHAR(MAX) = N'';

        SET @tpDate = @Start;
        WHILE(DATEDIFF(MM,@tpDate,@End)>=0)
        BEGIN
            SET @tbName = N'[dbo].[V_BatTime'+CONVERT(VARCHAR(6),@tpDate,112)+ N']';
            IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(@tbName) AND type in (N'U'))
            BEGIN
                SET @SQL += N'
		        DELETE FROM ' + @tbName + N' WHERE [DeviceId]=''' + @DeviceId + N''' AND [PointId]=''' + @PointId + N''' AND [PackId]='+ CAST(@PackId AS NVARCHAR) + N' AND [StartTime] BETWEEN ''' + CONVERT(NVARCHAR,@Start,120) + N''' AND ''' + CONVERT(NVARCHAR,@End,120) + N''';';                
            END
            SET @tpDate = DATEADD(MM,1,@tpDate);
        END
        EXECUTE sp_executesql @SQL;";

        /// <summary>
        /// V_Cuted Repository
        /// </summary>
        public const string Sql_V_Cuted_Repository_GetEntities = @"
        DECLARE @tpDate DATETIME, 
                @tbName NVARCHAR(255),
                @tableCnt INT = 0,
                @SQL NVARCHAR(MAX) = N'';

        SET @tpDate = @Start;
        WHILE(DATEDIFF(MM,@tpDate,@End)>=0)
        BEGIN
            SET @tbName = N'[dbo].[V_Cuted'+CONVERT(VARCHAR(6),@tpDate,112)+ N']';
            IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(@tbName) AND type in (N'U'))
            BEGIN
                IF(@tableCnt>0)
                BEGIN
                SET @SQL += N' 
                UNION ALL 
                ';
                END
        			
                SET @SQL += N'SELECT * FROM ' + @tbName + N' WHERE [StartTime] BETWEEN ''' + CONVERT(NVARCHAR,@Start,120) + N''' AND ''' + CONVERT(NVARCHAR,@End,120) + N'''';
                SET @tableCnt += 1;
            END
            SET @tpDate = DATEADD(MM,1,@tpDate);
        END

        IF(@tableCnt>0)
        BEGIN
	        SET @SQL = N';WITH Values AS
		        (
			        ' + @SQL + N'
		        )
		        SELECT * FROM Values ORDER BY [StartTime];'
        END

        EXECUTE sp_executesql @SQL;";
        public const string Sql_V_Cuted_Repository_GetEntitiesInType = @"
        DECLARE @tpDate DATETIME,
                @tbName NVARCHAR(255),
                @tableCnt INT = 0,
                @SQL NVARCHAR(MAX) = N'';

        SET @tpDate = @Start;
        WHILE(DATEDIFF(MM,@tpDate,@End)>=0)
        BEGIN
            SET @tbName = N'[dbo].[V_Cuted'+CONVERT(VARCHAR(6),@tpDate,112)+ N']';
            IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(@tbName) AND type in (N'U'))
            BEGIN
                IF(@tableCnt>0)
                BEGIN
                SET @SQL += N' 
                UNION ALL 
                ';
                END
        			
                SET @SQL += N'SELECT * FROM ' + @tbName + N' WHERE [Type] = '+ @Type + N' AND [StartTime] BETWEEN ''' + CONVERT(NVARCHAR,@Start,120) + N''' AND ''' + CONVERT(NVARCHAR,@End,120) + N'''';
                SET @tableCnt += 1;
            END
            SET @tpDate = DATEADD(MM,1,@tpDate);
        END

        IF(@tableCnt>0)
        BEGIN
	        SET @SQL = N';WITH Values AS
		        (
			        ' + @SQL + N'
		        )
		        SELECT * FROM Values ORDER BY [StartTime];'
        END

        EXECUTE sp_executesql @SQL;";
        public const string Sql_V_Cuted_Repository_SaveEntities = @"
        IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'V_Cuted{0}') AND type in (N'U'))
        BEGIN
            CREATE TABLE [dbo].[V_Cuted{0}](
	            [Id] [varchar](200) NOT NULL,
	            [Type] [int] NOT NULL,
	            [AreaId] [varchar](100) NOT NULL,
	            [StationId] [varchar](100) NOT NULL,
	            [RoomId] [varchar](100) NOT NULL,
	            [FsuId] [varchar](100) NOT NULL,
	            [DeviceId] [varchar](100) NOT NULL,
	            [PointId] [varchar](100) NOT NULL,
	            [StartTime] [datetime] NOT NULL,
	            [EndTime] [datetime] NOT NULL,
             CONSTRAINT [PK_V_Cut{0}] PRIMARY KEY CLUSTERED 
            (
	            [Id] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
            ) ON [PRIMARY]
        END
        DELETE FROM [dbo].[V_Cutting] WHERE [Id]=@Id AND [Type]=@Type;
        DELETE FROM [dbo].[V_Cuted{0}] WHERE [Id]=@Id AND [Type]=@Type;
        INSERT INTO [dbo].[V_Cuted{0}]([Id],[Type],[AreaId],[StationId],[RoomId],[FsuId],[DeviceId],[PointId],[StartTime],[EndTime]) VALUES(@Id,@Type,@AreaId,@StationId,@RoomId,@FsuId,@DeviceId,@PointId,,@StartTime,@EndTime);";

        /// <summary>
        /// V_Cutting Repository
        /// </summary>
        public const string Sql_V_Cutting_Repository_GetEntity = @"SELECT * FROM [dbo].[V_Cutting] WHERE [Id]=@Id AND [Type]=@Type;";
        public const string Sql_V_Cutting_Repository_GetEntities = @"SELECT * FROM [dbo].[V_Cutting];";
        public const string Sql_V_Cutting_Repository_SaveEntities = @"
        DELETE FROM [dbo].[V_Cutting] WHERE [Id]=@Id AND [Type]=@Type;
        INSERT INTO [dbo].[V_Cutting]([Id],[Type],[AreaId],[StationId],[RoomId],[FsuId],[DeviceId],[PointId],[StartTime]) VALUES(@Id,@Type,@AreaId,@StationId,@RoomId,@FsuId,@DeviceId,@PointId,,@StartTime);";

        /// <summary>
        /// V_Elec Repository
        /// </summary>
        public const string Sql_V_Elec_Repository_SaveActiveEntities = @"
        UPDATE [dbo].[V_Elec] SET [ComputeType] = @ComputeType,[StartTime] = @StartTime,[EndTime] = @EndTime,[Value] = @Value WHERE [Id] = @Id AND [Type] = @Type AND [FormulaType] = @FormulaType;
        IF(@@ROWCOUNT = 0)
        BEGIN
          INSERT INTO [dbo].[V_Elec]([Id],[Type],[FormulaType],[ComputeType],[StartTime],[EndTime],[Value]) VALUES(@Id,@Type,@FormulaType,@ComputeType,@StartTime,@EndTime,@Value);	
        END";
        public const string Sql_V_Elec_Repository_SaveEntities = @"
        IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'V_Elec{0}') AND type in (N'U'))
        BEGIN
            CREATE TABLE [dbo].[V_Elec{0}](
	            [Id] [varchar](100) NOT NULL,
	            [Type] [int] NOT NULL,
	            [FormulaType] [int] NOT NULL,
	            [ComputeType] [int] NOT NULL,
	            [StartTime] [datetime] NOT NULL,
	            [EndTime] [datetime] NOT NULL,
	            [Value] [float] NOT NULL,
             CONSTRAINT [PK_V_Elec{0}] PRIMARY KEY CLUSTERED 
            (
	            [Id] ASC,
	            [Type] ASC,
	            [FormulaType] ASC,
	            [StartTime] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
            ) ON [PRIMARY]
        END
        INSERT INTO [dbo].[V_Elec{0}]([Id],[Type],[FormulaType],[ComputeType],[StartTime],[EndTime],[Value]) VALUES(@Id,@Type,@FormulaType,@ComputeType,@StartTime,@EndTime,@Value);";
        public const string Sql_V_Elec_Repository_DeleteEntities = @"
        DECLARE @tpDate DATETIME, 
                @tbName NVARCHAR(255),
                @SQL NVARCHAR(MAX) = N'';

        SET @tpDate = @Start;
        WHILE(DATEDIFF(MM,@tpDate,@End)>=0)
        BEGIN
            SET @tbName = N'[dbo].[V_Elec'+CONVERT(VARCHAR(6),@tpDate,112)+ N']';
            IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(@tbName) AND type in (N'U'))
            BEGIN
                SET @SQL += N'
		        DELETE FROM ' + @tbName + N' WHERE [StartTime] BETWEEN ''' + CONVERT(NVARCHAR,@Start,120) + N''' AND ''' + CONVERT(NVARCHAR,@End,120) + N''';';
            END
            SET @tpDate = DATEADD(MM,1,@tpDate);
        END
        EXECUTE sp_executesql @SQL;";

        /// <summary>
        /// V_Load Repository
        /// </summary>
        public const string Sql_V_Load_Repository_SaveEntities = @"
        IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'V_Load{0}') AND type in (N'U'))
        BEGIN
        CREATE TABLE [dbo].[V_Load{0}](
	        [AreaId] [varchar](100) NOT NULL,
	        [StationId] [varchar](100) NOT NULL,
	        [RoomId] [varchar](100) NOT NULL,
	        [DeviceId] [varchar](100) NOT NULL,
	        [StartTime] [datetime] NOT NULL,
	        [EndTime] [datetime] NOT NULL,
	        [Value] [float] NOT NULL
        ) ON [PRIMARY]
        END
        INSERT INTO [dbo].[V_Load{0}]([AreaId],[StationId],[RoomId],[DeviceId],[StartTime],[EndTime],[Value]) VALUES(@AreaId,@StationId,@RoomId,@DeviceId,@StartTime,@EndTime,@Value);";
        public const string Sql_V_Load_Repository_DeleteEntities = @"
        DECLARE @tpDate DATETIME, 
                @tbName NVARCHAR(255),
                @SQL NVARCHAR(MAX) = N'';

        SET @tpDate = @Start;
        WHILE(DATEDIFF(MM,@tpDate,@End)>=0)
        BEGIN
            SET @tbName = N'[dbo].[V_Load'+CONVERT(VARCHAR(6),@tpDate,112)+ N']';
            IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(@tbName) AND type in (N'U'))
            BEGIN
                SET @SQL += N'
		        DELETE FROM ' + @tbName + N' WHERE [DeviceId] = ''' + @DeviceId + N''' AND [StartTime] BETWEEN ''' + CONVERT(NVARCHAR,@Start,120) + N''' AND ''' + CONVERT(NVARCHAR,@End,120) + N''';';
            END
            SET @tpDate = DATEADD(MM,1,@tpDate);
        END
        EXECUTE sp_executesql @SQL;";

        /// <summary>
        /// V_Static Repository
        /// </summary>
        public const string Sql_V_Static_Repository_SaveEntities = @"
        IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'V_Static{0}') AND type in (N'U'))
        BEGIN
        CREATE TABLE [dbo].[V_Static{0}](
	        [AreaId] [varchar](100) NULL,
	        [StationId] [varchar](100) NULL,
	        [RoomId] [varchar](100) NULL,
	        [DeviceId] [varchar](100) NOT NULL,
	        [PointId] [varchar](100) NOT NULL,
	        [StartTime] [datetime] NOT NULL,
	        [EndTime] [datetime] NOT NULL,
	        [MaxTime] [datetime] NOT NULL,
	        [MinTime] [datetime] NOT NULL,
	        [MaxValue] [float] NOT NULL,
	        [MinValue] [float] NOT NULL,
	        [AvgValue] [float] NOT NULL,
	        [Total] [int] NOT NULL
        ) ON [PRIMARY]
        END
        INSERT INTO [dbo].[V_Static{0}]([AreaId],[StationId],[RoomId],[DeviceId],[PointId],[StartTime],[EndTime],[MaxTime],[MinTime],[MaxValue],[MinValue],[AvgValue],[Total]) VALUES(@AreaId,@StationId,@RoomId,@DeviceId,@PointId,@StartTime,@EndTime,@MaxTime,@MinTime,@MaxValue,@MinValue,@AvgValue,@Total);";
        public const string Sql_V_Static_Repository_DeleteEntities = @"
        DECLARE @tpDate DATETIME, 
                @tbName NVARCHAR(255),
                @SQL NVARCHAR(MAX) = N'';

        SET @tpDate = @Start;
        WHILE(DATEDIFF(MM,@tpDate,@End)>=0)
        BEGIN
            SET @tbName = N'[dbo].[V_Static'+CONVERT(VARCHAR(6),@tpDate,112)+ N']';
            IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(@tbName) AND type in (N'U'))
            BEGIN
                SET @SQL += N'
		        DELETE FROM ' + @tbName + N' WHERE [DeviceId]=''' + @DeviceId + N''' AND [PointId]=''' + @PointId + N''' AND [StartTime] BETWEEN ''' + CONVERT(NVARCHAR,@Start,120) + N''' AND ''' + CONVERT(NVARCHAR,@End,120) + N''';';
            END
            SET @tpDate = DATEADD(MM,1,@tpDate);
        END
        EXECUTE sp_executesql @SQL;";

        /// <summary>
        /// V_HMeasure Repository
        /// </summary>
        public const string Sql_V_HMeasure_Repository_GetEntities = @"
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
        public const string Sql_V_HMeasure_Repository_GetEntitiesByDevice = @"
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
        			
                SET @SQL += N'SELECT * FROM ' + @tbName + N' WHERE [DeviceId] = ''' + @DeviceId + N''' AND [UpdateTime] BETWEEN ''' + CONVERT(NVARCHAR,@Start,120) + N''' AND ''' + CONVERT(NVARCHAR,@End,120) + N'''';
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
        public const string Sql_V_HMeasure_Repository_GetEntitiesByPoint = @"
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
        			
                SET @SQL += N'SELECT * FROM ' + @tbName + N' WHERE [DeviceId] = ''' + @DeviceId + N''' AND [PointId] = ''' + @PointId + N''' AND [UpdateTime] BETWEEN ''' + CONVERT(NVARCHAR,@Start,120) + N''' AND ''' + CONVERT(NVARCHAR,@End,120) + N'''';
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
        public const string Sql_V_HMeasure_Repository_GetFirst = @"
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
        			
                SET @SQL += N'SELECT * FROM ' + @tbName + N' WHERE [DeviceId] = ''' + @DeviceId + N''' AND [PointId] = ''' + @PointId + N''' AND [UpdateTime] BETWEEN ''' + CONVERT(NVARCHAR,@Start,120) + N''' AND ''' + CONVERT(NVARCHAR,@End,120) + N'''';
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
		        SELECT TOP1 * FROM HisValue ORDER BY [UpdateTime];'
        END

        EXECUTE sp_executesql @SQL;";
        public const string Sql_V_HMeasure_Repository_GetLast = @"
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
        			
                SET @SQL += N'SELECT * FROM ' + @tbName + N' WHERE [DeviceId] = ''' + @DeviceId + N''' AND [PointId] = ''' + @PointId + N''' AND [UpdateTime] BETWEEN ''' + CONVERT(NVARCHAR,@Start,120) + N''' AND ''' + CONVERT(NVARCHAR,@End,120) + N'''';
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
		        SELECT TOP1 * FROM HisValue ORDER BY [UpdateTime] DESC;'
        END

        EXECUTE sp_executesql @SQL;";
        public const string Sql_V_HMeasure_Repository_GetValDiff = @"
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
        			
                SET @SQL += N'SELECT * FROM ' + @tbName + N' WHERE [DeviceId] = ''' + @DeviceId + N''' AND [PointId] = ''' + @PointId + N''' AND [UpdateTime] BETWEEN ''' + CONVERT(NVARCHAR,@Start,120) + N''' AND ''' + CONVERT(NVARCHAR,@End,120) + N'''';
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
        public const string Sql_V_HMeasure_Repository_GetValAvg = @"
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
        			
                SET @SQL += N'SELECT * FROM ' + @tbName + N' WHERE [DeviceId] = ''' + @DeviceId + N''' AND [PointId] = ''' + @PointId + N''' AND [UpdateTime] BETWEEN ''' + CONVERT(NVARCHAR,@Start,120) + N''' AND ''' + CONVERT(NVARCHAR,@End,120) + N'''';
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
		        SELECT AVG([Value]) AS [AvgValue] FROM HisValue;'
        END

        EXECUTE sp_executesql @SQL;";

        /// <summary>
        /// V_ParamDiff Repository
        /// </summary>
        public const string Sql_V_ParamDiff_Repository_SaveEntities = @"
        IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[V_ParamDiff{0}]') AND type in (N'U'))
        BEGIN
        CREATE TABLE [dbo].[V_ParamDiff{0}](
	        [DeviceId] [varchar](100) NOT NULL,
	        [PointId] [varchar](100) NOT NULL,
	        [Threshold] [varchar](20) NULL,
	        [AlarmLevel] [varchar](20) NULL,
	        [NMAlarmID] [varchar](50) NULL,
	        [AbsoluteVal] [varchar](20) NULL,
	        [RelativeVal] [varchar](20) NULL,
	        [StorageInterval] [varchar](20) NULL,
	        [StorageRefTime] [varchar](20) NULL,
         CONSTRAINT [PK_V_ParamDiff{0}] PRIMARY KEY CLUSTERED 
        (
	        [DeviceId] ASC,
	        [PointId] ASC
        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
        ) ON [PRIMARY]
        END
        INSERT INTO [dbo].[V_ParamDiff{0}]([DeviceId],[PointId],[Threshold],[AlarmLevel],[NMAlarmID],[AbsoluteVal],[RelativeVal],[StorageInterval],[StorageRefTime]) VALUES(@DeviceId,@PointId,@Threshold,@AlarmLevel,@NMAlarmID,@AbsoluteVal,@RelativeVal,@StorageInterval,@StorageRefTime);";
        public const string Sql_V_ParamDiff_Repository_DeleteEntities = @"
        IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[V_ParamDiff{0}]') AND type in (N'U'))
        DROP TABLE [dbo].[V_ParamDiff{0}];";
    }
}
