﻿using iPem.Core;
using iPem.Data.Common;
using System;
using System.Data;
using System.Data.SqlClient;

namespace iPem.Data {
    public partial class Alarmer {

        #region Fields

        private readonly string _databaseConnectionString;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        public Alarmer() {
            this._databaseConnectionString = SqlHelper.ConnectionStringCsTransaction;
        }

        #endregion

        #region Methods

        public void Start(AlarmStart entity) {
            SqlParameter[] parms = {new SqlParameter("@Id", SqlDbType.VarChar, 100),
                                    new SqlParameter("@AreaId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@StationId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@RoomId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@FsuId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@FsuCode", SqlDbType.VarChar, 100),
                                    new SqlParameter("@DeviceId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@DeviceCode", SqlDbType.VarChar, 100),
                                    new SqlParameter("@PointId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@SerialNo", SqlDbType.VarChar, 100),
                                    new SqlParameter("@NMAlarmId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@AlarmTime", SqlDbType.DateTime),
                                    new SqlParameter("@AlarmLevel", SqlDbType.Int),
                                    new SqlParameter("@AlarmFlag", SqlDbType.Int),
                                    new SqlParameter("@AlarmValue", SqlDbType.Float),
                                    new SqlParameter("@AlarmDesc", SqlDbType.VarChar, 120),
                                    new SqlParameter("@AlarmRemark", SqlDbType.VarChar, 100),
                                    new SqlParameter("@ReservationId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@ReservationName", SqlDbType.VarChar, 200),
                                    new SqlParameter("@ReservationStart", SqlDbType.DateTime),
                                    new SqlParameter("@ReservationEnd", SqlDbType.DateTime),
                                    new SqlParameter("@PrimaryId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@RelatedId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@FilterId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@ReversalId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@ReversalCount", SqlDbType.Int),
                                    new SqlParameter("@Masked", SqlDbType.Bit) };

            using (var conn = new SqlConnection(this._databaseConnectionString)) {
                conn.Open();
                var trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                try {
                    parms[0].Value = SqlTypeConverter.DBNullStringChecker(entity.Id);
                    parms[1].Value = SqlTypeConverter.DBNullStringChecker(entity.AreaId);
                    parms[2].Value = SqlTypeConverter.DBNullStringChecker(entity.StationId);
                    parms[3].Value = SqlTypeConverter.DBNullStringChecker(entity.RoomId);
                    parms[4].Value = SqlTypeConverter.DBNullStringChecker(entity.FsuId);
                    parms[5].Value = SqlTypeConverter.DBNullStringChecker(entity.FsuCode);
                    parms[6].Value = SqlTypeConverter.DBNullStringChecker(entity.DeviceId);
                    parms[7].Value = SqlTypeConverter.DBNullStringChecker(entity.DeviceCode);
                    parms[8].Value = SqlTypeConverter.DBNullStringChecker(entity.PointId);
                    parms[9].Value = SqlTypeConverter.DBNullStringChecker(entity.SerialNo);
                    parms[10].Value = SqlTypeConverter.DBNullStringChecker(entity.NMAlarmId);
                    parms[11].Value = SqlTypeConverter.DBNullDateTimeChecker(entity.AlarmTime);
                    parms[12].Value = (int)entity.AlarmLevel;
                    parms[13].Value = (int)entity.AlarmFlag;
                    parms[14].Value = SqlTypeConverter.DBNullDoubleChecker(entity.AlarmValue);
                    parms[15].Value = SqlTypeConverter.DBNullStringChecker(entity.AlarmDesc);
                    parms[16].Value = SqlTypeConverter.DBNullStringChecker(entity.AlarmRemark);
                    parms[17].Value = SqlTypeConverter.DBNullStringChecker(entity.ReservationId);
                    parms[18].Value = SqlTypeConverter.DBNullStringChecker(entity.ReservationName);
                    parms[19].Value = SqlTypeConverter.DBNullDateTimeNullableChecker(entity.ReservationStart);
                    parms[20].Value = SqlTypeConverter.DBNullDateTimeNullableChecker(entity.ReservationEnd);
                    parms[21].Value = SqlTypeConverter.DBNullStringChecker(entity.PrimaryId);
                    parms[22].Value = SqlTypeConverter.DBNullStringChecker(entity.RelatedId);
                    parms[23].Value = SqlTypeConverter.DBNullStringChecker(entity.FilterId);
                    parms[24].Value = SqlTypeConverter.DBNullStringChecker(entity.ReversalId);
                    parms[25].Value = SqlTypeConverter.DBNullInt32Checker(entity.ReversalCount);
                    parms[26].Value = entity.Masked;
                    SqlHelper.ExecuteNonQuery(trans, CommandType.Text, SqlCommands_Cs.Sql_Alarm_Repository_Start, parms);
                    trans.Commit();
                } catch {
                    trans.Rollback();
                    throw;
                }
            }
        }

