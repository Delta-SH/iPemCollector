using iPem.Core.Rs;
using iPem.Data.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace iPem.Data {
    public partial class RedefinePointRepository {

        #region Fields

        private readonly string _databaseConnectionString;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        public RedefinePointRepository() {
            this._databaseConnectionString = SqlHelper.ConnectionStringRsTransaction;
        }

        #endregion

        #region Methods

        public RedefinePoint GetEntity(string device,string point) {
            SqlParameter[] parms = { new SqlParameter("@DeviceId", SqlDbType.VarChar, 100),
                                     new SqlParameter("@PointId", SqlDbType.VarChar, 100) };

            parms[0].Value = SqlTypeConverter.DBNullStringChecker(device);
            parms[1].Value = SqlTypeConverter.DBNullStringChecker(point);

            RedefinePoint entity = null;
            using (var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Rs.Sql_RedefinePoint_Repository_GetEntity, parms)) {
                if(rdr.Read()) {
                    entity = new RedefinePoint();
                    entity.DeviceId = SqlTypeConverter.DBNullStringHandler(rdr["DeviceId"]);
                    entity.PointId = SqlTypeConverter.DBNullStringHandler(rdr["PointId"]);
                    entity.NMAlarmId = SqlTypeConverter.DBNullStringHandler(rdr["NMAlarmId"]);
                    entity.AlarmLevel = SqlTypeConverter.DBNullInt32Handler(rdr["AlarmLevel"]);
                    entity.AlarmLimit = SqlTypeConverter.DBNullDoubleHandler(rdr["AlarmLimit"]);
                    entity.AlarmReturnDiff = SqlTypeConverter.DBNullDoubleHandler(rdr["AlarmReturnDiff"]);
                    entity.AlarmDelay = SqlTypeConverter.DBNullInt32Handler(rdr["AlarmDelay"]);
                    entity.AlarmRecoveryDelay = SqlTypeConverter.DBNullInt32Handler(rdr["AlarmRecoveryDelay"]);
                    entity.TriggerTypeID = SqlTypeConverter.DBNullInt32Handler(rdr["TriggerTypeID"]);
                    entity.SavedPeriod = SqlTypeConverter.DBNullInt32Handler(rdr["SavedPeriod"]);
                    entity.AbsoluteThreshold = SqlTypeConverter.DBNullDoubleHandler(rdr["AbsoluteThreshold"]);
                    entity.PerThreshold = SqlTypeConverter.DBNullDoubleHandler(rdr["PerThreshold"]);
                    entity.StaticPeriod = SqlTypeConverter.DBNullInt32Handler(rdr["StaticPeriod"]);
                    entity.InferiorAlarmStr = SqlTypeConverter.DBNullStringHandler(rdr["InferiorAlarmStr"]);
                    entity.ConnAlarmStr = SqlTypeConverter.DBNullStringHandler(rdr["ConnAlarmStr"]);
                    entity.AlarmFilteringStr = SqlTypeConverter.DBNullStringHandler(rdr["AlarmFilteringStr"]);
                    entity.AlarmReversalStr = SqlTypeConverter.DBNullStringHandler(rdr["AlarmReversalStr"]);
                    entity.Extend = SqlTypeConverter.DBNullStringHandler(rdr["Extend"]);
                }
            }
            return entity;
        }

        public List<RedefinePoint> GetEntities() {
            var entities = new List<RedefinePoint>();
            using (var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Rs.Sql_RedefinePoint_Repository_GetEntities, null)) {
                while(rdr.Read()) {
                    var entity = new RedefinePoint();
                    entity.DeviceId = SqlTypeConverter.DBNullStringHandler(rdr["DeviceId"]);
                    entity.PointId = SqlTypeConverter.DBNullStringHandler(rdr["PointId"]);
                    entity.NMAlarmId = SqlTypeConverter.DBNullStringHandler(rdr["NMAlarmId"]);
                    entity.AlarmLevel = SqlTypeConverter.DBNullInt32Handler(rdr["AlarmLevel"]);
                    entity.AlarmLimit = SqlTypeConverter.DBNullDoubleHandler(rdr["AlarmLimit"]);
                    entity.AlarmReturnDiff = SqlTypeConverter.DBNullDoubleHandler(rdr["AlarmReturnDiff"]);
                    entity.AlarmDelay = SqlTypeConverter.DBNullInt32Handler(rdr["AlarmDelay"]);
                    entity.AlarmRecoveryDelay = SqlTypeConverter.DBNullInt32Handler(rdr["AlarmRecoveryDelay"]);
                    entity.TriggerTypeID = SqlTypeConverter.DBNullInt32Handler(rdr["TriggerTypeID"]);
                    entity.SavedPeriod = SqlTypeConverter.DBNullInt32Handler(rdr["SavedPeriod"]);
                    entity.AbsoluteThreshold = SqlTypeConverter.DBNullDoubleHandler(rdr["AbsoluteThreshold"]);
                    entity.PerThreshold = SqlTypeConverter.DBNullDoubleHandler(rdr["PerThreshold"]);
                    entity.StaticPeriod = SqlTypeConverter.DBNullInt32Handler(rdr["StaticPeriod"]);
                    entity.InferiorAlarmStr = SqlTypeConverter.DBNullStringHandler(rdr["InferiorAlarmStr"]);
                    entity.ConnAlarmStr = SqlTypeConverter.DBNullStringHandler(rdr["ConnAlarmStr"]);
                    entity.AlarmFilteringStr = SqlTypeConverter.DBNullStringHandler(rdr["AlarmFilteringStr"]);
                    entity.AlarmReversalStr = SqlTypeConverter.DBNullStringHandler(rdr["AlarmReversalStr"]);
                    entity.Extend = SqlTypeConverter.DBNullStringHandler(rdr["Extend"]);
                    entity.UpdateTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["UpdateTime"]);
                    entities.Add(entity);
                }
            }
            return entities;
        }

        #endregion

    }
}
