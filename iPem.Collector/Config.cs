using iPem.Core;
using iPem.Data;
using iPem.Data.Common;
using iPem.Model;
using iPem.Task;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace iPem.Collector {
    public partial class Config : Form {
        private TestStatus _testStatus = TestStatus.Default;

        public Config() {
            InitializeComponent();
        }

        private void Config_Load(object sender, EventArgs e) {
            try {
                databasetype_rs.ValueMember = "Id";
                databasetype_rs.DisplayMember = "Name";
                databasetype_rs.DataSource = CommonHelper.GetDbTypeStore();

                databasetype_cs.ValueMember = "Id";
                databasetype_cs.DisplayMember = "Name";
                databasetype_cs.DataSource = CommonHelper.GetDbTypeStore();

                databasetype_sc.ValueMember = "Id";
                databasetype_sc.DisplayMember = "Name";
                databasetype_sc.DataSource = CommonHelper.GetDbTypeStore();

                cb_interval_act.ValueMember = "Id";
                cb_interval_act.DisplayMember = "Name";
                cb_interval_act.DataSource = CommonHelper.GetIntervalStore();

                this.SetActTasks();
                this.SetHisTasks();
                this.SetActPlanComment();
                this.SetHisPlanComment();
                this.InitConfig();
            } catch(Exception err) {
                Logger.Error(err.Message, err);
                MessageBox.Show(err.Message, "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region events
        private void sysTabs_SelectedIndexChanged(object sender, EventArgs e) {
            testButton.Visible = sysTabs.SelectedIndex < 3;
        }

        private void interval_act_ValueChanged(object sender, EventArgs e) {
            this.SetActPlanComment();
        }

        private void cb_interval_act_SelectedIndexChanged(object sender, EventArgs e) {
            this.SetActPlanComment();
        }

        private void begin_act_ValueChanged(object sender, EventArgs e) {
            this.SetActPlanComment();
        }

        private void end_act_ValueChanged(object sender, EventArgs e) {
            this.SetActPlanComment();
        }

        private void interval_his_ValueChanged(object sender, EventArgs e) {
            this.SetHisPlanComment();
        }

        private void time_his_ValueChanged(object sender, EventArgs e) {
            this.SetHisPlanComment();
        }

        private void testButton_Click(object sender, EventArgs e) {
            try {
                if(sysTabs.SelectedIndex == 0) {
                    #region test rs
                    if(string.IsNullOrWhiteSpace(databaseip_rs.Text)) {
                        databaseip_rs.Focus();
                        MessageBox.Show("数据库地址不能为空", "资源数据库", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if(String.IsNullOrWhiteSpace(databasename_rs.Text)) {
                        databasename_rs.Focus();
                        MessageBox.Show("数据库名称不能为空", "资源数据库", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if(String.IsNullOrWhiteSpace(databaseuid_rs.Text)) {
                        databaseuid_rs.Focus();
                        MessageBox.Show("登录名不能为空", "资源数据库", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if(String.IsNullOrWhiteSpace(databasepwd_rs.Text)) {
                        databasepwd_rs.Focus();
                        MessageBox.Show("密码不能为空", "资源数据库", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    var database = new DbEntity {
                        Id = DatabaseId.Rs,
                        Type = SqlTypeConverter.DBNullDatabaseTypeHandler(databasetype_rs.SelectedValue),
                        IP = databaseip_rs.Text.Trim(),
                        Port = (int)databaseport_rs.Value,
                        Name = databasename_rs.Text.Trim(),
                        Uid = databaseuid_rs.Text.Trim(),
                        Password = databasepwd_rs.Text
                    };

                    sysTabs.Enabled = false;
                    testButton.Enabled = false;
                    YesButton.Enabled = false;
                    var connectionString = SqlTypeConverter.CreateConnectionString(database);
                    var testthread = new Thread(() => {
                        try {
                            var message = "";
                            var timeout = 30;
                            var thread = new Thread(() => {
                                using(var conn = new SqlConnection(connectionString)) {
                                    try {
                                        conn.Open();
                                        conn.Close();
                                        _testStatus = TestStatus.Success;
                                    } catch(Exception err) {
                                        _testStatus = TestStatus.Failure;
                                        message = err.Message;
                                    }
                                }
                            });
                            _testStatus = TestStatus.Testing;
                            thread.IsBackground = true;
                            thread.Start();

                            var sw = System.Diagnostics.Stopwatch.StartNew();
                            var ts = TimeSpan.FromSeconds(timeout);
                            while(sw.Elapsed < ts) {
                                thread.Join(TimeSpan.FromMilliseconds(500));
                                if(_testStatus != TestStatus.Testing) { break; }
                            }
                            sw.Stop();

                            if(_testStatus == TestStatus.Testing) {
                                MessageBox.Show("SQL Server服务器连接超时", "资源数据库", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            } else if(_testStatus == TestStatus.Success) {
                                MessageBox.Show("测试连接成功", "资源数据库", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            } else if(_testStatus == TestStatus.Failure) {
                                MessageBox.Show(message, "资源数据库", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        } catch(Exception err) {
                            MessageBox.Show(err.Message, "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        } finally {
                            this.Invoke(new MethodInvoker(delegate {
                                sysTabs.Enabled = true;
                                testButton.Enabled = true;
                                YesButton.Enabled = true;
                                NoButton.Enabled = true;
                            }));
                            _testStatus = TestStatus.Default;
                        }
                    });
                    testthread.IsBackground = true;
                    testthread.Start();
                    #endregion
                } else if(sysTabs.SelectedIndex == 1) {
                    #region test cs
                    if(string.IsNullOrWhiteSpace(databaseip_cs.Text)) {
                        databaseip_cs.Focus();
                        MessageBox.Show("数据库地址不能为空", "动环数据库", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if(String.IsNullOrWhiteSpace(databasename_cs.Text)) {
                        databasename_cs.Focus();
                        MessageBox.Show("数据库名称不能为空", "动环数据库", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if(String.IsNullOrWhiteSpace(databaseuid_cs.Text)) {
                        databaseuid_cs.Focus();
                        MessageBox.Show("登录名不能为空", "动环数据库", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if(String.IsNullOrWhiteSpace(databasepwd_cs.Text)) {
                        databasepwd_cs.Focus();
                        MessageBox.Show("密码不能为空", "动环数据库", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    var database = new DbEntity {
                        Id = DatabaseId.Cs,
                        Type = SqlTypeConverter.DBNullDatabaseTypeHandler(databasetype_cs.SelectedValue),
                        IP = databaseip_cs.Text.Trim(),
                        Port = (int)databaseport_cs.Value,
                        Name = databasename_cs.Text.Trim(),
                        Uid = databaseuid_cs.Text.Trim(),
                        Password = databasepwd_cs.Text
                    };

                    sysTabs.Enabled = false;
                    testButton.Enabled = false;
                    YesButton.Enabled = false;
                    var connectionString = SqlTypeConverter.CreateConnectionString(database);
                    var testthread = new Thread(() => {
                        try {
                            var message = "";
                            var timeout = 30;
                            var thread = new Thread(() => {
                                using(var conn = new SqlConnection(connectionString)) {
                                    try {
                                        conn.Open();
                                        conn.Close();
                                        _testStatus = TestStatus.Success;
                                    } catch(Exception err) {
                                        _testStatus = TestStatus.Failure;
                                        message = err.Message;
                                    }
                                }
                            });
                            _testStatus = TestStatus.Testing;
                            thread.IsBackground = true;
                            thread.Start();

                            var sw = System.Diagnostics.Stopwatch.StartNew();
                            var ts = TimeSpan.FromSeconds(timeout);
                            while(sw.Elapsed < ts) {
                                thread.Join(TimeSpan.FromMilliseconds(500));
                                if(_testStatus != TestStatus.Testing) { break; }
                            }
                            sw.Stop();

                            if(_testStatus == TestStatus.Testing) {
                                MessageBox.Show("SQL Server服务器连接超时", "动环数据库", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            } else if(_testStatus == TestStatus.Success) {
                                MessageBox.Show("测试连接成功", "动环数据库", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            } else if(_testStatus == TestStatus.Failure) {
                                MessageBox.Show(message, "动环数据库", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        } catch(Exception err) {
                            MessageBox.Show(err.Message, "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        } finally {
                            this.Invoke(new MethodInvoker(delegate {
                                sysTabs.Enabled = true;
                                testButton.Enabled = true;
                                YesButton.Enabled = true;
                                NoButton.Enabled = true;
                            }));
                            _testStatus = TestStatus.Default;
                        }
                    });
                    testthread.IsBackground = true;
                    testthread.Start();
                    #endregion
                } else if(sysTabs.SelectedIndex == 2) {
                    #region test sc
                    if(string.IsNullOrWhiteSpace(databaseip_sc.Text)) {
                        databaseip_sc.Focus();
                        MessageBox.Show("数据库地址不能为空", "应用数据库", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if(String.IsNullOrWhiteSpace(databasename_sc.Text)) {
                        databasename_sc.Focus();
                        MessageBox.Show("数据库名称不能为空", "应用数据库", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if(String.IsNullOrWhiteSpace(databaseuid_sc.Text)) {
                        databaseuid_sc.Focus();
                        MessageBox.Show("登录名不能为空", "应用数据库", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if(String.IsNullOrWhiteSpace(databasepwd_sc.Text)) {
                        databasepwd_sc.Focus();
                        MessageBox.Show("密码不能为空", "应用数据库", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    var database = new DbEntity {
                        Id = DatabaseId.Sc,
                        Type = SqlTypeConverter.DBNullDatabaseTypeHandler(databasetype_sc.SelectedValue),
                        IP = databaseip_sc.Text.Trim(),
                        Port = (int)databaseport_sc.Value,
                        Name = databasename_sc.Text.Trim(),
                        Uid = databaseuid_sc.Text.Trim(),
                        Password = databasepwd_sc.Text
                    };

                    sysTabs.Enabled = false;
                    testButton.Enabled = false;
                    YesButton.Enabled = false;
                    var connectionString = SqlTypeConverter.CreateConnectionString(database);
                    var testthread = new Thread(() => {
                        try {
                            var message = "";
                            var timeout = 30;
                            var thread = new Thread(() => {
                                using(var conn = new SqlConnection(connectionString)) {
                                    try {
                                        conn.Open();
                                        conn.Close();
                                        _testStatus = TestStatus.Success;
                                    } catch(Exception err) {
                                        _testStatus = TestStatus.Failure;
                                        message = err.Message;
                                    }
                                }
                            });
                            _testStatus = TestStatus.Testing;
                            thread.IsBackground = true;
                            thread.Start();

                            var sw = System.Diagnostics.Stopwatch.StartNew();
                            var ts = TimeSpan.FromSeconds(timeout);
                            while(sw.Elapsed < ts) {
                                thread.Join(TimeSpan.FromMilliseconds(500));
                                if(_testStatus != TestStatus.Testing) { break; }
                            }
                            sw.Stop();

                            if(_testStatus == TestStatus.Testing) {
                                MessageBox.Show("SQL Server服务器连接超时", "应用数据库", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            } else if(_testStatus == TestStatus.Success) {
                                MessageBox.Show("测试连接成功", "应用数据库", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            } else if(_testStatus == TestStatus.Failure) {
                                MessageBox.Show(message, "应用数据库", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        } catch(Exception err) {
                            MessageBox.Show(err.Message, "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        } finally {
                            this.Invoke(new MethodInvoker(delegate {
                                sysTabs.Enabled = true;
                                testButton.Enabled = true;
                                YesButton.Enabled = true;
                                NoButton.Enabled = true;
                            }));
                            _testStatus = TestStatus.Default;
                        }
                    });
                    testthread.IsBackground = true;
                    testthread.Start();
                    #endregion
                }
            } catch(Exception err) {
                Logger.Error(err.Message, err);
                MessageBox.Show(err.Message, "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void NoButton_Click(object sender, EventArgs e) {
            try {
                if(_testStatus != TestStatus.Default) {
                    NoButton.Enabled = false;
                    _testStatus = TestStatus.Stop;
                } else {
                    this.DialogResult = DialogResult.Cancel;
                }
            } catch(Exception err) {
                Logger.Error(err.Message, err);
                MessageBox.Show(err.Message, "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void YesButton_Click(object sender, EventArgs e) {
            try {
                #region validate rs
                if(string.IsNullOrWhiteSpace(databaseip_rs.Text)) {
                    if(sysTabs.SelectedIndex != 0) { sysTabs.SelectedIndex = 0; }
                    databaseip_rs.Focus();
                    MessageBox.Show("数据库地址不能为空", "资源数据库", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if(string.IsNullOrWhiteSpace(databasename_rs.Text)) {
                    if(sysTabs.SelectedIndex != 0) { sysTabs.SelectedIndex = 0; }
                    databasename_rs.Focus();
                    MessageBox.Show("数据库名称不能为空", "资源数据库", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if(string.IsNullOrWhiteSpace(databaseuid_rs.Text)) {
                    if(sysTabs.SelectedIndex != 0) { sysTabs.SelectedIndex = 0; }
                    databaseuid_rs.Focus();
                    MessageBox.Show("登录名不能为空", "资源数据库", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if(string.IsNullOrWhiteSpace(databasepwd_rs.Text)) {
                    if(sysTabs.SelectedIndex != 0) { sysTabs.SelectedIndex = 0; }
                    databasepwd_rs.Focus();
                    MessageBox.Show("密码不能为空", "资源数据库", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                #endregion

                #region validate cs
                if(string.IsNullOrWhiteSpace(databaseip_cs.Text)) {
                    if(sysTabs.SelectedIndex != 1) { sysTabs.SelectedIndex = 1; }
                    databaseip_cs.Focus();
                    MessageBox.Show("数据库地址不能为空", "动环数据库", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if(string.IsNullOrWhiteSpace(databasename_cs.Text)) {
                    if(sysTabs.SelectedIndex != 1) { sysTabs.SelectedIndex = 1; }
                    databasename_cs.Focus();
                    MessageBox.Show("数据库名称不能为空", "动环数据库", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if(string.IsNullOrWhiteSpace(databaseuid_cs.Text)) {
                    if(sysTabs.SelectedIndex != 1) { sysTabs.SelectedIndex = 1; }
                    databaseuid_cs.Focus();
                    MessageBox.Show("登录名不能为空", "动环数据库", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if(string.IsNullOrWhiteSpace(databasepwd_cs.Text)) {
                    if(sysTabs.SelectedIndex != 1) { sysTabs.SelectedIndex = 1; }
                    databasepwd_cs.Focus();
                    MessageBox.Show("密码不能为空", "动环数据库", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                #endregion

                #region validate sc
                if(string.IsNullOrWhiteSpace(databaseip_sc.Text)) {
                    if(sysTabs.SelectedIndex != 2) { sysTabs.SelectedIndex = 2; }
                    databaseip_sc.Focus();
                    MessageBox.Show("数据库地址不能为空", "应用数据库", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if(string.IsNullOrWhiteSpace(databasename_sc.Text)) {
                    if(sysTabs.SelectedIndex != 2) { sysTabs.SelectedIndex = 2; }
                    databasename_sc.Focus();
                    MessageBox.Show("数据库名称不能为空", "应用数据库", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if(string.IsNullOrWhiteSpace(databaseuid_sc.Text)) {
                    if(sysTabs.SelectedIndex != 2) { sysTabs.SelectedIndex = 2; }
                    databaseuid_sc.Focus();
                    MessageBox.Show("登录名不能为空", "应用数据库", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if(string.IsNullOrWhiteSpace(databasepwd_sc.Text)) {
                    if(sysTabs.SelectedIndex != 2) { sysTabs.SelectedIndex = 2; }
                    databasepwd_sc.Focus();
                    MessageBox.Show("密码不能为空", "应用数据库", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                #endregion

                var taskCollection = new List<TaskEntity>();
                foreach(ListItem<string> task in actasks.CheckedItems) {
                    taskCollection.Add(new TaskEntity {
                        Id = task.Id,
                        Name = task.Text,
                        Plan = PlanId.Act,
                        Last = new DateTime(2016, 1, 1),
                        Next = new DateTime(2016, 1, 1),
                        Index = task.Index
                    });
                }

                foreach(ListItem<string> task in histasks.CheckedItems) {
                    taskCollection.Add(new TaskEntity {
                        Id = task.Id,
                        Name = task.Text,
                        Plan = PlanId.His,
                        Last = new DateTime(2016, 1, 1),
                        Next = new DateTime(2016, 1, 1),
                        Index = task.Index
                    });
                }

                if(taskCollection.Count == 0) {
                    if(sysTabs.SelectedIndex != 3) { sysTabs.SelectedIndex = 3; }
                    MessageBox.Show("必须选择一项计划任务", "任务列表", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if(begin_act.Value >= end_act.Value) {
                    if(sysTabs.SelectedIndex != 4) { sysTabs.SelectedIndex = 4; }
                    begin_act.Focus();
                    MessageBox.Show("执行时段：开始时间必须小于结束时间", "实时任务计划", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var rsDb = new DbEntity {
                    Id = DatabaseId.Rs,
                    Type = SqlTypeConverter.DBNullDatabaseTypeHandler(databasetype_rs.SelectedValue),
                    IP = databaseip_rs.Text.Trim(),
                    Port = (Int32)databaseport_rs.Value,
                    Name = databasename_rs.Text.Trim(),
                    Uid = databaseuid_rs.Text.Trim(),
                    Password = databasepwd_rs.Text
                };

                var csDb = new DbEntity {
                    Id = DatabaseId.Cs,
                    Type = SqlTypeConverter.DBNullDatabaseTypeHandler(databasetype_cs.SelectedValue),
                    IP = databaseip_cs.Text.Trim(),
                    Port = (Int32)databaseport_cs.Value,
                    Name = databasename_cs.Text.Trim(),
                    Uid = databaseuid_cs.Text.Trim(),
                    Password = databasepwd_cs.Text
                };

                var scDb = new DbEntity {
                    Id = DatabaseId.Sc,
                    Type = SqlTypeConverter.DBNullDatabaseTypeHandler(databasetype_sc.SelectedValue),
                    IP = databaseip_sc.Text.Trim(),
                    Port = (Int32)databaseport_sc.Value,
                    Name = databasename_sc.Text.Trim(),
                    Uid = databaseuid_sc.Text.Trim(),
                    Password = databasepwd_sc.Text
                };

                var planCollection = new List<ParamEntity>();
                planCollection.Add(new ParamEntity {
                    Id = PlanId.Act,
                    Name = "实时任务计划",
                    Interval = (long)interval_act.Value,
                    Unit = (int)cb_interval_act.SelectedValue,
                    Start = begin_act.Value,
                    End = end_act.Value
                });

                planCollection.Add(new ParamEntity {
                    Id = PlanId.His,
                    Name = "历史任务计划",
                    Interval = (long)interval_his.Value,
                    Unit = 24 * 3600,
                    Start = time_his.Value,
                    End = new DateTime(2099, 12, 31)
                });

                var _registry = new Registry(Application.StartupPath);
                var _tasks = _registry.GetTasks();
                foreach(var task in taskCollection) {
                    var _task = _tasks.Find(t => t.Id == task.Id && t.Plan == task.Plan);
                    if(_task == null) continue;
                    task.Last = _task.Last;
                    task.Next = _task.Next;
                }

                _registry.CleanDatabases();
                _registry.SaveDatabase(rsDb);
                _registry.SaveDatabase(csDb);
                _registry.SaveDatabase(scDb);
                _registry.CleanTasks();
                _registry.SaveTasks(taskCollection);
                _registry.CleanPlans();
                _registry.SavePlans(planCollection);
                MessageBox.Show("数据保存完成", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } catch(Exception err) {
                Logger.Error(err.Message, err);
                MessageBox.Show(err.Message, "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region custom
        private void SetActTasks() {
            var _data = new List<ListItem<string>>();
            foreach(var _class in Common.GetActTasks().OrderBy(d => d.Order)) {
                _data.Add(new ListItem<string> { Index = _class.Order, Id = _class.Id, Text = _class.Name });
            }

            actasks.DataSource = _data;
            actasks.ValueMember = "Id";
            actasks.DisplayMember = "Text";
        }

        private void SetHisTasks() {
            var _data = new List<ListItem<string>>();
            foreach(var _class in Common.GetHisTasks().OrderBy(d => d.Order)) {
                _data.Add(new ListItem<string> { Index = _class.Order, Id = _class.Id, Text = _class.Name });
            }

            histasks.DataSource = _data;
            histasks.ValueMember = "Id";
            histasks.DisplayMember = "Text";
        }

        private void SetActPlanComment() {
            comment_act.Text = string.Format("计划将在{0}至{1}时段内，每{2}{3}执行一次。", CommonHelper.ToTimeString(begin_act.Value), CommonHelper.ToTimeString(end_act.Value), interval_act.Value, cb_interval_act.Text.Replace("/次", ""));
        }

        private void SetHisPlanComment() {
            comment_his.Text = string.Format("计划将每{0}天执行一次，在{1}执行。", interval_his.Value, CommonHelper.ToTimeString(time_his.Value));
        }

        private void InitConfig() {
            var _registry = new Registry(Application.StartupPath);
            var _rsDb = _registry.GetDatabase(DatabaseId.Rs);
            if(_rsDb != null) {
                databasetype_rs.SelectedValue = (int)_rsDb.Type;
                databaseip_rs.Text = _rsDb.IP;
                databaseport_rs.Value = _rsDb.Port;
                databasename_rs.Text = _rsDb.Name;
                databaseuid_rs.Text = _rsDb.Uid;
                databasepwd_rs.Text = _rsDb.Password;
            }

            var _csDb = _registry.GetDatabase(DatabaseId.Cs);
            if(_csDb != null) {
                databasetype_cs.SelectedValue = (int)_csDb.Type;
                databaseip_cs.Text = _csDb.IP;
                databaseport_cs.Value = _csDb.Port;
                databasename_cs.Text = _csDb.Name;
                databaseuid_cs.Text = _csDb.Uid;
                databasepwd_cs.Text = _csDb.Password;
            }

            var _scDb = _registry.GetDatabase(DatabaseId.Sc);
            if(_scDb != null) {
                databasetype_sc.SelectedValue = (int)_scDb.Type;
                databaseip_sc.Text = _scDb.IP;
                databaseport_sc.Value = _scDb.Port;
                databasename_sc.Text = _scDb.Name;
                databaseuid_sc.Text = _scDb.Uid;
                databasepwd_sc.Text = _scDb.Password;
            }

            var tasks = _registry.GetTasks();
            for (int i = 0; i < actasks.Items.Count; i++) {
                var item = actasks.Items[i] as ListItem<string>;
                var check = tasks.Any(t=>t.Plan == PlanId.Act && t.Id == item.Id);
			    actasks.SetItemChecked(i, check);
		    }
            
            for (int i = 0; i < histasks.Items.Count; i++) {
                var item = histasks.Items[i] as ListItem<string>;
                var check = tasks.Any(t=>t.Plan == PlanId.His && t.Id == item.Id);
			    histasks.SetItemChecked(i, check);
		    }

            var plans = _registry.GetPlans();
            foreach(var plan in plans) {
                if(plan.Id == PlanId.Act) {
                    interval_act.Value = plan.Interval;
                    cb_interval_act.SelectedValue = plan.Unit;
                    begin_act.Value = plan.Start;
                    end_act.Value = plan.End;
                } else if(plan.Id == PlanId.His) {
                    interval_his.Value = plan.Interval;
                    time_his.Value = plan.Start;
                }
            }
        }
        #endregion
    }
}
