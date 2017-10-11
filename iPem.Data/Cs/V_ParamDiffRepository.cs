using iPem.Core;
using iPem.Data.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace iPem.Data {
    public partial class V_ParamDiffRepository {

        #region Fields

        private readonly string _databaseConnectionString;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        public V_ParamDiffRepository() {
            this._databaseConnectionString = SqlHelper.ConnectionStringCsTransaction;
        }

        #endregion

        #region Methods

        public void SaveEntities(List<V_ParamDiff> entities, DateTime curDate) {
            SqlParameter[] parms = { new SqlParameter("@DeviceId",SqlDbType.VarChar,100),
                                     new SqlParameter("@PointId",SqlDbType.VarChar,100),
                                     new SqlParameter("@Threshold",SqlDbType.VarChar,20),
                                     new SqlParameter("@AlarmLevel", SqlDbType.VarChar,20),
                                     new SqlParameter("@NMAlarmID", SqlDbType.VarChar,50),
                                     new SqlParameter("@AbsoluteVal",SqlDbType.VarChar,20),
                                     new SqlParameter("@RelativeVal", SqlDbType.VarChar,20),
                                     new SqlParameter("@StorageInterval", SqlDbType.VarChar,20),
                                     new SqlParameter("@StorageRefTime", SqlDbType.VarChar,50), 
                                     new SqlParameter("@Masked", SqlDbType.Bit)};

            using (var conn = new SqlConnection(this._databaseConnectionString)) {
                conn.Open();
                var trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                try {
                    SqlHelper.ExecuteNonQuery(trans, CommandType.Text, string.Format(SqlCommands_Cs.Sql_V_ParamDiff_Repository_DeleteEntities, curDate.ToString("yyyyMM")), null);
                    foreach (var entity in entities) {
                        parms[0].Value = SqlTypeConverter.DBNullStringChecker(entity.DeviceId);
                        parms[1].Value = SqlTypeConverter.DBNullStringChecker(entity.PointId);
                        parms[2].Value = SqlTypeConverter.DBNullStringChecker(entity.Threshold);
                        parms[3].Value = SqlTypeConverter.DBNullStringChecker(entity.AlarmLevel);
                        parms[4].Value = SqlTypeConverter.DBNullStringChecker(entity.NMAlarmID);
                        parms[5].Value = SqlTypeConverter.DBNullStringChecker(entity.AbsoluteVal);
                        parms[6].Value = SqlTypeConverter.DBNullStringChecker(entity.RelativeVal);
                        parms[7].Value = SqlTypeConverter.DBNullStringChecker(entity.StorageInterval);
                        parms[8].Value = SqlTypeConverter.DBNullStringChecker(entity.StorageRefTime);
                        parms[9].Value = entity.Masked;
                        SqlHelper.ExecuteNonQuery(trans, CommandType.Text, string.Format(SqlCommands_Cs.Sql_V_ParamDiff_Repository_SaveEntities, curDate.ToString("yyyyMM")), parms);
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
