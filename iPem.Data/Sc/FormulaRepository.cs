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

        public List<Formula> GetEntities(EnmFormula type) {
            SqlParameter[] parms = { new SqlParameter("@FormulaType", SqlDbType.Int) };
            parms[0].Value = (int)type;

            var entities = new List<Formula>();
            using(var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Sc.Sql_Formula_Repository_GetEntities, parms)) {
                while(rdr.Read()) {
                    var entity = new Formula();
                    entity.Id = SqlTypeConverter.DBNullStringHandler(rdr["Id"]);
                    entity.Type = SqlTypeConverter.DBNullEnmSSHHandler(rdr["Type"]);
                    entity.FormulaType = SqlTypeConverter.DBNullEnmFormulaHandler(rdr["FormulaType"]);
                    entity.ComputeType = SqlTypeConverter.DBNullEnmComputeHandler(rdr["ComputeType"]);
                    entity.FormulaText = SqlTypeConverter.DBNullStringHandler(rdr["FormulaText"]);
                    entity.FormulaValue = SqlTypeConverter.DBNullStringHandler(rdr["FormulaValue"]);
                    entity.Comment = SqlTypeConverter.DBNullStringHandler(rdr["Comment"]);
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
                    entity.Type = SqlTypeConverter.DBNullEnmSSHHandler(rdr["Type"]);
                    entity.FormulaType = SqlTypeConverter.DBNullEnmFormulaHandler(rdr["FormulaType"]);
                    entity.ComputeType = SqlTypeConverter.DBNullEnmComputeHandler(rdr["ComputeType"]);
                    entity.FormulaText = SqlTypeConverter.DBNullStringHandler(rdr["FormulaText"]);
                    entity.FormulaValue = SqlTypeConverter.DBNullStringHandler(rdr["FormulaValue"]);
                    entity.Comment = SqlTypeConverter.DBNullStringHandler(rdr["Comment"]);
                    entities.Add(entity);
                }
            }
            return entities;
        }

        #endregion

    }
}
