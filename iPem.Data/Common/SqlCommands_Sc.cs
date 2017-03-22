using System;

namespace iPem.Data.Common {
    public static class SqlCommands_Sc {
        //Appointment Repository
        public const string Sql_Appointment_Repository_GetEntities = @"SELECT [Id],[StartTime],[EndTime],[ProjectId],[Creator],[CreatedTime],[Comment],[Enabled] FROM [dbo].[M_Appointments] ORDER BY [CreatedTime];";
        public const string Sql_Appointment_Repository_GetEntitiesByDate = @"SELECT [Id],[StartTime],[EndTime],[ProjectId],[Creator],[CreatedTime],[Comment],[Enabled] FROM [dbo].[M_Appointments] WHERE NOT ([StartTime]>@endTime OR [EndTime]<@startTime) ORDER BY [CreatedTime];";
        //Dictionary Repository
        public const string Sql_Dictionary_Repository_GetEntity = @"SELECT [Id],[Name],[ValuesJson],[ValuesBinary],[LastUpdatedDate] FROM [dbo].[M_Dictionary] WHERE [Id]=@Id;";
        public const string Sql_Dictionary_Repository_GetEntities = @"SELECT [Id],[Name],[ValuesJson],[ValuesBinary],[LastUpdatedDate] FROM [dbo].[M_Dictionary];";
        //ExtendAlm Repository
        public const string Sql_ExtAlarm_Repository_GetEntities = @"SELECT * FROM [dbo].[H_ExtAlarms];";
        public const string Sql_ExtAlarm_Repository_SaveEntities = @"
        IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'H_ExtAlarms{0}') AND type in (N'U'))
        BEGIN
        CREATE TABLE [dbo].[H_ExtAlarms{0}](
	        [Id] [varchar](200) NOT NULL,
	        [SerialNo] [varchar](100) NOT NULL,
	        [Time] [datetime] NOT NULL,
	        [ProjectId] [varchar](100) NULL,
	        [Confirmed] [int] NULL,
	        [Confirmer] [varchar](100) NULL,
	        [ConfirmedTime] [datetime] NULL,
         CONSTRAINT [PK_H_ExtAlarms{0}] PRIMARY KEY CLUSTERED 
        (
	        [Id] ASC,
	        [SerialNo] ASC
        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
        ) ON [PRIMARY]
        END
        UPDATE [dbo].[H_ExtAlarms{0}] SET [Time] = @Time,[ProjectId] = @ProjectId,[Confirmed] = @Confirmed,[Confirmer] = @Confirmer,[ConfirmedTime] = @ConfirmedTime WHERE [Id] = @Id AND [SerialNo] = @SerialNo;
        IF(@@ROWCOUNT = 0)
        BEGIN
	        INSERT INTO [dbo].[H_ExtAlarms{0}]([Id],[SerialNo],[Time],[ProjectId],[Confirmed],[Confirmer],[ConfirmedTime]) VALUES(@Id,@SerialNo,@Time,@ProjectId,@Confirmed,@Confirmer,@ConfirmedTime);
        END";
        public const string Sql_ExtAlarm_Repository_DeleteEntities = @"
        DECLARE @tpDate DATETIME, 
                @tbName NVARCHAR(255),
                @SQL NVARCHAR(MAX) = N'';

        SET @tpDate = @Start;
        WHILE(DATEDIFF(MM,@tpDate,@End)>=0)
        BEGIN
            SET @tbName = N'[dbo].[H_ExtAlarms'+CONVERT(VARCHAR(6),@tpDate,112)+ N']';
            IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(@tbName) AND type in (N'U'))
            BEGIN
                SET @SQL += N'
		        DELETE FROM ' + @tbName + N' WHERE [Time] BETWEEN ''' + CONVERT(NVARCHAR,@Start,120) + N''' AND ''' + CONVERT(NVARCHAR,@End,120) + N''';';
            END
            SET @tpDate = DATEADD(MM,1,@tpDate);
        END
        EXECUTE sp_executesql @SQL;";
        public const string Sql_ExtAlarm_Repository_SaveActEntities = @"
        UPDATE [dbo].[H_ExtAlarms] SET [ProjectId] = @ProjectId WHERE [Id] = @Id AND [SerialNo] = @SerialNo;
        IF(@@ROWCOUNT = 0)
        BEGIN
	        INSERT INTO [dbo].[H_ExtAlarms]([Id],[SerialNo],[Time],[ProjectId]) VALUES(@Id,@SerialNo,@Time,@ProjectId);
        END";
        public const string Sql_ExtAlarm_Repository_DeleteActEntities = @"DELETE FROM [dbo].[H_ExtAlarms] WHERE [Id] = @Id AND [SerialNo] = @SerialNo;";
        //Formula Repository
        public const string Sql_Formula_Repository_GetEntity = @"SELECT [Id],[Type],[FormulaType],[Formula],[Comment],[CreatedTime] FROM [dbo].[M_Formulas] WHERE [Id]=@Id AND [Type]=@Type AND [FormulaType]=@FormulaType;";
        public const string Sql_Formula_Repository_GetEntities = @"SELECT [Id],[Type],[FormulaType],[Formula],[Comment],[CreatedTime] FROM [dbo].[M_Formulas] WHERE [Id]=@Id AND [Type]=@Type;";
        public const string Sql_Formula_Repository_GetAllEntities = @"SELECT [Id],[Type],[FormulaType],[Formula],[Comment],[CreatedTime] FROM [dbo].[M_Formulas];";
        //NodesInAppointment Repository
        public const string Sql_NodesInAppointment_Repository_GetEntities = @"SELECT [AppointmentId],[NodeId],[NodeType] FROM [dbo].[M_NodesInAppointment];";
        public const string Sql_NodesInAppointment_Repository_GetEntitiesByNodeType = @"SELECT [AppointmentId],[NodeId],[NodeType] FROM [dbo].[M_NodesInAppointment] WHERE [NodeType]=@NodeType;";
        public const string Sql_NodesInAppointment_Repository_GetEntitiesByAppointmentId = @"SELECT [AppointmentId],[NodeId],[NodeType] FROM [dbo].[M_NodesInAppointment] WHERE [AppointmentId]=@AppointmentId;";
        //Project Repository
        public const string Sql_Project_Repository_GetEntities = @"SELECT [Id],[Name],[StartTime],[EndTime],[Responsible],[ContactPhone],[Company],[Creator],[CreatedTime],[Comment],[Enabled] FROM [dbo].[M_Projects] ORDER BY [CreatedTime];";
        public const string Sql_Project_Repository_GetEntitiesByDate = @"SELECT [Id],[Name],[StartTime],[EndTime],[Responsible],[ContactPhone],[Company],[CreatedTime],[Creator],[Comment],[Enabled] FROM [dbo].[M_Projects] WHERE [StartTime]>=@starttime AND [EndTime]<=@endtime ORDER BY [Name];";
        //AmDevice Repository
        public const string Sql_AmDevice_Repository_SaveEntities = @"
        UPDATE [dbo].[AM_Devices] SET [Name] = @Name,[Type] = @Type,[ParentId] = @ParentId,[CreatedTime] = @CreatedTime WHERE [Id] = @Id;
        IF(@@ROWCOUNT=0)
        BEGIN
	        INSERT INTO [dbo].[AM_Devices]([Id],[Name],[Type],[ParentId],[CreatedTime]) VALUES(@Id,@Name,@Type,@ParentId,@CreatedTime);
        END";
        public const string Sql_AmDevice_Repository_DeleteEntities = @"TRUNCATE TABLE [dbo].[AM_Devices];";
        //AmStation Repository
        public const string Sql_AmStation_Repository_SaveEntities = @"
        UPDATE [dbo].[AM_Stations] SET [Name] = @Name,[Type] = @Type,[Parent] = @Parent,[CreatedTime] = @CreatedTime WHERE [Id] = @Id;
        IF(@@ROWCOUNT = 0)
        BEGIN
	        INSERT INTO [dbo].[AM_Stations]([Id],[Name],[Type],[Parent],[CreatedTime]) VALUES(@Id,@Name,@Type,@Parent,@CreatedTime);
        END";
        public const string Sql_AmStation_Repository_DeleteEntities = @"TRUNCATE TABLE [dbo].[AM_Stations];";
    }
}
