using System;

namespace iPem.Data.Common {
    public static class SqlCommands_Sc {
        /// <summary>
        /// Reservation Repository
        /// </summary>
        public const string Sql_Reservation_Repository_GetEntities1 = @"SELECT * FROM [dbo].[M_Reservations] WHERE [Enabled] = 1 ORDER BY [StartTime];";
        public const string Sql_Reservation_Repository_GetEntities2 = @"SELECT * FROM [dbo].[M_Reservations] WHERE [StartTime] IS NOT NULL AND [StartTime] >= @StartTime AND [Enabled] = 1 ORDER BY [StartTime];";
        
        /// <summary>
        /// Dictionary Repository
        /// </summary>
        public const string Sql_Dictionary_Repository_GetEntity = @"SELECT * FROM [dbo].[M_Dictionary] WHERE [Id]=@Id;";
        public const string Sql_Dictionary_Repository_GetEntities = @"SELECT * FROM [dbo].[M_Dictionary];";
        
        /// <summary>
        /// Formula Repository
        /// </summary>
        public const string Sql_Formula_Repository_GetEntity = @"SELECT * FROM [dbo].[M_Formulas] WHERE [Id]=@Id AND [Type]=@Type AND [FormulaType]=@FormulaType;";
        public const string Sql_Formula_Repository_GetEntities = @"SELECT * FROM [dbo].[M_Formulas] WHERE [Id]=@Id AND [Type]=@Type;";
        public const string Sql_Formula_Repository_GetAllEntities = @"SELECT * FROM [dbo].[M_Formulas];";
        
        /// <summary>
        /// NodesInReservation Repository
        /// </summary>
        public const string Sql_NodesInReservation_Repository_GetEntities = @"SELECT * FROM [dbo].[M_NodesInReservation];";
        public const string Sql_NodesInReservation_Repository_GetEntitiesByNodeType = @"SELECT * FROM [dbo].[M_NodesInReservation] WHERE [NodeType]=@NodeType;";
        public const string Sql_NodesInReservation_Repository_GetEntitiesById = @"SELECT * FROM [dbo].[M_NodesInReservation] WHERE [ReservationId]=@ReservationId;";
        
        /// <summary>
        /// Project Repository
        /// </summary>
        public const string Sql_Project_Repository_GetEntity = @"SELECT * FROM [dbo].[M_Projects] WHERE [Id]=@Id;";        
        public const string Sql_Project_Repository_GetEntities = @"SELECT * FROM [dbo].[M_Projects] ORDER BY [CreatedTime];";
        public const string Sql_Project_Repository_GetEntitiesByDate = @"SELECT * FROM [dbo].[M_Projects] WHERE [StartTime]>=@starttime AND [EndTime]<=@endtime ORDER BY [Name];";

        /// <summary>
        /// SerialNo Repository
        /// </summary>
        public const string Sql_ASerialNo_Repository_IncrAndGet = @"
        BEGIN TRAN
        DECLARE @ERR BIGINT = -1;
        IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[X_ASerialNo]') AND type in (N'U'))
        BEGIN
            SELECT @ERR AS [Code];
            ROLLBACK TRAN;
        END
        ELSE
        BEGIN
        BEGIN TRY
	        DELETE FROM [dbo].[X_ASerialNo] WHERE [Name] = @Name;
	        INSERT INTO [dbo].[X_ASerialNo]([Name],[CreatedTime]) VALUES(@Name,GETDATE());
	        SELECT [Code] FROM [dbo].[X_ASerialNo] WHERE [Name] = @Name;
	        COMMIT TRAN;
        END TRY
        BEGIN CATCH
	        SELECT @ERR AS [Code];
	        ROLLBACK TRAN;
        END CATCH
        END";
    }
}
