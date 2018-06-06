using iPem.Core;
using iPem.Data.Common;
using iPem.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace iPem.Data {
    public partial class V_HMeasureRepository {

        #region Fields

        private readonly string _databaseConnectionString;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        public V_HMeasureRepository() {
            this._databaseConnectionString = SqlHelper.ConnectionStringCsTransaction;
        }

        #endregion

        #region Methods

        public List<V_HMeasure> GetEntities(DateTime start, DateTime end) {
            SqlParameter[] parms = { new SqlParameter("@Start", SqlDbType.DateTime),
                                     new SqlParameter("@End", SqlDbType.DateTime) };

            parms[0].Value = SqlTypeConverter.DBNullDateTimeHandler(start);
            parms[1].Value = SqlTypeConverter.DBNullDateTimeHandler(end);

            var entities = new List<V_HMeasure>();
            using(var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Cs.Sql_V_HMeasure_Repository_GetEntities, parms)) {
                while(rdr.Read()) {
                    var entity = new V_HMeasure();
                    entity.AreaId = SqlTypeConverter.DBNullStringHandler(rdr["AreaId"]);
                    entity.StationId = SqlTypeConverter.DBNullStringHandler(rdr["StationId"]);
                    entity.RoomId = SqlTypeConverter.DBNullStringHandler(rdr["RoomId"]);
                    entity.FsuId = SqlTypeConverter.DBNullStringHandler(rdr["FsuId"]);
                    entity.DeviceId = SqlTypeConverter.DBNullStringHandler(rdr["DeviceId"]);
                    entity.PointId = SqlTypeConverter.DBNullStringHandler(rdr["PointId"]);
                    entity.SignalDesc = SqlTypeConverter.DBNullStringHandler(rdr["SignalDesc"]);
                    entity.Type = SqlTypeConverter.DBNullInt32Handler(rdr["Type"]);
                    entity.Value = SqlTypeConverter.DBNullDoubleHandler(rdr["Value"]);
                    entity.UpdateTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["UpdateTime"]);
                    entities.Add(entity);
                }
            }
            return entities;
        }

        public List<V_HMeasure> GetEntities(string device, DateTime start, DateTime end) {
            SqlParameter[] parms = { new SqlParameter("@DeviceId", SqlDbType.VarChar, 100),
                                     new SqlParameter("@Start", SqlDbType.DateTime),
                                     new SqlParameter("@End", SqlDbType.DateTime) };

            parms[0].Value = SqlTypeConverter.DBNullStringChecker(device);
            parms[1].Value = SqlTypeConverter.DBNullDateTimeHandler(start);
            parms[2].Value = SqlTypeConverter.DBNullDateTimeHandler(end);

            var entities = new List<V_HMeasure>();
            using (var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Cs.Sql_V_HMeasure_Repository_GetEntitiesByDevice, parms)) {
                while (rdr.Read()) {
                    var entity = new V_HMeasure();
                    entity.AreaId = SqlTypeConverter.DBNullStringHandler(rdr["AreaId"]);
                    entity.StationId = SqlTypeConverter.DBNullStringHandler(rdr["StationId"]);
                    entity.RoomId = SqlTypeConverter.DBNullStringHandler(rdr["RoomId"]);
                    entity.FsuId = SqlTypeConverter.DBNullStringHandler(rdr["FsuId"]);
                    entity.DeviceId = SqlTypeConverter.DBNullStringHandler(rdr["DeviceId"]);
                    entity.PointId = SqlTypeConverter.DBNullStringHandler(rdr["PointId"]);
                    entity.SignalDesc = SqlTypeConverter.DBNullStringHandler(rdr["SignalDesc"]);
                    entity.Type = SqlTypeConverter.DBNullInt32Handler(rdr["Type"]);
                    entity.Value = SqlTypeConverter.DBNullDoubleHandler(rdr["Value"]);
                    entity.UpdateTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["UpdateTime"]);
                    entities.Add(entity);
                }
            }
            return entities;
        }

        public List<V_HMeasure> GetEntities(string device, string point, DateTime start, DateTime end) {
            SqlParameter[] parms = { new SqlParameter("@DeviceId", SqlDbType.VarChar, 100),
                                     new SqlParameter("@PointId", SqlDbType.VarChar, 100),
                                     new SqlParameter("@Start", SqlDbType.DateTime),
                                     new SqlParameter("@End", SqlDbType.DateTime) };

            parms[0].Value = SqlTypeConverter.DBNullStringChecker(device);
            parms[1].Value = SqlTypeConverter.DBNullStringChecker(point);
            parms[2].Value = SqlTypeConverter.DBNullDateTimeHandler(start);
            parms[3].Value = SqlTypeConverter.DBNullDateTimeHandler(end);

            var entities = new List<V_HMeasure>();
            using(var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Cs.Sql_V_HMeasure_Repository_GetEntitiesByPoint, parms)) {
                while(rdr.Read()) {
                    var entity = new V_HMeasure();
                    entity.AreaId = SqlTypeConverter.DBNullStringHandler(rdr["AreaId"]);
                    entity.StationId = SqlTypeConverter.DBNullStringHandler(rdr["StationId"]);
                    entity.RoomId = SqlTypeConverter.DBNullStringHandler(rdr["RoomId"]);
                    entity.FsuId = SqlTypeConverter.DBNullStringHandler(rdr["FsuId"]);
                    entity.DeviceId = SqlTypeConverter.DBNullStringHandler(rdr["DeviceId"]);
                    entity.PointId = SqlTypeConverter.DBNullStringHandler(rdr["PointId"]);
                    entity.SignalDesc = SqlTypeConverter.DBNullStringHandler(rdr["SignalDesc"]);
                    entity.Type = SqlTypeConverter.DBNullInt32Handler(rdr["Type"]);
                    entity.Value = SqlTypeConverter.DBNullDoubleHandler(rdr["Value"]);
                    entity.UpdateTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["UpdateTime"]);
                    entities.Add(entity);
                }
            }
            return entities;
        }

        public V_HMeasure GetFirst(string device, string point, DateTime start, DateTime end) {
            SqlParameter[] parms = { new SqlParameter("@DeviceId", SqlDbType.VarChar, 100),
                                     new SqlParameter("@PointId", SqlDbType.VarChar, 100),
                                     new SqlParameter("@Start", SqlDbType.DateTime),
                                     new SqlParameter("@End", SqlDbType.DateTime) };

            parms[0].Value = SqlTypeConverter.DBNullStringChecker(device);
            parms[1].Value = SqlTypeConverter.DBNullStringChecker(point);
            parms[2].Value = SqlTypeConverter.DBNullDateTimeHandler(start);
            parms[3].Value = SqlTypeConverter.DBNullDateTimeHandler(end);

            V_HMeasure entity = null;
            using (var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Cs.Sql_V_HMeasure_Repository_GetFirst, parms)) {
                if (rdr.Read()) {
                    entity = new V_HMeasure();
                    entity.AreaId = SqlTypeConverter.DBNullStringHandler(rdr["AreaId"]);
                    entity.StationId = SqlTypeConverter.DBNullStringHandler(rdr["StationId"]);
                    entity.RoomId = SqlTypeConverter.DBNullStringHandler(rdr["RoomId"]);
                    entity.FsuId = SqlTypeConverter.DBNullStringHandler(rdr["FsuId"]);
                    entity.DeviceId = SqlTypeConverter.DBNullStringHandler(rdr["DeviceId"]);
                    entity.PointId = SqlTypeConverter.DBNullStringHandler(rdr["PointId"]);
                    entity.SignalDesc = SqlTypeConverter.DBNullStringHandler(rdr["SignalDesc"]);
                    entity.Type = SqlTypeConverter.DBNullInt32Handler(rdr["Type"]);
                    entity.Value = SqlTypeConverter.DBNullDoubleHandler(rdr["Value"]);
                    entity.UpdateTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["UpdateTime"]);
                }
            }
            return entity;
        }

        public V_HMeasure GetLast(string device, string point, DateTime start, DateTime end) {
            SqlParameter[] parms = { new SqlParameter("@DeviceId", SqlDbType.VarChar, 100),
                                     new SqlParameter("@PointId", SqlDbType.VarChar, 100),
                                     new SqlParameter("@Start", SqlDbType.DateTime),
                                     new SqlParameter("@End", SqlDbType.DateTime) };

            parms[0].Value = SqlTypeConverter.DBNullStringChecker(device);
            parms[1].Value = SqlTypeConverter.DBNullStringChecker(point);
            parms[2].Value = SqlTypeConverter.DBNullDateTimeHandler(start);
            parms[3].Value = SqlTypeConverter.DBNullDateTimeHandler(end);

            V_HMeasure entity = null;
            using (var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Cs.Sql_V_HMeasure_Repository_GetLast, parms)) {
                if (rdr.Read()) {
                    entity = new V_HMeasure();
                    entity.AreaId = SqlTypeConverter.DBNullStringHandler(rdr["AreaId"]);
                    entity.StationId = SqlTypeConverter.DBNullStringHandler(rdr["StationId"]);
                    entity.RoomId = SqlTypeConverter.DBNullStringHandler(rdr["RoomId"]);
                    entity.FsuId = SqlTypeConverter.DBNullStringHandler(rdr["FsuId"]);
                    entity.DeviceId = SqlTypeConverter.DBNullStringHandler(rdr["DeviceId"]);
                    entity.PointId = SqlTypeConverter.DBNullStringHandler(rdr["PointId"]);
                    entity.SignalDesc = SqlTypeConverter.DBNullStringHandler(rdr["SignalDesc"]);
                    entity.Type = SqlTypeConverter.DBNullInt32Handler(rdr["Type"]);
                    entity.Value = SqlTypeConverter.DBNullDoubleHandler(rdr["Value"]);
                    entity.UpdateTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["UpdateTime"]);
                }
            }
            return entity;
        }

        public double GetAvg(string device, string point, DateTime start, DateTime end) {
            SqlParameter[] parms = { new SqlParameter("@DeviceId", SqlDbType.VarChar, 100),
                                     new SqlParameter("@PointId", SqlDbType.VarChar, 100),
                                     new SqlParameter("@Start", SqlDbType.DateTime),
                                     new SqlParameter("@End", SqlDbType.DateTime) };

            parms[0].Value = SqlTypeConverter.DBNullStringChecker(device);
            parms[1].Value = SqlTypeConverter.DBNullStringChecker(point);
            parms[2].Value = SqlTypeConverter.DBNullDateTimeHandler(start);
            parms[3].Value = SqlTypeConverter.DBNullDateTimeHandler(end);

            var value = 0d;
            using (var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Cs.Sql_V_HMeasure_Repository_GetValAvg, parms)) {
                if (rdr.Read()) {
                    value = SqlTypeConverter.DBNullDoubleHandler(rdr["AvgValue"]);
                }
            }
            return value;
        }

        public void Save(List<V_HMeasure> entities) {
            SqlParameter[] parms = { new SqlParameter("@AreaId", SqlDbType.VarChar,100),
                                     new SqlParameter("@StationId", SqlDbType.VarChar,100),
                                     new SqlParameter("@RoomId", SqlDbType.VarChar,100),
                                     new SqlParameter("@FsuId", SqlDbType.VarChar,100),
                                     new SqlParameter("@DeviceId", SqlDbType.VarChar,100),
                                     new SqlParameter("@PointId", SqlDbType.VarChar,100),
                                     new SqlParameter("@SignalDesc", SqlDbType.VarChar,120),
                                     new SqlParameter("@Type", SqlDbType.Int),
                                     new SqlParameter("@Value", SqlDbType.Float),
                                     new SqlParameter("@UpdateTime", SqlDbType.DateTime)};

            using (var conn = new SqlConnection(this._databaseConnectionString)) {
                conn.Open();
                var trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                try {
                    foreach (var entity in entities) {
                        parms[0].Value = SqlTypeConverter.DBNullStringChecker(entity.AreaId);
                        parms[1].Value = SqlTypeConverter.DBNullStringChecker(entity.StationId);
                        parms[2].Value = SqlTypeConverter.DBNullStringChecker(entity.RoomId);
                        parms[3].Value = SqlTypeConverter.DBNullStringChecker(entity.FsuId);
                        parms[4].Value = SqlTypeConverter.DBNullStringChecker(entity.DeviceId);
                        parms[5].Value = SqlTypeConverter.DBNullStringChecker(entity.PointId);
                        parms[6].Value = SqlTypeConverter.DBNullStringChecker(entity.SignalDesc);
                        parms[7].Value = entity.Type;
                        parms[8].Value = SqlTypeConverter.DBNullDoubleChecker(entity.Value);
                        parms[9].Value = SqlTypeConverter.DBNullDateTimeChecker(entity.UpdateTime);
                        SqlHelper.ExecuteNonQuery(trans, CommandType.Text, string.Format(SqlCommands_Cs.Sql_V_HMeasure_Repository_Save, entity.UpdateTime.ToString("yyyyMM")), parms);
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
