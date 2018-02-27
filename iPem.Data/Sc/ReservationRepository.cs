using iPem.Core;
using iPem.Data.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace iPem.Data {
    public partial class ReservationRepository {

        #region Fields

        private readonly string _databaseConnectionString;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        public ReservationRepository() {
            this._databaseConnectionString = SqlHelper.ConnectionStringScTransaction;
        }

        #endregion

        #region Methods

        public List<Reservation> GetEntities() {
            var entities = new List<Reservation>();
            using(var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Sc.Sql_Reservation_Repository_GetEntities1, null)) {
                while(rdr.Read()) {
                    var entity = new Reservation();
                    entity.Id = SqlTypeConverter.DBNullStringHandler(rdr["Id"]);
                    entity.Name = SqlTypeConverter.DBNullStringHandler(rdr["Name"]);
                    entity.StartTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["StartTime"]);
                    entity.EndTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["EndTime"]);
                    entity.ProjectId = SqlTypeConverter.DBNullStringHandler(rdr["ProjectId"]);
                    entity.Creator = SqlTypeConverter.DBNullStringHandler(rdr["Creator"]);
                    entity.CreatedTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["CreatedTime"]);
                    entity.Comment = SqlTypeConverter.DBNullStringHandler(rdr["Comment"]);
                    entity.Enabled = SqlTypeConverter.DBNullBooleanHandler(rdr["Enabled"]);
                    entities.Add(entity);
                }
            }
            return entities;
        }

        public List<Reservation> GetEntities(DateTime time) {
            SqlParameter[] parms = { new SqlParameter("@StartTime", SqlDbType.DateTime) };
            parms[0].Value = SqlTypeConverter.DBNullDateTimeChecker(time);

            var entities = new List<Reservation>();
            using (var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Sc.Sql_Reservation_Repository_GetEntities2, parms)) {
                while (rdr.Read()) {
                    var entity = new Reservation();
                    entity.Id = SqlTypeConverter.DBNullStringHandler(rdr["Id"]);
                    entity.Name = SqlTypeConverter.DBNullStringHandler(rdr["Name"]);
                    entity.StartTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["StartTime"]);
                    entity.EndTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["EndTime"]);
                    entity.ProjectId = SqlTypeConverter.DBNullStringHandler(rdr["ProjectId"]);
                    entity.Creator = SqlTypeConverter.DBNullStringHandler(rdr["Creator"]);
                    entity.CreatedTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["CreatedTime"]);
                    entity.Comment = SqlTypeConverter.DBNullStringHandler(rdr["Comment"]);
                    entity.Enabled = SqlTypeConverter.DBNullBooleanHandler(rdr["Enabled"]);
                    entities.Add(entity);
                }
            }
            return entities;
        }

        public List<Reservation> GetEntities(DateTime startTime, DateTime endTime) {
            SqlParameter[] parms = { new SqlParameter("@StartTime", SqlDbType.DateTime), 
                                     new SqlParameter("@EndTime", SqlDbType.DateTime) };

            parms[0].Value = SqlTypeConverter.DBNullDateTimeChecker(startTime);
            parms[1].Value = SqlTypeConverter.DBNullDateTimeChecker(endTime);

            var entities = new List<Reservation>();
            using(var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Sc.Sql_Reservation_Repository_GetEntities3, parms)) {
                while(rdr.Read()) {
                    var entity = new Reservation();
                    entity.Id = SqlTypeConverter.DBNullStringHandler(rdr["Id"]);
                    entity.Name = SqlTypeConverter.DBNullStringHandler(rdr["Name"]);
                    entity.StartTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["StartTime"]);
                    entity.EndTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["EndTime"]);
                    entity.ProjectId = SqlTypeConverter.DBNullStringHandler(rdr["ProjectId"]);
                    entity.Creator = SqlTypeConverter.DBNullStringHandler(rdr["Creator"]);
                    entity.CreatedTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["CreatedTime"]);
                    entity.Comment = SqlTypeConverter.DBNullStringHandler(rdr["Comment"]);
                    entity.Enabled = SqlTypeConverter.DBNullBooleanHandler(rdr["Enabled"]);
                    entities.Add(entity);
                }
            }
            return entities;
        }

        #endregion

    }
}