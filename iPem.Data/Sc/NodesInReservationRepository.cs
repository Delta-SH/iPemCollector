﻿using iPem.Core;
using iPem.Data.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace iPem.Data {
    public partial class NodesInReservationRepository {

        #region Fields

        private readonly string _databaseConnectionString;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        public NodesInReservationRepository() {
            this._databaseConnectionString = SqlHelper.ConnectionStringScTransaction;
        }

        #endregion

        #region Method

        public List<NodesInReservation> GetEntities() {
            var entities = new List<NodesInReservation>();
            using(var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Sc.Sql_NodesInReservation_Repository_GetEntities, null)) {
                while(rdr.Read()) {
                    var entity = new NodesInReservation();
                    entity.ReservationId = SqlTypeConverter.DBNullStringHandler(rdr["ReservationId"]);
                    entity.NodeId = SqlTypeConverter.DBNullStringHandler(rdr["NodeId"]);
                    entity.NodeType = SqlTypeConverter.DBNullEnmSSHHandler(rdr["NodeType"]);
                    entities.Add(entity);
                }
            }
            return entities;
        }

        public List<NodesInReservation> GetEntities(EnmSSH type) {
            SqlParameter[] parms = { new SqlParameter("@NodeType", SqlDbType.Int) };
            parms[0].Value = (int)type;

            var entities = new List<NodesInReservation>();
            using(var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Sc.Sql_NodesInReservation_Repository_GetEntitiesByNodeType, parms)) {
                while(rdr.Read()) {
                    var entity = new NodesInReservation();
                    entity.ReservationId = SqlTypeConverter.DBNullStringHandler(rdr["ReservationId"]);
                    entity.NodeId = SqlTypeConverter.DBNullStringHandler(rdr["NodeId"]);
                    entity.NodeType = SqlTypeConverter.DBNullEnmSSHHandler(rdr["NodeType"]);
                    entities.Add(entity);
                }
            }
            return entities;
        }

        public List<NodesInReservation> GetEntities(string reservationId) {
            SqlParameter[] parms = { new SqlParameter("@ReservationId", SqlDbType.VarChar, 100) };
            parms[0].Value = SqlTypeConverter.DBNullStringChecker(reservationId);

            var entities = new List<NodesInReservation>();
            using (var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Sc.Sql_NodesInReservation_Repository_GetEntitiesById, parms)) {
                while(rdr.Read()) {
                    var entity = new NodesInReservation();
                    entity.ReservationId = SqlTypeConverter.DBNullStringHandler(rdr["ReservationId"]);
                    entity.NodeId = SqlTypeConverter.DBNullStringHandler(rdr["NodeId"]);
                    entity.NodeType = SqlTypeConverter.DBNullEnmSSHHandler(rdr["NodeType"]);
                    entities.Add(entity);
                }
            }
            return entities;
        }

        public List<NodesInReservation> GetEntities(params string[] reservationIds) {
            var entities = new List<NodesInReservation>();

            if (reservationIds.Length > 0) {
                var sql = string.Format(@"SELECT * FROM [dbo].[M_NodesInReservation] WHERE [ReservationId] IN ({0});", string.Join(",", reservationIds.Select(s => string.Format("'{0}'", s))));
                using (var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, sql, null)) {
                    while (rdr.Read()) {
                        var entity = new NodesInReservation();
                        entity.ReservationId = SqlTypeConverter.DBNullStringHandler(rdr["ReservationId"]);
                        entity.NodeId = SqlTypeConverter.DBNullStringHandler(rdr["NodeId"]);
                        entity.NodeType = SqlTypeConverter.DBNullEnmSSHHandler(rdr["NodeType"]);
                        entities.Add(entity);
                    }
                }
            }

            return entities;
        }

        #endregion

    }
}