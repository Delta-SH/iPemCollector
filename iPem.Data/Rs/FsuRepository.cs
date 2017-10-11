using iPem.Core;
using iPem.Data.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace iPem.Data {
    public partial class FsuRepository {

        #region Fields

        private readonly string _databaseConnectionString;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        public FsuRepository() {
            this._databaseConnectionString = SqlHelper.ConnectionStringRsTransaction;
        }

        #endregion

        #region Methods

        public Fsu GetEntity(string id) {
            SqlParameter[] parms = { new SqlParameter("@Id", SqlDbType.VarChar, 100) };
            parms[0].Value = SqlTypeConverter.DBNullStringChecker(id);

            Fsu entity = null;
            using (var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Rs.Sql_Fsu_Repository_GetEntity, parms)) {
                if (rdr.Read()) {
                    entity = new Fsu();
                    entity.Id = SqlTypeConverter.DBNullStringHandler(rdr["Id"]);
                    entity.Code = SqlTypeConverter.DBNullStringHandler(rdr["Code"]);
                    entity.Name = SqlTypeConverter.DBNullStringHandler(rdr["Name"]);
                    entity.AreaId = SqlTypeConverter.DBNullStringHandler(rdr["AreaId"]);
                    entity.AreaName = SqlTypeConverter.DBNullStringHandler(rdr["AreaName"]);
                    entity.StationId = SqlTypeConverter.DBNullStringHandler(rdr["StationId"]);
                    entity.StationName = SqlTypeConverter.DBNullStringHandler(rdr["StationName"]);
                    entity.StaTypeId = SqlTypeConverter.DBNullStringHandler(rdr["StaTypeId"]);
                    entity.RoomId = SqlTypeConverter.DBNullStringHandler(rdr["RoomId"]);
                    entity.RoomName = SqlTypeConverter.DBNullStringHandler(rdr["RoomName"]);
                    entity.Comment = SqlTypeConverter.DBNullStringHandler(rdr["Comment"]);

                    entity.VendorId = SqlTypeConverter.DBNullStringHandler(rdr["VendorId"]);
                    entity.VendorName = SqlTypeConverter.DBNullStringHandler(rdr["VendorName"]);
                    entity.Uid = SqlTypeConverter.DBNullStringHandler(rdr["Uid"]);
                    entity.Pwd = SqlTypeConverter.DBNullStringHandler(rdr["Pwd"]);
                    entity.FtpUid = SqlTypeConverter.DBNullStringHandler(rdr["FtpUid"]);
                    entity.FtpPwd = SqlTypeConverter.DBNullStringHandler(rdr["FtpPwd"]);
                    entity.FtpFilePath = SqlTypeConverter.DBNullStringHandler(rdr["FtpFilePath"]);
                    entity.FtpAuthority = SqlTypeConverter.DBNullInt32Handler(rdr["FtpAuthority"]);
                }
            }
            return entity;
        }

        public List<Fsu> GetEntities() {
            var entities = new List<Fsu>();
            using (var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Rs.Sql_Fsu_Repository_GetEntities, null)) {
                while (rdr.Read()) {
                    var entity = new Fsu();
                    entity.Id = SqlTypeConverter.DBNullStringHandler(rdr["Id"]);
                    entity.Code = SqlTypeConverter.DBNullStringHandler(rdr["Code"]);
                    entity.Name = SqlTypeConverter.DBNullStringHandler(rdr["Name"]);
                    entity.AreaId = SqlTypeConverter.DBNullStringHandler(rdr["AreaId"]);
                    entity.AreaName = SqlTypeConverter.DBNullStringHandler(rdr["AreaName"]);
                    entity.StationId = SqlTypeConverter.DBNullStringHandler(rdr["StationId"]);
                    entity.StationName = SqlTypeConverter.DBNullStringHandler(rdr["StationName"]);
                    entity.StaTypeId = SqlTypeConverter.DBNullStringHandler(rdr["StaTypeId"]);
                    entity.RoomId = SqlTypeConverter.DBNullStringHandler(rdr["RoomId"]);
                    entity.RoomName = SqlTypeConverter.DBNullStringHandler(rdr["RoomName"]);
                    entity.Comment = SqlTypeConverter.DBNullStringHandler(rdr["Comment"]);

                    entity.VendorId = SqlTypeConverter.DBNullStringHandler(rdr["VendorId"]);
                    entity.VendorName = SqlTypeConverter.DBNullStringHandler(rdr["VendorName"]);
                    entity.Uid = SqlTypeConverter.DBNullStringHandler(rdr["Uid"]);
                    entity.Pwd = SqlTypeConverter.DBNullStringHandler(rdr["Pwd"]);
                    entity.FtpUid = SqlTypeConverter.DBNullStringHandler(rdr["FtpUid"]);
                    entity.FtpPwd = SqlTypeConverter.DBNullStringHandler(rdr["FtpPwd"]);
                    entity.FtpFilePath = SqlTypeConverter.DBNullStringHandler(rdr["FtpFilePath"]);
                    entity.FtpAuthority = SqlTypeConverter.DBNullInt32Handler(rdr["FtpAuthority"]);
                    entities.Add(entity);
                }
            }
            return entities;
        }

        public ExtFsu GetExtEntity(string id) {
            SqlParameter[] parms = { new SqlParameter("@Id", SqlDbType.VarChar, 100) };
            parms[0].Value = SqlTypeConverter.DBNullStringChecker(id);

            ExtFsu entity = null;
            using (var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Rs.Sql_Fsu_Repository_GetExtEntity, parms)) {
                if (rdr.Read()) {
                    entity = new ExtFsu();
                    entity.Id = SqlTypeConverter.DBNullStringHandler(rdr["Id"]);
                    entity.IP = SqlTypeConverter.DBNullStringHandler(rdr["IP"]);
                    entity.Port = SqlTypeConverter.DBNullInt32Handler(rdr["Port"]);
                    entity.ChangeTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["ChangeTime"]);
                    entity.LastTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["LastTime"]);
                    entity.Status = SqlTypeConverter.DBNullBooleanHandler(rdr["Status"]);
                    entity.GroupId = SqlTypeConverter.DBNullStringHandler(rdr["GroupId"]);
                }
            }
            return entity;
        }

        public List<ExtFsu> GetExtEntities() {
            var entities = new List<ExtFsu>();
            using (var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Rs.Sql_Fsu_Repository_GetExtEntities, null)) {
                while (rdr.Read()) {
                    var entity = new ExtFsu();
                    entity.Id = SqlTypeConverter.DBNullStringHandler(rdr["Id"]);
                    entity.IP = SqlTypeConverter.DBNullStringHandler(rdr["IP"]);
                    entity.Port = SqlTypeConverter.DBNullInt32Handler(rdr["Port"]);
                    entity.ChangeTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["ChangeTime"]);
                    entity.LastTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["LastTime"]);
                    entity.Status = SqlTypeConverter.DBNullBooleanHandler(rdr["Status"]);
                    entity.GroupId = SqlTypeConverter.DBNullStringHandler(rdr["GroupId"]);
                    entities.Add(entity);
                }
            }
            return entities;
        }

        #endregion

    }
}