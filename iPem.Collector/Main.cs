using iPem.Core;
using iPem.Data;
using iPem.Data.Common;
using iPem.Model;
using iPem.Task;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace iPem.Collector {
    public partial class Main : Form {
        private RunStatus _runStatus;
        private Registry _registry;
        private List<IActTask> _actAllTasks;
        private List<IHisTask> _hisAllTasks;
        private List<IActTask> _actAutoTasks;
        private List<IHisTask> _hisAutoTasks;
        private Thread _workerThread;
        private List<Thread> _workerThreads;
        private Queue<Event> _eventQueue;
        private EventWaitHandle _allDone;

        private LogicTypeRepository _logicTypeRepository;
        private DeviceTypeRepository _deviceTypeRepository;
        private RoomTypeRepository _roomTypeRepository;
        private StationTypeRepository _stationTypeRepository;
        private EnumMethodsRepository _enumMethodsRepository;
        private PointRepository _pointRepository;
        private ProtocolRepository _protocolRepository;
        private DeviceRepository _deviceRepository;
        private FsuRepository _fsuRepository;
        private RoomRepository _roomRepository;
        private StationRepository _stationRepository;
        private AreaRepository _areaRepository;

        public Main() {
            InitializeComponent();
            this.Text = iPemStore.Name;
            this.sysNotifyIcon.Text = iPemStore.Name;
        }

        private void Main_Load(object sender, EventArgs e) {
            try {
                Logger.Information("#############################################");
                Logger.Information(String.Format("启动应用程序\"{0} {1}\"", iPemStore.Name, iPemStore.Version));

                _registry = new Registry(Application.StartupPath);
                _registry.CreateRegistry();

                _actAllTasks = Common.GetActTasks().OrderBy(d => d.Order).ToList();
                _hisAllTasks = Common.GetHisTasks().OrderBy(d => d.Order).ToList();
                this.SetActTasks();
                this.SetHisTasks();
            } catch(Exception err) {
                Logger.Error(err.Message, err);
                MessageBox.Show(err.Message, "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Start() {
            try {
                var _rsDbEntity = _registry.GetDatabase(DatabaseId.Rs);
                if(_rsDbEntity == null) throw new Exception("资源数据库未配置");
                SqlHelper.ConnectionStringRsTransaction = SqlTypeConverter.CreateConnectionString(_rsDbEntity);

                var _csDbEntity = _registry.GetDatabase(DatabaseId.Cs);
                if(_csDbEntity == null) throw new Exception("动环数据库未配置");
                SqlHelper.ConnectionStringCsTransaction = SqlTypeConverter.CreateConnectionString(_csDbEntity);

                var _scDbEntity = _registry.GetDatabase(DatabaseId.Sc);
                if(_scDbEntity == null) throw new Exception("应用数据库未配置");
                SqlHelper.ConnectionStringScTransaction = SqlTypeConverter.CreateConnectionString(_scDbEntity);

                var _taskEntities = _registry.GetTasks();
                if(_taskEntities.Count == 0) throw new Exception("计划任务未选择");

                var _planEntities = _registry.GetPlans();
                if(_planEntities.Count == 0) throw new Exception("任务计划未配置");

                //global
                _runStatus = RunStatus.Default;
                _actAutoTasks = new List<IActTask>();
                _hisAutoTasks = new List<IHisTask>();
                _workerThreads = new List<Thread>();
                _eventQueue = new Queue<Event>();
                _allDone = new EventWaitHandle(false, EventResetMode.ManualReset);

                _logicTypeRepository = new LogicTypeRepository();
                _deviceTypeRepository = new DeviceTypeRepository();
                _roomTypeRepository = new RoomTypeRepository();
                _stationTypeRepository = new StationTypeRepository();
                _enumMethodsRepository = new EnumMethodsRepository();
                _pointRepository = new PointRepository();
                _protocolRepository = new ProtocolRepository();
                _deviceRepository = new DeviceRepository();
                _fsuRepository = new FsuRepository();
                _roomRepository = new RoomRepository();
                _stationRepository = new StationRepository();
                _areaRepository = new AreaRepository();

                //tasks
                var _today = DateTime.Today;
                var _now = DateTime.Now;
                foreach(var _class in Common.GetActTasks()) {
                    var task = _taskEntities.Find(t => t.Plan == PlanId.Act && t.Id == _class.Id);
                    if(task == null) continue;
                    var plan = _planEntities.Find(p => p.Id == task.Plan);
                    if(plan == null) continue;
                    _class.Seconds = plan.Interval * plan.Unit;
                    _class.Start = plan.Start;
                    _class.End = plan.End;
                    _class.Last = task.Last < _today ? _today : task.Last;
                    _class.Next = task.Next < _now ? _now : task.Next;
                    _class.Events = new List<Event>();
                    _actAutoTasks.Add(_class);
                }

                foreach(var _class in Common.GetHisTasks()) {
                    var task = _taskEntities.Find(t => t.Plan == PlanId.His && t.Id == _class.Id);
                    if(task == null) continue;
                    var plan = _planEntities.Find(p => p.Id == task.Plan);
                    if(plan == null) continue;
                    _class.Seconds = plan.Interval * plan.Unit;
                    _class.Time = plan.Start;
                    _class.Last = task.Last < _today.AddDays(-30) ? _today.AddDays(-30) : task.Last;
                    _class.Next = task.Next < _now ? _now : task.Next;
                    _class.Events = new List<Event>();
                    _hisAutoTasks.Add(_class);
                }

                try {
                    svrStatus.Text = "启动服务";
                    this.Write("启动数据处理服务...");
                    Logger.Information("启动数据处理服务...");
                    sysMenu0101.Enabled = false;
                    sysMenu0103.Enabled = false;
                    notifyMenu01.Enabled = false;
                    notifyMenu03.Enabled = false;
                    shortcutStart.Enabled = false;
                    shortcutConfig.Enabled = false;
                    actDoTask.Enabled = false;
                    hisDoTask.Enabled = false;

                    //初始化全局变量
                    _allDone.Reset();
                    _runStatus = RunStatus.Default;

                    //创建线程初始化数据
                    _workerThreads = new List<Thread>();
                    _workerThread = new Thread(new ThreadStart(DoInit));
                    _workerThread.IsBackground = true;
                    _workerThread.Start();
                    _workerThreads.Add(_workerThread);
                    this.Write("创建数据初始化线程。");
                    Logger.Information("创建数据初始化线程。");

                    //创建线程处理实时任务
                    _workerThread = new Thread(new ThreadStart(DoActTasks));
                    _workerThread.IsBackground = true;
                    _workerThread.Start();
                    _workerThreads.Add(_workerThread);
                    this.Write("创建实时任务处理线程。");
                    Logger.Information("创建实时任务处理线程。");

                    //创建线程处理历史任务
                    _workerThread = new Thread(new ThreadStart(DoHisTasks));
                    _workerThread.IsBackground = true;
                    _workerThread.Start();
                    _workerThreads.Add(_workerThread);
                    this.Write("创建历史任务处理线程。");
                    Logger.Information("创建历史任务处理线程。");

                    //创建线程处理日志数据包
                    _workerThread = new Thread(new ThreadStart(DoEvents));
                    _workerThread.IsBackground = true;
                    _workerThread.Start();
                    _workerThreads.Add(_workerThread);
                    this.Write("创建日志处理线程。");
                    Logger.Information("创建日志处理线程。");

                    _runStatus = RunStatus.Init;
                } catch(Exception err) {
                    this.Write(err.Message);
                    Logger.Error(err.Message, err);
                } finally {
                    sysMenu0102.Enabled = true;
                    shortcutStop.Enabled = true;
                    notifyMenu02.Enabled = true;
                }
            } catch(Exception err) {
                svrStatus.Text = "启动服务时发生错误";
                this.Write(err.Message);
                Logger.Error(err.Message, err);
                MessageBox.Show(err.Message, "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Stop() {
            try {
                svrStatus.Text = "停止服务";
                this.Write("停止数据处理服务...");
                Logger.Information("停止数据处理服务...");
                sysMenu0102.Enabled = false;
                shortcutStop.Enabled = false;
                notifyMenu02.Enabled = false;
                actDoTask.Enabled = false;
                hisDoTask.Enabled = false;
                _runStatus = RunStatus.Stop;
                _allDone.Set();

                foreach(var workerThread in this._workerThreads) {
                    if(workerThread != null && workerThread.IsAlive) {
                        workerThread.Join(5000);
                    }
                }

                foreach(var task in _actAutoTasks) {
                    _registry.UpdateTask(new TaskEntity {
                        Id = task.Id,
                        Last = task.Last,
                        Next = task.Next
                    });
                }

                foreach(var task in _hisAutoTasks) {
                    _registry.UpdateTask(new TaskEntity {
                        Id = task.Id,
                        Last = task.Last,
                        Next = task.Next
                    });
                }

                sysMenu0101.Enabled = true;
                sysMenu0103.Enabled = true;
                notifyMenu01.Enabled = true;
                notifyMenu03.Enabled = true;
                shortcutStart.Enabled = true;
                shortcutConfig.Enabled = true;
                svrStatus.Text = "就绪";
                this.Write("数据处理服务已停止");
                Logger.Information("数据处理服务已停止");
            } catch(Exception err) {
                svrStatus.Text = "停止服务时发生错误";
                this.Write(err.Message);
                Logger.Error(err.Message, err);
                MessageBox.Show(err.Message, "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } finally {
                lock(_eventQueue) {
                    while(_eventQueue.Count > 0) {
                        var v = _eventQueue.Dequeue();
                        this.Write(v.Message);
                        Logger.Write(v);
                    }
                }
            }
        }

        private void Quit() {
            if(MessageBox.Show("您确定要退出系统吗？", "确认对话框", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK) {
                if(_runStatus != RunStatus.Default && _runStatus != RunStatus.Stop)
                    this.Stop();

                Logger.Information(string.Format("应用程序已退出{0}", Environment.NewLine));
                Application.Exit();
            }
        }

        private void DoInit() {
            while(_runStatus < RunStatus.Running) {
                try {
                    if(_runStatus == RunStatus.Init) {
                        this.Write("数据初始化...");
                        Logger.Information("数据初始化...");
                        this.Invoke(new MethodInvoker(delegate {
                            svrStatus.Text = "加载数据";
                        }));
                        
                        //TODO:数据初始化
                        _actAutoTasks = _actAutoTasks.OrderBy(t => t.Order).ToList();
                        _hisAutoTasks = _hisAutoTasks.OrderBy(t => t.Order).ToList();

                        //初始化配置
                        iPemWorkContext.LogicTypes = _logicTypeRepository.GetEntities();
                        iPemWorkContext.SubLogicTypes = _logicTypeRepository.GetSubEntities();
                        iPemWorkContext.DeviceTypes = _deviceTypeRepository.GetEntities();
                        iPemWorkContext.SubDeviceTypes = _deviceTypeRepository.GetSubEntities();
                        iPemWorkContext.RoomTypes = _roomTypeRepository.GetEntities();
                        iPemWorkContext.StationTypes = _stationTypeRepository.GetEntities();
                        iPemWorkContext.AreaTypes = _enumMethodsRepository.GetEntities(EnmMethodType.Area, "类型");
                        iPemWorkContext.Points = _pointRepository.GetEntities();
                        iPemWorkContext.Protocols = new List<WcProtocol>();
                        iPemWorkContext.Devices = new List<WcDevice>();
                        iPemWorkContext.Fsus = new List<WcFsu>();
                        iPemWorkContext.Rooms = new List<WcRoom>();
                        iPemWorkContext.Stations = new List<WcStation>();
                        iPemWorkContext.Areas = new List<WcArea>();

                        //protocols
                        if(iPemWorkContext.Protocols != null) {
                            var _protocols = _protocolRepository.GetEntities();
                            foreach(var _protocol in _protocols) {
                                var _points = _pointRepository.GetEntitiesByProtocol(_protocol.Id);
                                iPemWorkContext.Protocols.Add(new WcProtocol {
                                    Current = _protocol,
                                    Points = _points
                                });
                            }
                        }

                        //devices
                        if(iPemWorkContext.Devices != null) {
                            var _devices = _deviceRepository.GetEntities();
                            iPemWorkContext.Devices.AddRange(
                            from _device in _devices
                            join _protocol in iPemWorkContext.Protocols on _device.ProtocolId equals _protocol.Current.Id
                            select new WcDevice { Current = _device, Protocol = _protocol });
                        }
                        
                        //fsus
                        if(iPemWorkContext.Fsus != null) {
                            var _fsus = _fsuRepository.GetEntities();
                            var _devsets = from _device in iPemWorkContext.Devices
                                           group _device by _device.Current.FsuId into g
                                           select new { Id = g.Key, Devices = g };

                            iPemWorkContext.Fsus.AddRange(
                            from _fsu in _fsus
                            join _devset in _devsets on _fsu.Id equals _devset.Id into lt
                            from def in lt.DefaultIfEmpty()
                            select new WcFsu { Current = _fsu, Devices = def != null ? def.Devices.ToList() : new List<WcDevice>() });
                        }
                        
                        //rooms
                        if(iPemWorkContext.Rooms != null) {
                            var _rooms = _roomRepository.GetEntities();
                            var _devsets = from _device in iPemWorkContext.Devices
                                           group _device by _device.Current.RoomId into g
                                           select new { Id = g.Key, Devices = g };

                            var _fsusets = from _fsu in iPemWorkContext.Fsus
                                           group _fsu by _fsu.Current.RoomId into g
                                           select new { Id = g.Key, Fsus = g };

                            iPemWorkContext.Rooms.AddRange(
                            from _room in _rooms
                            join _devset in _devsets on _room.Id equals _devset.Id into lt1
                            from def1 in lt1.DefaultIfEmpty()
                            join _fsuset in _fsusets on _room.Id equals _fsuset.Id into lt2
                            from def2 in lt2.DefaultIfEmpty()
                            select new WcRoom { Current = _room, Devices = def1 != null ? def1.Devices.ToList() : new List<WcDevice>(), Fsus = def2 != null ? def2.Fsus.ToList() : new List<WcFsu>() });
                        }

                        //stations
                        if(iPemWorkContext.Stations != null) {
                            var _stations = _stationRepository.GetEntities();
                            var _roomsets = from _room in iPemWorkContext.Rooms
                                            group _room by _room.Current.StationId into g
                                            select new { Id = g.Key, Rooms = g };

                            iPemWorkContext.Stations.AddRange(
                            from _station in _stations
                            join _roomset in _roomsets on _station.Id equals _roomset.Id into lt
                            from def in lt.DefaultIfEmpty()
                            select new WcStation { Current = _station, Rooms = def != null ? def.Rooms.ToList() : new List<WcRoom>() });
                        }

                        //areas
                        if(iPemWorkContext.Areas != null) {
                            var _areas = _areaRepository.GetEntities();
                            var stationsets = from _station in iPemWorkContext.Stations
                                              group _station by _station.Current.AreaId into g
                                              select new { Id = g.Key, Stations = g };

                            iPemWorkContext.Areas.AddRange(
                            from _area in _areas
                            join stationset in stationsets on _area.Id equals stationset.Id into lt
                            from def in lt.DefaultIfEmpty()
                            select new WcArea { Current = _area, Stations = def != null ? def.Stations.ToList() : new List<WcStation>() });

                            foreach(var current in iPemWorkContext.Areas) {
                                current.Initializer(iPemWorkContext.Areas);
                            }
                        }

                        this.Invoke(new MethodInvoker(delegate {
                            actDoTask.Enabled = true;
                            hisDoTask.Enabled = true;
                            svrStatus.Text = "正在运行";
                        }));

                        this.Write("数据初始化完成");
                        Logger.Information("数据初始化完成");
                        _runStatus = RunStatus.Running;
                        _allDone.Set();
                        break;
                    }

                    Thread.Sleep(1000);
                } catch(Exception err) {
                    this.Invoke(new MethodInvoker(delegate {
                        svrStatus.Text = "加载数据时发生错误";
                    }));
                    Logger.Error(err.Message, err);
                    Thread.Sleep(60000);
                }
            }
        }

        private void DoActTasks() {
            _allDone.WaitOne();
            this.Information(string.Format("实时任务数量: {0}。", _actAutoTasks.Count));
            while(_runStatus != RunStatus.Stop) {
                if(_runStatus == RunStatus.Running) {
                    foreach(var task in _actAutoTasks) {
                        var now = DateTime.Now;
                        if(task.Next > now) continue;
                        var timeRangs = now.Hour * 3600 + now.Minute * 60 + now.Second;
                        var timeRangs1 = task.Start.Hour * 3600 + task.Start.Minute * 60 + task.Start.Second;
                        var timeRangs2 = task.End.Hour * 3600 + task.End.Minute * 60 + task.End.Second;
                        if(timeRangs < timeRangs1 || timeRangs > timeRangs2) continue;
                        try {
                            this.Information(string.Format("\"{0}\"开始执行...", task.Name));
                            task.Execute();
                            this.Information(string.Format("\"{0}\"执行成功。", task.Name));
                        } catch(Exception err) {
                            this.Error(string.Format("{0}: {1}", task.Name, err.Message), err);
                            if(task.Events.Count > 0) this.Event(task.Events);
                        } finally {
                            task.Events.Clear();
                            task.Last = task.Next;
                            task.Next = DateTime.Now.AddSeconds(task.Seconds);
                        }
                    }
                }

                Thread.Sleep(500);
            }
        }

        private void DoHisTasks() {
            _allDone.WaitOne();
            this.Information(string.Format("历史任务数量: {0}。", _hisAutoTasks.Count));
            while(_runStatus != RunStatus.Stop) {
                if(_runStatus == RunStatus.Running) {
                    foreach(var task in _hisAutoTasks) {
                        var now = DateTime.Now;
                        if(task.Next > now) continue;
                        var timeRangs = task.Time.Hour * 3600 + task.Time.Minute * 60 + task.Time.Second;
                        try {
                            this.Information(string.Format("\"{0}\"开始执行...", task.Name));
                            task.Execute();
                            this.Information(string.Format("\"{0}\"执行成功。", task.Name));
                        } catch(Exception err) {
                            this.Error(string.Format("{0}: {1}", task.Name, err.Message), err);
                            if(task.Events.Count > 0) this.Event(task.Events);
                        } finally {
                            task.Events.Clear();
                            task.Last = task.Next;
                            task.Next = DateTime.Today.AddSeconds(task.Seconds + timeRangs);
                        }
                    }
                }

                Thread.Sleep(500);
            }
        }

        private void DoEvents() {
            _allDone.WaitOne();
            var _events = new List<Event>();
            while(_runStatus != RunStatus.Stop) {
                if(_runStatus == RunStatus.Running) {
                    try {
                        lock(_eventQueue) {
                            while(_eventQueue.Count > 0) {
                                _events.Add(_eventQueue.Dequeue());
                            }
                        }

                        foreach(var v in _events)
                            Logger.Write(v);
                    } catch { } finally {
                        if(_events.Count > 0)
                            _events.Clear();
                    }
                }

                Thread.Sleep(1000);
            }
        }

        private void Event(List<Event> events) {
            lock(_eventQueue) {
                foreach(var e in events) {
                    _eventQueue.Enqueue(e);
                }
            }
        }

        private void Error(string message, Exception exception = null) {
            lock(_eventQueue) {
                _eventQueue.Enqueue(new Event {
                    Id = Guid.NewGuid(),
                    Type = EventType.Error,
                    Time = DateTime.Now,
                    Message = message,
                    FullMessage = exception == null ? "" : exception.StackTrace
                });
            }

            this.Write(message);
        }

        private void Warning(string message, Exception exception = null) {
            lock(_eventQueue) {
                _eventQueue.Enqueue(new Event {
                    Id = Guid.NewGuid(),
                    Type = EventType.Warning,
                    Time = DateTime.Now,
                    Message = message,
                    FullMessage = exception == null ? "" : exception.StackTrace
                });
            }

            this.Write(message);
        }

        private void Information(string message, Exception exception = null) {
            lock(_eventQueue) {
                _eventQueue.Enqueue(new Event {
                    Id = Guid.NewGuid(),
                    Type = EventType.Info,
                    Time = DateTime.Now,
                    Message = message,
                    FullMessage = exception == null ? "" : exception.StackTrace
                });
            }

            this.Write(message);
        }

        private void Write(string message) {
            try {
                var max = 1000;
                var line = string.Format("{0}  {1}{2}", DateTime.Now.ToString("MM/dd HH:mm:ss"), message, Environment.NewLine);
                if(this.InvokeRequired) {
                    this.Invoke(new MethodInvoker(delegate {
                        if(sysEvents.Lines.Length >= max)
                            sysEvents.Clear();

                        sysEvents.AppendText(line);
                    }));
                } else {
                    if(sysEvents.Lines.Length >= max)
                        sysEvents.Clear();

                    sysEvents.AppendText(line);
                }
            } catch { }
        }

        private void SetActTasks() {
            var _data = new List<ListItem<string>>();
            foreach(var _class in this._actAllTasks) {
                _data.Add(new ListItem<string> { Index = _class.Order, Id = _class.Id, Text = _class.Name });
            }

            actTasks.DataSource = _data;
            actTasks.ValueMember = "Id";
            actTasks.DisplayMember = "Text";
        }

        private void SetHisTasks() {
            var _data = new List<ListItem<string>>();
            foreach(var _class in this._hisAllTasks) {
                _data.Add(new ListItem<string> { Index = _class.Order, Id = _class.Id, Text = _class.Name });
            }

            hisTasks.DataSource = _data;
            hisTasks.ValueMember = "Id";
            hisTasks.DisplayMember = "Text";
        }

        #region events
        private void sysMenu0101_Click(object sender, EventArgs e) {
            this.Start();
        }

        private void sysMenu0102_Click(object sender, EventArgs e) {
            this.Stop();
        }

        private void sysMenu0103_Click(object sender, EventArgs e) {
            new Config().ShowDialog();
        }

        private void sysMenu0104_CheckedChanged(object sender, EventArgs e) {

        }

        private void sysMenu0105_Click(object sender, EventArgs e) {
            this.Quit();
        }

        private void sysMenu0201_Click(object sender, EventArgs e) {
            Help.ShowHelp(this, String.Format(@"file://{0}\Doc\help.chm", Environment.CurrentDirectory));
        }

        private void sysMenu0202_Click(object sender, EventArgs e) {
            new About().ShowDialog();
        }

        private void cleanEventsMenuItem_Click(object sender, EventArgs e) {
            sysEvents.Clear();
        }

        private void actCkAll_CheckedChanged(object sender, EventArgs e) {
            if(actTasks.Items.Count == 0) return;
            for(int i = 0; i < actTasks.Items.Count; i++) {
                actTasks.SetItemChecked(i, actCkAll.Checked);
            }
        }

        private void actCkFx_CheckedChanged(object sender, EventArgs e) {
            if(actTasks.Items.Count == 0) return;
            for(int i = 0; i < actTasks.Items.Count; i++) {
                actTasks.SetItemChecked(i, !actTasks.GetItemChecked(i));
            }
        }

        private void actDoTask_Click(object sender, EventArgs e) {
            var messages = new List<string>();
            try {
                actDoTask.Enabled = false;
                actMessageLbl.ResetText();

                var _tasks = new List<IActTask>();
                foreach(ListItem<string> item in actTasks.CheckedItems) {
                    var _task = this._actAllTasks.Find(t => t.Id == item.Id);
                    if(_task != null) _tasks.Add(_task);
                }

                if(_tasks.Count == 0) throw new Exception("至少选择一项任务。");
                if(MessageBox.Show("您确定执行选中的任务吗？", "确认对话框", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK) {
                    foreach(var _task in _tasks) {
                        try {
                            _task.Events = new List<Event>();
                            _task.Last = actStart.Value;
                            _task.Next = actEnd.Value;
                            _task.Execute();
                            messages.Add(string.Format("{0}> {1:HH:mm:ss} \"{2}\"执行成功。", messages.Count + 1, DateTime.Now, _task.Name));
                        } catch(Exception err) {
                            messages.Add(string.Format("{0}> {1:HH:mm:ss} \"{2}\"{3}", messages.Count + 1, DateTime.Now, _task.Name, err.Message));
                            foreach(var v in _task.Events) messages.Add(string.Format("{0}> {1:HH:mm:ss} {2}", messages.Count + 1, v.Time, v.Message));
                        }
                    }
                }
            } catch(Exception err) {
                messages.Add(string.Format("{0}> {1:HH:mm:ss} {2}", messages.Count + 1, DateTime.Now, err.Message));
            } finally {
                if(messages.Count > 0) actMessageLbl.Text = string.Join(Environment.NewLine, messages);
                messages.Clear();
                actDoTask.Enabled = true;
            }
        }

        private void hisCkAll_CheckedChanged(object sender, EventArgs e) {
            if(hisTasks.Items.Count == 0) return;
            for(int i = 0; i < hisTasks.Items.Count; i++) {
                hisTasks.SetItemChecked(i, hisCkAll.Checked);
            }
        }

        private void hisCkFx_CheckedChanged(object sender, EventArgs e) {
            if(hisTasks.Items.Count == 0) return;
            for(int i = 0; i < hisTasks.Items.Count; i++) {
                hisTasks.SetItemChecked(i, !hisTasks.GetItemChecked(i));
            }
        }

        private void hisDoTask_Click(object sender, EventArgs e) {
            var messages = new List<string>();
            try {
                hisDoTask.Enabled = false;
                hisMessageLbl.ResetText();

                var _tasks = new List<IHisTask>();
                foreach(ListItem<string> item in hisTasks.CheckedItems) {
                    var _task = this._hisAllTasks.Find(t => t.Id == item.Id);
                    if(_task != null) _tasks.Add(_task);
                }

                if(_tasks.Count == 0) throw new Exception("至少选择一项任务。");
                if(MessageBox.Show("您确定执行选中的任务吗？", "确认对话框", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK) {
                    foreach(var _task in _tasks) {
                        try {
                            _task.Events = new List<Event>();
                            _task.Last = hisStart.Value;
                            _task.Next = hisEnd.Value;
                            _task.Execute();
                            messages.Add(string.Format("{0}> {1:HH:mm:ss} \"{2}\"执行成功。", messages.Count + 1, DateTime.Now, _task.Name));
                        } catch(Exception err) {
                            messages.Add(string.Format("{0}> {1:HH:mm:ss} \"{2}\"{3}", messages.Count + 1, DateTime.Now, _task.Name, err.Message));
                            foreach(var v in _task.Events) messages.Add(string.Format("{0}> {1:HH:mm:ss} {2}", messages.Count + 1, v.Time, v.Message));
                        }
                    }
                }
            } catch(Exception err) {
                messages.Add(string.Format("{0}> {1:HH:mm:ss} {2}", messages.Count + 1, DateTime.Now, err.Message));
            } finally {
                if(messages.Count > 0) hisMessageLbl.Text = string.Join(Environment.NewLine, messages);
                messages.Clear();
                hisDoTask.Enabled = true;
            }
        }
        #endregion

        #region short
        private void shortcutStart_Click(object sender, EventArgs e) {
            this.sysMenu0101_Click(sender, e);
        }

        private void shortcutStop_Click(object sender, EventArgs e) {
            this.sysMenu0102_Click(sender, e);
        }

        private void shortcutConfig_Click(object sender, EventArgs e) {
            this.sysMenu0103_Click(sender, e);
        }

        private void shortcutExit_Click(object sender, EventArgs e) {
            this.sysMenu0105_Click(sender, e);
        }
        #endregion

        #region notify
        private bool _firstBalloonTip = true;

        private void Main_FormClosing(object sender, FormClosingEventArgs e) {
            if(e.CloseReason == CloseReason.UserClosing) {
                e.Cancel = true;
                HideNotify();
            } else {
                sysNotifyIcon.Visible = false;
                sysNotifyIcon.Dispose();
            }
        }

        private void sysNotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e) {
            ShowNotify();
        }

        private void notifyMenu01_Click(object sender, EventArgs e) {
            this.sysMenu0101_Click(sender, e);
        }

        private void notifyMenu02_Click(object sender, EventArgs e) {
            this.sysMenu0102_Click(sender, e);
        }

        private void notifyMenu03_Click(object sender, EventArgs e) {
            this.sysMenu0103_Click(sender, e);
        }

        private void notifyMenu04_CheckedChanged(object sender, EventArgs e) {
            this.sysMenu0104_CheckedChanged(sender, e);
        }

        private void notifyMenu05_Click(object sender, EventArgs e) {
            this.sysMenu0105_Click(sender, e);
        }

        private void ShowNotify() {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.Activate();
        }

        private void HideNotify() {
            this.WindowState = FormWindowState.Minimized;
            this.Hide();

            if(_firstBalloonTip)
                sysNotifyIcon.ShowBalloonTip(2000, "系统提示", "程序正在运行，已最小化到托盘。", ToolTipIcon.Info);

            _firstBalloonTip = false;
        }
        #endregion
    }
}
