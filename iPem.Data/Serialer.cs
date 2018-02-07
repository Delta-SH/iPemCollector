using iPem.Core;
using iPem.Data.Common;
using System;
using System.Data;
using System.Data.SqlClient;

namespace iPem.Data {
    public partial class Serialer {

        private static readonly KV<long, long> AlarmSerialQueue = new KV<long, long>(0, 0);

        #region Fields

        private readonly string _databaseConnectionString;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        public Serialer() {
            this._databaseConnectionString = SqlHelper.ConnectionStringScTransaction;
        }

        #endregion

        #region Methods

        public string GetAlarmSerialNo() {
            var name = "Alarm_Serial_No";
            var step = 100;
            lock (AlarmSerialQueue) {
                if (AlarmSerialQueue.Key >= AlarmSerialQueue.Value) {
                    var code = IncrAndGet(name);
                    if (code == -1) throw new Exception("取号异常,请重试");

                    AlarmSerialQueue.Key = code * step;
                    AlarmSerialQueue.Value = (code + 1) * step;
                }

                return (AlarmSerialQueue.Key++).ToString().PadLeft(10, '0');
            }
        }

        private long IncrAndGet(string name) {
            SqlParameter[] parms = { new SqlParameter("@Name", SqlDbType.VarChar, 50) };
            parms[0].Value = SqlTypeConverter.DBNullStringChecker(name);

            long result = -1;
            using (var rdr = SqlHelper.ExecuteReader(this._databaseConnectionString, CommandType.Text, SqlCommands_Sc.Sql_ASerialNo_Repository_IncrAndGet, parms)) {
                if (rdr.Read()) {
                    var val = rdr["Code"];
                    if (val != DBNull.Value) {
                        result = (long)val;
                    }
                }
            }

            return result;
        }

        #endregion

    }
}
