using iPem.Core;
using iPem.Data.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace iPem.Data {
    public partial class HisStaticRepository {

        #region Fields

        private readonly string _databaseConnectionString;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        public HisStaticRepository() {
            this._databaseConnectionString = SqlHelper.ConnectionStringCsTransaction;
        }

        #endregion

        #region Methods

        public List<HisStatic> GetEntities(DateTime start, DateTime end) {
            SqlParameter[] parms = { new SqlParameter("@Start", SqlDbType.DateTime),
                                     new SqlParameter("@End", SqlDbType.DateTime) };

            parms[0].Value = SqlTypeConverter.DBNullDateTimeHandler(start);
            parms[1].Value = SqlTypeConverter.DBNullDateTimeHandler(end);

            var entities = new List<HisStatic>();
            using(var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Cs.Sql_HisStatic_Repository_GetEntities, parms)) {
                while(rdr.Read()) {
                    var entity = new HisStatic();
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
