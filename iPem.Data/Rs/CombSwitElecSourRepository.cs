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
                while (rdr.Read()) {
                    var entity = new CombSwitElecSour();
                    entity.Id = SqlTypeConverter.DBNullStringHandler(rdr["Id"]);
                    entity.Name = SqlTypeConverter.DBNullStringHandler(rdr["Name"]);
                    entity.Type = new DeviceType { Id = SqlTypeConverter.DBNullStringHandler(rdr["DeviceTypeId"]), Name = SqlTypeConverter.DBNullStringHandler(rdr["DeviceTypeName"]) };
                    entity.SubType = new SubDeviceType { Id = SqlTypeConverter.DBNullStringHandler(rdr["SubDeviceTypeId"]), Name = SqlTypeConverter.DBNullStringHandler(rdr["SubDeviceTypeName"]) };
                    entity.AreaId = SqlTypeConverter.DBNullStringHandler(rdr["AreaId"]);
                    entity.AreaName = SqlTypeConverter.DBNullStringHandler(rdr["AreaName"]);
                    entity.StationId = SqlTypeConverter.DBNullStringHandler(rdr["StationId"]);
                    entity.StationName = SqlTypeConverter.DBNullStringHandler(rdr["StationName"]);
                    entity.StationType = new StationType { Id = SqlTypeConverter.DBNullStringHandler(rdr["StationTypeId"]), Name = SqlTypeConverter.DBNullStringHandler(rdr["StationTypeName"]) };
                    entity.RoomId = SqlTypeConverter.DBNullStringHandler(rdr["RoomId"]);
                    entity.RoomName = SqlTypeConverter.DBNullStringHandler(rdr["RoomName"]);
                    entity.FsuId = SqlTypeConverter.DBNullStringHandler(rdr["FsuId"]);
                    entity.Comment = SqlTypeConverter.DBNullStringHandler(rdr["Comment"]);

                    entity.RatedOutputVolt = SqlTypeConverter.DBNullDoubleHandler(rdr["RatedOutputVolt"]);
                    entity.MoniModuleModel = SqlTypeConverter.DBNullStringHandler(rdr["MoniModuleModel"]);
                    entity.RModuleModel = SqlTypeConverter.DBNullStringHandler(rdr["RModuleModel"]);
                    entity.RModuleRatedWorkVolt = SqlTypeConverter.DBNullInt32Handler(rdr["RModuleRatedWorkVolt"]);
                    entity.BattGFuseGNumber = SqlTypeConverter.DBNullInt32Handler(rdr["BattGFuseGNumber"]);
                    entity.OrCanSecoDownPower = SqlTypeConverter.DBNullStringHandler(rdr["OrCanSecoDownPower"]);

                    int exisRModuleCount; double singRModuleRatedOPCap, singGBattGFuseCap;
                    if (int.TryParse(SqlTypeConverter.DBNullStringHandler(rdr["ExisRModuleCount"]), out exisRModuleCount))
                        entity.ExisRModuleCount = exisRModuleCount;
                    else
                        entity.ExisRModuleCount = 0;

                    if (double.TryParse(SqlTypeConverter.DBNullStringHandler(rdr["SingRModuleRatedOPCap"]), out singRModuleRatedOPCap))
                        entity.SingRModuleRatedOPCap = singRModuleRatedOPCap;
                    else
                        entity.SingRModuleRatedOPCap = 0;

                    if (double.TryParse(SqlTypeConverter.DBNullStringHandler(rdr["SingGBattGFuseCap"]), out singGBattGFuseCap))
                        entity.SingGBattGFuseCap = singGBattGFuseCap;
                    else
                        entity.SingGBattGFuseCap = 0;

                    entities.Add(entity);
                }
            }
            return entities;
        }

        #endregion

    }
}
