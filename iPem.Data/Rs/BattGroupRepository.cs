using iPem.Core;
using iPem.Data.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace iPem.Data {
    public partial class BattGroupRepository {

        #region Fields

        private readonly string _databaseConnectionString;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        public BattGroupRepository() {
            this._databaseConnectionString = SqlHelper.ConnectionStringRsTransaction;
        }

        #endregion

        #region Methods

        public BattGroup GetEntity(string device) {
            SqlParameter[] parms = { new SqlParameter("@Id", SqlDbType.VarChar, 100) };
            parms[0].Value = SqlTypeConverter.DBNullStringChecker(device);

            BattGroup entity = null;
            using(var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Rs.Sql_BattGroup_Repository_GetEntity, parms)) {
                if(rdr.Read()) {
                    entity = new BattGroup();
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

                    var cap = SqlTypeConverter.DBNullStringHandler(rdr["SingGroupCap"]);
                    entity.SingGroupCap = string.IsNullOrWhiteSpace(cap) ? 0d : double.Parse(cap);
                    entity.SingVoltGrade = SqlTypeConverter.DBNullInt32Handler(rdr["SingVoltGrade"]);
                    var num = SqlTypeConverter.DBNullStringHandler(rdr["SingGroupBattNumber"]);
                    entity.SingGroupBattNumber = string.IsNullOrWhiteSpace(num) ? 0 : int.Parse(num);
                }
            }
            return entity;
        }

        public List<BattGroup> GetEntities() {
            var entities = new List<BattGroup>();
            using(var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Rs.Sql_BattGroup_Repository_GetEntities, null)) {
                while(rdr.Read()) {
                    var entity = new BattGroup();
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

                    var cap = SqlTypeConverter.DBNullStringHandler(rdr["SingGroupCap"]);
                    entity.SingGroupCap = string.IsNullOrWhiteSpace(cap) ? 0d : double.Parse(cap);
                    entity.SingVoltGrade = SqlTypeConverter.DBNullInt32Handler(rdr["SingVoltGrade"]);
                    var num = SqlTypeConverter.DBNullStringHandler(rdr["SingGroupBattNumber"]);
                    entity.SingGroupBattNumber = string.IsNullOrWhiteSpace(num) ? 0 : int.Parse(num);
                    entities.Add(entity);
                }
            }
            return entities;
        }

        #endregion

    }
}
