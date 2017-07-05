using iPem.Core;
using iPem.Data.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace iPem.Data {
    public partial class A_AAlarmRepository {

        #region Fields

        private readonly string _databaseConnectionString;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        public A_AAlarmRepository() {
            this._databaseConnectionString = SqlHelper.ConnectionStringCsTransaction;
        }

        #endregion

        #region Methods

        public A_AAlarm GetEntity(string id) {
            SqlParameter[] parms = { new SqlParameter("@Id", SqlDbType.VarChar,100) };
            parms[0].Value = SqlTypeConverter.DBNullStringChecker(id);

            A_AAlarm entity = null;
            using (var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Cs.Sql_A_AAlarm_Repository_GetEntity, parms)) {
                if (rdr.Read()) {
                    entity = new A_AAlarm();
                    entity.Id = SqlTypeConverter.DBNullStringHandler(rdr["Id"]);
                    entity.AreaId = SqlTypeConverter.DBNullStringHandler(rdr["AreaId"]);
                    entity.StationId = SqlTypeConverter.DBNullStringHandler(rdr["StationId"]);
                    entity.RoomId = SqlTypeConverter.DBNullStringHandler(rdr["RoomId"]);
                    entity.FsuId = SqlTypeConverter.DBNullStringHandler(rdr["FsuId"]);
                    entity.DeviceId = SqlTypeConverter.DBNullStringHandler(rdr["DeviceId"]);
                    entity.PointId = SqlTypeConverter.DBNullStringHandler(rdr["PointId"]);
                    entity.SerialNo = SqlTypeConverter.DBNullStringHandler(rdr["SerialNo"]);
                    entity.NMAlarmId = SqlTypeConverter.DBNullStringHandler(rdr["NMAlarmId"]);
                    entity.AlarmTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["AlarmTime"]);
                    entity.AlarmLevel = SqlTypeConverter.DBNullEnmLevelHandler(rdr["AlarmLevel"]);
                    entity.AlarmValue = SqlTypeConverter.DBNullDoubleHandler(rdr["AlarmValue"]);
                    entity.AlarmDesc = SqlTypeConverter.DBNullStringHandler(rdr["AlarmDesc"]);
                    entity.AlarmRemark = SqlTypeConverter.DBNullStringHandler(rdr["AlarmRemark"]);
                    entity.Confirmed = SqlTypeConverter.DBNullEnmConfirmHandler(rdr["Confirmed"]);
                    entity.Confirmer = SqlTypeConverter.DBNullStringHandler(rdr["Confirmer"]);
                    entity.ConfirmedTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["ConfirmedTime"]);
                    entity.ReservationId = SqlTypeConverter.DBNullStringHandler(rdr["ReservationId"]);
                    entity.PrimaryId = SqlTypeConverter.DBNullStringHandler(rdr["PrimaryId"]);
                    entity.RelatedId = SqlTypeConverter.DBNullStringHandler(rdr["RelatedId"]);
                    entity.FilterId = SqlTypeConverter.DBNullStringHandler(rdr["FilterId"]);
                    entity.ReversalId = SqlTypeConverter.DBNullStringHandler(rdr["ReversalId"]);
                    entity.ReversalCount = SqlTypeConverter.DBNullInt32Handler(rdr["ReversalCount"]);
                    entity.Masked = SqlTypeConverter.DBNullBooleanHandler(rdr["Masked"]);
                    entity.CreatedTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["CreatedTime"]);
                }
            }
            return entity;
        }

        public A_AAlarm GetEntityInPoint(string device, string point) {
            SqlParameter[] parms = { new SqlParameter("@DeviceId", SqlDbType.VarChar, 100),
                                     new SqlParameter("@PointId", SqlDbType.VarChar, 100) };
            parms[0].Value = SqlTypeConverter.DBNullStringChecker(device);
            parms[1].Value = SqlTypeConverter.DBNullStringChecker(point);

            A_AAlarm entity = null;
            using (var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Cs.Sql_A_AAlarm_Repository_GetEntityInPoint, parms)) {
                if (rdr.Read()) {
                    entity = new A_AAlarm();
                    entity.Id = SqlTypeConverter.DBNullStringHandler(rdr["Id"]);
                    entity.AreaId = SqlTypeConverter.DBNullStringHandler(rdr["AreaId"]);
                    entity.StationId = SqlTypeConverter.DBNullStringHandler(rdr["StationId"]);
                    entity.RoomId = SqlTypeConverter.DBNullStringHandler(rdr["RoomId"]);
                    entity.FsuId = SqlTypeConverter.DBNullStringHandler(rdr["FsuId"]);
                    entity.DeviceId = SqlTypeConverter.DBNullStringHandler(rdr["DeviceId"]);
                    entity.PointId = SqlTypeConverter.DBNullStringHandler(rdr["PointId"]);
                    entity.SerialNo = SqlTypeConverter.DBNullStringHandler(rdr["SerialNo"]);
                    entity.NMAlarmId = SqlTypeConverter.DBNullStringHandler(rdr["NMAlarmId"]);
                    entity.AlarmTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["AlarmTime"]);
                    entity.AlarmLevel = SqlTypeConverter.DBNullEnmLevelHandler(rdr["AlarmLevel"]);
                    entity.AlarmValue = SqlTypeConverter.DBNullDoubleHandler(rdr["AlarmValue"]);
                    entity.AlarmDesc = SqlTypeConverter.DBNullStringHandler(rdr["AlarmDesc"]);
                    entity.AlarmRemark = SqlTypeConverter.DBNullStringHandler(rdr["AlarmRemark"]);
                    entity.Confirmed = SqlTypeConverter.DBNullEnmConfirmHandler(rdr["Confirmed"]);
                    entity.Confirmer = SqlTypeConverter.DBNullStringHandler(rdr["Confirmer"]);
                    entity.ConfirmedTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["ConfirmedTime"]);
                    entity.ReservationId = SqlTypeConverter.DBNullStringHandler(rdr["ReservationId"]);
                    entity.PrimaryId = SqlTypeConverter.DBNullStringHandler(rdr["PrimaryId"]);
                    entity.RelatedId = SqlTypeConverter.DBNullStringHandler(rdr["RelatedId"]);
                    entity.FilterId = SqlTypeConverter.DBNullStringHandler(rdr["FilterId"]);
                    entity.ReversalId = SqlTypeConverter.DBNullStringHandler(rdr["ReversalId"]);
                    entity.ReversalCount = SqlTypeConverter.DBNullInt32Handler(rdr["ReversalCount"]);
                    entity.Masked = SqlTypeConverter.DBNullBooleanHandler(rdr["Masked"]);
                    entity.CreatedTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["CreatedTime"]);
                }
            }
            return entity;
        }

        public List<A_AAlarm> GetEntitiesInDevice(string id) {
            SqlParameter[] parms = { new SqlParameter("@DeviceId", SqlDbType.VarChar, 100) };
            parms[0].Value = SqlTypeConverter.DBNullStringChecker(id);

            var entities = new List<A_AAlarm>();
            using (var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Cs.Sql_A_AAlarm_Repository_GetEntitiesInDevice, parms)) {
                while (rdr.Read()) {
                    var entity = new A_AAlarm();
                    entity.Id = SqlTypeConverter.DBNullStringHandler(rdr["Id"]);
                    entity.AreaId = SqlTypeConverter.DBNullStringHandler(rdr["AreaId"]);
                    entity.StationId = SqlTypeConverter.DBNullStringHandler(rdr["StationId"]);
                    entity.RoomId = SqlTypeConverter.DBNullStringHandler(rdr["RoomId"]);
                    entity.FsuId = SqlTypeConverter.DBNullStringHandler(rdr["FsuId"]);
                    entity.DeviceId = SqlTypeConverter.DBNullStringHandler(rdr["DeviceId"]);
                    entity.PointId = SqlTypeConverter.DBNullStringHandler(rdr["PointId"]);
                    entity.SerialNo = SqlTypeConverter.DBNullStringHandler(rdr["SerialNo"]);
                    entity.NMAlarmId = SqlTypeConverter.DBNullStringHandler(rdr["NMAlarmId"]);
                    entity.AlarmTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["AlarmTime"]);
                    entity.AlarmLevel = SqlTypeConverter.DBNullEnmLevelHandler(rdr["AlarmLevel"]);
                    entity.AlarmValue = SqlTypeConverter.DBNullDoubleHandler(rdr["AlarmValue"]);
                    entity.AlarmDesc = SqlTypeConverter.DBNullStringHandler(rdr["AlarmDesc"]);
                    entity.AlarmRemark = SqlTypeConverter.DBNullStringHandler(rdr["AlarmRemark"]);
                    entity.Confirmed = SqlTypeConverter.DBNullEnmConfirmHandler(rdr["Confirmed"]);
                    entity.Confirmer = SqlTypeConverter.DBNullStringHandler(rdr["Confirmer"]);
                    entity.ConfirmedTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["ConfirmedTime"]);
                    entity.ReservationId = SqlTypeConverter.DBNullStringHandler(rdr["ReservationId"]);
                    entity.PrimaryId = SqlTypeConverter.DBNullStringHandler(rdr["PrimaryId"]);
                    entity.RelatedId = SqlTypeConverter.DBNullStringHandler(rdr["RelatedId"]);
                    entity.FilterId = SqlTypeConverter.DBNullStringHandler(rdr["FilterId"]);
                    entity.ReversalId = SqlTypeConverter.DBNullStringHandler(rdr["ReversalId"]);
                    entity.ReversalCount = SqlTypeConverter.DBNullInt32Handler(rdr["ReversalCount"]);
                    entity.Masked = SqlTypeConverter.DBNullBooleanHandler(rdr["Masked"]);
                    entity.CreatedTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["CreatedTime"]);
                    entities.Add(entity);
                }
            }
            return entities;
        }

        public List<A_AAlarm> GetEntities() {
            var entities = new List<A_AAlarm>();
            using (var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Cs.Sql_A_AAlarm_Repository_GetEntities, null)) {
                while (rdr.Read()) {
                    var entity = new A_AAlarm();
                    entity.Id = SqlTypeConverter.DBNullStringHandler(rdr["Id"]);
                    entity.AreaId = SqlTypeConverter.DBNullStringHandler(rdr["AreaId"]);
                    entity.StationId = SqlTypeConverter.DBNullStringHandler(rdr["StationId"]);
                    entity.RoomId = SqlTypeConverter.DBNullStringHandler(rdr["RoomId"]);
                    entity.FsuId = SqlTypeConverter.DBNullStringHandler(rdr["FsuId"]);
                    entity.DeviceId = SqlTypeConverter.DBNullStringHandler(rdr["DeviceId"]);
                    entity.PointId = SqlTypeConverter.DBNullStringHandler(rdr["PointId"]);
                    entity.SerialNo = SqlTypeConverter.DBNullStringHandler(rdr["SerialNo"]);
                    entity.NMAlarmId = SqlTypeConverter.DBNullStringHandler(rdr["NMAlarmId"]);
                    entity.AlarmTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["AlarmTime"]);
                    entity.AlarmLevel = SqlTypeConverter.DBNullEnmLevelHandler(rdr["AlarmLevel"]);
                    entity.AlarmValue = SqlTypeConverter.DBNullDoubleHandler(rdr["AlarmValue"]);
                    entity.AlarmDesc = SqlTypeConverter.DBNullStringHandler(rdr["AlarmDesc"]);
                    entity.AlarmRemark = SqlTypeConverter.DBNullStringHandler(rdr["AlarmRemark"]);
                    entity.Confirmed = SqlTypeConverter.DBNullEnmConfirmHandler(rdr["Confirmed"]);
                    entity.Confirmer = SqlTypeConverter.DBNullStringHandler(rdr["Confirmer"]);
                    entity.ConfirmedTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["ConfirmedTime"]);
                    entity.ReservationId = SqlTypeConverter.DBNullStringHandler(rdr["ReservationId"]);
                    entity.PrimaryId = SqlTypeConverter.DBNullStringHandler(rdr["PrimaryId"]);
                    entity.RelatedId = SqlTypeConverter.DBNullStringHandler(rdr["RelatedId"]);
                    entity.FilterId = SqlTypeConverter.DBNullStringHandler(rdr["FilterId"]);
                    entity.ReversalId = SqlTypeConverter.DBNullStringHandler(rdr["ReversalId"]);
                    entity.ReversalCount = SqlTypeConverter.DBNullInt32Handler(rdr["ReversalCount"]);
                    entity.Masked = SqlTypeConverter.DBNullBooleanHandler(rdr["Masked"]);
                    entity.CreatedTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["CreatedTime"]);
                    entities.Add(entity);
                }
            }
            return entities;
        }

        #endregion

    }
}
