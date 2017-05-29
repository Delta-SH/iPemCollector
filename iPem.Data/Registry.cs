using iPem.Data.Common;
using iPem.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text;

namespace iPem.Core {
    /// <summary>
    /// 服务相关配置类
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
        /// 获取数据库配置信息
        /// </summary>
        public List<DbEntity> GetDatabases() {
            var databases = new List<DbEntity>();
            using (var conn = new SQLiteConnection(registryConnectionString)) {
                conn.SetPassword(dbPassword);
                conn.Open();
                using (var command = new SQLiteCommand(SqliteCommands.Registry_Get_Databases, conn)) {
                    using (var rdr = command.ExecuteReader(CommandBehavior.CloseConnection)) {
                        while (rdr.Read()) {
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
        /// 获取数据库配置信息
        /// </summary>
        public DbEntity GetDatabase(string id) {
            SQLiteParameter[] parms = { new SQLiteParameter("@id", DbType.String) };
            parms[0].Value = id;

            DbEntity database = null;
            using (var conn = new SQLiteConnection(registryConnectionString)) {
                conn.SetPassword(dbPassword);
                conn.Open();
                using (var command = new SQLiteCommand(SqliteCommands.Registry_Get_Database, conn)) {
                    command.Parameters.AddRange(parms);
                    using (var rdr = command.ExecuteReader(CommandBehavior.CloseConnection)) {
                        if (rdr.Read()) {
                            database = new DbEntity();
                            database.Id = SqlTypeConverter.DBNullStringHandler(rdr["id"]);
                            database.Name = SqlTypeConverter.DBNullStringHandler(rdr["name"]);
                            database.Type = SqlTypeConverter.DBNullDatabaseTypeHandler(rdr["type"]);
                            database.IP = SqlTypeConverter.DBNullStringHandler(rdr["ip"]);
                            database.Port = SqlTypeConverter.DBNullInt32Handler(rdr["port"]);
                            database.Uid = SqlTypeConverter.DBNullStringHandler(rdr["uid"]);
                            database.Password = SqlTypeConverter.DBNullStringHandler(rdr["password"]);
                            database.Db = SqlTypeConverter.DBNullStringHandler(rdr["db"]);
                        }
                    }
                }
            }
            return database;
        }

        /// <summary>
        /// 获取计划任务配置信息
        /// </summary>
        public List<TaskEntity> GetTasks() {
            var tasks = new List<TaskEntity>();
            using (var conn = new SQLiteConnection(registryConnectionString)) {
                conn.SetPassword(dbPassword);
                conn.Open();
                using (var command = new SQLiteCommand(SqliteCommands.Registry_Get_Tasks, conn)) {
                    using (var rdr = command.ExecuteReader(CommandBehavior.CloseConnection)) {
                        while (rdr.Read()) {
                            var json = SqlTypeConverter.DBNullStringHandler(rdr["json"]);
                            tasks.Add(new TaskEntity {
                                Id = SqlTypeConverter.DBNullStringHandler(rdr["id"]),
                                Name = SqlTypeConverter.DBNullStringHandler(rdr["name"]),
                                Json = string.IsNullOrWhiteSpace(json) ? null : JsonConvert.DeserializeObject<TaskModel>(json),
                                Last = SqlTypeConverter.DBNullDateTimeHandler(rdr["last"]),
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
        /// 获取计划任务配置信息
        /// </summary>
        public TaskEntity GetTask(string id) {
            SQLiteParameter[] parms = { new SQLiteParameter("@id", DbType.String) };
            parms[0].Value = id;

            TaskEntity task = null;
            using (var conn = new SQLiteConnection(registryConnectionString)) {
                conn.SetPassword(dbPassword);
                conn.Open();
                using (var command = new SQLiteCommand(SqliteCommands.Registry_Get_Task, conn)) {
                    command.Parameters.AddRange(parms);
                    using (var rdr = command.ExecuteReader(CommandBehavior.CloseConnection)) {
                        if (rdr.Read()) {
                            var json = SqlTypeConverter.DBNullStringHandler(rdr["json"]);
                            task = new TaskEntity {
                                Id = SqlTypeConverter.DBNullStringHandler(rdr["id"]),
                                Name = SqlTypeConverter.DBNullStringHandler(rdr["name"]),
                                Json = string.IsNullOrWhiteSpace(json) ? null : JsonConvert.DeserializeObject<TaskModel>(json),
                                Last = SqlTypeConverter.DBNullDateTimeHandler(rdr["last"]),
                                Next = SqlTypeConverter.DBNullDateTimeHandler(rdr["next"]),
                                Index = SqlTypeConverter.DBNullInt32Handler(rdr["index"])
                            };
                        }
                    }
                }
            }
            return task;
        }

        /// <summary>
        /// 更新计划任务配置信息
        /// </summary>
        public void UpdateTasks(List<TaskEntity> tasks) {
            SQLiteParameter[] parms = { new SQLiteParameter("@id", DbType.String,50),
                                        new SQLiteParameter("@last", DbType.DateTime),
                                        new SQLiteParameter("@next", DbType.DateTime) };

            using (var conn = new SQLiteConnection(registryConnectionString)) {
                conn.SetPassword(dbPassword);
                conn.Open();
                using (var command = new SQLiteCommand(SqliteCommands.Registry_Update_Task, conn)) {
                    foreach (var task in tasks) {
                        parms[0].Value = SqlTypeConverter.DBNullStringChecker(task.Id);
                        parms[1].Value = SqlTypeConverter.DBNullDateTimeChecker(task.Last);
                        parms[2].Value = SqlTypeConverter.DBNullDateTimeChecker(task.Next);

                        command.Parameters.Clear();
                        command.Parameters.AddRange(parms);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// 获取基本参数信息
        /// </summary>
        public List<ParamEntity> GetParams() {
            var parameters = new List<ParamEntity>();
            using (var conn = new SQLiteConnection(registryConnectionString)) {
                conn.SetPassword(dbPassword);
                conn.Open();
                using (var command = new SQLiteCommand(SqliteCommands.Registry_Get_Params, conn)) {
                    using (var rdr = command.ExecuteReader(CommandBehavior.CloseConnection)) {
                        while (rdr.Read()) {
                            var json = SqlTypeConverter.DBNullStringHandler(rdr["json"]);
                            parameters.Add(new ParamEntity {
                                Id = SqlTypeConverter.DBNullStringHandler(rdr["id"]),
                                Name = SqlTypeConverter.DBNullStringHandler(rdr["name"]),
                                Json = string.IsNullOrWhiteSpace(json) ? null : JsonConvert.DeserializeObject<ParamModel>(json),
                                Time = SqlTypeConverter.DBNullDateTimeHandler(rdr["time"])
                            });
                        }
                    }
                }
            }
            return parameters;
        }

        /// <summary>
        /// 获取基本参数信息
        /// </summary>
        public ParamEntity GetParam(string id) {
            SQLiteParameter[] parms = { new SQLiteParameter("@id", DbType.String) };
            parms[0].Value = id;

            ParamEntity param = null;
            using (var conn = new SQLiteConnection(registryConnectionString)) {
                conn.SetPassword(dbPassword);
                conn.Open();
                using (var command = new SQLiteCommand(SqliteCommands.Registry_Get_Param, conn)) {
                    command.Parameters.AddRange(parms);
                    using (var rdr = command.ExecuteReader(CommandBehavior.CloseConnection)) {
                        if (rdr.Read()) {
                            var json = SqlTypeConverter.DBNullStringHandler(rdr["json"]);
                            param = new ParamEntity {
                                Id = SqlTypeConverter.DBNullStringHandler(rdr["id"]),
                                Name = SqlTypeConverter.DBNullStringHandler(rdr["name"]),
                                Json = string.IsNullOrWhiteSpace(json) ? null : JsonConvert.DeserializeObject<ParamModel>(json),
                                Time = SqlTypeConverter.DBNullDateTimeHandler(rdr["time"])
                            };
                        }
                    }
                }
            }
            return param;
        }
    }
}
