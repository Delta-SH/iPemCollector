using iPem.Core;
using iPem.Data.Common;
using iPem.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace iPem.Data {
    public partial class V_AMeasureRepository {

        #region Fields

        private readonly string _databaseConnectionString;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        public V_AMeasureRepository() {
            this._databaseConnectionString = SqlHelper.ConnectionStringCsTransaction;
        }

        #endregion

        #region Methods

        public V_AMeasure GetEntity(string device, string point) {
            SqlParameter[] parms = { new SqlParameter("@DeviceId", SqlDbType.VarChar, 100),
                                     new SqlParameter("@PointId", SqlDbType.VarChar, 100) };

            parms[0].Value = SqlTypeConverter.DBNullStringChecker(device);
            parms[1].Value = SqlTypeConverter.DBNullStringChecker(point);

            V_AMeasure entity = null;
            using (var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Cs.Sql_V_AMeasure_Repository_GetEntity, parms)) {
                if (rdr.Read()) {
                    entity = new V_AMeasure();
                    entity.AreaId = SqlTypeConverter.DBNullStringHandler(rdr["AreaId"]);
                    entity.StationId = SqlTypeConverter.DBNullStringHandler(rdr["StationId"]);
                    entity.RoomId = SqlTypeConverter.DBNullStringHandler(rdr["RoomId"]);
                    entity.FsuId = SqlTypeConverter.DBNullStringHandler(rdr["FsuId"]);
                    entity.DeviceId = SqlTypeConverter.DBNullStringHandler(rdr["DeviceId"]);
                    entity.PointId = SqlTypeConverter.DBNullStringHandler(rdr["PointId"]);
                    entity.SignalDesc = SqlTypeConverter.DBNullStringHandler(rdr["SignalDesc"]);
                    entity.Status = SqlTypeConverter.DBNullEnmStateHandler(rdr["Status"]);
                    entity.Value = SqlTypeConverter.DBNullDoubleHandler(rdr["Value"]);
                    entity.UpdateTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["UpdateTime"]);
                }
            }
            return entity;
        }

        public List<V_AMeasure> GetEntities(List<VariableDetail> keys) {
            if (keys == null || keys.Count == 0) 
                throw new ArgumentNullException("keys");

            var commands = new string[keys.Count];
            for (var i = 0; i < keys.Count; i++) {
                commands[i] = string.Format(@"SELECT '{0}' AS [DeviceId], '{1}' AS [PointId]", keys[i].DeviceId, keys[i].PointId);
            }

            var query = string.Format(@"
            ;WITH Keys AS (
                {0}
            )
            SELECT VA.* FROM [dbo].[V_AMeasure] VA INNER JOIN Keys K ON VA.[DeviceId]=K.[DeviceId] AND VA.[PointId]=K.[PointId];", string.Join(@" UNION ALL ", commands));

            var entities = new List<V_AMeasure>();
            using (var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, query, null)) {
                while (rdr.Read()) {
                    var entity = new V_AMeasure();
                    entity.AreaId = SqlTypeConverter.DBNullStringHandler(rdr["AreaId"]);
                    entity.StationId = SqlTypeConverter.DBNullStringHandler(rdr["StationId"]);
                    entity.RoomId = SqlTypeConverter.DBNullStringHandler(rdr["RoomId"]);
                    entity.FsuId = SqlTypeConverter.DBNullStringHandler(rdr["FsuId"]);
                    entity.DeviceId = SqlTypeConverter.DBNullStringHandler(rdr["DeviceId"]);
                    entity.PointId = SqlTypeConverter.DBNullStringHandler(rdr["PointId"]);
                    entity.SignalDesc = SqlTypeConverter.DBNullStringHandler(rdr["SignalDesc"]);
                    entity.Status = SqlTypeConverter.DBNullEnmStateHandler(rdr["Status"]);
                    entity.Value = SqlTypeConverter.DBNullDoubleHandler(rdr["Value"]);
                    entity.UpdateTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["UpdateTime"]);
                    entities.Add(entity);
                }
            }
            return entities;
        }

        public List<V_AMeasure> GetEntities(string device) {
            SqlParameter[] parms = { new SqlParameter("@DeviceId", SqlDbType.VarChar, 100) };
            parms[0].Value = SqlTypeConverter.DBNullStringChecker(device);

            var entities = new List<V_AMeasure>();
            using (var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Cs.Sql_V_AMeasure_Repository_GetEntitiesInDevice, parms)) {
                while (rdr.Read()) {
                    var entity = new V_AMeasure();
                    entity.AreaId = SqlTypeConverter.DBNullStringHandler(rdr["AreaId"]);
                    entity.StationId = SqlTypeConverter.DBNullStringHandler(rdr["StationId"]);
                    entity.RoomId = SqlTypeConverter.DBNullStringHandler(rdr["RoomId"]);
                    entity.FsuId = SqlTypeConverter.DBNullStringHandler(rdr["FsuId"]);
                    entity.DeviceId = SqlTypeConverter.DBNullStringHandler(rdr["DeviceId"]);
                    entity.PointId = SqlTypeConverter.DBNullStringHandler(rdr["PointId"]);
                    entity.SignalDesc = SqlTypeConverter.DBNullStringHandler(rdr["SignalDesc"]);
                    entity.Status = SqlTypeConverter.DBNullEnmStateHandler(rdr["Status"]);
                    entity.Value = SqlTypeConverter.DBNullDoubleHandler(rdr["Value"]);
                    entity.UpdateTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["UpdateTime"]);
                    entities.Add(entity);
                }
            }
            return entities;
        }

        public List<V_AMeasure> GetEntities() {
            var entities = new List<V_AMeasure>();
            using (var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Cs.Sql_V_AMeasure_Repository_GetEntities, null)) {
                while(rdr.Read()) {
                    var entity = new V_AMeasure();
                    entity.AreaId = SqlTypeConverter.DBNullStringHandler(rdr["AreaId"]);
                    entity.StationId = SqlTypeConverter.DBNullStringHandler(rdr["StationId"]);
                    entity.RoomId = SqlTypeConverter.DBNullStringHandler(rdr["RoomId"]);
                    entity.FsuId = SqlTypeConverter.DBNullStringHandler(rdr["FsuId"]);
                    entity.DeviceId = SqlTypeConverter.DBNullStringHandler(rdr["DeviceId"]);
                    entity.PointId = SqlTypeConverter.DBNullStringHandler(rdr["PointId"]);
                    entity.SignalDesc = SqlTypeConverter.DBNullStringHandler(rdr["SignalDesc"]);
                    entity.Status = SqlTypeConverter.DBNullEnmStateHandler(rdr["Status"]);
                    entity.Value = SqlTypeConverter.DBNullDoubleHandler(rdr["Value"]);
                    entity.UpdateTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["UpdateTime"]);
                    entities.Add(entity);
                }
            }
            return entities;
        }

        public void Save(List<V_AMeasure> entities) {
            SqlParameter[] parms = { new SqlParameter("@AreaId", SqlDbType.VarChar,100),
                                     new SqlParameter("@StationId", SqlDbType.VarChar,100),
                                     new SqlParameter("@RoomId", SqlDbType.VarChar,100),
                                     new SqlParameter("@FsuId", SqlDbType.VarChar,100),
                                     new SqlParameter("@DeviceId", SqlDbType.VarChar,100),
                                     new SqlParameter("@PointId", SqlDbType.VarChar,100),
                                     new SqlParameter("@SignalDesc", SqlDbType.VarChar,120),
                                     new SqlParameter("@Status", SqlDbType.Int),
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
                        parms[7].Value = (int)entity.Status;
                        parms[8].Value = SqlTypeConverter.DBNullDoubleChecker(entity.Value);
                        parms[9].Value = SqlTypeConverter.DBNullDateTimeChecker(entity.UpdateTime);
                        SqlHelper.ExecuteNonQuery(trans, CommandType.Text, SqlCommands_Cs.Sql_V_AMeasure_Repository_Save, parms);
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
