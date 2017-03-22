using iPem.Core;
using iPem.Data.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace iPem.Data {
    public partial class ActAlmRepository {

        #region Fields

        private readonly string _databaseConnectionString;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        public ActAlmRepository() {
            this._databaseConnectionString = SqlHelper.ConnectionStringCsTransaction;
        }

        #endregion

        #region Methods

        public List<ActAlm> GetEntities(DateTime start, DateTime end) {
            SqlParameter[] parms = { new SqlParameter("@Start", SqlDbType.DateTime),
                                     new SqlParameter("@End", SqlDbType.DateTime) };

            parms[0].Value = SqlTypeConverter.DBNullDateTimeHandler(start);
            parms[1].Value = SqlTypeConverter.DBNullDateTimeHandler(end);

            var entities = new List<ActAlm>();
            using(var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Cs.Sql_ActAlm_Repository_GetEntities1, parms)) {
                while(rdr.Read()) {
                    var entity = new ActAlm();
                    entity.Id = SqlTypeConverter.DBNullStringHandler(rdr["Id"]);
                    entity.SerialNo = SqlTypeConverter.DBNullStringHandler(rdr["SerialNo"]);
                    entity.AreaId = SqlTypeConverter.DBNullStringHandler(rdr["AreaId"]);
                    entity.StationId = SqlTypeConverter.DBNullStringHandler(rdr["StationId"]);
                    entity.RoomId = SqlTypeConverter.DBNullStringHandler(rdr["RoomId"]);
                    entity.FsuId = SqlTypeConverter.DBNullStringHandler(rdr["FsuId"]);
                    entity.DeviceId = SqlTypeConverter.DBNullStringHandler(rdr["DeviceId"]);
                    entity.PointId = SqlTypeConverter.DBNullStringHandler(rdr["PointId"]);
                    entity.SignalId = SqlTypeConverter.DBNullStringHandler(rdr["SignalId"]);
                    entity.SignalNumber = SqlTypeConverter.DBNullStringHandler(rdr["SignalNumber"]);
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

        public List<ActAlm> GetEntities() {
            var entities = new List<ActAlm>();
            using(var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Cs.Sql_ActAlm_Repository_GetEntities2, null)) {
                while(rdr.Read()) {
                    var entity = new ActAlm();
                    entity.Id = SqlTypeConverter.DBNullStringHandler(rdr["Id"]);
                    entity.SerialNo = SqlTypeConverter.DBNullStringHandler(rdr["SerialNo"]);
                    entity.AreaId = SqlTypeConverter.DBNullStringHandler(rdr["AreaId"]);
                    entity.StationId = SqlTypeConverter.DBNullStringHandler(rdr["StationId"]);
                    entity.RoomId = SqlTypeConverter.DBNullStringHandler(rdr["RoomId"]);
                    entity.FsuId = SqlTypeConverter.DBNullStringHandler(rdr["FsuId"]);
                    entity.DeviceId = SqlTypeConverter.DBNullStringHandler(rdr["DeviceId"]);
                    entity.PointId = SqlTypeConverter.DBNullStringHandler(rdr["PointId"]);
                    entity.SignalId = SqlTypeConverter.DBNullStringHandler(rdr["SignalId"]);
                    entity.SignalNumber = SqlTypeConverter.DBNullStringHandler(rdr["SignalNumber"]);
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

        #endregion

    }
}
