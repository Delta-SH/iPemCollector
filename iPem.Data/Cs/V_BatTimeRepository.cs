using iPem.Core;
using iPem.Data.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace iPem.Data {
    public partial class V_BatTimeRepository {

        #region Fields

        private readonly string _databaseConnectionString;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        public V_BatTimeRepository() {
            this._databaseConnectionString = SqlHelper.ConnectionStringCsTransaction;
        }

        #endregion

        #region Methods

        public List<V_BatTime> GetProcedures(DateTime start, DateTime end) {
            SqlParameter[] parms = { new SqlParameter("@Start", SqlDbType.DateTime),
                                     new SqlParameter("@End", SqlDbType.DateTime) };

            parms[0].Value = SqlTypeConverter.DBNullDateTimeChecker(start);
            parms[1].Value = SqlTypeConverter.DBNullDateTimeChecker(end);

            var entities = new List<V_BatTime>();
            using (var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Cs.Sql_V_BatTime_Repository_GetProcedures, parms)) {
                while (rdr.Read()) {
                    var entity = new V_BatTime();
                    entity.DeviceId = SqlTypeConverter.DBNullStringHandler(rdr["DeviceId"]);
                    entity.PackId = SqlTypeConverter.DBNullInt32Handler(rdr["PackId"]);
                    entity.Type = SqlTypeConverter.DBNullBatTypeHandler(rdr["Type"]);
                    entity.StartTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["StartTime"]);
                    entity.EndTime = entity.StartTime;
                    entity.ProcTime = entity.StartTime;
                    entities.Add(entity);
                }
            }
            return entities;
        }

        public V_BatTime GetPreProcedure(string device, int pack, DateTime start, DateTime end) {
            SqlParameter[] parms = { new SqlParameter("@DeviceId", SqlDbType.VarChar, 100),
                                     new SqlParameter("@PackId", SqlDbType.Int),
                                     new SqlParameter("@Start", SqlDbType.DateTime),
                                     new SqlParameter("@End", SqlDbType.DateTime) };

            parms[0].Value = SqlTypeConverter.DBNullStringChecker(device);
            parms[1].Value = SqlTypeConverter.DBNullInt32Checker(pack);
            parms[2].Value = SqlTypeConverter.DBNullDateTimeChecker(start);
            parms[3].Value = SqlTypeConverter.DBNullDateTimeChecker(end);

            V_BatTime entity = null;
            using (var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Cs.Sql_V_BatTime_Repository_GetPreProcedure, parms)) {
                if (rdr.Read()) {
                    entity.DeviceId = SqlTypeConverter.DBNullStringHandler(rdr["DeviceId"]);
                    entity.PackId = SqlTypeConverter.DBNullInt32Handler(rdr["PackId"]);
                    entity.Type = SqlTypeConverter.DBNullBatTypeHandler(rdr["Type"]);
                    entity.StartTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["StartTime"]);
                    entity.EndTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["EndTime"]);
                    entity.ProcTime = entity.StartTime;
                }
            }
            return entity;
        }

        public void SaveEntities(List<V_BatTime> entities) {
            SqlParameter[] parms = { new SqlParameter("@AreaId", SqlDbType.VarChar,100),
                                     new SqlParameter("@StationId", SqlDbType.VarChar,100),
                                     new SqlParameter("@RoomId", SqlDbType.VarChar,100),
                                     new SqlParameter("@DeviceId", SqlDbType.VarChar,100),
                                     new SqlParameter("@PackId", SqlDbType.Int),
                                     new SqlParameter("@Type", SqlDbType.Int),
                                     new SqlParameter("@StartTime", SqlDbType.DateTime),
                                     new SqlParameter("@EndTime", SqlDbType.DateTime),
                                     new SqlParameter("@ProcTime", SqlDbType.DateTime) };

            using(var conn = new SqlConnection(this._databaseConnectionString)) {
                conn.Open();
                var trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                try {
                    foreach(var entity in entities) {
                        parms[0].Value = SqlTypeConverter.DBNullStringChecker(entity.AreaId);
                        parms[1].Value = SqlTypeConverter.DBNullStringChecker(entity.StationId);
                        parms[2].Value = SqlTypeConverter.DBNullStringChecker(entity.RoomId);
                        parms[3].Value = SqlTypeConverter.DBNullStringChecker(entity.DeviceId);
                        parms[4].Value = SqlTypeConverter.DBNullInt32Checker(entity.PackId);
                        parms[5].Value = (int)entity.Type;
                        parms[6].Value = SqlTypeConverter.DBNullDateTimeChecker(entity.StartTime);
                        parms[7].Value = SqlTypeConverter.DBNullDateTimeChecker(entity.EndTime);
                        parms[8].Value = SqlTypeConverter.DBNullDateTimeChecker(entity.ProcTime);
                        SqlHelper.ExecuteNonQuery(trans, CommandType.Text, string.Format(SqlCommands_Cs.Sql_V_BatTime_Repository_SaveEntities, entity.StartTime.ToString("yyyyMM")), parms);
                    }
                    trans.Commit();
                } catch {
                    trans.Rollback();
                    throw;
                }
            }
        }

        public void DeleteEntities(DateTime start, DateTime end) {
            SqlParameter[] parms = { new SqlParameter("@Start", SqlDbType.DateTime),
                                     new SqlParameter("@End", SqlDbType.DateTime) };

            parms[0].Value = SqlTypeConverter.DBNullDateTimeChecker(start);
            parms[1].Value = SqlTypeConverter.DBNullDateTimeChecker(end);

            using(var conn = new SqlConnection(this._databaseConnectionString)) {
                conn.Open();
                var trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                try {
                    SqlHelper.ExecuteNonQuery(trans, CommandType.Text, SqlCommands_Cs.Sql_V_BatTime_Repository_DeleteEntities, parms);
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
