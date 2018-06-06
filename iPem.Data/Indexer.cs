using iPem.Data.Common;
using System;
using System.Data;
using System.Data.SqlClient;

namespace iPem.Data {
    public partial class Indexer {

        #region Fields

        private readonly string _databaseConnectionString;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        public Indexer() {
            this._databaseConnectionString = SqlHelper.ConnectionStringCsTransaction;
        }

        #endregion

        #region Methods

        public void Check(DateTime date) {
            using (var conn = new SqlConnection(this._databaseConnectionString)) {
                conn.Open();
                SqlHelper.ExecuteNonQuery(conn, CommandType.Text, string.Format(SqlCommands_Cs.Sql_Indexer_Check, date.ToString("yyyyMM")), null);
            }
        }

        #endregion

    }
}
