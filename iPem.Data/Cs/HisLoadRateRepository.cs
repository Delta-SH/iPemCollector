using iPem.Core;
using iPem.Data.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace iPem.Data.Cs {
    public partial class HisLoadRateRepository {

        #region Fields

        private readonly string _databaseConnectionString;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        public HisLoadRateRepository() {
            this._databaseConnectionString = SqlHelper.ConnectionStringCsTransaction;
        }

        #endregion

        #region Methods

        public void SaveEntities(List<HisLoadRate> entities) {
            SqlParameter[] parms = { new SqlParameter("@AreaId",SqlDbType.VarChar,100),
                                     new SqlParameter("@StationId",SqlDbType.VarChar,100),
                                     new SqlParameter("@RoomId",SqlDbType.VarChar,100),
                                     new SqlParameter("@DeviceId", SqlDbType.VarChar,100),
                                     new SqlParameter("@StartTime",SqlDbType.DateTime),
                                     new SqlParameter("@EndTime", SqlDbType.DateTime),
                                     new SqlParameter("@Value", SqlDbType.Float),
                                     new SqlParameter("@CreatedTime", SqlDbType.DateTime)};

            using(var conn = new SqlConnection(this._databaseConnectionString)) {
                conn.Open();
                var trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                try {
                    foreach(var entity in entities) {
                        parms[0].Value = SqlTypeConverter.DBNullStringChecker(entity.AreaId);
                        parms[1].Value = SqlTypeConverter.DBNullStringChecker(entity.StationId);
                        parms[2].Value = SqlTypeConverter.DBNullStringChecker(entity.RoomId);
                        parms[3].Value = SqlTypeConverter.DBNullStringChecker(entity.DeviceId);
                        parms[4].Value = SqlTypeConverter.DBNullDateTimeChecker(entity.StartTime);
                        parms[5].Value = SqlTypeConverter.DBNullDateTimeChecker(entity.EndTime);
                        parms[6].Value = SqlTypeConverter.DBNullDoubleChecker(entity.Value);
                        parms[7].Value = SqlTypeConverter.DBNullDateTimeChecker(entity.CreatedTime);
                        SqlHelper.ExecuteNonQuery(trans, CommandType.Text, string.Format(SqlCommands_Cs.Sql_HisLoadRate_Repository_SaveEntities, entity.StartTime.ToString("yyyyMM")), parms);
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
                    SqlHelper.ExecuteNonQuery(trans, CommandType.Text, SqlCommands_Cs.Sql_HisLoadRate_Repository_DeleteEntities, parms);
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
