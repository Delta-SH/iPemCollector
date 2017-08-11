﻿using iPem.Core;
using iPem.Data.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace iPem.Data {
    public partial class A_TAlarmRepository {

        #region Fields

        private readonly string _databaseConnectionString;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        public A_TAlarmRepository() {
            this._databaseConnectionString = SqlHelper.ConnectionStringCsTransaction;
        }

        #endregion

        #region Methods

        public List<A_TAlarm> GetEntities() {
            var entities = new List<A_TAlarm>();
            using (var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Cs.Sql_A_TAlarm_Repository_GetEntities1, null)) {
                while (rdr.Read()) {
                    var entity = new A_TAlarm();
                    entity.FsuId = SqlTypeConverter.DBNullStringHandler(rdr["FsuId"]);
                    entity.DeviceId = SqlTypeConverter.DBNullStringHandler(rdr["DeviceId"]);
                    entity.PointId = SqlTypeConverter.DBNullStringHandler(rdr["PointId"]);
                    entity.SignalId = SqlTypeConverter.DBNullStringHandler(rdr["SignalId"]);
                    entity.SignalNumber = SqlTypeConverter.DBNullStringHandler(rdr["SignalNumber"]);
                    entity.SerialNo = SqlTypeConverter.DBNullStringHandler(rdr["SerialNo"]);
                    entity.NMAlarmId = SqlTypeConverter.DBNullStringHandler(rdr["NMAlarmId"]);
                    entity.AlarmTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["AlarmTime"]);
                    entity.AlarmLevel = SqlTypeConverter.DBNullEnmLevelHandler(rdr["AlarmLevel"]);
                    entity.AlarmFlag = SqlTypeConverter.DBNullEnmFlagHandler(rdr["AlarmFlag"]);
                    entity.AlarmDesc = SqlTypeConverter.DBNullStringHandler(rdr["AlarmDesc"]);
                    entity.AlarmValue = SqlTypeConverter.DBNullDoubleHandler(rdr["AlarmValue"]);
                    entity.AlarmRemark = SqlTypeConverter.DBNullStringHandler(rdr["AlarmRemark"]);
                    entities.Add(entity);
                }
            }
            return entities;
        }

        public List<A_TAlarm> GetEntities(DateTime start, DateTime end) {
            SqlParameter[] parms = { new SqlParameter("@Start", SqlDbType.DateTime),
                                     new SqlParameter("@End", SqlDbType.DateTime) };

            parms[0].Value = SqlTypeConverter.DBNullDateTimeHandler(start);
            parms[1].Value = SqlTypeConverter.DBNullDateTimeHandler(end);

            var entities = new List<A_TAlarm>();
            using (var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Cs.Sql_A_TAlarm_Repository_GetEntities2, parms)) {
                while(rdr.Read()) {
                    var entity = new A_TAlarm();
                    entity.FsuId = SqlTypeConverter.DBNullStringHandler(rdr["FsuId"]);
                    entity.DeviceId = SqlTypeConverter.DBNullStringHandler(rdr["DeviceId"]);
                    entity.PointId = SqlTypeConverter.DBNullStringHandler(rdr["PointId"]);
                    entity.SignalId = SqlTypeConverter.DBNullStringHandler(rdr["SignalId"]);
                    entity.SignalNumber = SqlTypeConverter.DBNullStringHandler(rdr["SignalNumber"]);
                    entity.SerialNo = SqlTypeConverter.DBNullStringHandler(rdr["SerialNo"]);
                    entity.NMAlarmId = SqlTypeConverter.DBNullStringHandler(rdr["NMAlarmId"]);
                    entity.AlarmTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["AlarmTime"]);
                    entity.AlarmLevel = SqlTypeConverter.DBNullEnmLevelHandler(rdr["AlarmLevel"]);
                    entity.AlarmFlag = SqlTypeConverter.DBNullEnmFlagHandler(rdr["AlarmFlag"]);
                    entity.AlarmDesc = SqlTypeConverter.DBNullStringHandler(rdr["AlarmDesc"]);
                    entity.AlarmValue = SqlTypeConverter.DBNullDoubleHandler(rdr["AlarmValue"]);
                    entity.AlarmRemark = SqlTypeConverter.DBNullStringHandler(rdr["AlarmRemark"]);
                    entities.Add(entity);
                }
            }
            return entities;
        }

        public void SaveEntities(params A_FAlarm[] entities) {
            SqlParameter[] parms = { new SqlParameter("@FsuId",SqlDbType.VarChar,100),
                                     new SqlParameter("@DeviceId",SqlDbType.VarChar,100),
                                     new SqlParameter("@PointId",SqlDbType.VarChar,100),
                                     new SqlParameter("@SignalId", SqlDbType.VarChar,100),
                                     new SqlParameter("@SignalNumber", SqlDbType.VarChar,10),
                                     new SqlParameter("@SerialNo", SqlDbType.VarChar,100),
                                     new SqlParameter("@NMAlarmId", SqlDbType.VarChar,100),
                                     new SqlParameter("@AlarmTime",SqlDbType.DateTime),
                                     new SqlParameter("@AlarmLevel", SqlDbType.Int),
                                     new SqlParameter("@AlarmFlag", SqlDbType.Int),
                                     new SqlParameter("@AlarmDesc", SqlDbType.VarChar,120),
                                     new SqlParameter("@AlarmValue", SqlDbType.Float),
                                     new SqlParameter("@AlarmRemark", SqlDbType.VarChar,100)};

            using (var conn = new SqlConnection(this._databaseConnectionString)) {
                conn.Open();
                var trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                try {
                    foreach (var entity in entities) {
                        parms[0].Value = SqlTypeConverter.DBNullStringChecker(entity.FsuId);
                        parms[1].Value = SqlTypeConverter.DBNullStringChecker(entity.DeviceId);
                        parms[2].Value = SqlTypeConverter.DBNullStringChecker(entity.PointId);
                        parms[3].Value = SqlTypeConverter.DBNullStringChecker(entity.SignalId);
                        parms[4].Value = SqlTypeConverter.DBNullStringChecker(entity.SignalNumber);
                        parms[5].Value = SqlTypeConverter.DBNullStringChecker(entity.SerialNo);
                        parms[6].Value = SqlTypeConverter.DBNullStringChecker(entity.NMAlarmId);
                        parms[7].Value = SqlTypeConverter.DBNullDateTimeChecker(entity.AlarmTime);
                        parms[8].Value = (int)entity.AlarmLevel;
                        parms[9].Value = (int)entity.AlarmFlag;
                        parms[10].Value = SqlTypeConverter.DBNullStringChecker(entity.AlarmDesc);
                        parms[11].Value = SqlTypeConverter.DBNullDoubleChecker(entity.AlarmValue);
                        parms[12].Value = SqlTypeConverter.DBNullStringChecker(entity.AlarmRemark);
                        SqlHelper.ExecuteNonQuery(trans, CommandType.Text, SqlCommands_Cs.Sql_A_TAlarm_Repository_SaveEntities, parms);
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
