using iPem.Data.Common;
using iPem.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text;

namespace iPem.Core {
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
        public DbEntity GetDatabase(DatabaseId id) {
            SQLiteParameter[] parms = { new SQLiteParameter("@id", DbType.Int32) };
            parms[0].Value = (Int32)id;

            DbEntity database = null;
            using(var conn = new SQLiteConnection(registryConnectionString)) {
                conn.SetPassword(dbPassword);
                conn.Open();
                using(var command = new SQLiteCommand(SqliteCommands.Registry_Get_Database, conn)) {
                    command.Parameters.AddRange(parms);
                    using(var rdr = command.ExecuteReader(CommandBehavior.CloseConnection)) {
                        if(rdr.Read()) {
                            database = new DbEntity();
                            database.Id = SqlTypeConverter.DBNullDatabaseIdHandler(rdr["id"]);
                            database.Type = SqlTypeConverter.DBNullDatabaseTypeHandler(rdr["type"]);
                            database.IP = SqlTypeConverter.DBNullStringHandler(rdr["ip"]);
                            database.Port = SqlTypeConverter.DBNullInt32Handler(rdr["port"]);
                            database.Uid = SqlTypeConverter.DBNullStringHandler(rdr["uid"]);
                            database.Password = SqlTypeConverter.DBNullStringHandler(rdr["password"]);
                            database.Name = SqlTypeConverter.DBNullStringHandler(rdr["name"]);
                        }
                    }
                }
            }
            return database;
        }

