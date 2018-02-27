using iPem.Core;
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

        public void StartInterface(AlarmStart entity) {
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
                    parms[0].Value = SqlTypeConverter.DBNullInt64Checker(entity.TId);
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
                    parms[12].Value = SqlTypeConverter.DBNullDoubleChecker(entity.AlarmValue);
                    parms[13].Value = SqlTypeConverter.DBNullStringChecker(entity.AlarmDesc);
                    parms[14].Value = SqlTypeConverter.DBNullStringChecker(entity.AlarmRemark);
                    parms[15].Value = SqlTypeConverter.DBNullStringChecker(entity.ReservationId);
                    parms[16].Value = SqlTypeConverter.DBNullStringChecker(entity.ReservationName);
                    parms[17].Value = SqlTypeConverter.DBNullDateTimeNullableChecker(entity.ReservationStart);
                    parms[18].Value = SqlTypeConverter.DBNullDateTimeNullableChecker(entity.ReservationEnd);
                    parms[19].Value = SqlTypeConverter.DBNullStringChecker(entity.PrimaryId);
                    parms[20].Value = SqlTypeConverter.DBNullStringChecker(entity.RelatedId);
                    parms[21].Value = SqlTypeConverter.DBNullStringChecker(entity.FilterId);
                    parms[22].Value = SqlTypeConverter.DBNullStringChecker(entity.ReversalId);
                    parms[23].Value = SqlTypeConverter.DBNullInt32Checker(entity.ReversalCount);
                    parms[24].Value = entity.Masked;
                    SqlHelper.ExecuteNonQuery(trans, CommandType.Text, SqlCommands_Cs.Sql_Alarm_Repository_StartInterface, parms);
                    trans.Commit();
                } catch {
                    trans.Rollback();
                    throw;
                }
            }
        }

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

        public void EndInterface(AlarmEnd entity) {
            SqlParameter[] parms = {new SqlParameter("@Id", SqlDbType.VarChar, 100),
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
                    parms[0].Value = SqlTypeConverter.DBNullInt64Checker(entity.TId);
                    parms[1].Value = SqlTypeConverter.DBNullStringChecker(entity.AreaId);
                    parms[2].Value = SqlTypeConverter.DBNullStringChecker(entity.StationId);
                    parms[3].Value = SqlTypeConverter.DBNullStringChecker(entity.RoomId);
                    parms[4].Value = SqlTypeConverter.DBNullStringChecker(entity.FsuId);
                    parms[5].Value = SqlTypeConverter.DBNullStringChecker(entity.DeviceId);
                    parms[6].Value = SqlTypeConverter.DBNullStringChecker(entity.PointId);
                    parms[7].Value = SqlTypeConverter.DBNullStringChecker(entity.SerialNo);
                    parms[8].Value = SqlTypeConverter.DBNullStringChecker(entity.NMAlarmId);
                    parms[9].Value = SqlTypeConverter.DBNullDateTimeChecker(entity.StartTime);
                    parms[10].Value = SqlTypeConverter.DBNullDateTimeChecker(entity.EndTime);
                    parms[11].Value = (int)entity.AlarmLevel;
                    parms[12].Value = (int)entity.AlarmFlag;
                    parms[13].Value = SqlTypeConverter.DBNullDoubleChecker(entity.StartValue);
                    parms[14].Value = SqlTypeConverter.DBNullDoubleChecker(entity.EndValue);
                    parms[15].Value = SqlTypeConverter.DBNullStringChecker(entity.AlarmDesc);
                    parms[16].Value = SqlTypeConverter.DBNullStringChecker(entity.AlarmRemark);
                    parms[17].Value = (int)entity.Confirmed;
                    parms[18].Value = SqlTypeConverter.DBNullStringChecker(entity.Confirmer);
                    parms[19].Value = SqlTypeConverter.DBNullDateTimeNullableChecker(entity.ConfirmedTime);
                    parms[20].Value = SqlTypeConverter.DBNullStringChecker(entity.ReservationId);
                    parms[21].Value = SqlTypeConverter.DBNullStringChecker(entity.ReservationName);
                    parms[22].Value = SqlTypeConverter.DBNullDateTimeNullableChecker(entity.ReservationStart);
                    parms[23].Value = SqlTypeConverter.DBNullDateTimeNullableChecker(entity.ReservationEnd);
                    parms[24].Value = SqlTypeConverter.DBNullStringChecker(entity.PrimaryId);
                    parms[25].Value = SqlTypeConverter.DBNullStringChecker(entity.RelatedId);
                    parms[26].Value = SqlTypeConverter.DBNullStringChecker(entity.FilterId);
                    parms[27].Value = SqlTypeConverter.DBNullStringChecker(entity.ReversalId);
                    parms[28].Value = SqlTypeConverter.DBNullInt32Checker(entity.ReversalCount);
                    parms[29].Value = entity.Masked;
                    SqlHelper.ExecuteNonQuery(trans, CommandType.Text, SqlCommands_Cs.Sql_Alarm_Repository_EndInterface, parms);
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
