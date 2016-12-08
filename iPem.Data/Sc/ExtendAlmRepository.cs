using iPem.Core;
using iPem.Data.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace iPem.Data {
    public partial class ExtendAlmRepository {

        #region Fields

        private readonly string _databaseConnectionString;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        public ExtendAlmRepository() {
            this._databaseConnectionString = SqlHelper.ConnectionStringScTransaction;
        }

        #endregion

        #region Methods

        public List<ExtAlm> GetEntities() {
            var entities = new List<ExtAlm>();
            using(var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Sc.Sql_ExtendAlm_Repository_GetEntities, null)) {
                while(rdr.Read()) {
                    var entity = new ExtAlm();
                    entity.Id = SqlTypeConverter.DBNullStringHandler(rdr["Id"]);
                    entity.FsuId = SqlTypeConverter.DBNullStringHandler(rdr["FsuId"]);
                    entity.Start = SqlTypeConverter.DBNullDateTimeHandler(rdr["Start"]);
                    entity.End = SqlTypeConverter.DBNullDateTimeNullableHandler(rdr["End"]);
                    entity.ProjectId = SqlTypeConverter.DBNullGuidHandler(rdr["ProjectId"]);
                    entity.Confirmed = SqlTypeConverter.DBNullEnmConfirmHandler(rdr["Confirmed"]);
                    entity.Confirmer = SqlTypeConverter.DBNullStringHandler(rdr["Confirmer"]);
                    entity.ConfirmedTime = SqlTypeConverter.DBNullDateTimeNullableHandler(rdr["ConfirmedTime"]);
                    entities.Add(entity);
                }
            }
            return entities;
        }

        public void SaveEntities(List<ExtAlm> entities) {
            SqlParameter[] parms = { new SqlParameter("@Id", SqlDbType.VarChar,100),
                                     new SqlParameter("@FsuId", SqlDbType.VarChar,100),
                                     new SqlParameter("@Start", SqlDbType.DateTime),
                                     new SqlParameter("@End", SqlDbType.DateTime),
                                     new SqlParameter("@ProjectId", SqlDbType.VarChar,100),
                                     new SqlParameter("@Confirmed", SqlDbType.Int),
                                     new SqlParameter("@Confirmer", SqlDbType.VarChar,100),
                                     new SqlParameter("@ConfirmedTime", SqlDbType.DateTime) };

            using(var conn = new SqlConnection(this._databaseConnectionString)) {
                conn.Open();
                var trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                try {
                    foreach(var entity in entities) {
                        parms[0].Value = SqlTypeConverter.DBNullStringChecker(entity.Id);
                        parms[1].Value = SqlTypeConverter.DBNullStringChecker(entity.FsuId);
                        parms[2].Value = SqlTypeConverter.DBNullDateTimeChecker(entity.Start);
                        parms[3].Value = SqlTypeConverter.DBNullDateTimeNullableChecker(entity.End);
                        parms[4].Value = SqlTypeConverter.DBNullGuidChecker(entity.ProjectId);
                        parms[5].Value = (int)entity.Confirmed;
                        parms[6].Value = SqlTypeConverter.DBNullStringChecker(entity.Confirmer);
                        parms[7].Value = SqlTypeConverter.DBNullDateTimeNullableChecker(entity.ConfirmedTime);
                        SqlHelper.ExecuteNonQuery(trans, CommandType.Text, string.Format(SqlCommands_Sc.Sql_ExtendAlm_Repository_SaveEntities, entity.Start.ToString("yyyyMM")), parms);
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

            parms[0].Value = SqlTypeConverter.DBNullDateTimeHandler(start);
            parms[1].Value = SqlTypeConverter.DBNullDateTimeHandler(end);

            using(var conn = new SqlConnection(this._databaseConnectionString)) {
                conn.Open();
                var trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                try {
                    SqlHelper.ExecuteNonQuery(trans, CommandType.Text, SqlCommands_Sc.Sql_ExtendAlm_Repository_DeleteEntities, parms);
                    trans.Commit();
                } catch {
                    trans.Rollback();
                    throw;
                }
            }
        }

        public void SaveActEntities(List<ExtAlm> entities) {
            SqlParameter[] parms = { new SqlParameter("@Id", SqlDbType.VarChar,100),
                                     new SqlParameter("@FsuId", SqlDbType.VarChar,100),
                                     new SqlParameter("@Start", SqlDbType.DateTime),
                                     new SqlParameter("@End", SqlDbType.DateTime),
                                     new SqlParameter("@ProjectId", SqlDbType.VarChar,100) };

            using(var conn = new SqlConnection(this._databaseConnectionString)) {
                conn.Open();
                var trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                try {
                    foreach(var entity in entities) {
                        parms[0].Value = SqlTypeConverter.DBNullStringChecker(entity.Id);
                        parms[1].Value = SqlTypeConverter.DBNullStringChecker(entity.FsuId);
                        parms[2].Value = SqlTypeConverter.DBNullDateTimeChecker(entity.Start);
                        parms[3].Value = SqlTypeConverter.DBNullDateTimeNullableChecker(entity.End);
                        parms[4].Value = SqlTypeConverter.DBNullGuidChecker(entity.ProjectId);
                        SqlHelper.ExecuteNonQuery(trans, CommandType.Text, SqlCommands_Sc.Sql_ExtendAlm_Repository_SaveActEntities, parms);
                    }
                    trans.Commit();
                } catch {
                    trans.Rollback();
                    throw;
                }
            }
        }

        public void DeleteActEntities(List<ExtAlm> entities) {
            SqlParameter[] parms = { new SqlParameter("@Id", SqlDbType.VarChar,100),
                                     new SqlParameter("@FsuId", SqlDbType.VarChar,100) };

            using(var conn = new SqlConnection(this._databaseConnectionString)) {
                conn.Open();
                var trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                try {
                    foreach(var entity in entities) {
                        parms[0].Value = SqlTypeConverter.DBNullStringChecker(entity.Id);
                        parms[1].Value = SqlTypeConverter.DBNullStringChecker(entity.FsuId);
                        SqlHelper.ExecuteNonQuery(trans, CommandType.Text, SqlCommands_Sc.Sql_ExtendAlm_Repository_DeleteActEntities, parms);
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
