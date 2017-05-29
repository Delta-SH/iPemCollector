using iPem.Core;
using iPem.Data.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace iPem.Data {
    public partial class V_StaticRepository {

        #region Fields

        private readonly string _databaseConnectionString;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        public V_StaticRepository() {
            this._databaseConnectionString = SqlHelper.ConnectionStringCsTransaction;
        }

        #endregion

        #region Methods

        public List<V_Static> GetEntities(DateTime start, DateTime end) {
            SqlParameter[] parms = { new SqlParameter("@Start", SqlDbType.DateTime),
                                     new SqlParameter("@End", SqlDbType.DateTime) };

            parms[0].Value = SqlTypeConverter.DBNullDateTimeHandler(start);
            parms[1].Value = SqlTypeConverter.DBNullDateTimeHandler(end);

            var entities = new List<V_Static>();
            using(var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Cs.Sql_V_Static_Repository_GetEntities, parms)) {
                while(rdr.Read()) {
                    var entity = new V_Static();
                    entity.AreaId = SqlTypeConverter.DBNullStringHandler(rdr["AreaId"]);
                    entity.StationId = SqlTypeConverter.DBNullStringHandler(rdr["StationId"]);
                    entity.RoomId = SqlTypeConverter.DBNullStringHandler(rdr["RoomId"]);
                    entity.DeviceId = SqlTypeConverter.DBNullStringHandler(rdr["DeviceId"]);
                    entity.PointId = SqlTypeConverter.DBNullStringHandler(rdr["PointId"]);
                    entity.BeginTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["BeginTime"]);
                    entity.EndTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["EndTime"]);
                    entity.MaxValue = SqlTypeConverter.DBNullDoubleHandler(rdr["MaxValue"]);
                    entity.MinValue = SqlTypeConverter.DBNullDoubleHandler(rdr["MinValue"]);
                    entity.AvgValue = SqlTypeConverter.DBNullDoubleHandler(rdr["AvgValue"]);
                    entity.MaxTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["MaxTime"]);
                    entity.MinTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["MinTime"]);
                    entity.Total = SqlTypeConverter.DBNullInt32Handler(rdr["Total"]);
                    entities.Add(entity);
                }
            }
            return entities;
        }

        #endregion

    }
}
