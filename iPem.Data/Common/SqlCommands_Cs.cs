using System;

namespace iPem.Data.Common {
    public static class SqlCommands_Cs {
        /// <summary>
        /// 开始、结束告警
        /// </summary>
        public const string Sql_Alarm_Repository_Start = @"
        DELETE FROM [dbo].[A_TAlarm] WHERE [Id] = @TId;
        IF NOT EXISTS (SELECT 1 FROM [dbo].[A_AAlarm] WHERE [Id] = @Id)
        BEGIN
            INSERT INTO [dbo].[A_AAlarm]([Id],[AreaId],[StationId],[RoomId],[FsuId],[DeviceId],[PointId],[SerialNo],[NMAlarmId],[AlarmTime],[AlarmLevel],[AlarmValue],[AlarmDesc],[AlarmRemark],[ReservationId],[PrimaryId],[RelatedId],[FilterId],[ReversalId],[ReversalCount],[Masked],[CreatedTime]) VALUES(@Id,@AreaId,@StationId,@RoomId,@FsuId,@DeviceId,@PointId,@SerialNo,@NMAlarmId,@AlarmTime,@AlarmLevel,@AlarmValue,@AlarmDesc,@AlarmRemark,@ReservationId,@PrimaryId,@RelatedId,@FilterId,@ReversalId,@ReversalCount,@Masked,GETDATE());
        END";
        public const string Sql_Alarm_Repository_End = @"
        IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'A_HAlarm{0}') AND type in (N'U'))
        BEGIN
	        CREATE TABLE [dbo].[A_HAlarm{0}](
	            [Id] [varchar](100) NOT NULL,
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
	            [PrimaryId] [varchar](100) NULL,
	            [RelatedId] [varchar](100) NULL,
	            [FilterId] [varchar](100) NULL,
	            [ReversalId] [varchar](100) NULL,
	            [ReversalCount] [int] NOT NULL,
	            [Masked] [bit] NOT NULL,
	            [CreatedTime] [datetime] NOT NULL,
             CONSTRAINT [PK_A_HAlarm{0}] PRIMARY KEY NONCLUSTERED 
            (
	            [Id] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
            ) ON [PRIMARY]
        END

        DELETE FROM [dbo].[A_AAlarm] WHERE [Id] = @Id;
        DELETE FROM [dbo].[A_TAlarm] WHERE [Id] = @TId;
        IF NOT EXISTS (SELECT 1 FROM [dbo].[A_HAlarm{0}] WHERE [Id] = @Id)
        BEGIN
            INSERT INTO [dbo].[A_HAlarm{0}]([Id],[AreaId],[StationId],[RoomId],[FsuId],[DeviceId],[PointId],[SerialNo],[NMAlarmId],[StartTime],[EndTime],[AlarmLevel],[StartValue],[EndValue],[AlarmDesc],[AlarmRemark],[Confirmed],[Confirmer],[ConfirmedTime],[ReservationId],[PrimaryId],[RelatedId],[FilterId],[ReversalId],[ReversalCount],[Masked],[CreatedTime]) VALUES(@Id,@AreaId,@StationId,@RoomId,@FsuId,@DeviceId,@PointId,@SerialNo,@NMAlarmId,@StartTime,@EndTime,@AlarmLevel,@StartValue,@EndValue,@AlarmDesc,@AlarmRemark,@Confirmed,@Confirmer,@ConfirmedTime,@ReservationId,@PrimaryId,@RelatedId,@FilterId,@ReversalId,@ReversalCount,@Masked,GETDATE());
        END";
        public const string Sql_Alarm_Repository_SaveInterface = @"
        IF NOT EXISTS (SELECT 1 FROM [dbo].[A_IAlarm] WHERE [Id] = @Id)
        BEGIN
            INSERT INTO [dbo].[A_IAlarm]([Id],[AreaId],[StationId],[RoomId],[FsuId],[DeviceId],[PointId],[SerialNo],[NMAlarmId],[AlarmTime],[AlarmLevel],[AlarmFlag],[AlarmDesc],[AlarmValue],[AlarmRemark],[Confirmed],[Confirmer],[ConfirmedTime],[ReservationId],[ReservationName],[ReservationStart],[ReservationEnd],[PrimaryId],[RelatedId],[FilterId],[ReversalId],[Masked],[CreatedTime]) VALUES(@Id,@AreaId,@StationId,@RoomId,@FsuId,@DeviceId,@PointId,@SerialNo,@NMAlarmId,@AlarmTime,@AlarmLevel,@AlarmFlag,@AlarmDesc,@AlarmValue,@AlarmRemark,@Confirmed,@Confirmer,@ConfirmedTime,@ReservationId,@ReservationName,@ReservationStart,@ReservationEnd,@PrimaryId,@RelatedId,@FilterId,@ReversalId,@Masked,@CreatedTime);
        END";
        public const string Sql_Alarm_Repository_SaveMessage = @"
        IF NOT EXISTS (SELECT 1 FROM [dbo].[A_MAlarm] WHERE [Id] = @Id)
        BEGIN
            INSERT INTO [dbo].[A_MAlarm]([Id],[AreaId],[AreaName],[StationId],[StationName],[RoomId],[RoomName],[DeviceId],[DeviceName],[PointId],[PointName],[SerialNo],[NMAlarmId],[AlarmTime],[AlarmLevel],[AlarmFlag],[AlarmDesc],[AlarmValue],[AlarmRemark],[Confirmed],[Confirmer],[ConfirmedTime],[ReservationId],[PrimaryId],[RelatedId],[FilterId],[ReversalId],[Masked],[CreatedTime]) VALUES(@Id,@AreaId,@AreaName,@StationId,@StationName,@RoomId,@RoomName,@DeviceId,@DeviceName,@PointId,@PointName,@SerialNo,@NMAlarmId,@AlarmTime,@AlarmLevel,@AlarmFlag,@AlarmDesc,@AlarmValue,@AlarmRemark,@Confirmed,@Confirmer,@ConfirmedTime,@ReservationId,@PrimaryId,@RelatedId,@FilterId,@ReversalId,@Masked,@CreatedTime);
        END";
        public const string Sql_Alarm_Repository_SaveSpeech = @"
        IF NOT EXISTS (SELECT 1 FROM [dbo].[A_SAlarm] WHERE [Id] = @Id)
        BEGIN
            INSERT INTO [dbo].[A_SAlarm]([Id],[AreaId],[AreaName],[StationId],[StationName],[RoomId],[RoomName],[DeviceId],[DeviceName],[PointId],[PointName],[SerialNo],[NMAlarmId],[AlarmTime],[AlarmLevel],[AlarmFlag],[AlarmDesc],[AlarmValue],[AlarmRemark],[Confirmed],[Confirmer],[ConfirmedTime],[ReservationId],[PrimaryId],[RelatedId],[FilterId],[ReversalId],[Masked],[CreatedTime]) VALUES(@Id,@AreaId,@AreaName,@StationId,@StationName,@RoomId,@RoomName,@DeviceId,@DeviceName,@PointId,@PointName,@SerialNo,@NMAlarmId,@AlarmTime,@AlarmLevel,@AlarmFlag,@AlarmDesc,@AlarmValue,@AlarmRemark,@Confirmed,@Confirmer,@ConfirmedTime,@ReservationId,@PrimaryId,@RelatedId,@FilterId,@ReversalId,@Masked,@CreatedTime);
        END";
        
        /// <summary>
        /// 创建数据表索引
        /// </summary>
        public const string Sql_Indexer_Check = @"
        --■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        --创建索引[dbo].[A_HAlarm]
        IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[A_HAlarm{0}]') AND type in (N'U')) 
        AND NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[A_HAlarm{0}]') AND name = N'ClusteredIndex{0}')
        CREATE CLUSTERED INDEX [ClusteredIndex{0}] ON [dbo].[A_HAlarm{0}]
        (
	        [StartTime] ASC,
	        [DeviceId] ASC,
	        [PointId] ASC
        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

        --■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        --创建索引[dbo].[H_CardRecord]
        IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[H_CardRecord{0}]') AND type in (N'U')) 
        AND NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[H_CardRecord{0}]') AND name = N'ClusteredIndex{0}')
        CREATE CLUSTERED INDEX [ClusteredIndex{0}] ON [dbo].[H_CardRecord{0}]
        (
	        [PunchTime] ASC,
	        [CardId] ASC
        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

        --■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        --创建索引[dbo].[V_Bat]
        IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[V_Bat{0}]') AND type in (N'U')) 
        AND NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[V_Bat{0}]') AND name = N'ClusteredIndex{0}')
        CREATE CLUSTERED INDEX [ClusteredIndex{0}] ON [dbo].[V_Bat{0}]
        (
	        [ValueTime] ASC,
	        [DeviceId] ASC,
	        [PointId] ASC
        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

        IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[V_Bat{0}]') AND type in (N'U')) 
        AND NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[V_Bat{0}]') AND name = N'NonClusteredIndex{0}')
        CREATE NONCLUSTERED INDEX [NonClusteredIndex{0}] ON [dbo].[V_Bat{0}]
        (
	        [StartTime] ASC,
	        [DeviceId] ASC,
	        [PointId] ASC
        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

        --■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        --创建索引[dbo].[V_BatCurve]
        IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[V_BatCurve{0}]') AND type in (N'U')) 
        AND NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[V_BatCurve{0}]') AND name = N'ClusteredIndex{0}')
        CREATE CLUSTERED INDEX [ClusteredIndex{0}] ON [dbo].[V_BatCurve{0}]
        (
	        [ValueTime] ASC,
	        [DeviceId] ASC,
	        [PointId] ASC
        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

        IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[V_BatCurve{0}]') AND type in (N'U')) 
        AND NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[V_BatCurve{0}]') AND name = N'NonClusteredIndex{0}')
        CREATE NONCLUSTERED INDEX [NonClusteredIndex{0}] ON [dbo].[V_BatCurve{0}]
        (
	        [ProcTime] ASC,
	        [DeviceId] ASC,
	        [PointId] ASC
        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

        --■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        --创建索引[dbo].[V_BatTime]
        IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[V_BatTime{0}]') AND type in (N'U')) 
        AND NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[V_BatTime{0}]') AND name = N'ClusteredIndex{0}')
        CREATE CLUSTERED INDEX [ClusteredIndex{0}] ON [dbo].[V_BatTime{0}]
        (
	        [StartTime] ASC,
	        [DeviceId] ASC,
	        [PointId] ASC
        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

        IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[V_BatTime{0}]') AND type in (N'U')) 
        AND NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[V_BatTime{0}]') AND name = N'NonClusteredIndex{0}')
        CREATE NONCLUSTERED INDEX [NonClusteredIndex{0}] ON [dbo].[V_BatTime{0}]
        (
	        [ProcTime] ASC,
	        [DeviceId] ASC,
	        [PointId] ASC
        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

        --■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        --创建索引[dbo].[V_Cuted]
        IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[V_Cuted{0}]') AND type in (N'U')) 
        AND NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[V_Cuted{0}]') AND name = N'NonClusteredIndex{0}')
        CREATE NONCLUSTERED INDEX [NonClusteredIndex{0}] ON [dbo].[V_Cuted{0}]
        (
	        [StartTime] ASC,
	        [DeviceId] ASC,
	        [PointId] ASC
        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

        --■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        --创建索引[dbo].[V_Elec]
        IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[V_Elec{0}]') AND type in (N'U')) 
        AND NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[V_Elec{0}]') AND name = N'ClusteredIndex{0}')
        CREATE CLUSTERED INDEX [ClusteredIndex{0}] ON [dbo].[V_Elec{0}]
        (
	        [StartTime] ASC,
	        [Type] ASC,
	        [FormulaType] ASC
        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

        --■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        --创建索引[dbo].[V_HMeasure]
        IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[V_HMeasure{0}]') AND type in (N'U')) 
        AND NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[V_HMeasure{0}]') AND name = N'ClusteredIndex{0}')
        CREATE CLUSTERED INDEX [ClusteredIndex{0}] ON [dbo].[V_HMeasure{0}]
        (
	        [UpdateTime] ASC,
	        [DeviceId] ASC,
	        [PointId] ASC
        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

        --■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        --创建索引[dbo].[V_Load]
        IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[V_Load{0}]') AND type in (N'U')) 
        AND NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[V_Load{0}]') AND name = N'ClusteredIndex{0}')
        CREATE CLUSTERED INDEX [ClusteredIndex{0}] ON [dbo].[V_Load{0}]
        (
	        [StartTime] ASC,
	        [DeviceId] ASC
        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

        --■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        --创建索引[dbo].[V_Static]
        IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[V_Static{0}]') AND type in (N'U')) 
        AND NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[V_Static{0}]') AND name = N'ClusteredIndex{0}')
        CREATE CLUSTERED INDEX [ClusteredIndex{0}] ON [dbo].[V_Static{0}]
        (
	        [StartTime] ASC,
	        [DeviceId] ASC,
	        [PointId] ASC
        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]";
        
        /// <summary>
        /// 活动告警表
        /// </summary>
        public const string Sql_A_AAlarm_Repository_GetEntity = @"SELECT * FROM [dbo].[A_AAlarm] WHERE [Id] = @Id;";
        public const string Sql_A_AAlarm_Repository_GetEntityInPoint = @"SELECT * FROM [dbo].[A_AAlarm] WHERE [DeviceId] = @DeviceId AND [PointId]=@PointId;";
        public const string Sql_A_AAlarm_Repository_GetEntitiesInDevice = @"SELECT * FROM [dbo].[A_AAlarm] WHERE [DeviceId] = @DeviceId ORDER BY [AlarmTime];";
        public const string Sql_A_AAlarm_Repository_GetEntitiesInSpan = @"SELECT * FROM [dbo].[A_AAlarm] WHERE [AlarmTime] BETWEEN @Start AND @End ORDER BY [AlarmTime];";
        public const string Sql_A_AAlarm_Repository_GetEntities = @"SELECT * FROM [dbo].[A_AAlarm] ORDER BY [AlarmTime];";
        
        /// <summary>
        /// 历史告警表
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
        /// 告警流水表
        /// </summary>
        public const string Sql_A_TAlarm_Repository_GetEntities1 = @"SELECT * FROM [dbo].[A_TAlarm] ORDER BY [AlarmTime];";        
        public const string Sql_A_TAlarm_Repository_GetEntities2 = @"SELECT * FROM [dbo].[A_TAlarm] WHERE [AlarmTime] BETWEEN @Start AND @End ORDER BY [AlarmTime];";
        public const string Sql_A_TAlarm_Repository_Delete = @"DELETE FROM [dbo].[A_TAlarm] WHERE [Id] = @Id;";
        public const string Sql_A_TAlarm_Repository_Save = @"
        IF NOT EXISTS( SELECT 1 FROM [dbo].[A_TAlarm] WHERE [FsuId]=@FsuId AND [DeviceId]=@DeviceId AND [PointId]=@PointId AND [AlarmTime]=@AlarmTime AND [AlarmFlag]=@AlarmFlag)
        BEGIN
	        INSERT INTO [dbo].[A_TAlarm]([FsuId],[DeviceId],[PointId],[SignalId],[SignalNumber],[SerialNo],[NMAlarmId],[AlarmTime],[AlarmLevel],[AlarmFlag],[AlarmDesc],[AlarmValue],[AlarmRemark]) 
	        VALUES(@FsuId,@DeviceId,@PointId,@SignalId,@SignalNumber,@SerialNo,@NMAlarmId,@AlarmTime,@AlarmLevel,@AlarmFlag,@AlarmDesc,@AlarmValue,@AlarmRemark);
        END";

        /// <summary>
        /// 告警同步表
        /// </summary>
        public const string Sql_A_FAlarm_Repository_GetEntities1 = @"SELECT * FROM [dbo].[A_FAlarm] ORDER BY [AlarmTime];";
        public const string Sql_A_FAlarm_Repository_GetEntities2 = @"SELECT * FROM [dbo].[A_FAlarm] WHERE [AlarmTime] BETWEEN @Start AND @End ORDER BY [AlarmTime];";
        public const string Sql_A_FAlarm_Repository_Delete1 = @"DELETE FROM [dbo].[A_FAlarm] WHERE [FsuId]=@FsuId AND [SerialNo]=@SerialNo AND [AlarmFlag]=@AlarmFlag;";
        public const string Sql_A_FAlarm_Repository_Delete2 = @"DELETE FROM [dbo].[A_FAlarm] WHERE [AlarmTime] BETWEEN @Start AND @End;";

        /// <summary>
        /// FSU日志表
        /// </summary>
        public const string Sql_H_FsuEvent_Repository_GetEntities1 = @"SELECT * FROM [dbo].[H_FsuEvent] WHERE [EventTime] BETWEEN @Start AND @End ORDER BY [EventTime] DESC;";
        public const string Sql_H_FsuEvent_Repository_GetEntities2 = @"SELECT * FROM [dbo].[H_FsuEvent] WHERE [EventTime] BETWEEN @Start AND @End AND [EventType] = @EventType ORDER BY [EventTime] DESC;";

        /// <summary>
        /// 资管接口区域表
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
        /// 资管接口设备表
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
        /// 资管接口站点表
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
        /// 实时性能表
        /// </summary>
        public const string Sql_V_AMeasure_Repository_GetEntity = @"SELECT * FROM [dbo].[V_AMeasure] WHERE [DeviceId]=@DeviceId AND [PointId]=@PointId;";
        public const string Sql_V_AMeasure_Repository_GetEntities = @"SELECT * FROM [dbo].[V_AMeasure];";
        public const string Sql_V_AMeasure_Repository_GetEntitiesInDevice = @"SELECT * FROM [dbo].[V_AMeasure] WHERE [DeviceId]=@DeviceId;";

        /// <summary>
        /// 电池数据表
        /// </summary>
        public const string Sql_V_Bat_Repository_GetEntities1 = @"
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
	        SET @SQL = N';WITH V AS
	        (
		        ' + @SQL + N'
	        )
	        SELECT * FROM V;'
        END

        EXECUTE sp_executesql @SQL;";
        public const string Sql_V_Bat_Repository_GetEntities2 = @"
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
        			
                SET @SQL += N'SELECT * FROM ' + @tbName + N' WHERE [ValueTime] BETWEEN ''' + CONVERT(NVARCHAR,@Start,120) + N''' AND ''' + CONVERT(NVARCHAR,@End,120) + N''' AND [DeviceId]=''' + @DeviceId + N''' AND [PointId]='''+ @PointId + N'''';
                SET @tableCnt += 1;
            END
            SET @tpDate = DATEADD(MM,1,@tpDate);
        END

        IF(@tableCnt>0)
        BEGIN
	        SET @SQL = N';WITH V AS
	        (
		        ' + @SQL + N'
	        )
	        SELECT * FROM V ORDER BY [ValueTime];'
        END

        EXECUTE sp_executesql @SQL;";
        public const string Sql_V_Bat_Repository_GetProcedures = @"
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
	        SET @SQL = N';WITH V AS
	        (
		        ' + @SQL + N'
	        ),G AS 
	        (
		        SELECT *, ROW_NUMBER() OVER(PARTITION BY [StartTime],[DeviceId],[PointId] ORDER BY [ValueTime]) AS GINDEX FROM V
	        )
	        SELECT [AreaId],[StationId],[RoomId],[DeviceId],[PointId],[PackId],[Type],[StartTime],[Value],[ValueTime] FROM G WHERE GINDEX=1;'
        END

        EXECUTE sp_executesql @SQL;";
        public const string Sql_V_Bat_Repository_GetProcDetails = @"
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
        		
                SET @SQL += N'SELECT * FROM ' + @tbName + N' WHERE [StartTime] = ''' + CONVERT(NVARCHAR,@Start,120) + N''' AND [DeviceId]=''' + @DeviceId + N''' AND [PointId]='''+ @PointId + N'''';
                SET @tableCnt += 1;
            END
            SET @tpDate = DATEADD(MM,1,@tpDate);
        END

        IF(@tableCnt>0)
        BEGIN
	        SET @SQL = N';WITH V AS
	        (
		        ' + @SQL + N'
	        )
	        SELECT * FROM V ORDER BY [ValueTime];'
        END

        EXECUTE sp_executesql @SQL;";
        public const string Sql_V_Bat_Repository_GetFirst = @"
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
        			
	        SET @SQL += N'SELECT TOP 1 * FROM ' + @tbName + N' WHERE [ValueTime] > ''' + CONVERT(NVARCHAR,@Start,120) + N''' AND [ValueTime] <= ''' + CONVERT(NVARCHAR,@End,120) + N''' AND [DeviceId]=''' + @DeviceId + N''' AND [PointId]='''+ @PointId + N''' ORDER BY [ValueTime]';
	        SET @tableCnt += 1;
        END
        SET @tpDate = DATEADD(MM,1,@tpDate);
        END

        IF(@tableCnt>0)
        BEGIN
        SET @SQL = N';WITH V AS
        (
	        ' + @SQL + N'
        )
        SELECT TOP 1 * FROM V ORDER BY [ValueTime];'
        END

        EXECUTE sp_executesql @SQL;";
        public const string Sql_V_Bat_Repository_GetLast = @"
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
        			
	        SET @SQL += N'SELECT TOP 1 * FROM ' + @tbName + N' WHERE [ValueTime] >= ''' + CONVERT(NVARCHAR,@Start,120) + N''' AND [ValueTime] < ''' + CONVERT(NVARCHAR,@End,120) + N''' AND [DeviceId]=''' + @DeviceId + N''' AND [PointId]='''+ @PointId + N''' ORDER BY [ValueTime] DESC';
	        SET @tableCnt += 1;
        END
        SET @tpDate = DATEADD(MM,1,@tpDate);
        END

        IF(@tableCnt>0)
        BEGIN
        SET @SQL = N';WITH V AS
        (
	        ' + @SQL + N'
        )
        SELECT TOP 1 * FROM V ORDER BY [ValueTime] DESC;'
        END

        EXECUTE sp_executesql @SQL;";
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
	            [Type] [int] NOT NULL,
	            [StartTime] [datetime] NOT NULL,
	            [Value] [float] NOT NULL,
	            [ValueTime] [datetime] NOT NULL
            ) ON [PRIMARY]
        END
        INSERT INTO [dbo].[V_Bat{0}]([AreaId],[StationId],[RoomId],[DeviceId],[PointId],[PackId],[Type],[StartTime],[Value],[ValueTime]) VALUES(@AreaId,@StationId,@RoomId,@DeviceId,@PointId,@PackId,@Type,@StartTime,@Value,@ValueTime);";
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
		        DELETE FROM ' + @tbName + N' WHERE [StartTime] = ''' + CONVERT(NVARCHAR,@Start,120) + N''' AND [DeviceId]=''' + @DeviceId + N''' AND [PointId]=''' + @PointId + N''';';
            END
            SET @tpDate = DATEADD(MM,1,@tpDate);
        END

        EXECUTE sp_executesql @SQL;";

        /// <summary>
        /// 电池充放电过程表
        /// </summary>
        public const string Sql_V_BatTime_Repository_GetProcedures = @"
        DECLARE @tpDate DATETIME, 
                @tbName NVARCHAR(255),
                @tableCnt INT = 0,
                @SQL NVARCHAR(MAX) = N'';

        SET @tpDate = @Start;
        WHILE(DATEDIFF(MM,@tpDate,@End)>=0)
        BEGIN
            SET @tbName = N'[dbo].[V_BatTime'+CONVERT(VARCHAR(6),@tpDate,112)+ N']';
            IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(@tbName) AND type in (N'U'))
            BEGIN
                IF(@tableCnt>0)
                BEGIN
                SET @SQL += N' 
                UNION ALL 
                ';
                END
        			
                SET @SQL += N'SELECT * FROM ' + @tbName + N' WHERE [EndTime] BETWEEN ''' + CONVERT(NVARCHAR,@Start,120) + N''' AND ''' + CONVERT(NVARCHAR,@End,120) + N'''';
                SET @tableCnt += 1;
            END
            SET @tpDate = DATEADD(MM,1,@tpDate);
        END

        IF(@tableCnt>0)
        BEGIN
	        SET @SQL = N';WITH V AS
	        (
		        ' + @SQL + N'
	        ),G AS 
	        (
		        SELECT *, ROW_NUMBER() OVER(PARTITION BY [ProcTime],[DeviceId],[PointId] ORDER BY [StartTime]) AS GINDEX FROM V
	        )
	        SELECT [AreaId],[StationId],[RoomId],[DeviceId],[PointId],[PackId],[Type],[StartTime],[EndTime],[StartValue],[EndValue],[ProcTime] FROM G WHERE GINDEX=1;'
        END

        EXECUTE sp_executesql @SQL;";
        public const string Sql_V_BatTime_Repository_GetProcDetails = @"
        DECLARE @tpDate DATETIME, 
                @tbName NVARCHAR(255),
                @tableCnt INT = 0,
                @SQL NVARCHAR(MAX) = N'';

        SET @tpDate = @Start;
        WHILE(DATEDIFF(MM,@tpDate,@End)>=0)
        BEGIN
            SET @tbName = N'[dbo].[V_BatTime'+CONVERT(VARCHAR(6),@tpDate,112)+ N']';
            IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(@tbName) AND type in (N'U'))
            BEGIN
                IF(@tableCnt>0)
                BEGIN
                SET @SQL += N' 
                UNION ALL 
                ';
                END
        		
                SET @SQL += N'SELECT * FROM ' + @tbName + N' WHERE [ProcTime] = ''' + CONVERT(NVARCHAR,@Start,120) + N''' AND [DeviceId]=''' + @DeviceId + N''' AND [PointId]='''+ @PointId + N'''';
                SET @tableCnt += 1;
            END
            SET @tpDate = DATEADD(MM,1,@tpDate);
        END

        IF(@tableCnt>0)
        BEGIN
	        SET @SQL = N';WITH V AS
	        (
		        ' + @SQL + N'
	        )
	        SELECT * FROM V ORDER BY [StartTime];'
        END

        EXECUTE sp_executesql @SQL;";
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
	            [Type] [int] NOT NULL,
	            [StartTime] [datetime] NOT NULL,
	            [EndTime] [datetime] NOT NULL,
	            [StartValue] [float] NOT NULL,
	            [EndValue] [float] NOT NULL,
	            [ProcTime] [datetime] NOT NULL
            ) ON [PRIMARY]
        END
        DELETE FROM [dbo].[V_BatTime{0}] WHERE [StartTime] = @StartTime AND [DeviceId] = @DeviceId AND [PointId] = @PointId;
        INSERT INTO [dbo].[V_BatTime{0}]([AreaId],[StationId],[RoomId],[DeviceId],[PointId],[PackId],[Type],[StartTime],[EndTime],[StartValue],[EndValue],[ProcTime]) VALUES(@AreaId,@StationId,@RoomId,@DeviceId,@PointId,@PackId,@Type,@StartTime,@EndTime,@StartValue,@EndValue,@ProcTime);";
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
		        DELETE FROM ' + @tbName + N' WHERE [StartTime] BETWEEN ''' + CONVERT(NVARCHAR,@Start,120) + N''' AND ''' + CONVERT(NVARCHAR,@End,120) + N''';';                
            END
            SET @tpDate = DATEADD(MM,1,@tpDate);
        END

        EXECUTE sp_executesql @SQL;";

        /// <summary>
        /// 电池充放电曲线表
        /// </summary>
        public const string Sql_V_BatCurve_Repository_SaveEntities = @"
        IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'V_BatCurve{0}') AND type in (N'U'))
        BEGIN
            CREATE TABLE [dbo].[V_BatCurve{0}](
		        [AreaId] [varchar](100) NOT NULL,
		        [StationId] [varchar](100) NOT NULL,
		        [RoomId] [varchar](100) NOT NULL,
		        [DeviceId] [varchar](100) NOT NULL,
		        [PointId] [varchar](100) NOT NULL,
		        [PackId] [int] NOT NULL,
		        [Type] [int] NOT NULL,
		        [StartTime] [datetime] NOT NULL,
		        [Value] [float] NOT NULL,
		        [ValueTime] [datetime] NOT NULL,
		        [ProcTime] [datetime] NOT NULL
	        ) ON [PRIMARY]
        END
        INSERT INTO [dbo].[V_BatCurve{0}]([AreaId],[StationId],[RoomId],[DeviceId],[PointId],[PackId],[Type],[StartTime],[Value],[ValueTime],[ProcTime]) VALUES(@AreaId,@StationId,@RoomId,@DeviceId,@PointId,@PackId,@Type,@StartTime,@Value,@ValueTime,@ProcTime);";
        public const string Sql_V_BatCurve_Repository_DeleteEntities = @"
        DECLARE @tpDate DATETIME, 
                @tbName NVARCHAR(255),
                @SQL NVARCHAR(MAX) = N'';

        SET @tpDate = @Start;
        WHILE(DATEDIFF(MM,@tpDate,@End)>=0)
        BEGIN
            SET @tbName = N'[dbo].[V_BatCurve'+CONVERT(VARCHAR(6),@tpDate,112)+ N']';
            IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(@tbName) AND type in (N'U'))
            BEGIN
                SET @SQL += N'
		        DELETE FROM ' + @tbName + N' WHERE [ProcTime] = ''' + CONVERT(NVARCHAR,@Start,120) + N''' AND [DeviceId]=''' + @DeviceId + N''' AND [PointId]=''' + @PointId + N''';';                
            END
            SET @tpDate = DATEADD(MM,1,@tpDate);
        END

        EXECUTE sp_executesql @SQL;";

        /// <summary>
        /// 站点停电历史表
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
        			
                SET @SQL += N'SELECT * FROM ' + @tbName + N' WHERE [StartTime] BETWEEN ''' + CONVERT(NVARCHAR,@Start,120) + N''' AND ''' + CONVERT(NVARCHAR,@End,120) + N''' AND [Type] = ' + CAST(@Type AS NVARCHAR);
                SET @tableCnt += 1;
            END
            SET @tpDate = DATEADD(MM,1,@tpDate);
        END

        IF(@tableCnt>0)
        BEGIN
	        SET @SQL = N';WITH C AS
	        (
		        ' + @SQL + N'
	        )
	        SELECT * FROM C ORDER BY [StartTime];'
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
             CONSTRAINT [PK_V_Cuted{0}] PRIMARY KEY CLUSTERED 
            (
	            [Id] ASC,
	            [Type] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
            ) ON [PRIMARY]
        END
        DELETE FROM [dbo].[V_Cutting] WHERE [Id]=@Id AND [Type]=@Type;
        DELETE FROM [dbo].[V_Cuted{0}] WHERE [Id]=@Id AND [Type]=@Type;
        INSERT INTO [dbo].[V_Cuted{0}]([Id],[Type],[AreaId],[StationId],[RoomId],[FsuId],[DeviceId],[PointId],[StartTime],[EndTime]) VALUES(@Id,@Type,@AreaId,@StationId,@RoomId,@FsuId,@DeviceId,@PointId,,@StartTime,@EndTime);";

        /// <summary>
        /// 站点停电实时表
        /// </summary>
        public const string Sql_V_Cutting_Repository_GetEntity = @"SELECT * FROM [dbo].[V_Cutting] WHERE [Id]=@Id AND [Type]=@Type;";
        public const string Sql_V_Cutting_Repository_GetEntities = @"SELECT * FROM [dbo].[V_Cutting];";
        public const string Sql_V_Cutting_Repository_SaveEntities = @"
        DELETE FROM [dbo].[V_Cutting] WHERE [Id]=@Id AND [Type]=@Type;
        INSERT INTO [dbo].[V_Cutting]([Id],[Type],[AreaId],[StationId],[RoomId],[FsuId],[DeviceId],[PointId],[StartTime]) VALUES(@Id,@Type,@AreaId,@StationId,@RoomId,@FsuId,@DeviceId,@PointId,,@StartTime);";

        /// <summary>
        /// 能耗统计表
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
	            [Value] [float] NOT NULL
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
        /// 带载率统计表
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
		        DELETE FROM ' + @tbName + N' WHERE [StartTime] BETWEEN ''' + CONVERT(NVARCHAR,@Start,120) + N''' AND ''' + CONVERT(NVARCHAR,@End,120) + N''' AND [DeviceId] = ''' + @DeviceId + ''';';
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
		        DELETE FROM ' + @tbName + N' WHERE [StartTime] BETWEEN ''' + CONVERT(NVARCHAR,@Start,120) + N''' AND ''' + CONVERT(NVARCHAR,@End,120) + N''' AND [DeviceId]=''' + @DeviceId + N''' AND [PointId]=''' + @PointId + N''';';
            END
            SET @tpDate = DATEADD(MM,1,@tpDate);
        END

        EXECUTE sp_executesql @SQL;";

        /// <summary>
        /// 历史性能表
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
        			
                SET @SQL += N'SELECT * FROM ' + @tbName + N' WHERE [UpdateTime] BETWEEN ''' + CONVERT(NVARCHAR,@Start,120) + N''' AND ''' + CONVERT(NVARCHAR,@End,120) + N''' AND [DeviceId] = ''' + @DeviceId + N'''';
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
        			
                SET @SQL += N'SELECT * FROM ' + @tbName + N' WHERE [UpdateTime] BETWEEN ''' + CONVERT(NVARCHAR,@Start,120) + N''' AND ''' + CONVERT(NVARCHAR,@End,120) + N''' AND [DeviceId] = ''' + @DeviceId + N''' AND [PointId] = ''' + @PointId + N'''';
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

                SET @SQL += N'SELECT * FROM ' + @tbName + N' WHERE [UpdateTime] BETWEEN ''' + CONVERT(NVARCHAR,@Start,120) + N''' AND ''' + CONVERT(NVARCHAR,@End,120) + N''' AND [DeviceId] = ''' + @DeviceId + N''' AND [PointId] = ''' + @PointId + N'''';        		
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
		    SELECT TOP 1 * FROM HisValue ORDER BY [UpdateTime];'
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

                SET @SQL += N'SELECT * FROM ' + @tbName + N' WHERE [UpdateTime] BETWEEN ''' + CONVERT(NVARCHAR,@Start,120) + N''' AND ''' + CONVERT(NVARCHAR,@End,120) + N''' AND [DeviceId] = ''' + @DeviceId + N''' AND [PointId] = ''' + @PointId + N'''';        		        		
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
		    SELECT TOP 1 * FROM HisValue ORDER BY [UpdateTime] DESC;'
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

                SET @SQL += N'SELECT * FROM ' + @tbName + N' WHERE [UpdateTime] BETWEEN ''' + CONVERT(NVARCHAR,@Start,120) + N''' AND ''' + CONVERT(NVARCHAR,@End,120) + N''' AND [DeviceId] = ''' + @DeviceId + N''' AND [PointId] = ''' + @PointId + N'''';        		        		        		
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
		        SELECT ROUND(ISNULL(MAX([Value]),0),3) AS [MaxValue], ROUND(ISNULL(MIN([Value]),0),3) AS [MinValue] FROM HisValue;'
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

                SET @SQL += N'SELECT * FROM ' + @tbName + N' WHERE [UpdateTime] BETWEEN ''' + CONVERT(NVARCHAR,@Start,120) + N''' AND ''' + CONVERT(NVARCHAR,@End,120) + N''' AND [DeviceId] = ''' + @DeviceId + N''' AND [PointId] = ''' + @PointId + N'''';        		
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
		        SELECT ROUND(ISNULL(AVG([Value]),0),3) AS [AvgValue] FROM HisValue;'
        END

        EXECUTE sp_executesql @SQL;";

        /// <summary>
        /// 参数巡检表
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
	            [StorageRefTime] [varchar](50) NULL,
	            [Masked] [bit] NULL,
             CONSTRAINT [PK_V_ParamDiff{0}] PRIMARY KEY CLUSTERED 
            (
	            [DeviceId] ASC,
	            [PointId] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
            ) ON [PRIMARY]
        END
        INSERT INTO [dbo].[V_ParamDiff{0}]([DeviceId],[PointId],[Threshold],[AlarmLevel],[NMAlarmID],[AbsoluteVal],[RelativeVal],[StorageInterval],[StorageRefTime],[Masked]) VALUES(@DeviceId,@PointId,@Threshold,@AlarmLevel,@NMAlarmID,@AbsoluteVal,@RelativeVal,@StorageInterval,@StorageRefTime,@Masked);";
        public const string Sql_V_ParamDiff_Repository_DeleteEntities = @"
        IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[V_ParamDiff{0}]') AND type in (N'U'))
        DROP TABLE [dbo].[V_ParamDiff{0}];";
    }
}
