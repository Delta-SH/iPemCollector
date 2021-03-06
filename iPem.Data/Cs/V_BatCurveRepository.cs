﻿using iPem.Core;
using iPem.Data.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace iPem.Data {
    public partial class V_BatCurveRepository {

        #region Fields

        private readonly string _databaseConnectionString;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        public V_BatCurveRepository() {
            this._databaseConnectionString = SqlHelper.ConnectionStringCsTransaction;
        }

        #endregion

        #region Methods

        public void SaveEntities(List<V_BatCurve> entities) {
            SqlParameter[] parms = { new SqlParameter("@AreaId", SqlDbType.VarChar,100),
                                     new SqlParameter("@StationId", SqlDbType.VarChar,100),
                                     new SqlParameter("@RoomId", SqlDbType.VarChar,100),
                                     new SqlParameter("@DeviceId", SqlDbType.VarChar,100),
                                     new SqlParameter("@PointId", SqlDbType.VarChar,100),
                                     new SqlParameter("@PackId", SqlDbType.Int),
                                     new SqlParameter("@Type", SqlDbType.Int),
                                     new SqlParameter("@PType", SqlDbType.Int),
                                     new SqlParameter("@StartTime", SqlDbType.DateTime),
                                     new SqlParameter("@Value", SqlDbType.Float),
                                     new SqlParameter("@ValueTime", SqlDbType.DateTime),
                                     new SqlParameter("@ProcTime", SqlDbType.DateTime)};

            using (var conn = new SqlConnection(this._databaseConnectionString)) {
                conn.Open();
                var trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                try {
                    foreach (var entity in entities) {
                        parms[0].Value = SqlTypeConverter.DBNullStringChecker(entity.AreaId);
                        parms[1].Value = SqlTypeConverter.DBNullStringChecker(entity.StationId);
                        parms[2].Value = SqlTypeConverter.DBNullStringChecker(entity.RoomId);
                        parms[3].Value = SqlTypeConverter.DBNullStringChecker(entity.DeviceId);
                        parms[4].Value = SqlTypeConverter.DBNullStringChecker(entity.PointId);
                        parms[5].Value = SqlTypeConverter.DBNullInt32Checker(entity.PackId);
                        parms[6].Value = (int)entity.Type;
                        parms[7].Value = (int)entity.PType;
                        parms[8].Value = SqlTypeConverter.DBNullDateTimeChecker(entity.StartTime);
                        parms[9].Value = SqlTypeConverter.DBNullDoubleChecker(entity.Value);
                        parms[10].Value = SqlTypeConverter.DBNullDateTimeChecker(entity.ValueTime);
                        parms[11].Value = SqlTypeConverter.DBNullDateTimeChecker(entity.ProcTime);
                        SqlHelper.ExecuteNonQuery(trans, CommandType.Text, string.Format(SqlCommands_Cs.Sql_V_BatCurve_Repository_SaveEntities, entity.StartTime.ToString("yyyyMM")), parms);
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

            using (var conn = new SqlConnection(this._databaseConnectionString)) {
                conn.Open();
                var trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                try {
                    SqlHelper.ExecuteNonQuery(trans, CommandType.Text, SqlCommands_Cs.Sql_V_BatCurve_Repository_DeleteEntities, parms);
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