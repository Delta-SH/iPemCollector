using iPem.Core;
using iPem.Core.Rs;
using iPem.Data.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace iPem.Data {
    public partial class SignalRepository {

        #region Fields

        private readonly string _databaseConnectionString;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        public SignalRepository() {
            this._databaseConnectionString = SqlHelper.ConnectionStringRsTransaction;
        }

        #endregion

        #region Methods

        public List<Signal> GetEntities(string device) {
            SqlParameter[] parms = { new SqlParameter("@DeviceId", SqlDbType.VarChar, 100) };
            parms[0].Value = SqlTypeConverter.DBNullStringChecker(device);

            var entities = new List<Signal>();
            using (var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Rs.Sql_Signal_Repository_GetEntitiesInDevice, parms)) {
                while (rdr.Read()) {
                    var entity = new Signal();
                    entity.DeviceId = SqlTypeConverter.DBNullStringHandler(rdr["DeviceId"]);
                    entity.PointId = SqlTypeConverter.DBNullStringHandler(rdr["PointId"]);
                    entity.Name = SqlTypeConverter.DBNullStringHandler(rdr["Name"]);
                    entity.Type = SqlTypeConverter.DBNullEnmPointHandler(rdr["Type"]);
                    entity.UnitState = SqlTypeConverter.DBNullStringHandler(rdr["UnitState"]);
                    entity.AlarmId = SqlTypeConverter.DBNullStringHandler(rdr["AlarmId"]);
                    entity.NMAlarmId = SqlTypeConverter.DBNullStringHandler(rdr["NMAlarmId"]);
                    entity.AlarmLevel = SqlTypeConverter.DBNullInt32Handler(rdr["AlarmLevel"]);
                    entity.DeviceEffect = SqlTypeConverter.DBNullStringHandler(rdr["DeviceEffect"]);
                    entity.BusiEffect = SqlTypeConverter.DBNullStringHandler(rdr["BusiEffect"]);
                    entity.AlarmLimit = SqlTypeConverter.DBNullDoubleHandler(rdr["AlarmLimit"]);
                    entity.AlarmDelay = SqlTypeConverter.DBNullInt32Handler(rdr["AlarmDelay"]);
                    entity.AlarmRecoveryDelay = SqlTypeConverter.DBNullInt32Handler(rdr["AlarmRecoveryDelay"]);
                    entity.SavedPeriod = SqlTypeConverter.DBNullInt32Handler(rdr["SavedPeriod"]);
                    entity.AbsoluteThreshold = SqlTypeConverter.DBNullDoubleHandler(rdr["AbsoluteThreshold"]);
                    entity.PerThreshold = SqlTypeConverter.DBNullDoubleHandler(rdr["PerThreshold"]);
                    entity.StaticPeriod = SqlTypeConverter.DBNullInt32Handler(rdr["StaticPeriod"]);
                    entity.StorageRefTime = SqlTypeConverter.DBNullStringHandler(rdr["StorageRefTime"]);
                    entity.InferiorAlarmStr = SqlTypeConverter.DBNullStringHandler(rdr["InferiorAlarmStr"]);
                    entity.ConnAlarmStr = SqlTypeConverter.DBNullStringHandler(rdr["ConnAlarmStr"]);
                    entity.AlarmFilteringStr = SqlTypeConverter.DBNullStringHandler(rdr["AlarmFilteringStr"]);
                    entity.AlarmReversalStr = SqlTypeConverter.DBNullStringHandler(rdr["AlarmReversalStr"]);
                    entity.Extend = SqlTypeConverter.DBNullStringHandler(rdr["Extend"]);

                    //判断是否为告警信号
                    if (entity.Type == EnmPoint.DI && !string.IsNullOrWhiteSpace(entity.AlarmId)) {
                        entity.Type = EnmPoint.AL;
                    }

                    entities.Add(entity);
                }
            }
            return entities;
        }

        public List<Signal> GetEntities() {
            var entities = new List<Signal>();
            using (var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Rs.Sql_Signal_Repository_GetEntities, null)) {
                while(rdr.Read()) {
                    var entity = new Signal();
                    entity.DeviceId = SqlTypeConverter.DBNullStringHandler(rdr["DeviceId"]);
                    entity.PointId = SqlTypeConverter.DBNullStringHandler(rdr["PointId"]);
                    entity.Name = SqlTypeConverter.DBNullStringHandler(rdr["Name"]);
                    entity.Type = SqlTypeConverter.DBNullEnmPointHandler(rdr["Type"]);
                    entity.UnitState = SqlTypeConverter.DBNullStringHandler(rdr["UnitState"]);
                    entity.AlarmId = SqlTypeConverter.DBNullStringHandler(rdr["AlarmId"]);
                    entity.NMAlarmId = SqlTypeConverter.DBNullStringHandler(rdr["NMAlarmId"]);
                    entity.AlarmLevel = SqlTypeConverter.DBNullInt32Handler(rdr["AlarmLevel"]);
                    entity.DeviceEffect = SqlTypeConverter.DBNullStringHandler(rdr["DeviceEffect"]);
                    entity.BusiEffect = SqlTypeConverter.DBNullStringHandler(rdr["BusiEffect"]);
                    entity.AlarmLimit = SqlTypeConverter.DBNullDoubleHandler(rdr["AlarmLimit"]);
                    entity.AlarmDelay = SqlTypeConverter.DBNullInt32Handler(rdr["AlarmDelay"]);
                    entity.AlarmRecoveryDelay = SqlTypeConverter.DBNullInt32Handler(rdr["AlarmRecoveryDelay"]);
                    entity.SavedPeriod = SqlTypeConverter.DBNullInt32Handler(rdr["SavedPeriod"]);
                    entity.AbsoluteThreshold = SqlTypeConverter.DBNullDoubleHandler(rdr["AbsoluteThreshold"]);
                    entity.PerThreshold = SqlTypeConverter.DBNullDoubleHandler(rdr["PerThreshold"]);
                    entity.StaticPeriod = SqlTypeConverter.DBNullInt32Handler(rdr["StaticPeriod"]);
                    entity.StorageRefTime = SqlTypeConverter.DBNullStringHandler(rdr["StorageRefTime"]);
                    entity.InferiorAlarmStr = SqlTypeConverter.DBNullStringHandler(rdr["InferiorAlarmStr"]);
                    entity.ConnAlarmStr = SqlTypeConverter.DBNullStringHandler(rdr["ConnAlarmStr"]);
                    entity.AlarmFilteringStr = SqlTypeConverter.DBNullStringHandler(rdr["AlarmFilteringStr"]);
                    entity.AlarmReversalStr = SqlTypeConverter.DBNullStringHandler(rdr["AlarmReversalStr"]);
                    entity.Extend = SqlTypeConverter.DBNullStringHandler(rdr["Extend"]);

                    //判断是否为告警信号
                    if (entity.Type == EnmPoint.DI && !string.IsNullOrWhiteSpace(entity.AlarmId)) {
                        entity.Type = EnmPoint.AL;
                    }

                    entities.Add(entity);
                }
            }
            return entities;
        }

        public List<VSignal> GetVEntities(string device) {
            SqlParameter[] parms = { new SqlParameter("@DeviceId", SqlDbType.VarChar, 100) };
            parms[0].Value = SqlTypeConverter.DBNullStringChecker(device);

            var entities = new List<VSignal>();
            using (var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Rs.Sql_Signal_Repository_GetVEntitiesInDevice, parms)) {
                while (rdr.Read()) {
                    var entity = new VSignal();
                    entity.AreaId = SqlTypeConverter.DBNullStringHandler(rdr["AreaId"]);
                    entity.StationId = SqlTypeConverter.DBNullStringHandler(rdr["StationId"]);
                    entity.RoomId = SqlTypeConverter.DBNullStringHandler(rdr["RoomId"]);
                    entity.FsuId = SqlTypeConverter.DBNullStringHandler(rdr["FsuId"]);
                    entity.DeviceId = SqlTypeConverter.DBNullStringHandler(rdr["DeviceId"]);
                    entity.PointId = SqlTypeConverter.DBNullStringHandler(rdr["PointId"]);
                    entity.Name = SqlTypeConverter.DBNullStringHandler(rdr["Name"]);
                    entity.Type = SqlTypeConverter.DBNullEnmPointHandler(rdr["Type"]);
                    entity.FormulaText = SqlTypeConverter.DBNullStringHandler(rdr["FormulaText"]);
                    entity.FormulaValue = SqlTypeConverter.DBNullStringHandler(rdr["FormulaValue"]);
                    entity.UnitState = SqlTypeConverter.DBNullStringHandler(rdr["UnitState"]);
                    entity.SavedPeriod = SqlTypeConverter.DBNullInt32Handler(rdr["SavedPeriod"]);
                    entity.StaticPeriod = SqlTypeConverter.DBNullInt32Handler(rdr["StaticPeriod"]);
                    entity.Category = SqlTypeConverter.DBNullVSignalCategoryHandler(rdr["Category"]);
                    entity.Remark = SqlTypeConverter.DBNullStringHandler(rdr["Remark"]);
                    entities.Add(entity);
                }
            }
            return entities;
        }

        public List<VSignal> GetVEntities() {
            var entities = new List<VSignal>();
            using (var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Rs.Sql_Signal_Repository_GetVEntities, null)) {
                while (rdr.Read()) {
                    var entity = new VSignal();
                    entity.AreaId = SqlTypeConverter.DBNullStringHandler(rdr["AreaId"]);
                    entity.StationId = SqlTypeConverter.DBNullStringHandler(rdr["StationId"]);
                    entity.RoomId = SqlTypeConverter.DBNullStringHandler(rdr["RoomId"]);
                    entity.FsuId = SqlTypeConverter.DBNullStringHandler(rdr["FsuId"]);
                    entity.DeviceId = SqlTypeConverter.DBNullStringHandler(rdr["DeviceId"]);
                    entity.PointId = SqlTypeConverter.DBNullStringHandler(rdr["PointId"]);
                    entity.Name = SqlTypeConverter.DBNullStringHandler(rdr["Name"]);
                    entity.Type = SqlTypeConverter.DBNullEnmPointHandler(rdr["Type"]);
                    entity.FormulaText = SqlTypeConverter.DBNullStringHandler(rdr["FormulaText"]);
                    entity.FormulaValue = SqlTypeConverter.DBNullStringHandler(rdr["FormulaValue"]);
                    entity.UnitState = SqlTypeConverter.DBNullStringHandler(rdr["UnitState"]);
                    entity.SavedPeriod = SqlTypeConverter.DBNullInt32Handler(rdr["SavedPeriod"]);
                    entity.StaticPeriod = SqlTypeConverter.DBNullInt32Handler(rdr["StaticPeriod"]);
                    entity.Category = SqlTypeConverter.DBNullVSignalCategoryHandler(rdr["Category"]);
                    entity.Remark = SqlTypeConverter.DBNullStringHandler(rdr["Remark"]);
                    entities.Add(entity);
                }
            }
            return entities;
        }

        #endregion

    }
}
