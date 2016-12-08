using iPem.Core;
using iPem.Data.Common;
using System;
using System.Collections.Generic;
using System.Data;

namespace iPem.Data {
    public partial class CombSwitElecSourRepository {

        #region Fields

        private readonly string _databaseConnectionString;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        public CombSwitElecSourRepository() {
            this._databaseConnectionString = SqlHelper.ConnectionStringRsTransaction;
        }

        #endregion

        #region Methods

        public List<CombSwitElecSour> GetEntities() {
            var entities = new List<CombSwitElecSour>();
            using(var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Rs.Sql_CombSwitElecSour_Repository_GetEntities, null)) {
                while(rdr.Read()) {
                    var entity = new CombSwitElecSour();
                    entity.Id = SqlTypeConverter.DBNullStringHandler(rdr["Id"]);
                    entity.Code = SqlTypeConverter.DBNullStringHandler(rdr["Code"]);
                    entity.Name = SqlTypeConverter.DBNullStringHandler(rdr["Name"]);
                    entity.SysName = SqlTypeConverter.DBNullStringHandler(rdr["SysName"]);
                    entity.SysCode = SqlTypeConverter.DBNullStringHandler(rdr["SysCode"]);
                    entity.Type = new DeviceType { Id = SqlTypeConverter.DBNullStringHandler(rdr["DeviceTypeId"]), Name = SqlTypeConverter.DBNullStringHandler(rdr["DeviceTypeName"]) };
                    entity.SubType = new SubDeviceType { Id = SqlTypeConverter.DBNullStringHandler(rdr["SubDeviceTypeId"]), Name = SqlTypeConverter.DBNullStringHandler(rdr["SubDeviceTypeName"]) };
                    entity.Model = SqlTypeConverter.DBNullStringHandler(rdr["Model"]);
                    entity.ProdId = SqlTypeConverter.DBNullStringHandler(rdr["ProdId"]);
                    entity.BrandId = SqlTypeConverter.DBNullStringHandler(rdr["BrandId"]);
                    entity.SuppId = SqlTypeConverter.DBNullStringHandler(rdr["SuppId"]);
                    entity.SubCompId = SqlTypeConverter.DBNullStringHandler(rdr["SubCompId"]);
                    entity.StartTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["StartTime"]);
                    entity.ScrapTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["ScrapTime"]);
                    entity.StatusId = SqlTypeConverter.DBNullInt32Handler(rdr["StatusId"]);
                    entity.Contact = SqlTypeConverter.DBNullStringHandler(rdr["Contact"]);
                    entity.AreaId = SqlTypeConverter.DBNullStringHandler(rdr["AreaId"]);
                    entity.StationId = SqlTypeConverter.DBNullStringHandler(rdr["StationId"]);
                    entity.StationName = SqlTypeConverter.DBNullStringHandler(rdr["StationName"]);
                    entity.RoomId = SqlTypeConverter.DBNullStringHandler(rdr["RoomId"]);
                    entity.RoomName = SqlTypeConverter.DBNullStringHandler(rdr["RoomName"]);
                    entity.FsuId = SqlTypeConverter.DBNullStringHandler(rdr["FsuId"]);
                    entity.FsuName = SqlTypeConverter.DBNullStringHandler(rdr["FsuName"]);
                    entity.ProtocolId = SqlTypeConverter.DBNullStringHandler(rdr["ProtocolId"]);
                    entity.Comment = SqlTypeConverter.DBNullStringHandler(rdr["Comment"]);
                    entity.Enabled = SqlTypeConverter.DBNullBooleanHandler(rdr["Enabled"]);
                    entity.RatedOutputVolt = SqlTypeConverter.DBNullDoubleHandler(rdr["RatedOutputVolt"]);
                    entity.MoniModuleModel = SqlTypeConverter.DBNullStringHandler(rdr["MoniModuleModel"]);
                    entity.ExisRModuleCount = SqlTypeConverter.DBNullStringHandler(rdr["ExisRModuleCount"]);
                    entity.RModuleModel = SqlTypeConverter.DBNullStringHandler(rdr["RModuleModel"]);
                    entity.RModuleRatedWorkVolt = SqlTypeConverter.DBNullInt32Handler(rdr["RModuleRatedWorkVolt"]);
                    entity.SingRModuleRatedOPCap = SqlTypeConverter.DBNullStringHandler(rdr["SingRModuleRatedOPCap"]);
                    entity.SingGBattGFuseCap = SqlTypeConverter.DBNullStringHandler(rdr["SingGBattGFuseCap"]);
                    entity.BattGFuseGNumber = SqlTypeConverter.DBNullInt32Handler(rdr["BattGFuseGNumber"]);
                    entity.OrCanSecoDownPower = SqlTypeConverter.DBNullStringHandler(rdr["OrCanSecoDownPower"]);
                    entities.Add(entity);
                }
            }
            return entities;
        }

        #endregion

    }
}
