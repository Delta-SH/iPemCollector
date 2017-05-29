using iPem.Core;
using iPem.Core.Rs;
using iPem.Data;
using iPem.Data.Common;
using iPem.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace iPem.TaskServer {
    public partial class Simulator : Form {
        //全局变量
        private RunStatus _runStatus;
        private Registry _registry;
        private Thread _workerThread;
        private List<Thread> _workerThreads;
        private EventWaitHandle _allDone;

        //静态配置读写锁
        static ReaderWriterLockSlim _configWriterLock = new ReaderWriterLockSlim();

        //资源数据类
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
        private RedefinePointRepository _redefinePointRepository;

        //历史数据类
        private A_AAlarmRepository _aalmRepository;
        private A_TAlarmRepository _talmRepository;
        private Alarmer _alarmer;

        //应用数据类
        private ReservationRepository _reservationRepository;
        private NodesInReservationRepository _nodesInReservationRepository;

        public Simulator() {
            InitializeComponent();
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        private void startButton_Click(object sender, EventArgs e) {
            try {
                Logger.Information("#############################################");
                Logger.Information(String.Format("启动服务\"{0} {1}\"", iPemStore.Name, iPemStore.Version));

                //初始化全局变量
                _runStatus = RunStatus.Default;
                _registry = new Registry(Application.StartupPath);
                _workerThreads = new List<Thread>();
                _allDone = new EventWaitHandle(false, EventResetMode.ManualReset);

                var _dbRs = _registry.GetDatabase(DbIds.Rs);
                if (_dbRs == null || string.IsNullOrWhiteSpace(_dbRs.IP) || string.IsNullOrWhiteSpace(_dbRs.Uid) || string.IsNullOrWhiteSpace(_dbRs.Db)) throw new Exception("服务尚未配置数据库信息（资源数据库）。");
                SqlHelper.ConnectionStringRsTransaction = SqlTypeConverter.CreateConnectionString(_dbRs);

                var _dbCs = _registry.GetDatabase(DbIds.Cs);
                if (_dbCs == null || string.IsNullOrWhiteSpace(_dbCs.IP) || string.IsNullOrWhiteSpace(_dbCs.Uid) || string.IsNullOrWhiteSpace(_dbCs.Db)) throw new Exception("服务尚未配置数据库信息（历史数据库）。");
                SqlHelper.ConnectionStringCsTransaction = SqlTypeConverter.CreateConnectionString(_dbCs);

                var _dbSc = _registry.GetDatabase(DbIds.Sc);
                if (_dbSc == null || string.IsNullOrWhiteSpace(_dbSc.IP) || string.IsNullOrWhiteSpace(_dbSc.Uid) || string.IsNullOrWhiteSpace(_dbSc.Db)) throw new Exception("服务尚未配置数据库信息（应用数据库）。");
                SqlHelper.ConnectionStringScTransaction = SqlTypeConverter.CreateConnectionString(_dbSc);

                //初始化服务参数
                GlobalConfig.CurParam = _registry.GetParam(ParamIds.Default);
                if (GlobalConfig.CurParam == null || GlobalConfig.CurParam.Json == null) throw new Exception("服务尚未配置基本参数信息。");

                //初始化服务任务信息
                GlobalConfig.CurTasks = _registry.GetTasks().FindAll(t => t.Json != null && t.Json.EndDate >= DateTime.Now);
                if (GlobalConfig.CurTasks.Count == 0) {
                    Logger.Warning("未找到可执行的计划任务。");
                }

                //初始化告警翻转哈希表
                GlobalConfig.ReversalKeys = new Dictionary<string, ReversalModel>();

                //初始化资源数据类
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
                _redefinePointRepository = new RedefinePointRepository();

                //初始化历史数据类
                _aalmRepository = new A_AAlarmRepository();
                _talmRepository = new A_TAlarmRepository();
                _alarmer = new Alarmer();

                //初始化应用数据类
                _reservationRepository = new ReservationRepository();
                _nodesInReservationRepository = new NodesInReservationRepository();

                //线程挂起
                _allDone.Reset();

                //创建初始化线程
                _workerThread = new Thread(new ThreadStart(DoInit));
                _workerThread.IsBackground = true;
                _workerThread.Start();
                _workerThreads.Add(_workerThread);

                //创建告警处理线程
                _workerThread = new Thread(new ThreadStart(DoAlarm));
                _workerThread.IsBackground = true;
                _workerThread.Start();
                _workerThreads.Add(_workerThread);
            } catch (Exception exc) {
                Logger.Error(exc.Message, exc);
            }
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        private void stopButton_Click(object sender, EventArgs e) {
            try {

            } catch (Exception exc) {
                Logger.Error(exc.Message, exc);
            }
        }

        private void DoInit() {
            var maxRepeat = GlobalConfig.CurParam.Json.InitRepeatCount;
            while (_runStatus < RunStatus.Running) {
                try {
                    if (_runStatus == RunStatus.Init) {
                        maxRepeat--;
                        Logger.Information("数据初始化...");

                        #region 加载基础数据
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
                        if (iPemWorkContext.Protocols != null) {
                            var _protocols = _protocolRepository.GetEntities();
                            foreach (var _protocol in _protocols) {
                                var _points = _pointRepository.GetEntitiesByProtocol(_protocol.Id);
                                iPemWorkContext.Protocols.Add(new WcProtocol {
                                    Current = _protocol,
                                    Points = _points
                                });
                            }
                        }

                        //devices
                        if (iPemWorkContext.Devices != null) {
                            var _devices = _deviceRepository.GetEntities();
                            iPemWorkContext.Devices.AddRange(
                            from _device in _devices
                            join _protocol in iPemWorkContext.Protocols on _device.ProtocolId equals _protocol.Current.Id
                            select new WcDevice { Current = _device, Protocol = _protocol });
                        }

                        //fsus
                        if (iPemWorkContext.Fsus != null) {
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
                        if (iPemWorkContext.Rooms != null) {
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
                        if (iPemWorkContext.Stations != null) {
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
                        if (iPemWorkContext.Areas != null) {
                            var _areas = _areaRepository.GetEntities();
                            var stationsets = from _station in iPemWorkContext.Stations
                                              group _station by _station.Current.AreaId into g
                                              select new { Id = g.Key, Stations = g };

                            iPemWorkContext.Areas.AddRange(
                            from _area in _areas
                            join stationset in stationsets on _area.Id equals stationset.Id into lt
                            from def in lt.DefaultIfEmpty()
                            select new WcArea { Current = _area, Stations = def != null ? def.Stations.ToList() : new List<WcStation>() });

                            foreach (var current in iPemWorkContext.Areas) {
                                current.Initializer(iPemWorkContext.Areas);
                            }
                        }
                        #endregion

                        #region 加载信号重定义表
                        GlobalConfig.RedefinePoints = new Dictionary<string, RedefinePoint>();
                        GlobalConfig.RedefineLoadTime = DateTime.Now;
                        foreach (var point in _redefinePointRepository.GetEntities()) {
                            (GlobalConfig.RedefinePoints)[CommonHelper.JoinKeys(point.DeviceId, point.PointId)] = point;
                        }
                        #endregion

                        #region 加载工程预约相关表
                        GlobalConfig.Reservations = new List<ReservationModel>();
                        GlobalConfig.ReservationLoadTime = DateTime.Now;
                        var reservations = _reservationRepository.GetEntities();
                        var resNodes = _nodesInReservationRepository.GetEntities();
                        foreach (var reservation in reservations) {
                            var nodes = resNodes.FindAll(a => a.ReservationId == reservation.Id);
                            if (nodes.Count == 0) continue;

                            var resDevices = new HashSet<string>();
                            foreach (var node in nodes) {
                                if (node.NodeType == EnmOrganization.Area) {
                                    var current = iPemWorkContext.Areas.Find(a => a.Current.Id == node.NodeId);
                                    if (current == null) continue;
                                    var devices = iPemWorkContext.Devices.FindAll(d => current.Keys.Contains(d.Current.AreaId));
                                    foreach (var device in devices) {
                                        resDevices.Add(device.Current.Id);
                                    }
                                } else if (node.NodeType == EnmOrganization.Station) {
                                    var devices = iPemWorkContext.Devices.FindAll(d => d.Current.StationId == node.NodeId);
                                    foreach (var device in devices) {
                                        resDevices.Add(device.Current.Id);
                                    }
                                } else if (node.NodeType == EnmOrganization.Room) {
                                    var devices = iPemWorkContext.Devices.FindAll(d => d.Current.RoomId == node.NodeId);
                                    foreach (var device in devices) {
                                        resDevices.Add(device.Current.Id);
                                    }
                                } else if (node.NodeType == EnmOrganization.Device) {
                                    resDevices.Add(node.NodeId);
                                }
                            }

                            GlobalConfig.Reservations.Add(new ReservationModel {
                                Reservation = reservation,
                                Devices = resDevices
                            });
                        }
                        #endregion

                        #region 加载活动告警表
                        GlobalConfig.InitAlarm();
                        GlobalConfig.AlarmsLoadTime = DateTime.Now;
                        foreach (var alarm in _aalmRepository.GetEntities()) {
                            GlobalConfig.AddAlarm(alarm);
                        }
                        #endregion

                        Logger.Information("数据初始化完成");
                        _runStatus = RunStatus.Running;
                        _allDone.Set();
                        break;
                    }
                } catch (Exception err) {
                    Logger.Error(err.Message, err);
                    if (maxRepeat <= 0) {
                        Logger.Warning("数据初始化错误,服务启动失败。");
                        _runStatus = RunStatus.Stop;
                        _allDone.Set();
                        break;
                    } else {
                        Logger.Warning("数据初始化错误,稍后将重试。");
                        Thread.Sleep(29000);
                    }
                }
            }
        }

        private void DoAlarm() {
            _allDone.WaitOne();

            while (_runStatus != RunStatus.Stop) {
                if (_runStatus == RunStatus.Running) {
                    try {
                        _configWriterLock.EnterReadLock();
                        var allTAlarms = _talmRepository.GetEntities();
                        if (allTAlarms.Count > 0) {
                            var allTAlarmModels = from alarm in allTAlarms
                                                  join device in iPemWorkContext.Devices on new { alarm.FsuId, alarm.DeviceId } equals new { FsuId = device.Current.FsuCode, DeviceId = device.Current.Code }
                                                  select new TAlarmModel { Device = device.Current, Alarm = alarm };

                            var currentTAlarms = new List<TAlarmModel>();
                            #region 处理告警触发延迟、告警恢复延迟
                            foreach (var alarm in allTAlarmModels) {
                                if (alarm.Alarm.AlarmFlag == EnmFlag.Begin) {
                                    var key = CommonHelper.JoinKeys(alarm.Device.Id, alarm.Alarm.PointId);
                                    if (GlobalConfig.RedefinePoints.ContainsKey(key)) {
                                        var redefine = GlobalConfig.RedefinePoints[key];
                                        if (redefine.AlarmDelay > 0 && alarm.Alarm.AlarmTime.AddSeconds(redefine.AlarmDelay) > DateTime.Now) {
                                            continue;
                                        }
                                    }
                                } else if (alarm.Alarm.AlarmFlag == EnmFlag.End) {
                                    var key = CommonHelper.JoinKeys(alarm.Device.Id, alarm.Alarm.PointId);
                                    if (GlobalConfig.RedefinePoints.ContainsKey(key)) {
                                        var redefine = GlobalConfig.RedefinePoints[key];
                                        if (redefine.AlarmRecoveryDelay > 0 && alarm.Alarm.AlarmTime.AddSeconds(redefine.AlarmRecoveryDelay) > DateTime.Now) {
                                            continue;
                                        }
                                    }
                                }

                                currentTAlarms.Add(alarm);
                            }
                            #endregion

                            //为了防止服务器和FSU时间不一致，允许服务器和FSU时间有1小时的误差。
                            var finalReservations = GlobalConfig.Reservations.FindAll(a => DateTime.Now >= a.Reservation.StartTime.AddHours(-1) && DateTime.Now <= a.Reservation.EndTime.AddHours(1));
                            foreach (var alarm in currentTAlarms) {
                                try {
                                    var almId = string.Format("{0}-{1}-{2}-{3}", alarm.Device.FsuId, alarm.Device.Id, alarm.Alarm.PointId, alarm.Alarm.SerialNo);
                                    if (alarm.Alarm.AlarmFlag == EnmFlag.Begin) {
                                        #region 开始告警
                                        try {
                                            var current = new StartAlarm {
                                                Id = almId,
                                                AreaId = alarm.Device.AreaId,
                                                StationId = alarm.Device.StationId,
                                                RoomId = alarm.Device.RoomId,
                                                FsuId = alarm.Device.FsuId,
                                                FsuCode = alarm.Alarm.FsuId,
                                                DeviceId = alarm.Device.Id,
                                                DeviceCode = alarm.Alarm.DeviceId,
                                                PointId = alarm.Alarm.PointId,
                                                SerialNo = alarm.Alarm.SerialNo,
                                                NMAlarmId = alarm.Alarm.NMAlarmId,
                                                AlarmTime = alarm.Alarm.AlarmTime,
                                                AlarmLevel = alarm.Alarm.AlarmLevel,
                                                AlarmFlag = alarm.Alarm.AlarmFlag,
                                                AlarmValue = alarm.Alarm.AlarmValue,
                                                AlarmDesc = alarm.Alarm.AlarmDesc,
                                                AlarmRemark = alarm.Alarm.AlarmRemark
                                            };

                                            #region 工程预约
                                            foreach (var reservation in finalReservations) {
                                                if (reservation.Devices.Contains(current.DeviceId)) {
                                                    current.ReservationId = reservation.Reservation.Id;
                                                    current.ReservationName = reservation.Reservation.Name;
                                                    current.ReservationStart = reservation.Reservation.StartTime;
                                                    current.ReservationEnd = reservation.Reservation.EndTime;
                                                    break;
                                                }
                                            }
                                            #endregion

                                            #region 主次告警、关联告警、告警过滤、告警翻转
                                            var key = CommonHelper.JoinKeys(current.DeviceId, current.PointId);
                                            if (GlobalConfig.RedefinePoints.ContainsKey(key)) {
                                                var redefine = GlobalConfig.RedefinePoints[key];

                                                #region 主次告警
                                                if (!string.IsNullOrWhiteSpace(redefine.InferiorAlarmStr)) {
                                                    var dicKey = CommonHelper.JoinKeys(redefine.InferiorAlarmStr.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
                                                    if (GlobalConfig.AlarmKeys2.ContainsKey(dicKey)) {
                                                        current.PrimaryId = GlobalConfig.AlarmKeys2[dicKey].Id;
                                                    }
                                                }
                                                #endregion

                                                #region 关联告警
                                                if (!string.IsNullOrWhiteSpace(redefine.ConnAlarmStr)) {
                                                    var dicKey = CommonHelper.JoinKeys(redefine.ConnAlarmStr.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
                                                    if (GlobalConfig.AlarmKeys2.ContainsKey(dicKey)) {
                                                        current.RelatedId = GlobalConfig.AlarmKeys2[dicKey].Id;
                                                    }
                                                }
                                                #endregion

                                                #region 告警过滤
                                                if (!string.IsNullOrWhiteSpace(redefine.AlarmFilteringStr)) {
                                                    var dicKey = CommonHelper.JoinKeys(redefine.AlarmFilteringStr.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
                                                    if (GlobalConfig.AlarmKeys2.ContainsKey(dicKey)) {
                                                        current.FilterId = GlobalConfig.AlarmKeys2[dicKey].Id;
                                                    }
                                                }
                                                #endregion

                                                #region 告警翻转
                                                if (!string.IsNullOrWhiteSpace(redefine.AlarmReversalStr)) {
                                                    int reversalInterval;
                                                    if (int.TryParse(redefine.AlarmReversalStr, out reversalInterval) && reversalInterval > 0) {
                                                        if (GlobalConfig.ReversalKeys.ContainsKey(key)) {
                                                            var reversalTarget = GlobalConfig.ReversalKeys[key];
                                                            if (reversalTarget.AlarmId != current.Id && reversalTarget.AlarmTime.AddMinutes(reversalInterval) >= current.AlarmTime) {
                                                                current.ReversalId = reversalTarget.AlarmId;
                                                                current.ReversalCount = ++reversalTarget.ReversalCount;
                                                            } else {
                                                                reversalTarget.AlarmId = current.Id;
                                                                reversalTarget.AlarmTime = current.AlarmTime;
                                                                reversalTarget.ReversalCount = 1;
                                                                current.ReversalId = current.Id;
                                                                current.ReversalCount = 1;
                                                            }
                                                        } else {
                                                            GlobalConfig.ReversalKeys[key] = new ReversalModel { AlarmId = current.Id, AlarmTime = current.AlarmTime, ReversalCount = 1 };
                                                            current.ReversalId = current.Id;
                                                            current.ReversalCount = 1;
                                                        }
                                                    }
                                                }
                                                #endregion

                                            }
                                            #endregion

                                            #region 开始告警
                                            _alarmer.Start(current);
                                            GlobalConfig.AddAlarm(current);
                                            #endregion
                                        } catch (Exception err) {
                                            Logger.Error(string.Format("开始告警发生错误,详见错误日志({0})。", JsonConvert.SerializeObject(alarm.Alarm)), err);
                                        }
                                        #endregion
                                    } else if (alarm.Alarm.AlarmFlag == EnmFlag.End) {
                                        #region 结束告警
                                        try {
                                            var current = new EndAlarm {
                                                Id = almId,
                                                FsuId = alarm.Device.FsuId,
                                                FsuCode = alarm.Alarm.FsuId,
                                                DeviceId = alarm.Device.Id,
                                                DeviceCode = alarm.Alarm.DeviceId,
                                                PointId = alarm.Alarm.PointId,
                                                SerialNo = alarm.Alarm.SerialNo,
                                                NMAlarmId = alarm.Alarm.NMAlarmId,
                                                StartTime = alarm.Alarm.AlarmTime,
                                                EndTime = alarm.Alarm.AlarmTime,
                                                AlarmLevel = alarm.Alarm.AlarmLevel,
                                                AlarmFlag = alarm.Alarm.AlarmFlag,
                                                StartValue = alarm.Alarm.AlarmValue,
                                                EndValue = alarm.Alarm.AlarmValue,
                                                AlarmDesc = alarm.Alarm.AlarmDesc,
                                                AlarmRemark = alarm.Alarm.AlarmRemark
                                            };

                                            var active = _aalmRepository.GetEntity(current.Id);
                                            if (active != null) {
                                                current.StartTime = active.AlarmTime;
                                                current.StartValue = active.AlarmValue;

                                                _alarmer.End(current);
                                                GlobalConfig.RemoveAlarm(current);
                                            }
                                        } catch (Exception err) {
                                            Logger.Error(string.Format("结束告警发生错误,详见错误日志({0})。", JsonConvert.SerializeObject(alarm.Alarm)), err);
                                        }
                                        #endregion
                                    }
                                } catch (Exception err) {
                                    Logger.Error(string.Format("处理告警发生错误,详见错误日志({0})。", JsonConvert.SerializeObject(alarm.Alarm)), err);
                                }
                            }
                        }
                    } catch (Exception err) {
                        Logger.Error(err.Message, err);
                    } finally {
                        _configWriterLock.ExitReadLock();
                    }
                }

                Thread.Sleep(GlobalConfig.CurParam.Json.AlarmInterval * 1000);
            }
        }

        private void DoConfig() {
            _allDone.WaitOne();

            while (_runStatus != RunStatus.Stop) {
                if (_runStatus == RunStatus.Running) {
                    try {

                        #region 同步信号重定义表
                        GlobalConfig.RedefineLoadTime = DateTime.Now;
                        var syncRedefinePoints = _redefinePointRepository.GetEntities(GlobalConfig.RedefineLoadTime);
                        if (syncRedefinePoints.Count > 0) {
                            try {
                                //获取写数据锁
                                _configWriterLock.EnterWriteLock();
                                foreach (var point in syncRedefinePoints) {
                                    (GlobalConfig.RedefinePoints)[CommonHelper.JoinKeys(point.DeviceId, point.PointId)] = point;
                                }
                            } catch (Exception err) {
                                Logger.Error(err.Message, err);
                            } finally {
                                _configWriterLock.ExitWriteLock();
                            }
                        }
                        #endregion

                        #region 同步工程预约相关表
                        GlobalConfig.ReservationLoadTime = DateTime.Now;
                        var syncReservations = _reservationRepository.GetEntities(GlobalConfig.ReservationLoadTime);
                        if (syncReservations.Count > 0) {
                            try {
                                //获取写数据锁
                                _configWriterLock.EnterWriteLock();

                                foreach (var reservation in syncReservations) {
                                    var nodes = _nodesInReservationRepository.GetEntities(reservation.Id);
                                    if (nodes.Count == 0) continue;

                                    var resDevices = new HashSet<string>();
                                    foreach (var node in nodes) {
                                        if (node.NodeType == EnmOrganization.Area) {
                                            var current = iPemWorkContext.Areas.Find(a => a.Current.Id == node.NodeId);
                                            if (current == null) continue;
                                            var devices = iPemWorkContext.Devices.FindAll(d => current.Keys.Contains(d.Current.AreaId));
                                            foreach (var device in devices) {
                                                resDevices.Add(device.Current.Id);
                                            }
                                        } else if (node.NodeType == EnmOrganization.Station) {
                                            var devices = iPemWorkContext.Devices.FindAll(d => d.Current.StationId == node.NodeId);
                                            foreach (var device in devices) {
                                                resDevices.Add(device.Current.Id);
                                            }
                                        } else if (node.NodeType == EnmOrganization.Room) {
                                            var devices = iPemWorkContext.Devices.FindAll(d => d.Current.RoomId == node.NodeId);
                                            foreach (var device in devices) {
                                                resDevices.Add(device.Current.Id);
                                            }
                                        } else if (node.NodeType == EnmOrganization.Device) {
                                            resDevices.Add(node.NodeId);
                                        }
                                    }

                                    var origin = GlobalConfig.Reservations.Find(a => a.Reservation.Id == reservation.Id);
                                    if (origin != null) {
                                        origin.Reservation = reservation;
                                        origin.Devices = resDevices;
                                    } else {
                                        GlobalConfig.Reservations.Add(new ReservationModel {
                                            Reservation = reservation,
                                            Devices = resDevices
                                        });
                                    }
                                }
                            } catch (Exception err) {
                                Logger.Error(err.Message, err);
                            } finally {
                                _configWriterLock.ExitWriteLock();
                            }
                        }
                        #endregion

                        #region 同步活动告警表
                        if (GlobalConfig.AlarmsLoadTime.AddHours(GlobalConfig.CurParam.Json.AlarmSync) <= DateTime.Now) {
                            try {
                                //获取写数据锁
                                _configWriterLock.EnterWriteLock();

                                GlobalConfig.InitAlarm();
                                GlobalConfig.AlarmsLoadTime = DateTime.Now;
                                foreach (var alarm in _aalmRepository.GetEntities()) {
                                    GlobalConfig.AddAlarm(alarm);
                                }
                            } catch (Exception err) {
                                Logger.Error(err.Message, err);
                            } finally {
                                _configWriterLock.ExitWriteLock();
                            }
                        }
                        #endregion

                    } catch (Exception err) {
                        Logger.Error(err.Message, err);
                    }
                }

                Thread.Sleep(GlobalConfig.CurParam.Json.ConfigInterval * 1000);
            }
        }
    }
}
