using iPem.Core;
using iPem.Data.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace iPem.Data {
    public partial class FormulaRepository {

        #region Fields

        private readonly string _databaseConnectionString;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        public FormulaRepository() {
            this._databaseConnectionString = SqlHelper.ConnectionStringScTransaction;
        }

        #endregion

        #region Methods

        public Formula GetEntity(string id, EnmSSH type, EnmFormula formulaType) {
            SqlParameter[] parms = { new SqlParameter("@Id", SqlDbType.VarChar, 100),
                                     new SqlParameter("@Type", SqlDbType.Int),
                                     new SqlParameter("@FormulaType", SqlDbType.Int) };
            parms[0].Value = id;
            parms[1].Value = (int)type;
            parms[2].Value = (int)formulaType;

            Formula entity = null;
            using(var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Sc.Sql_Formula_Repository_GetEntity, parms)) {
                if(rdr.Read()) {
                    entity = new Formula();
                    entity.Id = SqlTypeConverter.DBNullStringHandler(rdr["Id"]);
                    entity.Type = SqlTypeConverter.DBNullEnmOrganizationHandler(rdr["Type"]);
                    entity.FormulaType = SqlTypeConverter.DBNullEnmFormulaHandler(rdr["FormulaType"]);
                    entity.ComputeType = SqlTypeConverter.DBNullEnmComputeHandler(rdr["ComputeType"]);
                    entity.FormulaText = SqlTypeConverter.DBNullStringHandler(rdr["Formula"]);
                    entity.Comment = SqlTypeConverter.DBNullStringHandler(rdr["Comment"]);
                    entity.CreatedTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["CreatedTime"]);
                }
            }
            return entity;
        }

        public List<Formula> GetEntities(string id, EnmSSH type) {
            SqlParameter[] parms = { new SqlParameter("@Id", SqlDbType.VarChar, 100),
                                     new SqlParameter("@Type", SqlDbType.Int) };
            parms[0].Value = id;
            parms[1].Value = (int)type;

            var entities = new List<Formula>();
            using(var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Sc.Sql_Formula_Repository_GetEntities, parms)) {
                while(rdr.Read()) {
                    var entity = new Formula();
                    entity.Id = SqlTypeConverter.DBNullStringHandler(rdr["Id"]);
                    entity.Type = SqlTypeConverter.DBNullEnmOrganizationHandler(rdr["Type"]);
                    entity.FormulaType = SqlTypeConverter.DBNullEnmFormulaHandler(rdr["FormulaType"]);
                    entity.ComputeType = SqlTypeConverter.DBNullEnmComputeHandler(rdr["ComputeType"]);
                    entity.FormulaText = SqlTypeConverter.DBNullStringHandler(rdr["Formula"]);
                    entity.Comment = SqlTypeConverter.DBNullStringHandler(rdr["Comment"]);
                    entity.CreatedTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["CreatedTime"]);
                    entities.Add(entity);
                }
            }
            return entities;
        }

        public List<Formula> GetEntities() {
            var entities = new List<Formula>();
            using(var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Sc.Sql_Formula_Repository_GetAllEntities, null)) {
                while(rdr.Read()) {
                    var entity = new Formula();
                    entity.Id = SqlTypeConverter.DBNullStringHandler(rdr["Id"]);
                    entity.Type = SqlTypeConverter.DBNullEnmOrganizationHandler(rdr["Type"]);
                    entity.FormulaType = SqlTypeConverter.DBNullEnmFormulaHandler(rdr["FormulaType"]);
                    entity.ComputeType = SqlTypeConverter.DBNullEnmComputeHandler(rdr["ComputeType"]);
                    entity.FormulaText = SqlTypeConverter.DBNullStringHandler(rdr["Formula"]);
                    entity.Comment = SqlTypeConverter.DBNullStringHandler(rdr["Comment"]);
                    entity.CreatedTime = SqlTypeConverter.DBNullDateTimeHandler(rdr["CreatedTime"]);
                    entities.Add(entity);
                }
            }
            return entities;
        }

        #endregion

    }
}
