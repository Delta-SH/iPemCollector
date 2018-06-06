using iPem.Core;
using iPem.Data.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace iPem.Data {
    public partial class V_ElecRepository {

        #region Fields

        private readonly string _databaseConnectionString;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        public V_ElecRepository() {
            this._databaseConnectionString = SqlHelper.ConnectionStringCsTransaction;
        }

        #endregion

        #region Methods

        public void SaveActive(List<V_Elec> entities) {
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
                        SqlHelper.ExecuteNonQuery(trans, CommandType.Text, SqlCommands_Cs.Sql_V_Elec_Repository_SaveActive, parms);
                    }
                    trans.Commit();
                } catch {
                    trans.Rollback();
                    throw;
                }
            }
        }

        public void SaveHistory(List<V_Elec> entities) {
            SqlParameter[] parms = { new SqlParameter("@Id", SqlDbType.VarChar,100),
                                     new SqlParameter("@Type", SqlDbType.Int),
                                     new SqlParameter("@FormulaType", SqlDbType.Int),
                                     new SqlParameter("@StartTime", SqlDbType.DateTime),
                                     new SqlParameter("@EndTime", SqlDbType.DateTime),
                                     new SqlParameter("@Value", SqlDbType.Float)};

            using(var conn = new SqlConnection(this._databaseConnectionString)) {
                conn.Open();
                var trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                try {
                    foreach(var entity in entities) {
                        parms[0].Value = SqlTypeConverter.DBNullStringChecker(entity.Id);
                        parms[1].Value = (int)entity.Type;
                        parms[2].Value = (int)entity.FormulaType;
                        parms[3].Value = SqlTypeConverter.DBNullDateTimeChecker(entity.StartTime);
                        parms[4].Value = SqlTypeConverter.DBNullDateTimeChecker(entity.EndTime);
                        parms[5].Value = SqlTypeConverter.DBNullDoubleChecker(entity.Value);
                        SqlHelper.ExecuteNonQuery(trans, CommandType.Text, string.Format(SqlCommands_Cs.Sql_V_Elec_Repository_SaveHistory, entity.StartTime.ToString("yyyyMM")), parms);
                    }
                    trans.Commit();
                } catch {
                    trans.Rollback();
                    throw;
                }
            }
        }

        public void DeleteHistory(DateTime start, DateTime end) {
            SqlParameter[] parms = { new SqlParameter("@Start", SqlDbType.DateTime),
                                     new SqlParameter("@End", SqlDbType.DateTime) };

            parms[0].Value = SqlTypeConverter.DBNullDateTimeHandler(start);
            parms[1].Value = SqlTypeConverter.DBNullDateTimeHandler(end);

            using(var conn = new SqlConnection(this._databaseConnectionString)) {
                conn.Open();
                var trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                try {
                    SqlHelper.ExecuteNonQuery(trans, CommandType.Text, SqlCommands_Cs.Sql_V_Elec_Repository_DeleteHistory, parms);
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
