using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text;

namespace iPem.Configurator {
    /// <summary>
    /// The Registry class is intended to encapsulate high performance, 
    /// scalable best practices for common uses of System.Data.SQLite.
    /// </summary>
    public class Registry {
        //Database connection parameters
        private readonly string registryConnectionString;
        private readonly string dbPassword;

        /// <summary>
        /// Class Constructor
        /// </summary>
        public Registry(string filePath) {
            registryConnectionString = String.Format(@"data source={0}\Registry.db;Pooling=True;Max Pool Size=100;FailIfMissing=False", filePath);
            dbPassword = "1qaz2wsx3edc";
        }

        /// <summary>
        /// Class Constructor
        /// </summary>
        /// <param name="filePath">database file path</param>
        /// <param name="dbPwd">database password</param>
        public Registry(string filePath, string dbPwd) {
            registryConnectionString = String.Format(@"data source={0}\Registry.db;Pooling=True;Max Pool Size=100;FailIfMissing=False", filePath);
            dbPassword = dbPwd;
        }

        /// <summary>
        /// Create registry database file.
        /// </summary>
        public void CreateRegistry() {
            using(var conn = new SQLiteConnection(registryConnectionString)) {
                conn.SetPassword(dbPassword);
                conn.Open();
                using(var command = new SQLiteCommand()) {
                    command.Connection = conn;
                    command.CommandText = SqliteCommands.Registry_Create_Tables;
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Get database entity
        /// </summary>
        /// <param name="id">database id</param>
        /// <returns>database setting</returns>
        public List<DbEntity> GetDatabases() {
            var databases = new List<DbEntity>();
            using(var conn = new SQLiteConnection(registryConnectionString)) {
                conn.SetPassword(dbPassword);
                conn.Open();
                using(var command = new SQLiteCommand(SqliteCommands.Registry_Get_Database, conn)) {
                    using(var rdr = command.ExecuteReader(CommandBehavior.CloseConnection)) {
                        while(rdr.Read()) {
                            var database = new DbEntity();
                            database.Id = SqlTypeConverter.DBNullStringHandler(rdr["id"]);
                            database.Name = SqlTypeConverter.DBNullStringHandler(rdr["name"]);
                            database.Type = SqlTypeConverter.DBNullDatabaseTypeHandler(rdr["type"]);
                            database.IP = SqlTypeConverter.DBNullStringHandler(rdr["ip"]);
                            database.Port = SqlTypeConverter.DBNullInt32Handler(rdr["port"]);
                            database.Uid = SqlTypeConverter.DBNullStringHandler(rdr["uid"]);
                            database.Password = SqlTypeConverter.DBNullStringHandler(rdr["password"]);
                            database.Db = SqlTypeConverter.DBNullStringHandler(rdr["db"]);
                            databases.Add(database);
                        }
                    }
                }
            }
            return databases;
        }

        /// <summary>
        /// Save database entity.
        /// </summary>
        /// <param name="database">database entity</param>
        public void SaveDatabases(List<DbEntity> databases) {
            SQLiteParameter[] parms = { new SQLiteParameter("@id", DbType.String,50),
                                        new SQLiteParameter("@name", DbType.String,200),
                                        new SQLiteParameter("@type", DbType.Int32),
                                        new SQLiteParameter("@ip", DbType.String,128),
                                        new SQLiteParameter("@port", DbType.Int32),
                                        new SQLiteParameter("@uid", DbType.String,50),
                                        new SQLiteParameter("@password", DbType.String,50),
                                        new SQLiteParameter("@db", DbType.String,100) };

            using(var conn = new SQLiteConnection(registryConnectionString)) {
                conn.SetPassword(dbPassword);
                conn.Open();
                using(var command = new SQLiteCommand(SqliteCommands.Registry_Save_Database, conn)) {
                    foreach (var database in databases) {
                        parms[0].Value = SqlTypeConverter.DBNullStringChecker(database.Id);
                        parms[1].Value = SqlTypeConverter.DBNullStringChecker(database.Name);
                        parms[2].Value = (int)database.Type;
                        parms[3].Value = SqlTypeConverter.DBNullStringChecker(database.IP);
                        parms[4].Value = SqlTypeConverter.DBNullInt32Checker(database.Port);
                        parms[5].Value = SqlTypeConverter.DBNullStringChecker(database.Uid);
                        parms[6].Value = SqlTypeConverter.DBNullStringChecker(database.Password);
                        parms[7].Value = SqlTypeConverter.DBNullStringChecker(database.Db);

                        command.Parameters.Clear();
                        command.Parameters.AddRange(parms);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Clean databases.
        /// </summary>
        public void CleanDatabases() {
            using(var conn = new SQLiteConnection(registryConnectionString)) {
                conn.SetPassword(dbPassword);
                conn.Open();
                using(var command = new SQLiteCommand()) {
                    command.Connection = conn;
                    command.CommandText = SqliteCommands.Registry_Clean_Databases;
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Get all tasks.
        /// </summary>
        /// <returns>all tasks</returns>
        public List<TaskEntity> GetTasks() {
            var tasks = new List<TaskEntity>();
            using(var conn = new SQLiteConnection(registryConnectionString)) {
                conn.SetPassword(dbPassword);
                conn.Open();
                using(var command = new SQLiteCommand(SqliteCommands.Registry_Get_Tasks, conn)) {
                    using(var rdr = command.ExecuteReader(CommandBehavior.CloseConnection)) {
                        while(rdr.Read()) {
                            tasks.Add(new TaskEntity {
                                Id = SqlTypeConverter.DBNullStringHandler(rdr["id"]),
                                Name = SqlTypeConverter.DBNullStringHandler(rdr["name"]),
                                Json = SqlTypeConverter.DBNullStringHandler(rdr["json"]),
                                Start = SqlTypeConverter.DBNullDateTimeHandler(rdr["start"]),
                                End = SqlTypeConverter.DBNullDateTimeHandler(rdr["end"]),
                                Next = SqlTypeConverter.DBNullDateTimeHandler(rdr["next"]),
                                Index = SqlTypeConverter.DBNullInt32Handler(rdr["index"])
                            });
                        }
                    }
                }
            }
            return tasks;
        }

        /// <summary>
        /// Save all tasks
        /// </summary>
        /// <param name="tasks">tasks</param>
        public void SaveTasks(List<TaskEntity> tasks) {
            SQLiteParameter[] parms = { new SQLiteParameter("@id", DbType.String,50),
                                        new SQLiteParameter("@name", DbType.String,200),
                                        new SQLiteParameter("@json", DbType.String),
                                        new SQLiteParameter("@index", DbType.Int32) };

            using(var conn = new SQLiteConnection(registryConnectionString)) {
                conn.SetPassword(dbPassword);
                conn.Open();
                using(var command = new SQLiteCommand(SqliteCommands.Registry_Save_Task, conn)) {
                    foreach(var task in tasks) {
                        parms[0].Value = SqlTypeConverter.DBNullStringChecker(task.Id);
                        parms[1].Value = SqlTypeConverter.DBNullStringChecker(task.Name);
                        parms[2].Value = SqlTypeConverter.DBNullStringChecker(task.Json);
                        parms[3].Value = task.Index;

                        command.Parameters.Clear();
                        command.Parameters.AddRange(parms);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Clean all tasks
        /// </summary>
        public void CleanTasks() {
            using(var conn = new SQLiteConnection(registryConnectionString)) {
                conn.SetPassword(dbPassword);
                conn.Open();
                using(var command = new SQLiteCommand()) {
                    command.Connection = conn;
                    command.CommandText = SqliteCommands.Registry_Clean_Tasks;
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Get all params.
        /// </summary>
        /// <returns>all params</returns>
        public List<ParamEntity> GetParams() {
            var parameters = new List<ParamEntity>();
            using(var conn = new SQLiteConnection(registryConnectionString)) {
                conn.SetPassword(dbPassword);
                conn.Open();
                using (var command = new SQLiteCommand(SqliteCommands.Registry_Get_Params, conn)) {
                    using(var rdr = command.ExecuteReader(CommandBehavior.CloseConnection)) {
                        while(rdr.Read()) {
                            parameters.Add(new ParamEntity {
                                Id = SqlTypeConverter.DBNullStringHandler(rdr["id"]),
                                Name = SqlTypeConverter.DBNullStringHandler(rdr["name"]),
                                Json = SqlTypeConverter.DBNullStringHandler(rdr["json"]),
                                Time = SqlTypeConverter.DBNullDateTimeHandler(rdr["time"])
                            });
                        }
                    }
                }
            }
            return parameters;
        }

        /// <summary>
        /// Save all params.
        /// </summary>
        /// <param name="entities">all entities</param>
        public void SaveParams(List<ParamEntity> entities) {
            SQLiteParameter[] parms = { new SQLiteParameter("@id", DbType.String,50),
                                        new SQLiteParameter("@name", DbType.String,200),
                                        new SQLiteParameter("@json", DbType.String),
                                        new SQLiteParameter("@time", DbType.DateTime)};

            using(var conn = new SQLiteConnection(registryConnectionString)) {
                conn.SetPassword(dbPassword);
                conn.Open();
                using (var command = new SQLiteCommand(SqliteCommands.Registry_Save_Params, conn)) {
                    foreach (var entity in entities) {
                        parms[0].Value = SqlTypeConverter.DBNullStringChecker(entity.Id);
                        parms[1].Value = SqlTypeConverter.DBNullStringChecker(entity.Name);
                        parms[2].Value = SqlTypeConverter.DBNullStringChecker(entity.Json);
                        parms[3].Value = SqlTypeConverter.DBNullDateTimeChecker(entity.Time);

                        command.Parameters.Clear();
                        command.Parameters.AddRange(parms);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Clean all params.
        /// </summary>
        public void CleanPlans() {
            using(var conn = new SQLiteConnection(registryConnectionString)) {
                conn.SetPassword(dbPassword);
                conn.Open();
                using(var command = new SQLiteCommand()) {
                    command.Connection = conn;
                    command.CommandText = SqliteCommands.Registry_Clean_Params;
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
