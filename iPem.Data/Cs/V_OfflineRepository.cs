using iPem.Core;
using iPem.Data.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace iPem.Data {
    public partial class V_OfflineRepository {

        #region Fields

        private readonly string _databaseConnectionString;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        public V_OfflineRepository() {
            this._databaseConnectionString = SqlHelper.ConnectionStringCsTransaction;
        }

        #endregion

        #region Methods

        public List<V_Offline> GetActive() {
            var entities = new List<V_Offline>();
            using (var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Cs.Sql_V_Offline_Repository_GetActive1, null)) {
                while (rdr.Read()) {
                    var entity = new V_Offline();
                    entity.Id = SqlTypeConverter.DBNullStringHandler(rdr["Id"]);
                    entity.Type = SqlTypeConverter.DBNullEnmSSHHandler(rdr["Type"]);
                    entity.FormulaType = SqlTypeConverter.DBNullEnmFormulaHandler(rdr["FormulaType"]);
                    entity.StartTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["StartTime"]);
                    entity.EndTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["EndTime"]);
                    entity.Value = SqlTypeConverter.DBNullDoubleHandler(rdr["Value"]);
                    entities.Add(entity);
                }
            }
            return entities;
        }

        public List<V_Offline> GetActive(EnmFormula ftype) {
            SqlParameter[] parms = { new SqlParameter("@FormulaType", SqlDbType.Int) };
            parms[0].Value = (int)ftype;

            var entities = new List<V_Offline>();
            using (var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Cs.Sql_V_Offline_Repository_GetActive2, parms)) {
                while (rdr.Read()) {
                    var entity = new V_Offline();
                    entity.Id = SqlTypeConverter.DBNullStringHandler(rdr["Id"]);
                    entity.Type = SqlTypeConverter.DBNullEnmSSHHandler(rdr["Type"]);
                    entity.FormulaType = SqlTypeConverter.DBNullEnmFormulaHandler(rdr["FormulaType"]);
                    entity.StartTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["StartTime"]);
                    entity.EndTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["EndTime"]);
                    entity.Value = SqlTypeConverter.DBNullDoubleHandler(rdr["Value"]);
                    entities.Add(entity);
                }
            }
            return entities;
        }

        public List<V_Offline> GetHistory(DateTime start, DateTime end) {
            SqlParameter[] parms = { new SqlParameter("@Start", SqlDbType.DateTime),
                                     new SqlParameter("@End", SqlDbType.DateTime) };

            parms[0].Value = SqlTypeConverter.DBNullDateTimeHandler(start);
            parms[1].Value = SqlTypeConverter.DBNullDateTimeHandler(end);

            var entities = new List<V_Offline>();
            using (var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Cs.Sql_V_Offline_Repository_GetHistory1, parms)) {
                while (rdr.Read()) {
                    var entity = new V_Offline();
                    entity.Id = SqlTypeConverter.DBNullStringHandler(rdr["Id"]);
                    entity.Type = SqlTypeConverter.DBNullEnmSSHHandler(rdr["Type"]);
                    entity.FormulaType = SqlTypeConverter.DBNullEnmFormulaHandler(rdr["FormulaType"]);
                    entity.StartTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["StartTime"]);
                    entity.EndTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["EndTime"]);
                    entity.Value = SqlTypeConverter.DBNullDoubleHandler(rdr["Value"]);
                    entities.Add(entity);
                }
            }
            return entities;
        }

        public List<V_Offline> GetHistory(EnmFormula ftype, DateTime start, DateTime end) {
            SqlParameter[] parms = { new SqlParameter("@Start", SqlDbType.DateTime),
                                     new SqlParameter("@End", SqlDbType.DateTime),
                                     new SqlParameter("@FormulaType", SqlDbType.Int) };

            parms[0].Value = SqlTypeConverter.DBNullDateTimeHandler(start);
            parms[1].Value = SqlTypeConverter.DBNullDateTimeHandler(end);
            parms[2].Value = (int)ftype;

            var entities = new List<V_Offline>();
            using (var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Cs.Sql_V_Offline_Repository_GetHistory2, parms)) {
                while (rdr.Read()) {
                    var entity = new V_Offline();
                    entity.Id = SqlTypeConverter.DBNullStringHandler(rdr["Id"]);
                    entity.Type = SqlTypeConverter.DBNullEnmSSHHandler(rdr["Type"]);
                    entity.FormulaType = SqlTypeConverter.DBNullEnmFormulaHandler(rdr["FormulaType"]);
                    entity.StartTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["StartTime"]);
                    entity.EndTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["EndTime"]);
                    entity.Value = SqlTypeConverter.DBNullDoubleHandler(rdr["Value"]);
                    entities.Add(entity);
                }
            }
            return entities;
        }

        public void SaveActive(List<V_Offline> entities) {
            SqlParameter[] parms = { new SqlParameter("@Id", SqlDbType.VarChar,100),
                                     new SqlParameter("@Type", SqlDbType.Int),
                                     new SqlParameter("@FormulaType", SqlDbType.Int),
                                     new SqlParameter("@StartTime", SqlDbType.DateTime),
                                     new SqlParameter("@EndTime", SqlDbType.DateTime),
                                     new SqlParameter("@Value", SqlDbType.Float)};

            using (var conn = new SqlConnection(this._databaseConnectionString)) {
                conn.Open();
                var trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                try {
                    foreach (var entity in entities) {
                        parms[0].Value = SqlTypeConverter.DBNullStringChecker(entity.Id);
                        parms[1].Value = (int)entity.Type;
                        parms[2].Value = (int)entity.FormulaType;
                        parms[3].Value = SqlTypeConverter.DBNullDateTimeChecker(entity.StartTime);
                        parms[4].Value = SqlTypeConverter.DBNullDateTimeChecker(entity.EndTime);
                        parms[5].Value = SqlTypeConverter.DBNullDoubleChecker(entity.Value);
                        SqlHelper.ExecuteNonQuery(trans, CommandType.Text, SqlCommands_Cs.Sql_V_Offline_Repository_SaveActive, parms);
                    }
                    trans.Commit();
                } catch {
                    trans.Rollback();
                    throw;
                }
            }
        }

        public void UpdateActive(List<V_Offline> entities) {
            SqlParameter[] parms = { new SqlParameter("@Id", SqlDbType.VarChar,100),
                                     new SqlParameter("@Type", SqlDbType.Int),
                                     new SqlParameter("@FormulaType", SqlDbType.Int),
                                     new SqlParameter("@StartTime", SqlDbType.DateTime),
                                     new SqlParameter("@EndTime", SqlDbType.DateTime),
                                     new SqlParameter("@Value", SqlDbType.Float)};

            using (var conn = new SqlConnection(this._databaseConnectionString)) {
                conn.Open();
                var trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                try {
                    foreach (var entity in entities) {
                        parms[0].Value = SqlTypeConverter.DBNullStringChecker(entity.Id);
                        parms[1].Value = (int)entity.Type;
                        parms[2].Value = (int)entity.FormulaType;
                        parms[3].Value = SqlTypeConverter.DBNullDateTimeChecker(entity.StartTime);
                        parms[4].Value = SqlTypeConverter.DBNullDateTimeChecker(entity.EndTime);
                        parms[5].Value = SqlTypeConverter.DBNullDoubleChecker(entity.Value);
                        SqlHelper.ExecuteNonQuery(trans, CommandType.Text, SqlCommands_Cs.Sql_V_Offline_Repository_UpdateActive, parms);
                    }
                    trans.Commit();
                } catch {
                    trans.Rollback();
                    throw;
                }
            }
        }

        public void SaveHistory(List<V_Offline> entities) {
            SqlParameter[] parms = { new SqlParameter("@Id", SqlDbType.VarChar,100),
                                     new SqlParameter("@Type", SqlDbType.Int),
                                     new SqlParameter("@FormulaType", SqlDbType.Int),
                                     new SqlParameter("@StartTime", SqlDbType.DateTime),
                                     new SqlParameter("@EndTime", SqlDbType.DateTime),
                                     new SqlParameter("@Value", SqlDbType.Float)};

            using (var conn = new SqlConnection(this._databaseConnectionString)) {
                conn.Open();
                var trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                try {
                    foreach (var entity in entities) {
                        parms[0].Value = SqlTypeConverter.DBNullStringChecker(entity.Id);
                        parms[1].Value = (int)entity.Type;
                        parms[2].Value = (int)entity.FormulaType;
                        parms[3].Value = SqlTypeConverter.DBNullDateTimeChecker(entity.StartTime);
                        parms[4].Value = SqlTypeConverter.DBNullDateTimeChecker(entity.EndTime);
                        parms[5].Value = SqlTypeConverter.DBNullDoubleChecker(entity.Value);
                        SqlHelper.ExecuteNonQuery(trans, CommandType.Text, string.Format(SqlCommands_Cs.Sql_V_Offline_Repository_SaveHistory, entity.StartTime.ToString("yyyyMM")), parms);
                    }
                    trans.Commit();
                } catch {
                    trans.Rollback();
                    throw;
                }
            }
        }

        public void UpdateHistory(List<V_Offline> entities) {
            SqlParameter[] parms = { new SqlParameter("@Id", SqlDbType.VarChar,100),
                                     new SqlParameter("@Type", SqlDbType.Int),
                                     new SqlParameter("@FormulaType", SqlDbType.Int),
                                     new SqlParameter("@StartTime", SqlDbType.DateTime),
                                     new SqlParameter("@EndTime", SqlDbType.DateTime),
                                     new SqlParameter("@Value", SqlDbType.Float)};

            using (var conn = new SqlConnection(this._databaseConnectionString)) {
                conn.Open();
                var trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                try {
                    foreach (var entity in entities) {
                        parms[0].Value = SqlTypeConverter.DBNullStringChecker(entity.Id);
                        parms[1].Value = (int)entity.Type;
                        parms[2].Value = (int)entity.FormulaType;
                        parms[3].Value = SqlTypeConverter.DBNullDateTimeChecker(entity.StartTime);
                        parms[4].Value = SqlTypeConverter.DBNullDateTimeChecker(entity.EndTime);
                        parms[5].Value = SqlTypeConverter.DBNullDoubleChecker(entity.Value);
                        SqlHelper.ExecuteNonQuery(trans, CommandType.Text, string.Format(SqlCommands_Cs.Sql_V_Offline_Repository_UpdateHistory, entity.StartTime.ToString("yyyyMM")), parms);
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