        public void End(AlarmEnd entity) {
            SqlParameter[] parms = {new SqlParameter("@Id", SqlDbType.VarChar, 100),
                                    new SqlParameter("@AreaId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@StationId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@RoomId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@FsuId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@FsuCode", SqlDbType.VarChar, 100),
                                    new SqlParameter("@DeviceId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@DeviceCode", SqlDbType.VarChar, 100),
                                    new SqlParameter("@PointId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@SerialNo", SqlDbType.VarChar, 100),
                                    new SqlParameter("@NMAlarmId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@StartTime", SqlDbType.DateTime),
                                    new SqlParameter("@EndTime", SqlDbType.DateTime),
                                    new SqlParameter("@AlarmLevel", SqlDbType.Int),
                                    new SqlParameter("@AlarmFlag", SqlDbType.Int),
                                    new SqlParameter("@StartValue", SqlDbType.Float),
                                    new SqlParameter("@EndValue", SqlDbType.Float),
                                    new SqlParameter("@AlarmDesc", SqlDbType.VarChar, 120),
                                    new SqlParameter("@AlarmRemark", SqlDbType.VarChar, 100),
                                    new SqlParameter("@Confirmed", SqlDbType.Int),
                                    new SqlParameter("@Confirmer", SqlDbType.VarChar, 100),
                                    new SqlParameter("@ConfirmedTime", SqlDbType.DateTime),
                                    new SqlParameter("@ReservationId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@ReservationName", SqlDbType.VarChar, 200),
                                    new SqlParameter("@ReservationStart", SqlDbType.DateTime),
                                    new SqlParameter("@ReservationEnd", SqlDbType.DateTime),
                                    new SqlParameter("@PrimaryId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@RelatedId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@FilterId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@ReversalId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@ReversalCount", SqlDbType.Int),
                                    new SqlParameter("@Masked", SqlDbType.Bit) };

            using (var conn = new SqlConnection(this._databaseConnectionString)) {
                conn.Open();
                var trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                try {
                    parms[0].Value = SqlTypeConverter.DBNullStringChecker(entity.Id);
                    parms[1].Value = SqlTypeConverter.DBNullStringChecker(entity.AreaId);
                    parms[2].Value = SqlTypeConverter.DBNullStringChecker(entity.StationId);
                    parms[3].Value = SqlTypeConverter.DBNullStringChecker(entity.RoomId);
                    parms[4].Value = SqlTypeConverter.DBNullStringChecker(entity.FsuId);
                    parms[5].Value = SqlTypeConverter.DBNullStringChecker(entity.FsuCode);
                    parms[6].Value = SqlTypeConverter.DBNullStringChecker(entity.DeviceId);
                    parms[7].Value = SqlTypeConverter.DBNullStringChecker(entity.DeviceCode);
                    parms[8].Value = SqlTypeConverter.DBNullStringChecker(entity.PointId);
                    parms[9].Value = SqlTypeConverter.DBNullStringChecker(entity.SerialNo);
                    parms[10].Value = SqlTypeConverter.DBNullStringChecker(entity.NMAlarmId);
                    parms[11].Value = SqlTypeConverter.DBNullDateTimeChecker(entity.StartTime);
                    parms[12].Value = SqlTypeConverter.DBNullDateTimeChecker(entity.EndTime);
                    parms[13].Value = (int)entity.AlarmLevel;
                    parms[14].Value = (int)entity.AlarmFlag;
                    parms[15].Value = SqlTypeConverter.DBNullDoubleChecker(entity.StartValue);
                    parms[16].Value = SqlTypeConverter.DBNullDoubleChecker(entity.EndValue);
                    parms[17].Value = SqlTypeConverter.DBNullStringChecker(entity.AlarmDesc);
                    parms[18].Value = SqlTypeConverter.DBNullStringChecker(entity.AlarmRemark);
                    parms[19].Value = (int)entity.Confirmed;
                    parms[20].Value = SqlTypeConverter.DBNullStringChecker(entity.Confirmer);
                    parms[21].Value = SqlTypeConverter.DBNullDateTimeNullableChecker(entity.ConfirmedTime);
                    parms[22].Value = SqlTypeConverter.DBNullStringChecker(entity.ReservationId);
                    parms[23].Value = SqlTypeConverter.DBNullStringChecker(entity.ReservationName);
                    parms[24].Value = SqlTypeConverter.DBNullDateTimeNullableChecker(entity.ReservationStart);
                    parms[25].Value = SqlTypeConverter.DBNullDateTimeNullableChecker(entity.ReservationEnd);
                    parms[26].Value = SqlTypeConverter.DBNullStringChecker(entity.PrimaryId);
                    parms[27].Value = SqlTypeConverter.DBNullStringChecker(entity.RelatedId);
                    parms[28].Value = SqlTypeConverter.DBNullStringChecker(entity.FilterId);
                    parms[29].Value = SqlTypeConverter.DBNullStringChecker(entity.ReversalId);
                    parms[30].Value = SqlTypeConverter.DBNullInt32Checker(entity.ReversalCount);
                    parms[31].Value = entity.Masked;
                    SqlHelper.ExecuteNonQuery(trans, CommandType.Text, string.Format(SqlCommands_Cs.Sql_Alarm_Repository_End, entity.StartTime.ToString("yyyyMM")), parms);
                    trans.Commit();
                } catch {
                    trans.Rollback();
                    throw;
                }
            }
        }

        public void Delete(A_TAlarm entity) {
            SqlParameter[] parms = { new SqlParameter("@FsuId", SqlDbType.VarChar, 100), 
                                     new SqlParameter("@SerialNo", SqlDbType.VarChar, 100), 
                                     new SqlParameter("@AlarmFlag", SqlDbType.Int) };

            using (var conn = new SqlConnection(this._databaseConnectionString)) {
                conn.Open();
                var trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                try {
                    parms[0].Value = SqlTypeConverter.DBNullStringChecker(entity.FsuId);
                    parms[1].Value = SqlTypeConverter.DBNullStringChecker(entity.SerialNo);
                    parms[2].Value = (int)entity.AlarmFlag;
                    SqlHelper.ExecuteNonQuery(trans, CommandType.Text, SqlCommands_Cs.Sql_Alarm_Repository_Delete, parms);
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
