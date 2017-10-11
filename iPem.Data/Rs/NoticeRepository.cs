using iPem.Core.Rs;
using iPem.Data.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace iPem.Data {
    public partial class NoticeRepository {

        #region Fields

        private readonly string _databaseConnectionString;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        public NoticeRepository() {
            this._databaseConnectionString = SqlHelper.ConnectionStringRsTransaction;
        }

        #endregion

        #region Methods

        public List<Notice> GetEntities() {
            var entities = new List<Notice>();
            using (var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Rs.Sql_Notice_Repository_GetEntities, null)) {
                while (rdr.Read()) {
                    var entity = new Notice();
                    entity.Id = SqlTypeConverter.DBNullInt32Handler(rdr["Id"]);
                    entity.SysType = SqlTypeConverter.DBNullInt32Handler(rdr["SysType"]);
                    entity.Name = SqlTypeConverter.DBNullStringHandler(rdr["Name"]);
                    entity.Time = SqlTypeConverter.DBNullDateTimeHandler(rdr["Time"]);
                    entity.Desc = SqlTypeConverter.DBNullStringHandler(rdr["Desc"]);
                    entities.Add(entity);
                }
            }
            return entities;
        }

        public void Delete(List<Notice> entities) {
            SqlParameter[] parms = { new SqlParameter("@Id", SqlDbType.Int) };
            using (var conn = new SqlConnection(this._databaseConnectionString)) {
                conn.Open();
                var trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                try {
                    foreach (var entity in entities) {
                        parms[0].Value = SqlTypeConverter.DBNullInt32Checker(entity.Id);
                        SqlHelper.ExecuteNonQuery(trans, CommandType.Text, SqlCommands_Rs.Sql_Notice_Repository_Delete, parms);
                    }
                    trans.Commit();
                } catch {
                    trans.Rollback();
                    throw;
                }
            }
        }

        public void Clear() {
            using (var conn = new SqlConnection(this._databaseConnectionString)) {
                conn.Open();
                var trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                try {
                    SqlHelper.ExecuteNonQuery(trans, CommandType.Text, SqlCommands_Rs.Sql_Notice_Repository_Clear, null);
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
