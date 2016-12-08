using iPem.Core;
using iPem.Data.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace iPem.Data {
    public partial class NodesInAppointmentRepository {

        #region Fields

        private readonly string _databaseConnectionString;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        public NodesInAppointmentRepository() {
            this._databaseConnectionString = SqlHelper.ConnectionStringScTransaction;
        }

        #endregion

        #region Method

        public List<NodesInAppointment> GetEntities() {
            var entities = new List<NodesInAppointment>();
            using(var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Sc.Sql_NodesInAppointment_Repository_GetEntities, null)) {
                while(rdr.Read()) {
                    var entity = new NodesInAppointment();
                    entity.AppointmentId = SqlTypeConverter.DBNullGuidHandler(rdr["AppointmentId"]);
                    entity.NodeId = SqlTypeConverter.DBNullStringHandler(rdr["NodeId"]);
                    entity.NodeType = SqlTypeConverter.DBNullEnmOrganizationHandler(rdr["NodeType"]);
                    entities.Add(entity);
                }
            }
            return entities;
        }

        public List<NodesInAppointment> GetEntities(EnmOrganization type) {
            SqlParameter[] parms = { new SqlParameter("@NodeType", SqlDbType.Int) };
            parms[0].Value = (int)type;

            var entities = new List<NodesInAppointment>();
            using(var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Sc.Sql_NodesInAppointment_Repository_GetEntitiesByNodeType, parms)) {
                while(rdr.Read()) {
                    var entity = new NodesInAppointment();
                    entity.AppointmentId = SqlTypeConverter.DBNullGuidHandler(rdr["AppointmentId"]);
                    entity.NodeId = SqlTypeConverter.DBNullStringHandler(rdr["NodeId"]);
                    entity.NodeType = SqlTypeConverter.DBNullEnmOrganizationHandler(rdr["NodeType"]);
                    entities.Add(entity);
                }
            }
            return entities;
        }

        public List<NodesInAppointment> GetEntities(Guid appointmentId) {
            SqlParameter[] parms = { new SqlParameter("@AppointmentId", SqlDbType.VarChar, 100) };
            parms[0].Value = SqlTypeConverter.DBNullGuidChecker(appointmentId);

            var entities = new List<NodesInAppointment>();
            using(var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Sc.Sql_NodesInAppointment_Repository_GetEntitiesByAppointmentId, parms)) {
                while(rdr.Read()) {
                    var entity = new NodesInAppointment();
                    entity.AppointmentId = SqlTypeConverter.DBNullGuidHandler(rdr["AppointmentId"]);
                    entity.NodeId = SqlTypeConverter.DBNullStringHandler(rdr["NodeId"]);
                    entity.NodeType = SqlTypeConverter.DBNullEnmOrganizationHandler(rdr["NodeType"]);
                    entities.Add(entity);
                }
            }
            return entities;
        }

        #endregion

    }
}