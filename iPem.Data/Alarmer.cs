using iPem.Core;
using iPem.Data.Common;
using System;
using System.Collections.Generic;
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

        /// <summary>
        /// 开始告警
        /// </summary>
        public void Start(AlarmStart entity) {
            SqlParameter[] parms = {new SqlParameter("@Id", SqlDbType.VarChar, 100),
                                    new SqlParameter("@TId", SqlDbType.BigInt),
                                    new SqlParameter("@AreaId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@StationId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@RoomId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@FsuId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@DeviceId", SqlDbType.VarChar, 100),
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
                    parms[1].Value = SqlTypeConverter.DBNullInt64Checker(entity.TId);
                    parms[2].Value = SqlTypeConverter.DBNullStringChecker(entity.AreaId);
                    parms[3].Value = SqlTypeConverter.DBNullStringChecker(entity.StationId);
                    parms[4].Value = SqlTypeConverter.DBNullStringChecker(entity.RoomId);
                    parms[5].Value = SqlTypeConverter.DBNullStringChecker(entity.FsuId);
                    parms[6].Value = SqlTypeConverter.DBNullStringChecker(entity.DeviceId);
                    parms[7].Value = SqlTypeConverter.DBNullStringChecker(entity.PointId);
                    parms[8].Value = SqlTypeConverter.DBNullStringChecker(entity.SerialNo);
                    parms[9].Value = SqlTypeConverter.DBNullStringChecker(entity.NMAlarmId);
                    parms[10].Value = SqlTypeConverter.DBNullDateTimeChecker(entity.AlarmTime);
                    parms[11].Value = (int)entity.AlarmLevel;
                    parms[12].Value = (int)entity.AlarmFlag;
                    parms[13].Value = SqlTypeConverter.DBNullDoubleChecker(entity.AlarmValue);
                    parms[14].Value = SqlTypeConverter.DBNullStringChecker(entity.AlarmDesc);
                    parms[15].Value = SqlTypeConverter.DBNullStringChecker(entity.AlarmRemark);
                    parms[16].Value = SqlTypeConverter.DBNullStringChecker(entity.ReservationId);
                    parms[17].Value = SqlTypeConverter.DBNullStringChecker(entity.ReservationName);
                    parms[18].Value = SqlTypeConverter.DBNullDateTimeNullableChecker(entity.ReservationStart);
                    parms[19].Value = SqlTypeConverter.DBNullDateTimeNullableChecker(entity.ReservationEnd);
                    parms[20].Value = SqlTypeConverter.DBNullStringChecker(entity.PrimaryId);
                    parms[21].Value = SqlTypeConverter.DBNullStringChecker(entity.RelatedId);
                    parms[22].Value = SqlTypeConverter.DBNullStringChecker(entity.FilterId);
                    parms[23].Value = SqlTypeConverter.DBNullStringChecker(entity.ReversalId);
                    parms[24].Value = SqlTypeConverter.DBNullInt32Checker(entity.ReversalCount);
                    parms[25].Value = entity.Masked;
                    SqlHelper.ExecuteNonQuery(trans, CommandType.Text, SqlCommands_Cs.Sql_Alarm_Repository_Start, parms);
                    trans.Commit();
                } catch {
                    trans.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// 结束告警
        /// </summary>
        public void End(AlarmEnd entity) {
            SqlParameter[] parms = {new SqlParameter("@Id", SqlDbType.VarChar, 100),
                                    new SqlParameter("@TId", SqlDbType.BigInt),
                                    new SqlParameter("@AreaId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@StationId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@RoomId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@FsuId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@DeviceId", SqlDbType.VarChar, 100),
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
                    parms[1].Value = SqlTypeConverter.DBNullInt64Checker(entity.TId);
                    parms[2].Value = SqlTypeConverter.DBNullStringChecker(entity.AreaId);
                    parms[3].Value = SqlTypeConverter.DBNullStringChecker(entity.StationId);
                    parms[4].Value = SqlTypeConverter.DBNullStringChecker(entity.RoomId);
                    parms[5].Value = SqlTypeConverter.DBNullStringChecker(entity.FsuId);
                    parms[6].Value = SqlTypeConverter.DBNullStringChecker(entity.DeviceId);
                    parms[7].Value = SqlTypeConverter.DBNullStringChecker(entity.PointId);
                    parms[8].Value = SqlTypeConverter.DBNullStringChecker(entity.SerialNo);
                    parms[9].Value = SqlTypeConverter.DBNullStringChecker(entity.NMAlarmId);
                    parms[10].Value = SqlTypeConverter.DBNullDateTimeChecker(entity.StartTime);
                    parms[11].Value = SqlTypeConverter.DBNullDateTimeChecker(entity.EndTime);
                    parms[12].Value = (int)entity.AlarmLevel;
                    parms[13].Value = (int)entity.AlarmFlag;
                    parms[14].Value = SqlTypeConverter.DBNullDoubleChecker(entity.StartValue);
                    parms[15].Value = SqlTypeConverter.DBNullDoubleChecker(entity.EndValue);
                    parms[16].Value = SqlTypeConverter.DBNullStringChecker(entity.AlarmDesc);
                    parms[17].Value = SqlTypeConverter.DBNullStringChecker(entity.AlarmRemark);
                    parms[18].Value = (int)entity.Confirmed;
                    parms[19].Value = SqlTypeConverter.DBNullStringChecker(entity.Confirmer);
                    parms[20].Value = SqlTypeConverter.DBNullDateTimeNullableChecker(entity.ConfirmedTime);
                    parms[21].Value = SqlTypeConverter.DBNullStringChecker(entity.ReservationId);
                    parms[22].Value = SqlTypeConverter.DBNullStringChecker(entity.ReservationName);
                    parms[23].Value = SqlTypeConverter.DBNullDateTimeNullableChecker(entity.ReservationStart);
                    parms[24].Value = SqlTypeConverter.DBNullDateTimeNullableChecker(entity.ReservationEnd);
                    parms[25].Value = SqlTypeConverter.DBNullStringChecker(entity.PrimaryId);
                    parms[26].Value = SqlTypeConverter.DBNullStringChecker(entity.RelatedId);
                    parms[27].Value = SqlTypeConverter.DBNullStringChecker(entity.FilterId);
                    parms[28].Value = SqlTypeConverter.DBNullStringChecker(entity.ReversalId);
                    parms[29].Value = SqlTypeConverter.DBNullInt32Checker(entity.ReversalCount);
                    parms[30].Value = entity.Masked;
                    SqlHelper.ExecuteNonQuery(trans, CommandType.Text, string.Format(SqlCommands_Cs.Sql_Alarm_Repository_End, entity.StartTime.ToString("yyyyMM")), parms);
                    trans.Commit();
                } catch {
                    trans.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// 告警接口
        /// </summary>
        public void SaveInterface(List<A_IAlarm> entities) {
            SqlParameter[] parms = {new SqlParameter("@Id", SqlDbType.BigInt),
                                    new SqlParameter("@AreaId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@StationId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@RoomId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@FsuId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@DeviceId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@PointId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@SerialNo", SqlDbType.VarChar, 100),
                                    new SqlParameter("@NMAlarmId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@AlarmTime", SqlDbType.DateTime),
                                    new SqlParameter("@AlarmLevel", SqlDbType.Int),
                                    new SqlParameter("@AlarmFlag", SqlDbType.Int),
                                    new SqlParameter("@AlarmDesc", SqlDbType.VarChar, 120),
                                    new SqlParameter("@AlarmValue", SqlDbType.Float),
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
                                    new SqlParameter("@Masked", SqlDbType.Bit),
                                    new SqlParameter("@CreatedTime", SqlDbType.DateTime) };

            using (var conn = new SqlConnection(this._databaseConnectionString)) {
                conn.Open();
                var trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                try {
                    foreach (var entity in entities) {
                        parms[0].Value = SqlTypeConverter.DBNullInt64Checker(entity.Id);
                        parms[1].Value = SqlTypeConverter.DBNullStringChecker(entity.AreaId);
                        parms[2].Value = SqlTypeConverter.DBNullStringChecker(entity.StationId);
                        parms[3].Value = SqlTypeConverter.DBNullStringChecker(entity.RoomId);
                        parms[4].Value = SqlTypeConverter.DBNullStringChecker(entity.FsuId);
                        parms[5].Value = SqlTypeConverter.DBNullStringChecker(entity.DeviceId);
                        parms[6].Value = SqlTypeConverter.DBNullStringChecker(entity.PointId);
                        parms[7].Value = SqlTypeConverter.DBNullStringChecker(entity.SerialNo);
                        parms[8].Value = SqlTypeConverter.DBNullStringChecker(entity.NMAlarmId);
                        parms[9].Value = SqlTypeConverter.DBNullDateTimeChecker(entity.AlarmTime);
                        parms[10].Value = (int)entity.AlarmLevel;
                        parms[11].Value = (int)entity.AlarmFlag;
                        parms[12].Value = SqlTypeConverter.DBNullStringChecker(entity.AlarmDesc);
                        parms[13].Value = SqlTypeConverter.DBNullDoubleChecker(entity.AlarmValue);
                        parms[14].Value = SqlTypeConverter.DBNullStringChecker(entity.AlarmRemark);
                        parms[15].Value = (int)entity.Confirmed;
                        parms[16].Value = SqlTypeConverter.DBNullStringChecker(entity.Confirmer);
                        parms[17].Value = SqlTypeConverter.DBNullDateTimeNullableChecker(entity.ConfirmedTime);
                        parms[18].Value = SqlTypeConverter.DBNullStringChecker(entity.ReservationId);
                        parms[19].Value = SqlTypeConverter.DBNullStringChecker(entity.ReservationName);
                        parms[20].Value = SqlTypeConverter.DBNullDateTimeNullableChecker(entity.ReservationStart);
                        parms[21].Value = SqlTypeConverter.DBNullDateTimeNullableChecker(entity.ReservationEnd);
                        parms[22].Value = SqlTypeConverter.DBNullStringChecker(entity.PrimaryId);
                        parms[23].Value = SqlTypeConverter.DBNullStringChecker(entity.RelatedId);
                        parms[24].Value = SqlTypeConverter.DBNullStringChecker(entity.FilterId);
                        parms[25].Value = SqlTypeConverter.DBNullStringChecker(entity.ReversalId);
                        parms[26].Value = entity.Masked;
                        parms[27].Value = SqlTypeConverter.DBNullDateTimeChecker(entity.CreatedTime);
                        SqlHelper.ExecuteNonQuery(trans, CommandType.Text, SqlCommands_Cs.Sql_Alarm_Repository_SaveInterface, parms);
                    }

                    trans.Commit();
                } catch {
                    trans.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// 短信告警
        /// </summary>
        public void SaveMessage(List<A_MAlarm> entities) {
            SqlParameter[] parms = {new SqlParameter("@Id", SqlDbType.BigInt),
                                    new SqlParameter("@AreaId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@AreaName", SqlDbType.VarChar, 200),
                                    new SqlParameter("@StationId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@StationName", SqlDbType.VarChar, 200),
                                    new SqlParameter("@RoomId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@RoomName", SqlDbType.VarChar, 200),
                                    new SqlParameter("@DeviceId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@DeviceName", SqlDbType.VarChar, 200),
                                    new SqlParameter("@PointId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@PointName", SqlDbType.VarChar, 200),
                                    new SqlParameter("@SerialNo", SqlDbType.VarChar, 100),
                                    new SqlParameter("@NMAlarmId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@AlarmTime", SqlDbType.DateTime),
                                    new SqlParameter("@AlarmLevel", SqlDbType.Int),
                                    new SqlParameter("@AlarmFlag", SqlDbType.Int),
                                    new SqlParameter("@AlarmDesc", SqlDbType.VarChar, 120),
                                    new SqlParameter("@AlarmValue", SqlDbType.Float),
                                    new SqlParameter("@AlarmRemark", SqlDbType.VarChar, 100),
                                    new SqlParameter("@Confirmed", SqlDbType.Int),
                                    new SqlParameter("@Confirmer", SqlDbType.VarChar, 100),
                                    new SqlParameter("@ConfirmedTime", SqlDbType.DateTime),
                                    new SqlParameter("@ReservationId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@PrimaryId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@RelatedId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@FilterId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@ReversalId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@Masked", SqlDbType.Bit),
                                    new SqlParameter("@CreatedTime", SqlDbType.DateTime) };

            using (var conn = new SqlConnection(this._databaseConnectionString)) {
                conn.Open();
                var trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                try {
                    foreach (var entity in entities) {
                        parms[0].Value = SqlTypeConverter.DBNullInt64Checker(entity.Id);
                        parms[1].Value = SqlTypeConverter.DBNullStringChecker(entity.AreaId);
                        parms[2].Value = SqlTypeConverter.DBNullStringChecker(entity.AreaName);
                        parms[3].Value = SqlTypeConverter.DBNullStringChecker(entity.StationId);
                        parms[4].Value = SqlTypeConverter.DBNullStringChecker(entity.StationName);
                        parms[5].Value = SqlTypeConverter.DBNullStringChecker(entity.RoomId);
                        parms[6].Value = SqlTypeConverter.DBNullStringChecker(entity.RoomName);
                        parms[7].Value = SqlTypeConverter.DBNullStringChecker(entity.DeviceId);
                        parms[8].Value = SqlTypeConverter.DBNullStringChecker(entity.DeviceName);
                        parms[9].Value = SqlTypeConverter.DBNullStringChecker(entity.PointId);
                        parms[10].Value = SqlTypeConverter.DBNullStringChecker(entity.PointName);
                        parms[11].Value = SqlTypeConverter.DBNullStringChecker(entity.SerialNo);
                        parms[12].Value = SqlTypeConverter.DBNullStringChecker(entity.NMAlarmId);
                        parms[13].Value = SqlTypeConverter.DBNullDateTimeChecker(entity.AlarmTime);
                        parms[14].Value = (int)entity.AlarmLevel;
                        parms[15].Value = (int)entity.AlarmFlag;
                        parms[16].Value = SqlTypeConverter.DBNullStringChecker(entity.AlarmDesc);
                        parms[17].Value = SqlTypeConverter.DBNullDoubleChecker(entity.AlarmValue);
                        parms[18].Value = SqlTypeConverter.DBNullStringChecker(entity.AlarmRemark);
                        parms[19].Value = (int)entity.Confirmed;
                        parms[20].Value = SqlTypeConverter.DBNullStringChecker(entity.Confirmer);
                        parms[21].Value = SqlTypeConverter.DBNullDateTimeNullableChecker(entity.ConfirmedTime);
                        parms[22].Value = SqlTypeConverter.DBNullStringChecker(entity.ReservationId);
                        parms[23].Value = SqlTypeConverter.DBNullStringChecker(entity.PrimaryId);
                        parms[24].Value = SqlTypeConverter.DBNullStringChecker(entity.RelatedId);
                        parms[25].Value = SqlTypeConverter.DBNullStringChecker(entity.FilterId);
                        parms[26].Value = SqlTypeConverter.DBNullStringChecker(entity.ReversalId);
                        parms[27].Value = entity.Masked;
                        parms[28].Value = SqlTypeConverter.DBNullDateTimeChecker(entity.CreatedTime);
                        SqlHelper.ExecuteNonQuery(trans, CommandType.Text, SqlCommands_Cs.Sql_Alarm_Repository_SaveMessage, parms);
                    }

                    trans.Commit();
                } catch {
                    trans.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// 语音告警
        /// </summary>
        public void SaveSpeech(List<A_SAlarm> entities) {
            SqlParameter[] parms = {new SqlParameter("@Id", SqlDbType.BigInt),
                                    new SqlParameter("@AreaId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@AreaName", SqlDbType.VarChar, 200),
                                    new SqlParameter("@StationId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@StationName", SqlDbType.VarChar, 200),
                                    new SqlParameter("@RoomId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@RoomName", SqlDbType.VarChar, 200),
                                    new SqlParameter("@DeviceId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@DeviceName", SqlDbType.VarChar, 200),
                                    new SqlParameter("@PointId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@PointName", SqlDbType.VarChar, 200),
                                    new SqlParameter("@SerialNo", SqlDbType.VarChar, 100),
                                    new SqlParameter("@NMAlarmId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@AlarmTime", SqlDbType.DateTime),
                                    new SqlParameter("@AlarmLevel", SqlDbType.Int),
                                    new SqlParameter("@AlarmFlag", SqlDbType.Int),
                                    new SqlParameter("@AlarmDesc", SqlDbType.VarChar, 120),
                                    new SqlParameter("@AlarmValue", SqlDbType.Float),
                                    new SqlParameter("@AlarmRemark", SqlDbType.VarChar, 100),
                                    new SqlParameter("@Confirmed", SqlDbType.Int),
                                    new SqlParameter("@Confirmer", SqlDbType.VarChar, 100),
                                    new SqlParameter("@ConfirmedTime", SqlDbType.DateTime),
                                    new SqlParameter("@ReservationId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@PrimaryId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@RelatedId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@FilterId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@ReversalId", SqlDbType.VarChar, 100),
                                    new SqlParameter("@Masked", SqlDbType.Bit),
                                    new SqlParameter("@CreatedTime", SqlDbType.DateTime) };

            using (var conn = new SqlConnection(this._databaseConnectionString)) {
                conn.Open();
                var trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                try {
                    foreach (var entity in entities) {
                        parms[0].Value = SqlTypeConverter.DBNullInt64Checker(entity.Id);
                        parms[1].Value = SqlTypeConverter.DBNullStringChecker(entity.AreaId);
                        parms[2].Value = SqlTypeConverter.DBNullStringChecker(entity.AreaName);
                        parms[3].Value = SqlTypeConverter.DBNullStringChecker(entity.StationId);
                        parms[4].Value = SqlTypeConverter.DBNullStringChecker(entity.StationName);
                        parms[5].Value = SqlTypeConverter.DBNullStringChecker(entity.RoomId);
                        parms[6].Value = SqlTypeConverter.DBNullStringChecker(entity.RoomName);
                        parms[7].Value = SqlTypeConverter.DBNullStringChecker(entity.DeviceId);
                        parms[8].Value = SqlTypeConverter.DBNullStringChecker(entity.DeviceName);
                        parms[9].Value = SqlTypeConverter.DBNullStringChecker(entity.PointId);
                        parms[10].Value = SqlTypeConverter.DBNullStringChecker(entity.PointName);
                        parms[11].Value = SqlTypeConverter.DBNullStringChecker(entity.SerialNo);
                        parms[12].Value = SqlTypeConverter.DBNullStringChecker(entity.NMAlarmId);
                        parms[13].Value = SqlTypeConverter.DBNullDateTimeChecker(entity.AlarmTime);
                        parms[14].Value = (int)entity.AlarmLevel;
                        parms[15].Value = (int)entity.AlarmFlag;
                        parms[16].Value = SqlTypeConverter.DBNullStringChecker(entity.AlarmDesc);
                        parms[17].Value = SqlTypeConverter.DBNullDoubleChecker(entity.AlarmValue);
                        parms[18].Value = SqlTypeConverter.DBNullStringChecker(entity.AlarmRemark);
                        parms[19].Value = (int)entity.Confirmed;
                        parms[20].Value = SqlTypeConverter.DBNullStringChecker(entity.Confirmer);
                        parms[21].Value = SqlTypeConverter.DBNullDateTimeNullableChecker(entity.ConfirmedTime);
                        parms[22].Value = SqlTypeConverter.DBNullStringChecker(entity.ReservationId);
                        parms[23].Value = SqlTypeConverter.DBNullStringChecker(entity.PrimaryId);
                        parms[24].Value = SqlTypeConverter.DBNullStringChecker(entity.RelatedId);
                        parms[25].Value = SqlTypeConverter.DBNullStringChecker(entity.FilterId);
                        parms[26].Value = SqlTypeConverter.DBNullStringChecker(entity.ReversalId);
                        parms[27].Value = entity.Masked;
                        parms[28].Value = SqlTypeConverter.DBNullDateTimeChecker(entity.CreatedTime);
                        SqlHelper.ExecuteNonQuery(trans, CommandType.Text, SqlCommands_Cs.Sql_Alarm_Repository_SaveSpeech, parms);
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
