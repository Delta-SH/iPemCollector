using iPem.Core;
using iPem.Data.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace iPem.Data {
    public partial class PointRepository {

        #region Fields

        private readonly string _databaseConnectionString;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        public PointRepository() {
            this._databaseConnectionString = SqlHelper.ConnectionStringRsTransaction;
        }

        #endregion

        #region Methods

        public List<Point> GetEntities(string device) {
            SqlParameter[] parms = { new SqlParameter("@DeviceId", SqlDbType.VarChar,100) };
            parms[0].Value = device;

            var entities = new List<Point>();
            using(var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Rs.Sql_Point_Repository_GetEntitiesByDevice, parms)) {
                while(rdr.Read()) {
                    var entity = new Point();
                    entity.Id = SqlTypeConverter.DBNullStringHandler(rdr["Id"]);
                    entity.Code = SqlTypeConverter.DBNullStringHandler(rdr["Code"]);
                    entity.Name = SqlTypeConverter.DBNullStringHandler(rdr["Name"]);
                    entity.Type = SqlTypeConverter.DBNullEnmPointHandler(rdr["Type"]);
                    entity.UnitState = SqlTypeConverter.DBNullStringHandler(rdr["UnitState"]);
                    entity.Number = SqlTypeConverter.DBNullStringHandler(rdr["Number"]);
                    entity.AlarmId = SqlTypeConverter.DBNullStringHandler(rdr["AlarmId"]);
                    entity.NMAlarmId = SqlTypeConverter.DBNullStringHandler(rdr["NMAlarmId"]);
                    entity.DeviceType = new DeviceType { Id = SqlTypeConverter.DBNullStringHandler(rdr["DeviceTypeId"]), Name = SqlTypeConverter.DBNullStringHandler(rdr["DeviceTypeName"]) };
                    entity.LogicType = new LogicType { Id = SqlTypeConverter.DBNullStringHandler(rdr["LogicTypeId"]), Name = SqlTypeConverter.DBNullStringHandler(rdr["LogicTypeName"]) };
                    entity.DeviceEffect = SqlTypeConverter.DBNullStringHandler(rdr["DeviceEffect"]);
                    entity.BusiEffect = SqlTypeConverter.DBNullStringHandler(rdr["BusiEffect"]);
                    entity.Comment = SqlTypeConverter.DBNullStringHandler(rdr["Comment"]);
                    entity.Interpret = SqlTypeConverter.DBNullStringHandler(rdr["Interpret"]);
                    entity.ExtSet = SqlTypeConverter.DBNullStringHandler(rdr["ExtSet"]);
                    entity.Enabled = SqlTypeConverter.DBNullBooleanHandler(rdr["Enabled"]);
                    entities.Add(entity);
                }
            }
            return entities;
        }

        public List<Point> GetEntities() {
            var entities = new List<Point>();
            using(var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Rs.Sql_Point_Repository_GetEntities, null)) {
                while(rdr.Read()) {
                    var entity = new Point();
                    entity.Id = SqlTypeConverter.DBNullStringHandler(rdr["Id"]);
                    entity.Code = SqlTypeConverter.DBNullStringHandler(rdr["Code"]);
                    entity.Name = SqlTypeConverter.DBNullStringHandler(rdr["Name"]);
                    entity.Type = SqlTypeConverter.DBNullEnmPointHandler(rdr["Type"]);
                    entity.UnitState = SqlTypeConverter.DBNullStringHandler(rdr["UnitState"]);
                    entity.Number = SqlTypeConverter.DBNullStringHandler(rdr["Number"]);
                    entity.AlarmId = SqlTypeConverter.DBNullStringHandler(rdr["AlarmId"]);
                    entity.NMAlarmId = SqlTypeConverter.DBNullStringHandler(rdr["NMAlarmId"]);
                    entity.DeviceType = new DeviceType { Id = SqlTypeConverter.DBNullStringHandler(rdr["DeviceTypeId"]), Name = SqlTypeConverter.DBNullStringHandler(rdr["DeviceTypeName"]) };
                    entity.LogicType = new LogicType { Id = SqlTypeConverter.DBNullStringHandler(rdr["LogicTypeId"]), Name = SqlTypeConverter.DBNullStringHandler(rdr["LogicTypeName"]) };
                    entity.DeviceEffect = SqlTypeConverter.DBNullStringHandler(rdr["DeviceEffect"]);
                    entity.BusiEffect = SqlTypeConverter.DBNullStringHandler(rdr["BusiEffect"]);
                    entity.Comment = SqlTypeConverter.DBNullStringHandler(rdr["Comment"]);
                    entity.Interpret = SqlTypeConverter.DBNullStringHandler(rdr["Interpret"]);
                    entity.ExtSet = SqlTypeConverter.DBNullStringHandler(rdr["ExtSet"]);
                    entity.Enabled = SqlTypeConverter.DBNullBooleanHandler(rdr["Enabled"]);
                    entities.Add(entity);
                }
            }
            return entities;
        }

        #endregion

    }
}
