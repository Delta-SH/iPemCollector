﻿using System;
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
        /// Get order entity
        /// </summary>
        public List<OrderEntity> GetOrders() {
            var orders = new List<OrderEntity>();
            using (var conn = new SQLiteConnection(registryConnectionString)) {
                conn.SetPassword(dbPassword);
                conn.Open();
                using (var command = new SQLiteCommand(SqliteCommands.Registry_Get_Order, conn)) {
                    using (var rdr = command.ExecuteReader(CommandBehavior.CloseConnection)) {
                        while (rdr.Read()) {
                            var order = new OrderEntity();
                            order.Id = SqlTypeConverter.DBNullOrderIdHandler(rdr["id"]);
                            order.Param = SqlTypeConverter.DBNullStringHandler(rdr["param"]);
                            order.Time = SqlTypeConverter.DBNullDateTimeHandler(rdr["time"]);
                            orders.Add(order);
                        }
                    }
                }
            }
            return orders;
        }

        /// <summary>
        /// Save order entity.
        /// </summary>
        public void SaveOrders(List<OrderEntity> orders) {
            SQLiteParameter[] parms = { new SQLiteParameter("@id", DbType.Int32),
                                        new SQLiteParameter("@param", DbType.String, 1024) };

            using (var conn = new SQLiteConnection(registryConnectionString)) {
                conn.SetPassword(dbPassword);
                conn.Open();
                using (var command = new SQLiteCommand(SqliteCommands.Registry_Save_Order, conn)) {
                    foreach (var order in orders) {
                        parms[0].Value = (int)order.Id;
                        parms[1].Value = SqlTypeConverter.DBNullStringChecker(order.Param);
                        command.Parameters.Clear();
                        command.Parameters.AddRange(parms);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Delete order entity.
        /// </summary>
        public void DelOrders(List<OrderEntity> orders) {
            SQLiteParameter[] parms = { new SQLiteParameter("@id", DbType.Int32) };

            using (var conn = new SQLiteConnection(registryConnectionString)) {
                conn.SetPassword(dbPassword);
                conn.Open();
                using (var command = new SQLiteCommand(SqliteCommands.Registry_Delete_Order, conn)) {
                    foreach (var order in orders) {
                        parms[0].Value = (int)order.Id;
                        command.Parameters.Clear();
                        command.Parameters.AddRange(parms);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Clean orders.
        /// </summary>
        public void CleanOrders() {
            using (var conn = new SQLiteConnection(registryConnectionString)) {
                conn.SetPassword(dbPassword);
                conn.Open();
                using (var command = new SQLiteCommand()) {
                    command.Connection = conn;
                    command.CommandText = SqliteCommands.Registry_Clean_Order;
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Get paramter entity
        /// </summary>
        public List<ParamEntity> GetParams() {
            var _params = new List<ParamEntity>();
            using (var conn = new SQLiteConnection(registryConnectionString)) {
                conn.SetPassword(dbPassword);
                conn.Open();
                using (var command = new SQLiteCommand(SqliteCommands.Registry_Get_Param, conn)) {
                    using (var rdr = command.ExecuteReader(CommandBehavior.CloseConnection)) {
                        while (rdr.Read()) {
                            var _param = new ParamEntity();
                            _param.Id = SqlTypeConverter.DBNullParamIdHandler(rdr["id"]);
                            _param.Value = SqlTypeConverter.DBNullStringHandler(rdr["value"]);
                            _param.Time = SqlTypeConverter.DBNullDateTimeHandler(rdr["time"]);
                            _params.Add(_param);
                        }
                    }
                }
            }
            return _params;
        }

        /// <summary>
        /// Save param entity.
        /// </summary>
        public void SaveParams(List<ParamEntity> _params) {
            SQLiteParameter[] parms = { new SQLiteParameter("@id", DbType.Int32),
                                        new SQLiteParameter("@value", DbType.String, 1024) };

            using (var conn = new SQLiteConnection(registryConnectionString)) {
                conn.SetPassword(dbPassword);
                conn.Open();
                using (var command = new SQLiteCommand(SqliteCommands.Registry_Save_Param, conn)) {
                    foreach (var _param in _params) {
                        parms[0].Value = (int)_param.Id;
                        parms[1].Value = SqlTypeConverter.DBNullStringChecker(_param.Value);
                        command.Parameters.Clear();
                        command.Parameters.AddRange(parms);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Delete param entity.
        /// </summary>
        public void DelParams(List<ParamEntity> _params) {
            SQLiteParameter[] parms = { new SQLiteParameter("@id", DbType.Int32) };

            using (var conn = new SQLiteConnection(registryConnectionString)) {
                conn.SetPassword(dbPassword);
                conn.Open();
                using (var command = new SQLiteCommand(SqliteCommands.Registry_Delete_Param, conn)) {
                    foreach (var _param in _params) {
                        parms[0].Value = (int)_param.Id;
                        command.Parameters.Clear();
                        command.Parameters.AddRange(parms);
                        command.ExecuteNonQuery();
                    }
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
    }
}
