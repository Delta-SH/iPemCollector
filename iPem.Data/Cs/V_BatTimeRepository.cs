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
                    entity.AreaId = SqlTypeConverter.DBNullStringHandler(rdr["AreaId"]);
                    entity.StationId = SqlTypeConverter.DBNullStringHandler(rdr["StationId"]);
                    entity.RoomId = SqlTypeConverter.DBNullStringHandler(rdr["RoomId"]);
                    entity.DeviceId = SqlTypeConverter.DBNullStringHandler(rdr["DeviceId"]);
                    entity.PointId = SqlTypeConverter.DBNullStringHandler(rdr["PointId"]);
                    entity.PackId = SqlTypeConverter.DBNullInt32Handler(rdr["PackId"]);
                    entity.Type = SqlTypeConverter.DBNullBatStatusHandler(rdr["Type"]);
                    entity.StartTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["StartTime"]);
                    entity.EndTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["EndTime"]);
                    entity.StartValue = SqlTypeConverter.DBNullDoubleHandler(rdr["StartValue"]);
                    entity.EndValue = SqlTypeConverter.DBNullDoubleHandler(rdr["EndValue"]);
                    entity.ProcTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["ProcTime"]);
                    entities.Add(entity);
                }
            }
            return entities;
        }

        public List<V_BatTime> GetProcDetails(string device, string point, DateTime start, DateTime end) {
            SqlParameter[] parms = { new SqlParameter("@DeviceId", SqlDbType.VarChar, 100),
                                     new SqlParameter("@PointId", SqlDbType.VarChar, 100),
                                     new SqlParameter("@Start", SqlDbType.DateTime),
                                     new SqlParameter("@End", SqlDbType.DateTime) };

            parms[0].Value = SqlTypeConverter.DBNullStringChecker(device);
            parms[1].Value = SqlTypeConverter.DBNullStringChecker(point);
            parms[2].Value = SqlTypeConverter.DBNullDateTimeChecker(start);
            parms[3].Value = SqlTypeConverter.DBNullDateTimeChecker(end);

            var entities = new List<V_BatTime>();
            using (var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Cs.Sql_V_BatTime_Repository_GetProcDetails, parms)) {
                while (rdr.Read()) {
                    var entity = new V_BatTime();
                    entity.AreaId = SqlTypeConverter.DBNullStringHandler(rdr["AreaId"]);
                    entity.StationId = SqlTypeConverter.DBNullStringHandler(rdr["StationId"]);
                    entity.RoomId = SqlTypeConverter.DBNullStringHandler(rdr["RoomId"]);
                    entity.DeviceId = SqlTypeConverter.DBNullStringHandler(rdr["DeviceId"]);
                    entity.PointId = SqlTypeConverter.DBNullStringHandler(rdr["PointId"]);
                    entity.PackId = SqlTypeConverter.DBNullInt32Handler(rdr["PackId"]);
                    entity.Type = SqlTypeConverter.DBNullBatStatusHandler(rdr["Type"]);
                    entity.StartTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["StartTime"]);
                    entity.EndTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["EndTime"]);
                    entity.StartValue = SqlTypeConverter.DBNullDoubleHandler(rdr["StartValue"]);
                    entity.EndValue = SqlTypeConverter.DBNullDoubleHandler(rdr["EndValue"]);
                    entity.ProcTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["ProcTime"]);
                    entities.Add(entity);
                }
            }
            return entities;
        }

        public void SaveEntities(List<V_BatTime> entities) {
            SqlParameter[] parms = { new SqlParameter("@AreaId", SqlDbType.VarChar,100),
                                     new SqlParameter("@StationId", SqlDbType.VarChar,100),
                                     new SqlParameter("@RoomId", SqlDbType.VarChar,100),
                                     new SqlParameter("@DeviceId", SqlDbType.VarChar,100),
                                     new SqlParameter("@PointId", SqlDbType.VarChar,100),
                                     new SqlParameter("@PackId", SqlDbType.Int),
                                     new SqlParameter("@Type", SqlDbType.Int),
                                     new SqlParameter("@StartTime", SqlDbType.DateTime),
                                     new SqlParameter("@EndTime", SqlDbType.DateTime),
                                     new SqlParameter("@StartValue", SqlDbType.Float),
                                     new SqlParameter("@EndValue", SqlDbType.Float),
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
                        parms[4].Value = SqlTypeConverter.DBNullStringChecker(entity.PointId);
                        parms[5].Value = SqlTypeConverter.DBNullInt32Checker(entity.PackId);
                        parms[6].Value = (int)entity.Type;
                        parms[7].Value = SqlTypeConverter.DBNullDateTimeChecker(entity.StartTime);
                        parms[8].Value = SqlTypeConverter.DBNullDateTimeChecker(entity.EndTime);
                        parms[9].Value = SqlTypeConverter.DBNullDoubleChecker(entity.StartValue);
                        parms[10].Value = SqlTypeConverter.DBNullDoubleChecker(entity.EndValue);
                        parms[11].Value = SqlTypeConverter.DBNullDateTimeChecker(entity.ProcTime);
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
