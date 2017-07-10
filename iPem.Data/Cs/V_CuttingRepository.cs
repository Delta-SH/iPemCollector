﻿using iPem.Core;
using iPem.Data.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace iPem.Data {
    public partial class V_CuttingRepository {

        #region Fields

        private readonly string _databaseConnectionString;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        public V_CuttingRepository() {
            this._databaseConnectionString = SqlHelper.ConnectionStringCsTransaction;
        }

        #endregion

        #region Methods

        public V_Cutting GetEntity(string id, EnmCutType type) {
            SqlParameter[] parms = { new SqlParameter("@Id", SqlDbType.VarChar, 100),
                                     new SqlParameter("@Type", SqlDbType.Int) };

            parms[0].Value = SqlTypeConverter.DBNullStringChecker(id);
            parms[1].Value = (int)type;

            V_Cutting entity = null;
            using (var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Cs.Sql_V_Cutting_Repository_GetEntity, parms)) {
                while (rdr.Read()) {
                    entity = new V_Cutting();
                    entity.Id = SqlTypeConverter.DBNullStringHandler(rdr["Id"]);
                    entity.Type = SqlTypeConverter.DBNullEnmCutTypeHandler(rdr["Type"]);
                    entity.AreaId = SqlTypeConverter.DBNullStringHandler(rdr["AreaId"]);
                    entity.StationId = SqlTypeConverter.DBNullStringHandler(rdr["StationId"]);
                    entity.RoomId = SqlTypeConverter.DBNullStringHandler(rdr["RoomId"]);
                    entity.FsuId = SqlTypeConverter.DBNullStringHandler(rdr["FsuId"]);
                    entity.DeviceId = SqlTypeConverter.DBNullStringHandler(rdr["DeviceId"]);
                    entity.PointId = SqlTypeConverter.DBNullStringHandler(rdr["PointId"]);
                    entity.StartTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["StartTime"]);
                }
            }
            return entity;
        }

        public List<V_Cutting> GetEntities() {
            var entities = new List<V_Cutting>();
            using (var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Cs.Sql_V_Cutting_Repository_GetEntities, null)) {
                while (rdr.Read()) {
                    var entity = new V_Cutting();
                    entity.Id = SqlTypeConverter.DBNullStringHandler(rdr["Id"]);
                    entity.Type = SqlTypeConverter.DBNullEnmCutTypeHandler(rdr["Type"]);
                    entity.AreaId = SqlTypeConverter.DBNullStringHandler(rdr["AreaId"]);
                    entity.StationId = SqlTypeConverter.DBNullStringHandler(rdr["StationId"]);
                    entity.RoomId = SqlTypeConverter.DBNullStringHandler(rdr["RoomId"]);
                    entity.FsuId = SqlTypeConverter.DBNullStringHandler(rdr["FsuId"]);
                    entity.DeviceId = SqlTypeConverter.DBNullStringHandler(rdr["DeviceId"]);
                    entity.PointId = SqlTypeConverter.DBNullStringHandler(rdr["PointId"]);
                    entity.StartTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["StartTime"]);
                    entities.Add(entity);
                }
            }
            return entities;
        }

        public void SaveEntities(List<V_Cutting> entities) {
            SqlParameter[] parms = { new SqlParameter("@Id", SqlDbType.VarChar,200),
                                     new SqlParameter("@Type", SqlDbType.Int),
                                     new SqlParameter("@AreaId", SqlDbType.VarChar,100),
                                     new SqlParameter("@StationId", SqlDbType.VarChar,100),
                                     new SqlParameter("@RoomId", SqlDbType.VarChar,100),
                                     new SqlParameter("@FsuId", SqlDbType.VarChar,100),
                                     new SqlParameter("@DeviceId", SqlDbType.VarChar,100),
                                     new SqlParameter("@PointId", SqlDbType.VarChar,100),
                                     new SqlParameter("@StartTime", SqlDbType.DateTime)};

            using (var conn = new SqlConnection(this._databaseConnectionString)) {
                conn.Open();
                var trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                try {
                    foreach (var entity in entities) {
                        parms[0].Value = SqlTypeConverter.DBNullStringChecker(entity.Id);
                        parms[1].Value = (int)entity.Type;
                        parms[2].Value = SqlTypeConverter.DBNullStringChecker(entity.AreaId);
                        parms[3].Value = SqlTypeConverter.DBNullStringChecker(entity.StationId);
                        parms[4].Value = SqlTypeConverter.DBNullStringChecker(entity.RoomId);
                        parms[5].Value = SqlTypeConverter.DBNullStringChecker(entity.FsuId);
                        parms[6].Value = SqlTypeConverter.DBNullStringChecker(entity.DeviceId);
                        parms[7].Value = SqlTypeConverter.DBNullStringChecker(entity.PointId);
                        parms[8].Value = SqlTypeConverter.DBNullDateTimeChecker(entity.StartTime);
                        SqlHelper.ExecuteNonQuery(trans, CommandType.Text, SqlCommands_Cs.Sql_V_Cutting_Repository_SaveEntities, parms);
                    }
                    trans.Commit();
                } catch {
                    trans.Rollback();
                    throw;
                }
            }
        }

        #endregion

    }
}