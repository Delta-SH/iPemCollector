using iPem.Core;
using iPem.Data.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace iPem.Data {
    public partial class StationRepository {

        #region Fields

        private readonly string _databaseConnectionString;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        public StationRepository() {
            this._databaseConnectionString = SqlHelper.ConnectionStringRsTransaction;
        }

        #endregion

        #region Methods

        public Station GetEntity(string id) {
            SqlParameter[] parms = { new SqlParameter("@Id", SqlDbType.VarChar, 100) };
            parms[0].Value = SqlTypeConverter.DBNullStringChecker(id);

            Station entity = null;
            using(var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Rs.Sql_Station_Repository_GetEntity, parms)) {
                if(rdr.Read()) {
                    entity = new Station();
                    entity.Id = SqlTypeConverter.DBNullStringHandler(rdr["Id"]);
                    entity.Code = SqlTypeConverter.DBNullStringHandler(rdr["Code"]);
                    entity.Name = SqlTypeConverter.DBNullStringHandler(rdr["Name"]);
                    entity.Type = new StationType { Id = SqlTypeConverter.DBNullStringHandler(rdr["StaTypeId"]), Name = SqlTypeConverter.DBNullStringHandler(rdr["StaTypeName"]) };
                    entity.Longitude = SqlTypeConverter.DBNullStringHandler(rdr["Longitude"]);
                    entity.Latitude = SqlTypeConverter.DBNullStringHandler(rdr["Latitude"]);
                    entity.Altitude = SqlTypeConverter.DBNullStringHandler(rdr["Altitude"]);
                    entity.CityElecCap = SqlTypeConverter.DBNullStringHandler(rdr["CityElecCap"]);
                    entity.CityElectNumber = SqlTypeConverter.DBNullInt32Handler(rdr["CityElectNumber"]);
                    entity.AreaId = SqlTypeConverter.DBNullStringHandler(rdr["AreaId"]);
                    entity.Comment = SqlTypeConverter.DBNullStringHandler(rdr["Comment"]);
                    entity.Enabled = SqlTypeConverter.DBNullBooleanHandler(rdr["Enabled"]);
                }
            }
            return entity;
        }

        public List<Station> GetEntities(string parent) {
            SqlParameter[] parms = { new SqlParameter("@AreaId", SqlDbType.VarChar, 100) };
            parms[0].Value = SqlTypeConverter.DBNullStringChecker(parent);

            var entities = new List<Station>();
            using(var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Rs.Sql_Station_Repository_GetEntitiesByParent, parms)) {
                while(rdr.Read()) {
                    var entity = new Station();
                    entity.Id = SqlTypeConverter.DBNullStringHandler(rdr["Id"]);
                    entity.Code = SqlTypeConverter.DBNullStringHandler(rdr["Code"]);
                    entity.Name = SqlTypeConverter.DBNullStringHandler(rdr["Name"]);
                    entity.Type = new StationType { Id = SqlTypeConverter.DBNullStringHandler(rdr["StaTypeId"]), Name = SqlTypeConverter.DBNullStringHandler(rdr["StaTypeName"]) };
                    entity.Longitude = SqlTypeConverter.DBNullStringHandler(rdr["Longitude"]);
                    entity.Latitude = SqlTypeConverter.DBNullStringHandler(rdr["Latitude"]);
                    entity.Altitude = SqlTypeConverter.DBNullStringHandler(rdr["Altitude"]);
                    entity.CityElecCap = SqlTypeConverter.DBNullStringHandler(rdr["CityElecCap"]);
                    entity.CityElectNumber = SqlTypeConverter.DBNullInt32Handler(rdr["CityElectNumber"]);
                    entity.AreaId = SqlTypeConverter.DBNullStringHandler(rdr["AreaId"]);
                    entity.Comment = SqlTypeConverter.DBNullStringHandler(rdr["Comment"]);
                    entity.Enabled = SqlTypeConverter.DBNullBooleanHandler(rdr["Enabled"]);
                    entities.Add(entity);
                }
            }
            return entities;
        }

        public List<Station> GetEntities() {
            var entities = new List<Station>();
            using(var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Rs.Sql_Station_Repository_GetEntities, null)) {
                while(rdr.Read()) {
                    var entity = new Station();
                    entity.Id = SqlTypeConverter.DBNullStringHandler(rdr["Id"]);
                    entity.Code = SqlTypeConverter.DBNullStringHandler(rdr["Code"]);
                    entity.Name = SqlTypeConverter.DBNullStringHandler(rdr["Name"]);
                    entity.Type = new StationType { Id = SqlTypeConverter.DBNullStringHandler(rdr["StaTypeId"]), Name = SqlTypeConverter.DBNullStringHandler(rdr["StaTypeName"]) };
                    entity.Longitude = SqlTypeConverter.DBNullStringHandler(rdr["Longitude"]);
                    entity.Latitude = SqlTypeConverter.DBNullStringHandler(rdr["Latitude"]);
                    entity.Altitude = SqlTypeConverter.DBNullStringHandler(rdr["Altitude"]);
                    entity.CityElecCap = SqlTypeConverter.DBNullStringHandler(rdr["CityElecCap"]);
                    entity.CityElectNumber = SqlTypeConverter.DBNullInt32Handler(rdr["CityElectNumber"]);
                    entity.AreaId = SqlTypeConverter.DBNullStringHandler(rdr["AreaId"]);
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
