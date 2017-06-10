using System;
using System.Data.SqlClient;

namespace iPem.Configurator {
    public class SqlTypeConverter {
        /// <summary>
        /// DBNull String Handler
        /// </summary>
        /// <param name="val">val</param>
        public static string DBNullStringHandler(object val) {
            if(val == DBNull.Value) { return default(String); }
            return val.ToString();
        }

        /// <summary>
        /// DBNull String Checker
        /// </summary>
        /// <param name="val">val</param>
        public static object DBNullStringChecker(string val) {
            if(val == default(String)) { return DBNull.Value; }
            return val;
        }

        /// <summary>
        /// DBNull Int32 Handler
        /// </summary>
        /// <param name="val">val</param>
        public static int DBNullInt32Handler(object val) {
            if(val == DBNull.Value) { return int.MinValue; }
            return (Int32)val;
        }

        /// <summary>
        /// DBNull Int32 Checker
        /// </summary>
        /// <param name="val">val</param>
        public static object DBNullInt32Checker(int val) {
            if(val == int.MinValue) { return DBNull.Value; }
            return val;
        }

        /// <summary>
        /// DBNull Int64 Handler
        /// </summary>
        /// <param name="val">val</param>
        public static long DBNullInt64Handler(object val) {
            if(val == DBNull.Value) { return long.MinValue; }
            return (Int64)val;
        }

        /// <summary>
        /// DBNull Int64 Checker
        /// </summary>
        /// <param name="val">val</param>
        public static object DBNullInt64Checker(long val) {
            if(val == long.MinValue) { return DBNull.Value; }
            return val;
        }

        /// <summary>
        /// DBNull Float Handler
        /// </summary>
        /// <param name="val">val</param>
        public static float DBNullFloatHandler(object val) {
            if(val == DBNull.Value) { return float.MinValue; }
            return (Single)val;
        }

        /// <summary>
        /// DBNull Float Checker
        /// </summary>
        /// <param name="val">val</param>
        public static object DBNullFloatChecker(float val) {
            if(val == float.MinValue) { return DBNull.Value; }
            return val;
        }

        /// <summary>
        /// DBNull Double Handler
        /// </summary>
        /// <param name="val">val</param>
        public static double DBNullDoubleHandler(object val) {
            if(val == DBNull.Value) { return double.MinValue; }
            return (Double)val;
        }

        /// <summary>
        /// DBNull Double Checker
        /// </summary>
        /// <param name="val">val</param>
        public static object DBNullDoubleChecker(double val) {
            if(val == double.MinValue) { return DBNull.Value; }
            return val;
        }

        /// <summary>
        /// DBNull DateTime Handler
        /// </summary>
        /// <param name="val">val</param>
        public static DateTime DBNullDateTimeHandler(object val) {
            if(val == DBNull.Value) { return default(DateTime); }
            return (DateTime)val;
        }

        /// <summary>
        /// DBNull DateTime Checker
        /// </summary>
        /// <param name="val">val</param>
        public static object DBNullDateTimeChecker(DateTime val) {
            if(val == default(DateTime)) { return DBNull.Value; }
            return val;
        }

        /// <summary>
        /// DBNull DateTime Nullable Handler
        /// </summary>
        /// <param name="val">val</param>
        public static DateTime? DBNullDateTimeNullableHandler(object val) {
            if(val == DBNull.Value) { return null; }
            return (DateTime)val;
        }

        /// <summary>
        /// DBNull DateTime Nullable Checker
        /// </summary>
        /// <param name="val">val</param>
        public static object DBNullDateTimeNullableChecker(DateTime? val) {
            if(!val.HasValue) { return DBNull.Value; }
            return val.Value;
        }

        /// <summary>
        /// DBNull Boolean Handler
        /// </summary>
        /// <param name="val">val</param>
        public static bool DBNullBooleanHandler(object val) {
            if(val == DBNull.Value) { return default(Boolean); }
            return (Boolean)val;
        }

        /// <summary>
        /// DBNull Guid Handler
        /// </summary>
        /// <param name="val">val</param>
        public static Guid DBNullGuidHandler(object val) {
            if(val == DBNull.Value) { return default(Guid); }
            return new Guid(val.ToString());
        }

        /// <summary>
        /// DBNull Guid Handler
        /// </summary>
        /// <param name="val">val</param>
        public static object DBNullGuidChecker(Guid val) {
            if(val == default(Guid)) { return DBNull.Value; }
            return val.ToString();
        }

        /// <summary>
        /// DBNull Bytes Handler
        /// </summary>
        /// <param name="val">val</param>
        public static byte[] DBNullBytesHandler(object val) {
            if(val == DBNull.Value) { return null; }
            return (byte[])val;
        }

        public static DatabaseType DBNullDatabaseTypeHandler(object val) {
            if(val == DBNull.Value) { return DatabaseType.SQLServer; }

            var v = (Int32)val;
            return Enum.IsDefined(typeof(DatabaseType), v) ? (DatabaseType)v : DatabaseType.SQLServer;
        }

        public static OrderId DBNullOrderIdHandler(object val) {
            if (val == DBNull.Value) { return OrderId.Null; }

            var v = (Int32)val;
            return Enum.IsDefined(typeof(OrderId), v) ? (OrderId)v : OrderId.Null;
        }

        public static ParamId DBNullParamIdHandler(object val) {
            if (val == DBNull.Value) { return ParamId.Null; }

            var v = (Int32)val;
            return Enum.IsDefined(typeof(ParamId), v) ? (ParamId)v : ParamId.Null;
        }

        public static string CreateConnectionString(bool trustedConnection, string serverName, int portNumber, string databaseName, string userName, string password, int timeout) {
            var builder = new SqlConnectionStringBuilder();
            builder.IntegratedSecurity = trustedConnection;
            builder.DataSource = String.Format("{0},{1}", serverName, portNumber);
            builder.InitialCatalog = databaseName;
            if (!trustedConnection) { builder.UserID = userName; builder.Password = password; }
            builder.PersistSecurityInfo = false;
            builder.MultipleActiveResultSets = true;
            builder.ConnectTimeout = timeout;
            return builder.ConnectionString;
        }
    }
}
