using iPem.Core;
using iPem.Data.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace iPem.Data {
    public partial class ProjectRepository {

        #region Fields

        private readonly string _databaseConnectionString;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        public ProjectRepository() {
            this._databaseConnectionString = SqlHelper.ConnectionStringScTransaction;
        }

        #endregion

        #region Methods

        public virtual Project GetEntity(string id) {
            SqlParameter[] parms = { new SqlParameter("@Id", SqlDbType.VarChar, 100) };
            parms[0].Value = SqlTypeConverter.DBNullStringChecker(id);

            Project entity = null;
            using (var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Sc.Sql_Project_Repository_GetEntity, parms)) {
                if (rdr.Read()) {
                    entity = new Project();
                    entity.Id = SqlTypeConverter.DBNullStringHandler(rdr["Id"]);
                    entity.Name = SqlTypeConverter.DBNullStringHandler(rdr["Name"]);
                    entity.StartTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["StartTime"]);
                    entity.EndTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["EndTime"]);
                    entity.Responsible = SqlTypeConverter.DBNullStringHandler(rdr["Responsible"]);
                    entity.ContactPhone = SqlTypeConverter.DBNullStringHandler(rdr["ContactPhone"]);
                    entity.Company = SqlTypeConverter.DBNullStringHandler(rdr["Company"]);
                    entity.Creator = SqlTypeConverter.DBNullStringHandler(rdr["Creator"]);
                    entity.CreatedTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["CreatedTime"]);
                    entity.Comment = SqlTypeConverter.DBNullStringHandler(rdr["Comment"]);
                    entity.Enabled = SqlTypeConverter.DBNullBooleanHandler(rdr["Enabled"]);
                }
            }

            return entity;
        }

        public virtual List<Project> GetEntities() {
            var entities = new List<Project>();
            using(var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Sc.Sql_Project_Repository_GetEntities, null)) {
                while(rdr.Read()) {
                    var entity = new Project();
                    entity.Id = SqlTypeConverter.DBNullStringHandler(rdr["Id"]);
                    entity.Name = SqlTypeConverter.DBNullStringHandler(rdr["Name"]);
                    entity.StartTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["StartTime"]);
                    entity.EndTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["EndTime"]);
                    entity.Responsible = SqlTypeConverter.DBNullStringHandler(rdr["Responsible"]);
                    entity.ContactPhone = SqlTypeConverter.DBNullStringHandler(rdr["ContactPhone"]);
                    entity.Company = SqlTypeConverter.DBNullStringHandler(rdr["Company"]);
                    entity.Creator = SqlTypeConverter.DBNullStringHandler(rdr["Creator"]);
                    entity.CreatedTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["CreatedTime"]);
                    entity.Comment = SqlTypeConverter.DBNullStringHandler(rdr["Comment"]);
                    entity.Enabled = SqlTypeConverter.DBNullBooleanHandler(rdr["Enabled"]);
                    entities.Add(entity);
                }
            }
            return entities;
        }

        public virtual List<Project> GetEntities(DateTime starttime, DateTime endtime) {
            SqlParameter[] parms = { new SqlParameter("@starttime",SqlDbType.DateTime),
                                     new SqlParameter("@endtime",SqlDbType.DateTime) };

            parms[0].Value = SqlTypeConverter.DBNullDateTimeChecker(starttime);
            parms[1].Value = SqlTypeConverter.DBNullDateTimeChecker(endtime);

            var entities = new List<Project>();
            using(var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Sc.Sql_Project_Repository_GetEntitiesByDate, parms)) {
                while(rdr.Read()) {
                    var entity = new Project();
                    entity.Id = SqlTypeConverter.DBNullStringHandler(rdr["Id"]);
                    entity.Name = SqlTypeConverter.DBNullStringHandler(rdr["Name"]);
                    entity.StartTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["StartTime"]);
                    entity.EndTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["EndTime"]);
                    entity.Responsible = SqlTypeConverter.DBNullStringHandler(rdr["Responsible"]);
                    entity.ContactPhone = SqlTypeConverter.DBNullStringHandler(rdr["ContactPhone"]);
                    entity.Company = SqlTypeConverter.DBNullStringHandler(rdr["Company"]);
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