        /// <summary>
        /// Save database entity.
        /// </summary>
        /// <param name="database">database entity</param>
        public void SaveDatabase(DbEntity database) {
            SQLiteParameter[] parms = { new SQLiteParameter("@id", DbType.Int32),
                                        new SQLiteParameter("@type", DbType.Int32),
                                        new SQLiteParameter("@ip", DbType.String,128),
                                        new SQLiteParameter("@port", DbType.Int32),
                                        new SQLiteParameter("@uid", DbType.String,50),
                                        new SQLiteParameter("@password", DbType.String,50),
                                        new SQLiteParameter("@name", DbType.String,128)};

            using(var conn = new SQLiteConnection(registryConnectionString)) {
                conn.SetPassword(dbPassword);
                conn.Open();
                using(var command = new SQLiteCommand(SqliteCommands.Registry_Save_Database, conn)) {
                    parms[0].Value = (Int32)database.Id;
                    parms[1].Value = (Int32)database.Type;
                    parms[2].Value = SqlTypeConverter.DBNullStringChecker(database.IP);
                    parms[3].Value = SqlTypeConverter.DBNullInt32Checker(database.Port);
                    parms[4].Value = SqlTypeConverter.DBNullStringChecker(database.Uid);
                    parms[5].Value = SqlTypeConverter.DBNullStringChecker(database.Password);
                    parms[6].Value = SqlTypeConverter.DBNullStringChecker(database.Name);

                    command.Parameters.AddRange(parms);
                    command.ExecuteNonQuery();
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
                                Plan = SqlTypeConverter.DBNullPlanIdHandler(rdr["plan"]),
                                Last = SqlTypeConverter.DBNullDateTimeHandler(rdr["last"]),
                                Next = SqlTypeConverter.DBNullDateTimeHandler(rdr["next"]),
                                Order = SqlTypeConverter.DBNullInt32Handler(rdr["order"])
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
                                        new SQLiteParameter("@name", DbType.String,128),
                                        new SQLiteParameter("@plan", DbType.Int32),
                                        new SQLiteParameter("@last", DbType.DateTime),
                                        new SQLiteParameter("@next", DbType.DateTime),
                                        new SQLiteParameter("@order", DbType.Int32) };

            using(var conn = new SQLiteConnection(registryConnectionString)) {
                conn.SetPassword(dbPassword);
                conn.Open();
                using(var command = new SQLiteCommand(SqliteCommands.Registry_Save_Task, conn)) {
                    foreach(var task in tasks) {
                        parms[0].Value = SqlTypeConverter.DBNullStringChecker(task.Id);
                        parms[1].Value = SqlTypeConverter.DBNullStringChecker(task.Name);
                        parms[2].Value = (int)task.Plan;
                        parms[3].Value = SqlTypeConverter.DBNullDateTimeChecker(task.Last);
                        parms[4].Value = SqlTypeConverter.DBNullDateTimeChecker(task.Next);
                        parms[5].Value = task.Order;

                        command.Parameters.Clear();
                        command.Parameters.AddRange(parms);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Update a task
        /// </summary>
        /// <param name="task">task</param>
        public void UpdateTask(TaskEntity task) {
            SQLiteParameter[] parms = { new SQLiteParameter("@id", DbType.String,50),
                                        new SQLiteParameter("@last", DbType.DateTime),
                                        new SQLiteParameter("@next", DbType.DateTime)};

            using(var conn = new SQLiteConnection(registryConnectionString)) {
                conn.SetPassword(dbPassword);
                conn.Open();
                using(var command = new SQLiteCommand(SqliteCommands.Registry_Update_Task, conn)) {
                    parms[0].Value = SqlTypeConverter.DBNullStringChecker(task.Id);
                    parms[1].Value = SqlTypeConverter.DBNullDateTimeChecker(task.Last);
                    parms[2].Value = SqlTypeConverter.DBNullDateTimeChecker(task.Next);

                    command.Parameters.Clear();
                    command.Parameters.AddRange(parms);
                    command.ExecuteNonQuery();
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
        /// Get all plans.
        /// </summary>
        /// <returns>all plans</returns>
        public List<PlanEntity> GetPlans() {
            var parameters = new List<PlanEntity>();
            using(var conn = new SQLiteConnection(registryConnectionString)) {
                conn.SetPassword(dbPassword);
                conn.Open();
                using(var command = new SQLiteCommand(SqliteCommands.Registry_Get_Plans, conn)) {
                    using(var rdr = command.ExecuteReader(CommandBehavior.CloseConnection)) {
                        while(rdr.Read()) {
                            parameters.Add(new PlanEntity {
                                Id = SqlTypeConverter.DBNullPlanIdHandler(rdr["id"]),
                                Name = SqlTypeConverter.DBNullStringHandler(rdr["name"]),
                                Interval = SqlTypeConverter.DBNullInt64Handler(rdr["interval"]),
                                Unit = SqlTypeConverter.DBNullInt32Handler(rdr["unit"]),
                                Start = SqlTypeConverter.DBNullDateTimeHandler(rdr["start"]),
                                End = SqlTypeConverter.DBNullDateTimeHandler(rdr["end"])
                            });
                        }
                    }
                }
            }
            return parameters;
        }

        /// <summary>
        /// Save all plans.
        /// </summary>
        /// <param name="plans">all plans</param>
        public void SavePlans(List<PlanEntity> plans) {
            SQLiteParameter[] parms = { new SQLiteParameter("@id", DbType.Int32),
                                        new SQLiteParameter("@name", DbType.String),
                                        new SQLiteParameter("@interval", DbType.Int64),
                                        new SQLiteParameter("@unit", DbType.Int32),
                                        new SQLiteParameter("@start", DbType.DateTime),
                                        new SQLiteParameter("@end", DbType.DateTime)};

            using(var conn = new SQLiteConnection(registryConnectionString)) {
                conn.SetPassword(dbPassword);
                conn.Open();
                using(var command = new SQLiteCommand(SqliteCommands.Registry_Save_Plan, conn)) {
                    foreach(var plan in plans) {
                        parms[0].Value = (int)plan.Id;
                        parms[1].Value = SqlTypeConverter.DBNullStringChecker(plan.Name);
                        parms[2].Value = SqlTypeConverter.DBNullInt64Checker(plan.Interval);
                        parms[3].Value = SqlTypeConverter.DBNullInt32Checker(plan.Unit);
                        parms[4].Value = SqlTypeConverter.DBNullDateTimeChecker(plan.Start);
                        parms[5].Value = SqlTypeConverter.DBNullDateTimeChecker(plan.End);

                        command.Parameters.Clear();
                        command.Parameters.AddRange(parms);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Clean all plans.
        /// </summary>
        public void CleanPlans() {
            using(var conn = new SQLiteConnection(registryConnectionString)) {
                conn.SetPassword(dbPassword);
                conn.Open();
                using(var command = new SQLiteCommand()) {
                    command.Connection = conn;
                    command.CommandText = SqliteCommands.Registry_Clean_Plans;
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
