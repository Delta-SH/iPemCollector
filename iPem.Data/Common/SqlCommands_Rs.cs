using System;

namespace iPem.Data.Common {
    public static class SqlCommands_Rs {
        //Area Repository
        public const string Sql_Area_Repository_GetEntity = @"SELECT [Id],[Code],[Name],[NodeLevel],[ParentId],[Desc] AS [Comment],[Enabled] FROM [dbo].[A_Area] WHERE [Id] = @Id;";
        public const string Sql_Area_Repository_GetEntities = @"SELECT [Id],[Code],[Name],[NodeLevel],[ParentId],[Desc] AS [Comment],[Enabled] FROM [dbo].[A_Area];";
        //Device Repository
        public const string Sql_Device_Repository_GetEntity = @"
        WITH Fsus AS (
	        SELECT D.[Id] AS [FsuId],D.[Name] AS [FsuName] FROM [dbo].[D_Device] D INNER JOIN [dbo].[D_FSU] F ON D.[Id] = F.[DeviceId]
        )
        SELECT D.[Id],D.[Code],D.[Name],D.[SysName],D.[SysCode],DT.[Id] AS [DeviceTypeId],DT.[Name] AS [DeviceTypeName],D.[SubDeviceTypeId],SD.[Name] AS [SubDeviceTypeName],D.[Model],D.[ProdId],D.[BrandId],D.[SuppId],D.[SubCompId],D.[StartTime],D.[ScrapTime],D.[StatusId],D.[Contact],S.[AreaId],S.[Id] AS [StationId],S.[Name] AS [StationName],D.[RoomId],R.[Name] AS [RoomName],D.[FsuId],F.[FsuName],D.[ProtocolId],D.[Desc] AS [Comment],D.[Enabled] FROM [dbo].[D_Device] D
        INNER JOIN Fsus F ON D.[FsuId] = F.[FsuId]
        INNER JOIN [dbo].[S_Room] R ON D.[RoomID] = R.[ID]
        INNER JOIN [dbo].[S_Station] S ON R.[StationID] = S.[ID]
        INNER JOIN [dbo].[C_SubDeviceType] SD ON D.[SubDeviceTypeId] = SD.[Id]
        INNER JOIN [dbo].[C_DeviceType] DT ON SD.[DeviceTypeId] = DT.[Id]
        WHERE D.[Id] = @Id;";
        public const string Sql_Device_Repository_GetEntitiesByParent = @"
        WITH Fsus AS (
	        SELECT D.[Id] AS [FsuId],D.[Name] AS [FsuName] FROM [dbo].[D_Device] D INNER JOIN [dbo].[D_FSU] F ON D.[Id] = F.[DeviceId]
        )
        SELECT D.[Id],D.[Code],D.[Name],D.[SysName],D.[SysCode],DT.[Id] AS [DeviceTypeId],DT.[Name] AS [DeviceTypeName],D.[SubDeviceTypeId],SD.[Name] AS [SubDeviceTypeName],D.[Model],D.[ProdId],D.[BrandId],D.[SuppId],D.[SubCompId],D.[StartTime],D.[ScrapTime],D.[StatusId],D.[Contact],S.[AreaId],S.[Id] AS [StationId],S.[Name] AS [StationName],D.[RoomId],R.[Name] AS [RoomName],D.[FsuId],F.[FsuName],D.[ProtocolId],D.[Desc] AS [Comment],D.[Enabled] FROM [dbo].[D_Device] D
        INNER JOIN Fsus F ON D.[FsuId] = F.[FsuId]
        INNER JOIN [dbo].[S_Room] R ON D.[RoomID] = R.[ID]
        INNER JOIN [dbo].[S_Station] S ON R.[StationID] = S.[ID]
        INNER JOIN [dbo].[C_SubDeviceType] SD ON D.[SubDeviceTypeId] = SD.[Id]
        INNER JOIN [dbo].[C_DeviceType] DT ON SD.[DeviceTypeId] = DT.[Id]
        WHERE D.[RoomId]=@Parent;";
        public const string Sql_Device_Repository_GetEntities = @"
        WITH Fsus AS (
	        SELECT D.[Id] AS [FsuId],D.[Name] AS [FsuName] FROM [dbo].[D_Device] D INNER JOIN [dbo].[D_FSU] F ON D.[Id] = F.[DeviceId]
        )
        SELECT D.[Id],D.[Code],D.[Name],D.[SysName],D.[SysCode],DT.[Id] AS [DeviceTypeId],DT.[Name] AS [DeviceTypeName],D.[SubDeviceTypeId],SD.[Name] AS [SubDeviceTypeName],D.[Model],D.[ProdId],D.[BrandId],D.[SuppId],D.[SubCompId],D.[StartTime],D.[ScrapTime],D.[StatusId],D.[Contact],S.[AreaId],S.[Id] AS [StationId],S.[Name] AS [StationName],D.[RoomId],R.[Name] AS [RoomName],D.[FsuId],F.[FsuName],D.[ProtocolId],D.[Desc] AS [Comment],D.[Enabled] FROM [dbo].[D_Device] D
        INNER JOIN Fsus F ON D.[FsuId] = F.[FsuId]
        INNER JOIN [dbo].[S_Room] R ON D.[RoomID] = R.[ID]
        INNER JOIN [dbo].[S_Station] S ON R.[StationID] = S.[ID]
        INNER JOIN [dbo].[C_SubDeviceType] SD ON D.[SubDeviceTypeId] = SD.[Id]
        INNER JOIN [dbo].[C_DeviceType] DT ON SD.[DeviceTypeId] = DT.[Id];";
        //DeviceType Repository
        public const string Sql_DeviceType_Repository_GetEntity = @"SELECT [Id],[Name],[Desc] AS [Comment] FROM [dbo].[C_DeviceType] WHERE [Id] = @Id;";
        public const string Sql_DeviceType_Repository_GetEntities = @"SELECT [Id],[Name],[Desc] AS [Comment] FROM [dbo].[C_DeviceType] ORDER BY [Id];";
        public const string Sql_DeviceType_Repository_GetSubEntity = @"SELECT [Id],[Name],[DeviceTypeId] FROM [dbo].[C_SubDeviceType] WHERE [Id]=@Id;";
        public const string Sql_DeviceType_Repository_GetSubEntities = @"SELECT [Id],[Name],[DeviceTypeId] FROM [dbo].[C_SubDeviceType] ORDER BY [Id];";
        //EnumMethods Repository
        public const string Sql_EnumMethods_Repository_GetEntity = @"SELECT [Id],[Name],[TypeId],[Index],[Desc] AS [Comment] FROM [dbo].[C_EnumMethods] WHERE [Id] = @Id;";
        public const string Sql_EnumMethods_Repository_GetEntitiesByType = @"SELECT [Id],[Name],[TypeId],[Index],[Desc] AS [Comment] FROM [dbo].[C_EnumMethods] WHERE [TypeId] = @TypeId AND [Desc] = @Comment ORDER BY [Index];";
        public const string Sql_EnumMethods_Repository_GetEntities = @"SELECT [Id],[Name],[TypeId],[Index],[Desc] AS [Comment] FROM [dbo].[C_EnumMethods] ORDER BY [TypeId],[Desc],[Index];";
        //Fsu Repository
        public const string Sql_Fsu_Repository_GetEntity = @"
        SELECT D.[Id],D.[Code],D.[Name],D.[SysName],D.[SysCode],DT.[Id] AS [DeviceTypeId],DT.[Name] AS [DeviceTypeName],D.[SubDeviceTypeId],SD.[Name] AS [SubDeviceTypeName],D.[Model],D.[ProdId],D.[BrandId],D.[SuppId],D.[SubCompId],D.[StartTime],D.[ScrapTime],D.[StatusId],D.[Contact],S.[AreaId],S.[Id] AS [StationId],S.[Name] AS [StationName],D.[RoomId],R.[Name] AS [RoomName],D.[FsuId],D.[Name] AS [FsuName],D.[ProtocolId],D.[Desc] AS [Comment],D.[Enabled],F.[Uid],F.[Pwd],F.[FtpUid],F.[FtpPwd],F.[FtpFilePath],F.[FtpAuthority] FROM [dbo].[D_FSU] F
        INNER JOIN [dbo].[D_Device] D ON F.[DeviceId] = D.[Id]
        INNER JOIN [dbo].[S_Room] R ON D.[RoomId] = R.[Id]
        INNER JOIN [dbo].[S_Station] S ON R.[StationId] = S.[Id]
        INNER JOIN [dbo].[C_SubDeviceType] SD ON D.[SubDeviceTypeId] = SD.[Id]
        INNER JOIN [dbo].[C_DeviceType] DT ON SD.[DeviceTypeId] = DT.[Id]
        WHERE F.[DeviceId] = @Id;";
        public const string Sql_Fsu_Repository_GetEntitiesByParent = @"
        SELECT D.[Id],D.[Code],D.[Name],D.[SysName],D.[SysCode],DT.[Id] AS [DeviceTypeId],DT.[Name] AS [DeviceTypeName],D.[SubDeviceTypeId],SD.[Name] AS [SubDeviceTypeName],D.[Model],D.[ProdId],D.[BrandId],D.[SuppId],D.[SubCompId],D.[StartTime],D.[ScrapTime],D.[StatusId],D.[Contact],S.[AreaId],S.[Id] AS [StationId],S.[Name] AS [StationName],D.[RoomId],R.[Name] AS [RoomName],D.[FsuId],D.[Name] AS [FsuName],D.[ProtocolId],D.[Desc] AS [Comment],D.[Enabled],F.[Uid],F.[Pwd],F.[FtpUid],F.[FtpPwd],F.[FtpFilePath],F.[FtpAuthority] FROM [dbo].[D_FSU] F
        INNER JOIN [dbo].[D_Device] D ON F.[DeviceId] = D.[Id]
        INNER JOIN [dbo].[S_Room] R ON D.[RoomId] = R.[Id]
        INNER JOIN [dbo].[S_Station] S ON R.[StationId] = S.[Id]
        INNER JOIN [dbo].[C_SubDeviceType] SD ON D.[SubDeviceTypeId] = SD.[Id]
        INNER JOIN [dbo].[C_DeviceType] DT ON SD.[DeviceTypeId] = DT.[Id]
        WHERE D.[RoomID] = @RoomId;";
        public const string Sql_Fsu_Repository_GetEntities = @"
        SELECT D.[Id],D.[Code],D.[Name],D.[SysName],D.[SysCode],DT.[Id] AS [DeviceTypeId],DT.[Name] AS [DeviceTypeName],D.[SubDeviceTypeId],SD.[Name] AS [SubDeviceTypeName],D.[Model],D.[ProdId],D.[BrandId],D.[SuppId],D.[SubCompId],D.[StartTime],D.[ScrapTime],D.[StatusId],D.[Contact],S.[AreaId],S.[Id] AS [StationId],S.[Name] AS [StationName],D.[RoomId],R.[Name] AS [RoomName],D.[FsuId],D.[Name] AS [FsuName],D.[ProtocolId],D.[Desc] AS [Comment],D.[Enabled],F.[Uid],F.[Pwd],F.[FtpUid],F.[FtpPwd],F.[FtpFilePath],F.[FtpAuthority] FROM [dbo].[D_FSU] F
        INNER JOIN [dbo].[D_Device] D ON F.[DeviceId] = D.[Id]
        INNER JOIN [dbo].[S_Room] R ON D.[RoomId] = R.[Id]
        INNER JOIN [dbo].[S_Station] S ON R.[StationId] = S.[Id]
        INNER JOIN [dbo].[C_SubDeviceType] SD ON D.[SubDeviceTypeId] = SD.[Id]
        INNER JOIN [dbo].[C_DeviceType] DT ON SD.[DeviceTypeId] = DT.[Id];";
        //LogicType Repository
        public const string Sql_LogicType_Repository_GetEntity = @"SELECT [Id],[Name],[DeviceTypeId] FROM [dbo].[C_LogicType] WHERE [Id] = @Id;";
        public const string Sql_LogicType_Repository_GetSubEntity = @"SELECT [Id],[Name],[LogicTypeId] FROM [dbo].[C_SubLogicType] WHERE [Id] = @Id;";
        public const string Sql_LogicType_Repository_GetEntities = @"SELECT [Id],[Name],[DeviceTypeId] FROM [dbo].[C_LogicType] ORDER BY [Id];";
        public const string Sql_LogicType_Repository_GetSubEntitiesByParent = @"SELECT [Id],[Name],[LogicTypeId] FROM [dbo].[C_SubLogicType] WHERE [LogicTypeId] = @LogicTypeId ORDER BY [Id];";
        public const string Sql_LogicType_Repository_GetSubEntities = @"SELECT [Id],[Name],[LogicTypeId] FROM [dbo].[C_SubLogicType] ORDER BY [Id];";
        //Point Repository
        public const string Sql_Point_Repository_GetEntitiesByDevice = @"
        SELECT P.[Id],P.[Code],P.[Name],P.[Type],P.[UnitState],P.[Number],P.[StationTypeId],ST.[Name] AS [StationTypeName],P.[SubDeviceTypeId],SD.[Name] AS [SubDeviceTypeName],DT.[Id] AS [DeviceTypeId],DT.[Name] AS [DeviceTypeName],P.[SubLogicTypeId],SL.[Name] AS [SubLogicTypeName],LT.[Id] AS [LogicTypeId],LT.[Name] AS [LogicTypeName],P.[AlarmTimeDesc] AS [AlarmComment],P.[NormalTimeDesc] AS [NormalComment],P.[AlarmLevel],P.[TriggerTypeId],P.[Interpret],P.[AlarmLimit],P.[AlarmReturnDiff],P.[AlarmRecoveryDelay],P.[AlarmDelay],P.[SavedPeriod],P.[StaticPeriod],P.[AbsoluteThreshold],P.[PerThreshold],P.[Extend1] AS [ExtSet1],P.[Extend2] AS [ExtSet2],P.[Comment],P.[Desc] AS [Description],P.[Enabled] FROM [dbo].[P_Point] P 
        INNER JOIN [dbo].[P_PointsInProtocol] PP ON PP.[PointId] = P.[Id] 
        INNER JOIN [dbo].[D_Device] D ON D.[ProtocolId] = PP.[ProtocolId]
        INNER JOIN [dbo].[C_SubLogicType] SL ON P.[SubLogicTypeId] = SL.[Id]
        INNER JOIN [dbo].[C_LogicType] LT ON SL.[LogicTypeId] = LT.[Id]
        INNER JOIN [dbo].[C_SubDeviceType] SD ON P.[SubDeviceTypeId] = SD.[Id]
        INNER JOIN [dbo].[C_DeviceType] DT ON SD.[DeviceTypeId] = DT.[Id]
        INNER JOIN [dbo].[C_StationType] ST ON P.[StationTypeId] = ST.[Id]
        WHERE D.[Id] = @DeviceId;";
        public const string Sql_Point_Repository_GetEntitiesByProtcol = @"
        SELECT P.[Id],P.[Code],P.[Name],P.[Type],P.[UnitState],P.[Number],P.[StationTypeId],ST.[Name] AS [StationTypeName],P.[SubDeviceTypeId],SD.[Name] AS [SubDeviceTypeName],DT.[Id] AS [DeviceTypeId],DT.[Name] AS [DeviceTypeName],P.[SubLogicTypeId],SL.[Name] AS [SubLogicTypeName],LT.[Id] AS [LogicTypeId],LT.[Name] AS [LogicTypeName],P.[AlarmTimeDesc] AS [AlarmComment],P.[NormalTimeDesc] AS [NormalComment],P.[AlarmLevel],P.[TriggerTypeId],P.[Interpret],P.[AlarmLimit],P.[AlarmReturnDiff],P.[AlarmRecoveryDelay],P.[AlarmDelay],P.[SavedPeriod],P.[StaticPeriod],P.[AbsoluteThreshold],P.[PerThreshold],P.[Extend1] AS [ExtSet1],P.[Extend2] AS [ExtSet2],P.[Comment],P.[Desc] AS [Description],P.[Enabled] FROM [dbo].[P_Point] P 
        INNER JOIN [dbo].[P_PointsInProtocol] PP ON PP.[PointId] = P.[Id] 
        INNER JOIN [dbo].[C_SubLogicType] SL ON P.[SubLogicTypeId] = SL.[Id]
        INNER JOIN [dbo].[C_LogicType] LT ON SL.[LogicTypeId] = LT.[Id]
        INNER JOIN [dbo].[C_SubDeviceType] SD ON P.[SubDeviceTypeId] = SD.[Id]
        INNER JOIN [dbo].[C_DeviceType] DT ON SD.[DeviceTypeId] = DT.[Id]
        INNER JOIN [dbo].[C_StationType] ST ON P.[StationTypeId] = ST.[Id]
        WHERE PP.[ProtocolId] = @ProtocolId;";
        public const string Sql_Point_Repository_GetEntities = @"
        SELECT P.[Id],P.[Code],P.[Name],P.[Type],P.[UnitState],P.[Number],P.[StationTypeId],ST.[Name] AS [StationTypeName],P.[SubDeviceTypeId],SD.[Name] AS [SubDeviceTypeName],DT.[Id] AS [DeviceTypeId],DT.[Name] AS [DeviceTypeName],P.[SubLogicTypeId],SL.[Name] AS [SubLogicTypeName],LT.[Id] AS [LogicTypeId],LT.[Name] AS [LogicTypeName],P.[AlarmTimeDesc] AS [AlarmComment],P.[NormalTimeDesc] AS [NormalComment],P.[AlarmLevel],P.[TriggerTypeId],P.[Interpret],P.[AlarmLimit],P.[AlarmReturnDiff],P.[AlarmRecoveryDelay],P.[AlarmDelay],P.[SavedPeriod],P.[StaticPeriod],P.[AbsoluteThreshold],P.[PerThreshold],P.[Extend1] AS [ExtSet1],P.[Extend2] AS [ExtSet2],P.[Comment],P.[Desc] AS [Description],P.[Enabled] FROM [dbo].[P_Point] P 
        INNER JOIN [dbo].[C_SubLogicType] SL ON P.[SubLogicTypeId] = SL.[Id]
        INNER JOIN [dbo].[C_LogicType] LT ON SL.[LogicTypeId] = LT.[Id]
        INNER JOIN [dbo].[C_SubDeviceType] SD ON P.[SubDeviceTypeId] = SD.[Id]
        INNER JOIN [dbo].[C_DeviceType] DT ON SD.[DeviceTypeId] = DT.[Id]
        INNER JOIN [dbo].[C_StationType] ST ON P.[StationTypeId] = ST.[Id];";
        //Protocol Repository
        public const string Sql_Protocol_Repository_GetEntities = @"
        SELECT PP.[Id],PP.[Name],DT.[Id] AS [DeviceTypeId],DT.[Name] AS [DeviceTypeName],PP.[SubDevTypeId] AS [SubDeviceTypeId],SD.[Name] AS [SubDeviceTypeName],PP.[Desc] AS [Comment],PP.[Enabled] FROM [dbo].[P_Protocol] PP
        INNER JOIN [dbo].[C_SubDeviceType] SD ON PP.[SubDevTypeId] = SD.[Id] 
        INNER JOIN [dbo].[C_DeviceType] DT ON SD.[DeviceTypeId] = DT.[Id];";
        //Room Repository
        public const string Sql_Room_Repository_GetEntity = @"
        SELECT R.[Id],R.[Code],R.[Name],R.[RoomTypeId],RT.[Name] AS [RoomTypeName],R.[Floor],R.[PropertyId],R.[Address],R.[Length],R.[Width],R.[Heigth],R.[FloorLoad],R.[LineHeigth],R.[Square],R.[EffeSquare],R.[FireFighEuip],R.[Owner],R.[QueryPhone],R.[PowerSubMain],R.[TranSubMain],R.[EnviSubMain],R.[FireSubMain],R.[AirSubMain],R.[Contact],S.[AreaId],R.[StationId],S.[Name] AS [StationName],R.[Desc] AS [Comment],R.[Enabled] FROM [dbo].[S_Room] R
        INNER JOIN [dbo].[C_RoomType] RT ON R.[RoomTypeId] = RT.[Id]
        INNER JOIN [dbo].[S_Station] S ON R.[StationId] = S.[Id]
        WHERE R.[Id]=@Id;";
        public const string Sql_Room_Repository_GetEntitiesByParent = @"
        SELECT R.[Id],R.[Code],R.[Name],R.[RoomTypeId],RT.[Name] AS [RoomTypeName],R.[Floor],R.[PropertyId],R.[Address],R.[Length],R.[Width],R.[Heigth],R.[FloorLoad],R.[LineHeigth],R.[Square],R.[EffeSquare],R.[FireFighEuip],R.[Owner],R.[QueryPhone],R.[PowerSubMain],R.[TranSubMain],R.[EnviSubMain],R.[FireSubMain],R.[AirSubMain],R.[Contact],S.[AreaId],R.[StationId],S.[Name] AS [StationName],R.[Desc] AS [Comment],R.[Enabled] FROM [dbo].[S_Room] R
        INNER JOIN [dbo].[C_RoomType] RT ON R.[RoomTypeId] = RT.[Id]
        INNER JOIN [dbo].[S_Station] S ON R.[StationId] = S.[Id]
        WHERE R.[StationId]=@StationId;";
        public const string Sql_Room_Repository_GetEntities = @"
        SELECT R.[Id],R.[Code],R.[Name],R.[RoomTypeId],RT.[Name] AS [RoomTypeName],R.[Floor],R.[PropertyId],R.[Address],R.[Length],R.[Width],R.[Heigth],R.[FloorLoad],R.[LineHeigth],R.[Square],R.[EffeSquare],R.[FireFighEuip],R.[Owner],R.[QueryPhone],R.[PowerSubMain],R.[TranSubMain],R.[EnviSubMain],R.[FireSubMain],R.[AirSubMain],R.[Contact],S.[AreaId],R.[StationId],S.[Name] AS [StationName],R.[Desc] AS [Comment],R.[Enabled] FROM [dbo].[S_Room] R
        INNER JOIN [dbo].[C_RoomType] RT ON R.[RoomTypeId] = RT.[Id]
        INNER JOIN [dbo].[S_Station] S ON R.[StationId] = S.[Id];";
        //RoomType Repository
        public const string Sql_RoomType_Repository_GetEntity = @"SELECT [Id],[Name],[Desc] AS [Comment] FROM [dbo].[C_RoomType] WHERE [Id]=@Id;";
        public const string Sql_RoomType_Repository_GetEntities = @"SELECT [Id],[Name],[Desc] AS [Comment] FROM [dbo].[C_RoomType] ORDER BY [Id];";
        //Station Repository
        public const string Sql_Station_Repository_GetEntity = @"
        SELECT S.[Id],S.[Code],S.[Name],S.[StaTypeId],ST.[Name] AS [StaTypeName],S.[Longitude],S.[Latitude],S.[Altitude],S.[CityElecLoadTypeId],S.[CityElecCap],S.[CityElecLoad],S.[Contact],S.[LineRadiusSize],S.[LineLength],S.[SuppPowerTypeId],S.[TranInfo],S.[TranContNo],S.[TranPhone],S.[AreaId],S.[Desc] AS [Comment],S.[Enabled] FROM [dbo].[S_Station] S 
        INNER JOIN [dbo].[C_StationType] ST ON S.[StaTypeId] = ST.[Id]
        WHERE S.[Id]=@Id;";
        public const string Sql_Station_Repository_GetEntitiesByParent = @"
        SELECT S.[Id],S.[Code],S.[Name],S.[StaTypeId],ST.[Name] AS [StaTypeName],S.[Longitude],S.[Latitude],S.[Altitude],S.[CityElecLoadTypeId],S.[CityElecCap],S.[CityElecLoad],S.[Contact],S.[LineRadiusSize],S.[LineLength],S.[SuppPowerTypeId],S.[TranInfo],S.[TranContNo],S.[TranPhone],S.[AreaId],S.[Desc] AS [Comment],S.[Enabled] FROM [dbo].[S_Station] S 
        INNER JOIN [dbo].[C_StationType] ST ON S.[StaTypeId] = ST.[Id]
        WHERE S.[AreaId]=@AreaId;";
        public const string Sql_Station_Repository_GetEntities = @"
        SELECT S.[Id],S.[Code],S.[Name],S.[StaTypeId],ST.[Name] AS [StaTypeName],S.[Longitude],S.[Latitude],S.[Altitude],S.[CityElecLoadTypeId],S.[CityElecCap],S.[CityElecLoad],S.[Contact],S.[LineRadiusSize],S.[LineLength],S.[SuppPowerTypeId],S.[TranInfo],S.[TranContNo],S.[TranPhone],S.[AreaId],S.[Desc] AS [Comment],S.[Enabled] FROM [dbo].[S_Station] S 
        INNER JOIN [dbo].[C_StationType] ST ON S.[StaTypeId] = ST.[Id];";
        //StationType Repository
        public const string Sql_StationType_Repository_GetEntity = @"SELECT [Id],[Name],[Desc] AS [Comment] FROM [dbo].[C_StationType] WHERE [Id]=@Id;";
        public const string Sql_StationType_Repository_GetEntities = @"SELECT [Id],[Name],[Desc] AS [Comment] FROM [dbo].[C_StationType] ORDER BY [Id];";
        //BattGroup Repository
        public const string Sql_BattGroup_Repository_GetEntities = @"
        WITH Fsus AS (
	        SELECT D.[Id] AS [FsuId],D.[Name] AS [FsuName] FROM [dbo].[D_Device] D INNER JOIN [dbo].[D_FSU] F ON D.[Id] = F.[DeviceId]
        )
        SELECT D.[Id],D.[Code],D.[Name],D.[SysName],D.[SysCode],DT.[Id] AS [DeviceTypeId],DT.[Name] AS [DeviceTypeName],D.[SubDeviceTypeId],SD.[Name] AS [SubDeviceTypeName],D.[Model],D.[ProdId],D.[BrandId],D.[SuppId],D.[SubCompId],D.[StartTime],D.[ScrapTime],D.[StatusId],D.[Contact],S.[AreaId],S.[Id] AS [StationId],S.[Name] AS [StationName],D.[RoomId],R.[Name] AS [RoomName],D.[FsuId],F.[FsuName],D.[ProtocolId],D.[Desc] AS [Comment],D.[Enabled],BG.[SingGroupCap],BG.[SingVoltGrade],BG.[SingGroupBattNumber] FROM [dbo].[D_Device] D
        INNER JOIN [dbo].[D_BattGroup] BG ON D.[Id] = BG.[DeviceID]
        INNER JOIN Fsus F ON D.[FsuId] = F.[FsuId]
        INNER JOIN [dbo].[S_Room] R ON D.[RoomID] = R.[ID]
        INNER JOIN [dbo].[S_Station] S ON R.[StationID] = S.[ID]
        INNER JOIN [dbo].[C_SubDeviceType] SD ON D.[SubDeviceTypeId] = SD.[Id]
        INNER JOIN [dbo].[C_DeviceType] DT ON SD.[DeviceTypeId] = DT.[Id];";
        //CombSwitElecSour Repository
        public const string Sql_CombSwitElecSour_Repository_GetEntities = @"
        WITH Fsus AS (
	        SELECT D.[Id] AS [FsuId],D.[Name] AS [FsuName] FROM [dbo].[D_Device] D INNER JOIN [dbo].[D_FSU] F ON D.[Id] = F.[DeviceId]
        )
        SELECT D.[Id],D.[Code],D.[Name],D.[SysName],D.[SysCode],DT.[Id] AS [DeviceTypeId],DT.[Name] AS [DeviceTypeName],D.[SubDeviceTypeId],SD.[Name] AS [SubDeviceTypeName],D.[Model],D.[ProdId],D.[BrandId],D.[SuppId],D.[SubCompId],D.[StartTime],D.[ScrapTime],D.[StatusId],D.[Contact],S.[AreaId],S.[Id] AS [StationId],S.[Name] AS [StationName],D.[RoomId],R.[Name] AS [RoomName],D.[FsuId],F.[FsuName],D.[ProtocolId],D.[Desc] AS [Comment],D.[Enabled],SP.[RatedOutputVolt],SP.[MoniModuleModel],SP.[ExisRModuleCount],SP.[RModuleModel],SP.[RModuleRatedWorkVolt],SP.[SingRModuleRatedOPCap],SP.[SingGBattGFuseCap],SP.[BattGFuseGNumber],SP.[OrCanSecoDownPower] FROM [dbo].[D_Device] D
        INNER JOIN [dbo].[D_CombSwitElecSour] SP ON D.[Id] = SP.[DeviceID]
        INNER JOIN Fsus F ON D.[FsuId] = F.[FsuId]
        INNER JOIN [dbo].[S_Room] R ON D.[RoomID] = R.[ID]
        INNER JOIN [dbo].[S_Station] S ON R.[StationID] = S.[ID]
        INNER JOIN [dbo].[C_SubDeviceType] SD ON D.[SubDeviceTypeId] = SD.[Id]
        INNER JOIN [dbo].[C_DeviceType] DT ON SD.[DeviceTypeId] = DT.[Id];";
        //DivSwitElecSour Repository
        public const string Sql_DivSwitElecSour_Repository_GetEntities = @"
        WITH Fsus AS (
	        SELECT D.[Id] AS [FsuId],D.[Name] AS [FsuName] FROM [dbo].[D_Device] D INNER JOIN [dbo].[D_FSU] F ON D.[Id] = F.[DeviceId]
        )
        SELECT D.[Id],D.[Code],D.[Name],D.[SysName],D.[SysCode],DT.[Id] AS [DeviceTypeId],DT.[Name] AS [DeviceTypeName],D.[SubDeviceTypeId],SD.[Name] AS [SubDeviceTypeName],D.[Model],D.[ProdId],D.[BrandId],D.[SuppId],D.[SubCompId],D.[StartTime],D.[ScrapTime],D.[StatusId],D.[Contact],S.[AreaId],S.[Id] AS [StationId],S.[Name] AS [StationName],D.[RoomId],R.[Name] AS [RoomName],D.[FsuId],F.[FsuName],D.[ProtocolId],D.[Desc] AS [Comment],D.[Enabled],SP.[RatedOutputVolt],SP.[MoniModuleModel],SP.[ExisRModuleCount],SP.[RModuleModel],SP.[RModuleRatedWorkVolt],SP.[SingRModuleRatedOPCap],SP.[SingGBattGFuseCap],SP.[BattGFuseGNumber],SP.[OPDistBoardModel],SP.[OPDistBoardNumber] FROM [dbo].[D_Device] D
        INNER JOIN [dbo].[D_DivSwitElecSour] SP ON D.[Id] = SP.[DeviceID]
        INNER JOIN Fsus F ON D.[FsuId] = F.[FsuId]
        INNER JOIN [dbo].[S_Room] R ON D.[RoomID] = R.[ID]
        INNER JOIN [dbo].[S_Station] S ON R.[StationID] = S.[ID]
        INNER JOIN [dbo].[C_SubDeviceType] SD ON D.[SubDeviceTypeId] = SD.[Id]
        INNER JOIN [dbo].[C_DeviceType] DT ON SD.[DeviceTypeId] = DT.[Id];";
    }
}
