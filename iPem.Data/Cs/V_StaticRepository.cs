﻿using iPem.Core;
using iPem.Data.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace iPem.Data {
    public partial class V_StaticRepository {

        #region Fields

        private readonly string _databaseConnectionString;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        public V_StaticRepository() {
            this._databaseConnectionString = SqlHelper.ConnectionStringCsTransaction;
        }

        #endregion

        #region Methods

        public void SaveEntities(List<V_Static> entities) {
            SqlParameter[] parms = { new SqlParameter("@AreaId",SqlDbType.VarChar,100),
                                     new SqlParameter("@StationId",SqlDbType.VarChar,100),
                                     new SqlParameter("@RoomId",SqlDbType.VarChar,100),
                                     new SqlParameter("@DeviceId", SqlDbType.VarChar,100),
                                     new SqlParameter("@PointId", SqlDbType.VarChar,100),
                                     new SqlParameter("@StartTime",SqlDbType.DateTime),
                                     new SqlParameter("@EndTime", SqlDbType.DateTime),
                                     new SqlParameter("@MaxValue", SqlDbType.Float),
                                     new SqlParameter("@MinValue", SqlDbType.Float),
                                     new SqlParameter("@AvgValue", SqlDbType.Float),
                                     new SqlParameter("@MaxTime", SqlDbType.DateTime),
                                     new SqlParameter("@MinTime", SqlDbType.DateTime),
                                     new SqlParameter("@Total", SqlDbType.Int)};

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
                        parms[5].Value = SqlTypeConverter.DBNullDateTimeChecker(entity.StartTime);
                        parms[6].Value = SqlTypeConverter.DBNullDateTimeChecker(entity.EndTime);
                        parms[7].Value = SqlTypeConverter.DBNullDoubleChecker(entity.MaxValue);
                        parms[8].Value = SqlTypeConverter.DBNullDoubleChecker(entity.MinValue);
                        parms[9].Value = SqlTypeConverter.DBNullDoubleChecker(entity.AvgValue);
                        parms[10].Value = SqlTypeConverter.DBNullDateTimeChecker(entity.MaxTime);
                        parms[11].Value = SqlTypeConverter.DBNullDateTimeChecker(entity.MinTime);
                        parms[12].Value = SqlTypeConverter.DBNullInt32Checker(entity.Total);
                        SqlHelper.ExecuteNonQuery(trans, CommandType.Text, string.Format(SqlCommands_Cs.Sql_V_Static_Repository_SaveEntities, entity.StartTime.ToString("yyyyMM")), parms);
                    }
                    trans.Commit();
                } catch {
                    trans.Rollback();
                    throw;
                }
            }
        }

        public void DeleteEntities(string device, string point, DateTime start, DateTime end) {
            SqlParameter[] parms = { new SqlParameter("@DeviceId", SqlDbType.VarChar, 100),
                                     new SqlParameter("@PointId", SqlDbType.VarChar, 100),
                                     new SqlParameter("@Start", SqlDbType.DateTime),
                                     new SqlParameter("@End", SqlDbType.DateTime) };

            parms[0].Value = SqlTypeConverter.DBNullStringChecker(device);
            parms[1].Value = SqlTypeConverter.DBNullStringChecker(point);
            parms[2].Value = SqlTypeConverter.DBNullDateTimeChecker(start);
            parms[3].Value = SqlTypeConverter.DBNullDateTimeChecker(end);

            using (var conn = new SqlConnection(this._databaseConnectionString)) {
                conn.Open();
                var trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                try {
                    SqlHelper.ExecuteNonQuery(trans, CommandType.Text, SqlCommands_Cs.Sql_V_Static_Repository_DeleteEntities, parms);
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
