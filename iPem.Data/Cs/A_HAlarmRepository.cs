using iPem.Core;
using iPem.Data.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace iPem.Data {
    public partial class A_HAlarmRepository {

        #region Fields

        private readonly string _databaseConnectionString;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        public A_HAlarmRepository() {
            this._databaseConnectionString = SqlHelper.ConnectionStringCsTransaction;
        }

        #endregion

        #region Methods

        public List<A_HAlarm> GetEntities(DateTime start, DateTime end) {
            SqlParameter[] parms = { new SqlParameter("@Start", SqlDbType.DateTime),
                                     new SqlParameter("@End", SqlDbType.DateTime) };

            parms[0].Value = SqlTypeConverter.DBNullDateTimeHandler(start);
            parms[1].Value = SqlTypeConverter.DBNullDateTimeHandler(end);

            var entities = new List<A_HAlarm>();
            using (var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Cs.Sql_A_HAlarm_Repository_GetEntities, parms)) {
                while(rdr.Read()) {
                    var entity = new A_HAlarm();
                    entity.Id = SqlTypeConverter.DBNullStringHandler(rdr["Id"]);
                    entity.AreaId = SqlTypeConverter.DBNullStringHandler(rdr["AreaId"]);
                    entity.StationId = SqlTypeConverter.DBNullStringHandler(rdr["StationId"]);
                    entity.RoomId = SqlTypeConverter.DBNullStringHandler(rdr["RoomId"]);
                    entity.FsuId = SqlTypeConverter.DBNullStringHandler(rdr["FsuId"]);
                    entity.DeviceId = SqlTypeConverter.DBNullStringHandler(rdr["DeviceId"]);
                    entity.PointId = SqlTypeConverter.DBNullStringHandler(rdr["PointId"]);
                    entity.SerialNo = SqlTypeConverter.DBNullStringHandler(rdr["SerialNo"]);
                    entity.NMAlarmId = SqlTypeConverter.DBNullStringHandler(rdr["NMAlarmId"]);
                    entity.StartTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["StartTime"]);
                    entity.EndTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["EndTime"]);
                    entity.AlarmLevel = SqlTypeConverter.DBNullEnmLevelHandler(rdr["AlarmLevel"]);
                    entity.StartValue = SqlTypeConverter.DBNullDoubleHandler(rdr["StartValue"]);
                    entity.EndValue = SqlTypeConverter.DBNullDoubleHandler(rdr["EndValue"]);
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
                    entity.CreatedTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["CreatedTime"]);
                    entities.Add(entity);
                }
            }
            return entities;
        }

        #endregion

    }
}
