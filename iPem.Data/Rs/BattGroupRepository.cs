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
                    entity.FsuCode = SqlTypeConverter.DBNullStringHandler(rdr["FsuCode"]);
                    entity.ProtocolId = SqlTypeConverter.DBNullStringHandler(rdr["ProtocolId"]);
                    entity.Comment = SqlTypeConverter.DBNullStringHandler(rdr["Comment"]);
                    entity.Enabled = SqlTypeConverter.DBNullBooleanHandler(rdr["Enabled"]);
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
                    entity.FsuCode = SqlTypeConverter.DBNullStringHandler(rdr["FsuCode"]);
                    entity.ProtocolId = SqlTypeConverter.DBNullStringHandler(rdr["ProtocolId"]);
                    entity.Comment = SqlTypeConverter.DBNullStringHandler(rdr["Comment"]);
                    entity.Enabled = SqlTypeConverter.DBNullBooleanHandler(rdr["Enabled"]);
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
