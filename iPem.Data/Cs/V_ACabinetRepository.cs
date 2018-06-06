using iPem.Core;
using iPem.Data.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace iPem.Data {
    public partial class V_ACabinetRepository {

        #region Fields

        private readonly string _databaseConnectionString;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        public V_ACabinetRepository() {
            this._databaseConnectionString = SqlHelper.ConnectionStringCsTransaction;
        }

        #endregion

        #region Methods

        public void Save(List<V_ACabinet> entities) {
            SqlParameter[] parms = { new SqlParameter("@DeviceId", SqlDbType.VarChar,100),
                                     new SqlParameter("@PointId", SqlDbType.VarChar,100),
                                     new SqlParameter("@Category", SqlDbType.Int),
                                     new SqlParameter("@Value", SqlDbType.Float),
                                     new SqlParameter("@ValueTime", SqlDbType.DateTime),
                                     new SqlParameter("@AValue", SqlDbType.Float),
                                     new SqlParameter("@AValueTime", SqlDbType.DateTime),
                                     new SqlParameter("@BValue", SqlDbType.Float),
                                     new SqlParameter("@BValueTime", SqlDbType.DateTime),
                                     new SqlParameter("@CValue", SqlDbType.Float),
                                     new SqlParameter("@CValueTime", SqlDbType.DateTime)};

            using (var conn = new SqlConnection(this._databaseConnectionString)) {
                conn.Open();
                var trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                try {
                    foreach (var entity in entities) {
                        parms[0].Value = SqlTypeConverter.DBNullStringChecker(entity.DeviceId);
                        parms[1].Value = SqlTypeConverter.DBNullStringChecker(entity.PointId);
                        parms[2].Value = (int)entity.Category;
                        parms[3].Value = SqlTypeConverter.DBNullDoubleChecker(entity.Value);
                        parms[4].Value = SqlTypeConverter.DBNullDateTimeChecker(entity.ValueTime);
                        parms[5].Value = SqlTypeConverter.DBNullDoubleChecker(entity.AValue);
                        parms[6].Value = SqlTypeConverter.DBNullDateTimeChecker(entity.AValueTime);
                        parms[7].Value = SqlTypeConverter.DBNullDoubleChecker(entity.BValue);
                        parms[8].Value = SqlTypeConverter.DBNullDateTimeChecker(entity.BValueTime);
                        parms[9].Value = SqlTypeConverter.DBNullDoubleChecker(entity.CValue);
                        parms[10].Value = SqlTypeConverter.DBNullDateTimeChecker(entity.CValueTime);
                        SqlHelper.ExecuteNonQuery(trans, CommandType.Text, string.Format(SqlCommands_Cs.Sql_V_ACabinet_Repository_Save, entity.ValueTime.ToString("yyyyMM")), parms);
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
