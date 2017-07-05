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
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace iPem.TaskService {
    public partial class TaskService : ServiceBase {
        //全局变量
        private RunStatus _runStatus;
        private Registry _registry;
        private Thread _workerThread;
        private List<Thread> _workerThreads;
        private EventWaitHandle _allDone;
        private DataTable _computer;

        //静态配置读写锁
        static ReaderWriterLockSlim _configWriterLock = new ReaderWriterLockSlim();

        //资源数据类
        private LogicTypeRepository _logicTypeRepository;
        private DeviceTypeRepository _deviceTypeRepository;
        private RoomTypeRepository _roomTypeRepository;
        private StationTypeRepository _stationTypeRepository;
        private EnumMethodsRepository _enumMethodsRepository;
        private PointRepository _pointRepository;
        private SubPointRepository _subPointRepository;
        private ProtocolRepository _protocolRepository;
        private DeviceRepository _deviceRepository;
        private FsuRepository _fsuRepository;
        private RoomRepository _roomRepository;
        private StationRepository _stationRepository;
        private AreaRepository _areaRepository;
        private RedefinePointRepository _redefinePointRepository;
        private CombSwitElecSourRepository _combSwitElecSourRepository;
        private DivSwitElecSourRepository _divSwitElecSourRepository;
        private MaskingRepository _maskingRepository;
        private GroupRepository _groupRepository;

        //历史数据类
        private A_AAlarmRepository _aalmRepository;
        private A_TAlarmRepository _talmRepository;
        private H_IDeviceRepository _iDeviceRepository;
        private H_IStationRepository _iStationRepository;
        private H_IAreaRepository _iAreaRepository;
        private V_HMeasureRepository _measureRepository;
        private V_BatRepository _batRepository;
        private V_BatTimeRepository _batTimeRepository;
        private V_ElecRepository _elecRepository;
        private V_StaticRepository _staticRepository;
        private V_LoadRepository _loadRepository;
        private V_CuttingRepository _cuttingRepository;
        private V_CutedRepository _cutedRepository;
        private V_ParamDiffRepository _paramDiffRepository;
        private Alarmer _alarmer;

        //应用数据类
        private ReservationRepository _reservationRepository;
        private NodesInReservationRepository _nodesInReservationRepository;
        private FormulaRepository _formulaRepository;

        public TaskService() {
            InitializeComponent();
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        protected override void OnStart(string[] args) {
            try {
                Logger.Information("#############################################");
                Logger.Information(String.Format("启动服务\"{0} {1}\"", iPemStore.Name, iPemStore.Version));

                //初始化全局变量
                _runStatus = RunStatus.Default;
                _registry = new Registry(AppDomain.CurrentDomain.BaseDirectory);
                _workerThreads = new List<Thread>();
                _allDone = new EventWaitHandle(false, EventResetMode.ManualReset);
                _computer = new DataTable();

                var _dbRs = _registry.GetDatabase(DbIds.Rs);
                if (_dbRs == null || string.IsNullOrWhiteSpace(_dbRs.IP) || string.IsNullOrWhiteSpace(_dbRs.Uid) || string.IsNullOrWhiteSpace(_dbRs.Db)) throw new Exception("服务尚未配置数据库信息（资源数据库）。");
                SqlHelper.ConnectionStringRsTransaction = SqlTypeConverter.CreateConnectionString(_dbRs);

                var _dbCs = _registry.GetDatabase(DbIds.Cs);
                if (_dbCs == null || string.IsNullOrWhiteSpace(_dbCs.IP) || string.IsNullOrWhiteSpace(_dbCs.Uid) || string.IsNullOrWhiteSpace(_dbCs.Db)) throw new Exception("服务尚未配置数据库信息（历史数据库）。");
                SqlHelper.ConnectionStringCsTransaction = SqlTypeConverter.CreateConnectionString(_dbCs);

                var _dbSc = _registry.GetDatabase(DbIds.Sc);
                if (_dbSc == null || string.IsNullOrWhiteSpace(_dbSc.IP) || string.IsNullOrWhiteSpace(_dbSc.Uid) || string.IsNullOrWhiteSpace(_dbSc.Db)) throw new Exception("服务尚未配置数据库信息（应用数据库）。");
                SqlHelper.ConnectionStringScTransaction = SqlTypeConverter.CreateConnectionString(_dbSc);

                //清空命令表
                _registry.CleanOrders();
                
                //初始化服务参数
                GlobalConfig.CurParams = _registry.GetParams();
                if (GlobalConfig.CurParams.Count == 0) throw new Exception("服务尚未配置基本参数信息。");

                //初始化服务任务信息
                GlobalConfig.CurTasks = _registry.GetTasks();
                if (GlobalConfig.CurTasks.Count == 0) throw new Exception("服务尚未配置计划任务信息。");

                //初始化告警翻转哈希表
                GlobalConfig.ReversalKeys = new Dictionary<string, ReversalModel>();

                //初始化资源数据类
                _logicTypeRepository = new LogicTypeRepository();
                _deviceTypeRepository = new DeviceTypeRepository();
                _roomTypeRepository = new RoomTypeRepository();
                _stationTypeRepository = new StationTypeRepository();
                _enumMethodsRepository = new EnumMethodsRepository();
                _pointRepository = new PointRepository();
                _subPointRepository = new SubPointRepository();
                _protocolRepository = new ProtocolRepository();
                _deviceRepository = new DeviceRepository();
                _fsuRepository = new FsuRepository();
                _roomRepository = new RoomRepository();
                _stationRepository = new StationRepository();
                _areaRepository = new AreaRepository();
                _redefinePointRepository = new RedefinePointRepository();
                _combSwitElecSourRepository = new CombSwitElecSourRepository();
                _divSwitElecSourRepository = new DivSwitElecSourRepository();
                _maskingRepository = new MaskingRepository();
                _groupRepository = new GroupRepository();

                //初始化历史数据类
                _aalmRepository = new A_AAlarmRepository();
                _talmRepository = new A_TAlarmRepository();
                _iDeviceRepository = new H_IDeviceRepository();
                _iStationRepository = new H_IStationRepository();
                _iAreaRepository = new H_IAreaRepository();
                _measureRepository = new V_HMeasureRepository();
                _batRepository = new V_BatRepository();
                _batTimeRepository = new V_BatTimeRepository();
                _elecRepository = new V_ElecRepository();
                _staticRepository = new V_StaticRepository();
                _loadRepository = new V_LoadRepository();
                _cuttingRepository = new V_CuttingRepository();
                _cutedRepository = new V_CutedRepository();
                _paramDiffRepository = new V_ParamDiffRepository();
                _alarmer = new Alarmer();

                //初始化应用数据类
                _reservationRepository = new ReservationRepository();
                _nodesInReservationRepository = new NodesInReservationRepository();
                _formulaRepository = new FormulaRepository();

                //线程挂起
                _allDone.Reset();

                //创建初始化线程
                _workerThread = new Thread(new ThreadStart(DoInit));
                _workerThread.IsBackground = true;
                _workerThread.Start();
                _workerThreads.Add(_workerThread);

                //创建初始化线程
                _workerThread = new Thread(new ThreadStart(DoConfig));
                _workerThread.IsBackground = true;
                _workerThread.Start();
                _workerThreads.Add(_workerThread);

                //创建告警处理线程
                _workerThread = new Thread(new ThreadStart(DoAlarm));
                _workerThread.IsBackground = true;
                _workerThread.Start();
                _workerThreads.Add(_workerThread);

                //创建自检处理线程
                _workerThread = new Thread(new ThreadStart(DoChecking));
                _workerThread.IsBackground = true;
                _workerThread.Start();
                _workerThreads.Add(_workerThread);

                //创建SC心跳线程
                _workerThread = new Thread(new ThreadStart(DoScHeartbeat));
                _workerThread.IsBackground = true;
                _workerThread.Start();
                _workerThreads.Add(_workerThread);

                //创建能耗处理线程
                _workerThread = new Thread(new ThreadStart(DoTask001));
                _workerThread.IsBackground = true;
                _workerThread.Start();
                _workerThreads.Add(_workerThread);

                //创建电池放电处理线程
                _workerThread = new Thread(new ThreadStart(DoTask002));
                _workerThread.IsBackground = true;
                _workerThread.Start();
                _workerThreads.Add(_workerThread);

                //创建信号测值统计线程
                _workerThread = new Thread(new ThreadStart(DoTask003));
                _workerThread.IsBackground = true;
                _workerThread.Start();
                _workerThreads.Add(_workerThread);

                //创建开关电源带载率统计线程
                _workerThread = new Thread(new ThreadStart(DoTask004));
                _workerThread.IsBackground = true;
                _workerThread.Start();
                _workerThreads.Add(_workerThread);

                //创建资管接口同步线程
                _workerThread = new Thread(new ThreadStart(DoTask005));
                _workerThread.IsBackground = true;
                _workerThread.Start();
                _workerThreads.Add(_workerThread);

                _runStatus = RunStatus.Init;
            } catch (Exception exc) {
                _runStatus = RunStatus.Stop;
                Logger.Error(exc.Message, exc);
            }
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        protected override void OnStop() {
            try {
                Logger.Information("停止服务...");
                _runStatus = RunStatus.Stop;
                _allDone.Set();

                foreach (var workerThread in _workerThreads) {
                    if (workerThread != null && workerThread.IsAlive) {
                        workerThread.Join(5000);
                    }
                }

                _registry.UpdateTasks(GlobalConfig.CurTasks);
                Logger.Information("服务已停止");
            } catch (Exception exc) {
                Logger.Error(exc.Message, exc);
            }
        }

        /// <summary>
        /// 服务初始化任务
        /// </summary>
        private void DoInit() {
            var maxRepeat = 3;
            while (_runStatus < RunStatus.Running) {
                Thread.Sleep(1000);
                try {
                    if (_runStatus == RunStatus.Init) {
                        maxRepeat--;
                        Logger.Information("数据初始化...");

                        LoadBase();
                        LoadRedefine();
                        LoadMaskings();
                        LoadScHeartbeats();
                        LoadReservation();
                        LoadTasks(false);
                        LoadStatic();
                        LoadBat();
                        LoadCut();
                        LoadActiveAlarm();

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

        /// <summary>
        /// 同步配置任务
        /// </summary>
        private void DoConfig() {
            _allDone.WaitOne();

            Logger.Information("命令监听线程已启动。");
            while (_runStatus != RunStatus.Stop) {
                Thread.Sleep(1000);
                if (_runStatus == RunStatus.Running) {
                    try {
                        var orders = _registry.GetOrders();
                        if (orders.Count == 0) continue;

                        #region 重启服务
                        var reboot = orders.Find(o => o.Id == OrderId.Restart);
                        if (reboot != null) {
                            _registry.DelOrders(orders);
                            Restart();
                            continue;
                        }
                        #endregion

                        #region 重载数据
                        var reload = orders.Find(o => o.Id == OrderId.Reload);
                        if (reload != null) {
                            try {
                                //获取写资源锁
                                _configWriterLock.EnterWriteLock();
                                Logger.Information("重载数据...");

                                try {
                                    LoadBase();
                                } catch (Exception err) {
                                    Logger.Error(string.Format("同步基本配置数据错误，{0}", err.Message), err);
                                }

                                try {
                                    LoadRedefine();
                                } catch (Exception err) {
                                    Logger.Error(string.Format("同步重定义数据错误，{0}", err.Message), err);
                                }

                                try {
                                    LoadMaskings();
                                } catch (Exception err) {
                                    Logger.Error(string.Format("同步告警屏蔽数据错误，{0}", err.Message), err);
                                }

                                try {
                                    LoadScHeartbeats();
                                } catch (Exception err) {
                                    Logger.Error(string.Format("同步SC采集组数据错误，{0}", err.Message), err);
                                }

                                try {
                                    LoadReservation();
                                } catch (Exception err) {
                                    Logger.Error(string.Format("同步工程预约数据错误，{0}", err.Message), err);
                                }

                                try {
                                    LoadTasks(true);
                                } catch (Exception err) {
                                    Logger.Error(string.Format("同步计划任务数据错误，{0}", err.Message), err);
                                }

                                try {
                                    LoadParams();
                                } catch (Exception err) {
                                    Logger.Error(string.Format("同步服务配置数据错误，{0}", err.Message), err);
                                }

                                try {
                                    LoadStatic();
                                } catch (Exception err) {
                                    Logger.Error(string.Format("同步信号测值统计策略数据错误，{0}", err.Message), err);
                                }

                                try {
                                    LoadBat();
                                } catch (Exception err) {
                                    Logger.Error(string.Format("同步电池放电统计策略数据错误，{0}", err.Message), err);
                                }

                                try {
                                    LoadCut();
                                } catch (Exception err) {
                                    Logger.Error(string.Format("同步断站、停电、发电策略数据错误，{0}", err.Message), err);
                                }

                                try {
                                    LoadActiveAlarm();
                                } catch (Exception err) {
                                    Logger.Error(string.Format("同步活动告警数据错误，{0}", err.Message), err);
                                }

                                Logger.Information("重载数据完成。");
                            } finally {
                                _configWriterLock.ExitWriteLock();
                            }

                            _registry.DelOrders(orders);
                            continue;
                        }
                        #endregion

                        #region 处理命令
                        foreach (var order in orders) {
                            if (order.Id == OrderId.SyncConfig) {
                                try {
                                    //获取写资源锁
                                    _configWriterLock.EnterWriteLock();
                                    Logger.Information("同步配置...");

                                    try {
                                        LoadBase();
                                    } catch (Exception err) {
                                        Logger.Error(string.Format("同步基本配置数据错误，{0}", err.Message), err);
                                    }

                                    try {
                                        LoadRedefine();
                                    } catch (Exception err) {
                                        Logger.Error(string.Format("同步重定义数据错误，{0}", err.Message), err);
                                    }

                                    try {
                                        LoadMaskings();
                                    } catch (Exception err) {
                                        Logger.Error(string.Format("同步告警屏蔽数据错误，{0}", err.Message), err);
                                    }

                                    try {
                                        LoadScHeartbeats();
                                    } catch (Exception err) {
                                        Logger.Error(string.Format("同步SC采集组数据错误，{0}", err.Message), err);
                                    }

                                    try {
                                        LoadReservation();
                                    } catch (Exception err) {
                                        Logger.Error(string.Format("同步工程预约数据错误，{0}", err.Message), err);
                                    }

                                    try {
                                        LoadStatic();
                                    } catch (Exception err) {
                                        Logger.Error(string.Format("同步信号测值统计策略数据错误，{0}", err.Message), err);
                                    }

                                    try {
                                        LoadBat();
                                    } catch (Exception err) {
                                        Logger.Error(string.Format("同步电池放电统计策略数据错误，{0}", err.Message), err);
                                    }

                                    try {
                                        LoadCut();
                                    } catch (Exception err) {
                                        Logger.Error(string.Format("同步断站、停电、发电策略数据错误，{0}", err.Message), err);
                                    }

                                    Logger.Information("同步配置完成。");
                                } finally {
                                    _configWriterLock.ExitWriteLock();
                                }
                            } else if (order.Id == OrderId.SyncAlarm) {
                                try {
                                    //获取写资源锁
                                    _configWriterLock.EnterWriteLock();

                                    Logger.Information("同步告警...");
                                    LoadActiveAlarm();
                                    Logger.Information("同步告警完成。");
                                } catch (Exception err) {
                                    Logger.Error(string.Format("同步活动告警数据错误，{0}", err.Message), err);
                                } finally {
                                    _configWriterLock.ExitWriteLock();
                                }
                            }
                        }
                        #endregion

                        _registry.DelOrders(orders);
                    } catch (Exception err) {
                        Logger.Error(err.Message, err);
                    }
                }
            }
        }

        /// <summary>
        /// 告警处理任务
        /// </summary>
        private void DoAlarm() {
            _allDone.WaitOne();

            Logger.Information("告警处理线程已启动。");
            while (_runStatus != RunStatus.Stop) {
                Thread.Sleep(1000);
                if (_runStatus == RunStatus.Running) {
                    try {
                        //获取读资源锁
                        _configWriterLock.EnterReadLock();

                        var allTAlarms = _talmRepository.GetEntities();
                        if (allTAlarms.Count > 0) {
                            var allTAlarmModels = from alarm in allTAlarms
                                                  join point in iPemWorkContext.Points on new { alarm.SignalId, alarm.SignalNumber } equals new { SignalId = point.Code, SignalNumber = point.Number }
                                                  join device in iPemWorkContext.Devices on new { alarm.FsuId, alarm.DeviceId } equals new { FsuId = device.Current.FsuCode, DeviceId = device.Current.Code }
                                                  select new TAlarmModel { Device = device.Current, Point = point, Alarm = alarm };

                            var currentTAlarms = new List<TAlarmModel>();
                            #region 处理告警触发延迟、告警恢复延迟
                            foreach (var alarm in allTAlarmModels) {
                                if (alarm.Alarm.AlarmFlag == EnmFlag.Begin) {
                                    var key = CommonHelper.JoinKeys(alarm.Device.Id, alarm.Point.Id);
                                    if (GlobalConfig.RedefinePoints.ContainsKey(key)) {
                                        var redefine = GlobalConfig.RedefinePoints[key];
                                        if (redefine.AlarmDelay > 0 && alarm.Alarm.AlarmTime.AddSeconds(redefine.AlarmDelay) > DateTime.Now) {
                                            continue;
                                        }
                                    }
                                } else if (alarm.Alarm.AlarmFlag == EnmFlag.End) {
                                    var key = CommonHelper.JoinKeys(alarm.Device.Id, alarm.Point.Id);
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
                                    var key = CommonHelper.JoinKeys(alarm.Device.Id, alarm.Point.Id);
                                    var almId = string.Format("{0}-{1}-{2}-{3}", alarm.Alarm.SerialNo, alarm.Point.Id, alarm.Device.Id, alarm.Device.FsuId);
                                    if (alarm.Alarm.AlarmFlag == EnmFlag.Begin) {
                                        #region 开始告警
                                        try {
                                            var current = new AlarmStart {
                                                Id = almId,
                                                AreaId = alarm.Device.AreaId,
                                                StationId = alarm.Device.StationId,
                                                RoomId = alarm.Device.RoomId,
                                                FsuId = alarm.Device.FsuId,
                                                FsuCode = alarm.Device.FsuCode,
                                                DeviceId = alarm.Device.Id,
                                                DeviceCode = alarm.Device.Code,
                                                PointId = alarm.Point.Id,
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

                                            #region 主次告警、关联告警、告警过滤、告警翻转、告警屏蔽
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
                                                            if (reversalTarget.AlarmTime.AddMinutes(reversalInterval) >= current.AlarmTime) {
                                                                current.ReversalId = reversalTarget.AlarmId;
                                                                current.ReversalCount = ++reversalTarget.ReversalCount;
                                                            } else {
                                                                reversalTarget.AlarmId = current.Id;
                                                                reversalTarget.AlarmTime = current.AlarmTime;
                                                                reversalTarget.ReversalCount = 0;
                                                                current.ReversalId = current.Id;
                                                                current.ReversalCount = 0;
                                                            }
                                                        } else {
                                                            GlobalConfig.ReversalKeys[key] = new ReversalModel { AlarmId = current.Id, AlarmTime = current.AlarmTime, ReversalCount = 0 };
                                                            current.ReversalId = current.Id;
                                                            current.ReversalCount = 0;
                                                        }
                                                    }
                                                }
                                                #endregion

                                                #region 告警屏蔽

                                                current.Masked = GlobalConfig.Maskings.Contains(CommonHelper.JoinKeys(current.DeviceId, "masking-all"));
                                                if (!current.Masked) current.Masked = GlobalConfig.Maskings.Contains(key);

                                                #endregion

                                            }
                                            #endregion

                                            #region 开始告警
                                            _alarmer.Start(current);
                                            GlobalConfig.AddAlarm(current);
                                            #endregion

                                            #region 断站、停电、发电
                                            var cuttings = new List<V_Cutting>();

                                            #region 断站告警
                                            if (GlobalConfig.Offs.Contains(key)) {
                                                cuttings.Add(new V_Cutting {
                                                    Id = current.Id,
                                                    Type = EnmCutType.Off,
                                                    AreaId = current.AreaId,
                                                    StationId = current.StationId,
                                                    RoomId = current.RoomId,
                                                    FsuId = current.FsuId,
                                                    DeviceId = current.DeviceId,
                                                    PointId = current.PointId,
                                                    StartTime = current.AlarmTime
                                                });
                                            }
                                            #endregion

                                            #region 停电告警
                                            if (GlobalConfig.Cuttings.Contains(key)) {
                                                cuttings.Add(new V_Cutting {
                                                    Id = current.Id,
                                                    Type = EnmCutType.Cut,
                                                    AreaId = current.AreaId,
                                                    StationId = current.StationId,
                                                    RoomId = current.RoomId,
                                                    FsuId = current.FsuId,
                                                    DeviceId = current.DeviceId,
                                                    PointId = current.PointId,
                                                    StartTime = current.AlarmTime
                                                });
                                            }
                                            #endregion

                                            #region 发电告警
                                            if (GlobalConfig.Powers.Contains(key)) {
                                                cuttings.Add(new V_Cutting {
                                                    Id = current.Id,
                                                    Type = EnmCutType.Power,
                                                    AreaId = current.AreaId,
                                                    StationId = current.StationId,
                                                    RoomId = current.RoomId,
                                                    FsuId = current.FsuId,
                                                    DeviceId = current.DeviceId,
                                                    PointId = current.PointId,
                                                    StartTime = current.AlarmTime
                                                });
                                            }
                                            #endregion

                                            _cuttingRepository.SaveEntities(cuttings);
                                            #endregion

                                        } catch (Exception err) {
                                            Logger.Error(err.Message);
                                            Logger.Error(string.Format("开始告警发生错误,详见错误日志({0})。", JsonConvert.SerializeObject(alarm.Alarm)), err);
                                        }
                                        #endregion
                                    } else if (alarm.Alarm.AlarmFlag == EnmFlag.End) {
                                        #region 结束告警
                                        try {
                                            var current = new AlarmEnd {
                                                Id = almId,
                                                AreaId = alarm.Device.AreaId,
                                                StationId = alarm.Device.StationId,
                                                RoomId = alarm.Device.RoomId,
                                                FsuId = alarm.Device.FsuId,
                                                FsuCode = alarm.Device.FsuCode,
                                                DeviceId = alarm.Device.Id,
                                                DeviceCode = alarm.Device.Code,
                                                PointId = alarm.Point.Id,
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
                                                current.Confirmed = active.Confirmed;
                                                current.Confirmer = active.Confirmer;
                                                current.ConfirmedTime = active.ConfirmedTime;
                                                current.ReservationId = active.ReservationId;
                                                current.PrimaryId = active.PrimaryId;
                                                current.RelatedId = active.RelatedId;
                                                current.FilterId = active.FilterId;
                                                current.ReversalId = active.ReversalId;
                                                current.ReversalCount = active.ReversalCount;
                                                current.Masked = active.Masked;
                                                if (!string.IsNullOrWhiteSpace(current.ReservationId)) {
                                                    var reservation = GlobalConfig.Reservations.Find(r => r.Reservation.Id == current.ReservationId);
                                                    if (reservation != null) {
                                                        current.ReservationName = reservation.Reservation.Name;
                                                        current.ReservationStart = reservation.Reservation.StartTime;
                                                        current.ReservationEnd = reservation.Reservation.EndTime;
                                                    }
                                                }

                                                #region 断站、停电、发电
                                                var cuteds = new List<V_Cuted>();

                                                #region 断站告警
                                                if (GlobalConfig.Offs.Contains(key)) {
                                                    cuteds.Add(new V_Cuted {
                                                        Id = current.Id,
                                                        Type = EnmCutType.Off,
                                                        AreaId = current.AreaId,
                                                        StationId = current.StationId,
                                                        RoomId = current.RoomId,
                                                        FsuId = current.FsuId,
                                                        DeviceId = current.DeviceId,
                                                        PointId = current.PointId,
                                                        StartTime = current.StartTime,
                                                        EndTime = current.EndTime
                                                    });
                                                }
                                                #endregion

                                                #region 停电告警
                                                if (GlobalConfig.Cuttings.Contains(key)) {
                                                    cuteds.Add(new V_Cuted {
                                                        Id = current.Id,
                                                        Type = EnmCutType.Cut,
                                                        AreaId = current.AreaId,
                                                        StationId = current.StationId,
                                                        RoomId = current.RoomId,
                                                        FsuId = current.FsuId,
                                                        DeviceId = current.DeviceId,
                                                        PointId = current.PointId,
                                                        StartTime = current.StartTime,
                                                        EndTime = current.EndTime
                                                    });
                                                }
                                                #endregion

                                                #region 发电告警
                                                if (GlobalConfig.Powers.Contains(key)) {
                                                    cuteds.Add(new V_Cuted {
                                                        Id = current.Id,
                                                        Type = EnmCutType.Power,
                                                        AreaId = current.AreaId,
                                                        StationId = current.StationId,
                                                        RoomId = current.RoomId,
                                                        FsuId = current.FsuId,
                                                        DeviceId = current.DeviceId,
                                                        PointId = current.PointId,
                                                        StartTime = current.StartTime,
                                                        EndTime = current.EndTime
                                                    });
                                                }
                                                #endregion

                                                _cutedRepository.SaveEntities(cuteds);
                                                #endregion

                                                #region 结束告警
                                                _alarmer.End(current);
                                                GlobalConfig.RemoveAlarm(current);
                                                #endregion
                                            }
                                        } catch (Exception err) {
                                            Logger.Error(err.Message);
                                            Logger.Error(string.Format("结束告警发生错误,详见错误日志({0})。", JsonConvert.SerializeObject(alarm.Alarm)), err);
                                        }
                                        #endregion
                                    }
                                } catch (Exception err) {
                                    Logger.Error(err.Message);
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
            }
        }

        /// <summary>
        /// 自检告警任务
        /// </summary>
        private void DoChecking() {
            _allDone.WaitOne();

            Logger.Information("自检处理线程已启动。");

            #region 定义变量
            WcPoint _ScOffPoint = null, _FsuOffPoint = null;
            try {
                var _scOff = GlobalConfig.CurParams.Find(p => p.Id == ParamId.ScOff);
                if (_scOff != null && !string.IsNullOrWhiteSpace(_scOff.Value)) {
                    var _scPoint = iPemWorkContext.Points.Find(p => p.Id == _scOff.Value);
                    if (_scPoint != null) {
                        _ScOffPoint = new WcPoint { 
                            Current = _scPoint, 
                            SubPoint = iPemWorkContext.SubPoints.Find(p => p.PointId == _scPoint.Id)
                        };
                    } else {
                        Logger.Error("SC通信中断告警编码配置错误");
                    }
                } else {
                    Logger.Error("SC通信中断告警编码尚未配置");
                }

                var _fsuOff = GlobalConfig.CurParams.Find(p => p.Id == ParamId.FsuOff);
                if (_fsuOff != null && !string.IsNullOrWhiteSpace(_fsuOff.Value)) {
                    var _fsuPoint = iPemWorkContext.Points.Find(p => p.Id == _fsuOff.Value);
                    if (_fsuPoint != null) {
                        _FsuOffPoint = new WcPoint {
                            Current = _fsuPoint, 
                            SubPoint = null 
                        };
                    } else {
                        Logger.Error("FSU通信中断告警编码配置错误");
                    }
                } else {
                    Logger.Error("FSU通信中断告警编码尚未配置");
                }
            } catch (Exception err) {
                Logger.Error(err.Message, err);
            }
            #endregion

            while (_runStatus != RunStatus.Stop) {
                Thread.Sleep(1000);
                if (_runStatus == RunStatus.Running) {
                    try {
                        _configWriterLock.EnterReadLock();

                        #region SC通信中断
                        try {
                            if (_ScOffPoint != null) {
                                foreach (var sc in GlobalConfig.ScHeartbeats) {
                                    var key = CommonHelper.JoinKeys(sc.Current.Id, _ScOffPoint.Current.Id);
                                    if (sc.Current.Status && GlobalConfig.AlarmKeys2.ContainsKey(key)) {
                                        var _alarm = _aalmRepository.GetEntityInPoint(sc.Current.Id, _ScOffPoint.Current.Id);
                                        if (_alarm != null) {
                                            #region 结束告警
                                            var current = new AlarmEnd {
                                                Id = _alarm.Id,
                                                AreaId = _alarm.AreaId,
                                                StationId = _alarm.StationId,
                                                RoomId = _alarm.RoomId,
                                                FsuId = _alarm.FsuId,
                                                FsuCode = _alarm.FsuId,
                                                DeviceId = _alarm.DeviceId,
                                                DeviceCode = _alarm.DeviceId,
                                                PointId = _alarm.PointId,
                                                SerialNo = _alarm.SerialNo,
                                                NMAlarmId = _alarm.NMAlarmId,
                                                StartTime = _alarm.AlarmTime,
                                                EndTime = DateTime.Now,
                                                AlarmLevel = _alarm.AlarmLevel,
                                                AlarmFlag = EnmFlag.End,
                                                StartValue = _alarm.AlarmValue,
                                                EndValue = 1,
                                                AlarmDesc = _alarm.AlarmDesc,
                                                AlarmRemark = _alarm.AlarmRemark,
                                                Confirmed = _alarm.Confirmed,
                                                Confirmer = _alarm.Confirmer,
                                                ConfirmedTime = _alarm.ConfirmedTime,
                                                ReservationId = _alarm.ReservationId,
                                                PrimaryId = _alarm.PrimaryId,
                                                RelatedId = _alarm.RelatedId,
                                                FilterId = _alarm.FilterId,
                                                ReversalId = _alarm.ReversalId,
                                                ReversalCount = _alarm.ReversalCount,
                                                Masked = _alarm.Masked
                                            };

                                            _alarmer.End(current);
                                            GlobalConfig.RemoveAlarm(current);
                                            #endregion
                                        }
                                    } else if (!sc.Current.Status && !GlobalConfig.AlarmKeys2.ContainsKey(key)) {
                                        #region 开始告警
                                        var current = new AlarmStart {
                                            Id = null,
                                            AreaId = "-1",
                                            StationId = "-1",
                                            RoomId = "-1",
                                            FsuId = "-1",
                                            FsuCode = "-1",
                                            DeviceId = sc.Current.Id,
                                            DeviceCode = sc.Current.Id,
                                            PointId = _ScOffPoint.Current.Id,
                                            SerialNo = CommonHelper.GetIdAsString(),
                                            NMAlarmId = _ScOffPoint.Current.NMAlarmId,
                                            AlarmTime = DateTime.Now,
                                            AlarmLevel = _ScOffPoint.SubPoint != null ? _ScOffPoint.SubPoint.AlarmLevel : EnmAlarm.Level2,
                                            AlarmFlag = EnmFlag.Begin,
                                            AlarmValue = 0,
                                            AlarmDesc = null,
                                            AlarmRemark = null
                                        };
                                        current.Id = string.Format("{0}-{1}-{2}-{3}", current.SerialNo, current.PointId, current.DeviceId, current.FsuId);

                                        _alarmer.Start(current);
                                        GlobalConfig.AddAlarm(current);
                                        #endregion
                                    }
                                }
                            }
                        } catch (Exception err) {
                            Logger.Error(err.Message, err);
                        }
                        #endregion

                        #region FSU通信中断
                        try {
                            if (_FsuOffPoint != null) {
                                var extFsus = _fsuRepository.GetExtEntities();
                                foreach (var ext in extFsus) {
                                    var _alarm = _aalmRepository.GetEntityInPoint(ext.Id, _FsuOffPoint.Current.Id);
                                    if (ext.Status && _alarm != null) {
                                        #region 结束告警
                                        var current = new AlarmEnd {
                                            Id = _alarm.Id,
                                            AreaId = _alarm.AreaId,
                                            StationId = _alarm.StationId,
                                            RoomId = _alarm.RoomId,
                                            FsuId = _alarm.FsuId,
                                            FsuCode = _alarm.FsuId,
                                            DeviceId = _alarm.DeviceId,
                                            DeviceCode = _alarm.DeviceId,
                                            PointId = _alarm.PointId,
                                            SerialNo = _alarm.SerialNo,
                                            NMAlarmId = _alarm.NMAlarmId,
                                            StartTime = _alarm.AlarmTime,
                                            EndTime = DateTime.Now,
                                            AlarmLevel = _alarm.AlarmLevel,
                                            AlarmFlag = EnmFlag.End,
                                            StartValue = _alarm.AlarmValue,
                                            EndValue = 1,
                                            AlarmDesc = _alarm.AlarmDesc,
                                            AlarmRemark = _alarm.AlarmRemark,
                                            Confirmed = _alarm.Confirmed,
                                            Confirmer = _alarm.Confirmer,
                                            ConfirmedTime = _alarm.ConfirmedTime,
                                            ReservationId = _alarm.ReservationId,
                                            PrimaryId = _alarm.PrimaryId,
                                            RelatedId = _alarm.RelatedId,
                                            FilterId = _alarm.FilterId,
                                            ReversalId = _alarm.ReversalId,
                                            ReversalCount = _alarm.ReversalCount,
                                            Masked = _alarm.Masked
                                        };

                                        _alarmer.End(current);
                                        GlobalConfig.RemoveAlarm(current);
                                        #endregion
                                    } else if(!ext.Status && _alarm == null) {
                                        #region 开始告警
                                        var fsu = iPemWorkContext.Fsus.Find(d => d.Current.Id == ext.Id);
                                        if (fsu != null) {
                                            _FsuOffPoint.SubPoint = iPemWorkContext.SubPoints.Find(p => p.PointId == _FsuOffPoint.Current.Id && p.StationType.Id == fsu.Current.StationType.Id);
                                            var current = new AlarmStart {
                                                Id = null,
                                                AreaId = fsu.Current.AreaId,
                                                StationId = fsu.Current.StationId,
                                                RoomId = fsu.Current.RoomId,
                                                FsuId = fsu.Current.Id,
                                                FsuCode = fsu.Current.Code,
                                                DeviceId = fsu.Current.Id,
                                                DeviceCode = fsu.Current.Code,
                                                PointId = _FsuOffPoint.Current.Id,
                                                SerialNo = CommonHelper.GetIdAsString(),
                                                NMAlarmId = _FsuOffPoint.Current.NMAlarmId,
                                                AlarmTime = DateTime.Now,
                                                AlarmLevel = _FsuOffPoint.SubPoint != null ? _FsuOffPoint.SubPoint.AlarmLevel : EnmAlarm.Level2,
                                                AlarmFlag = EnmFlag.Begin,
                                                AlarmValue = 0,
                                                AlarmDesc = null,
                                                AlarmRemark = null
                                            };
                                            current.Id = string.Format("{0}-{1}-{2}-{3}", current.SerialNo, current.PointId, current.DeviceId, current.FsuId);

                                            _alarmer.Start(current);
                                            GlobalConfig.AddAlarm(current);
                                        }
                                        #endregion
                                    }
                                }
                            }
                        } catch (Exception err) {
                            Logger.Error(err.Message, err);
                        }
                        #endregion

                    } catch (Exception err) {
                        Logger.Error(err.Message, err);
                    } finally {
                        _configWriterLock.ExitReadLock();
                    }
                }
            }
        }

        /// <summary>
        /// SC心跳任务
        /// </summary>
        private void DoScHeartbeat() {
            _allDone.WaitOne();

            Logger.Information("SC心跳线程已启动。");
            while (_runStatus != RunStatus.Stop) {
                Thread.Sleep(1000);
                if (_runStatus == RunStatus.Running) {
                    try {
                        _configWriterLock.EnterReadLock();
                        foreach (var beat in GlobalConfig.ScHeartbeats) {
                            try {
                                #region 发送心跳
                                if (beat.IsOk) {
                                    var package = beat.Keep();
                                    if (package.Result == EnmBIResult.FAILURE) throw new Exception("SC通信中断");
                                    #region 通信正常
                                    try {
                                        if (!beat.Current.Status) {
                                            var now = DateTime.Now;
                                            _groupRepository.SetOn(beat.Current.Id, now);
                                            beat.Current.Status = true;
                                            beat.Current.ChangeTime = now;
                                        }
                                    } catch (Exception err) {
                                        Logger.Error(err.Message, err);
                                    } finally {
                                        beat.SetCount(true);
                                    }
                                    #endregion
                                }
                                #endregion
                            } catch {
                                #region 通信中断
                                try {
                                    if (beat.IsOff && beat.Current.Status) {
                                        var now = DateTime.Now;
                                        _groupRepository.SetOff(beat.Current.Id, now);
                                        beat.Current.Status = false;
                                        beat.Current.LastTime = now;
                                    }
                                } catch (Exception err) {
                                    Logger.Error(err.Message, err);
                                } finally {
                                    beat.SetCount();
                                }
                                #endregion
                            } finally {
                                beat.SetInterval();
                            }
                        }
                    } catch (Exception err) {
                        Logger.Error(err.Message, err);
                    } finally {
                        _configWriterLock.ExitReadLock();
                    }
                }
            }
        }

        /// <summary>
        /// 能耗处理任务
        /// </summary>
        private void DoTask001() {
            _allDone.WaitOne();

            var _curTask = GlobalConfig.CurTasks.Find(t => t.Id == "T001");
            if (_curTask == null) return;

            Logger.Information(string.Format("{0}线程已启动。", _curTask.Name));
            if (_curTask.Json == null) {
                Logger.Warning(string.Format("{0}配置无效，线程退出。", _curTask.Name));
                return;
            }

            if (_curTask.Json.EndDate < DateTime.Now) {
                Logger.Warning(string.Format("{0}已过期，线程退出。", _curTask.Name));
                return;
            }

            if (_curTask.Json.StartDate > DateTime.Now) {
                Logger.Warning(string.Format("{0}未到执行日期，将从{1}开始执行。", _curTask.Name, CommonHelper.ToDateString(_curTask.Json.StartDate)));
            }

            while (_runStatus != RunStatus.Stop) {
                Thread.Sleep(1000);
                if (_runStatus == RunStatus.Running) {
                    if (_curTask.Json.StartDate > DateTime.Now) continue;//执行日期未到 
                    if (_curTask.Next > DateTime.Now) continue;//执行时间未到
                    if (_curTask.Json.EndDate < DateTime.Now) {
                        Logger.Warning(string.Format("{0}已过期，线程退出。", _curTask.Name));
                        break;
                    }

                    try {
                        var _times = CommonHelper.GetHourSpan(_curTask.Start, _curTask.End);
                        if (_times.Count == 0) continue;

                        var _formulas = _formulaRepository.GetEntities();
                        foreach (var _formula in _formulas) {
                            try {
                                var _formulaText = _formula.FormulaText;
                                if (string.IsNullOrWhiteSpace(_formulaText)) continue;
                                if (!CommonHelper.ValidateFormula(_formulaText)) throw new Exception("无效的公式。");
                                var _variables = CommonHelper.GetFormulaVariables(_formulaText);
                                if (_variables == null) throw new Exception("无效的公式。");

                                var _devices = new List<WcDevice>();
                                if (_formula.Type == EnmOrganization.Station) {
                                    var _current = iPemWorkContext.Stations.Find(c => c.Current.Id == _formula.Id);
                                    if (_current == null) throw new Exception("未找到公式所对应的站点。");
                                    _devices.AddRange(_current.Rooms.SelectMany(r => r.Devices));
                                } else if (_formula.Type == EnmOrganization.Room) {
                                    var _current = iPemWorkContext.Rooms.Find(c => c.Current.Id == _formula.Id);
                                    if (_current == null) throw new Exception("未找到公式所对应的机房。");
                                    _devices.AddRange(_current.Devices);
                                }

                                var _details = new List<VariableDetail>();
                                foreach (var _variable in _variables) {
                                    var _factors = _variable.Split(new string[] { ">>" }, StringSplitOptions.None);
                                    var _devkey = _factors[0].Substring(1);
                                    var _potkey = _factors[1];
                                    var _device = _devices.Find(d => d.Current.Name == _devkey);
                                    if (_device == null) throw new Exception(string.Format("未找到变量{0}中的设备信息。", _variable));
                                    var _point = _device.Points.Find(p => p.Current.Name == _potkey);
                                    if (_point == null) throw new Exception(string.Format("未找到变量{0}中的信号信息。", _variable));
                                    _details.Add(new VariableDetail { Device = _device.Current, Point = _point.Current, Variable = _variable });
                                }

                                var _result = new List<V_Elec>();
                                foreach (var _time in _times) {
                                    var _start = _time;
                                    var _end = _time.AddHours(1).AddMilliseconds(-1);
                                    var _current = _formulaText;
                                    var _value = 0d;
                                    try {
                                        foreach (var _detail in _details) {
                                            var _diff = _measureRepository.GetValDiff(_detail.Device.Id, _detail.Point.Code, _detail.Point.Number, _start, _end);
                                            _current = _current.Replace(_detail.Variable, _diff.ToString());
                                        }

                                        var __value = _computer.Compute(_current, "");
                                        if (__value != DBNull.Value) _value = Convert.ToDouble(__value);
                                        if (double.IsNaN(_value) || double.IsInfinity(_value)) _value = 0d;
                                    } catch { }

                                    _result.Add(new V_Elec {
                                        Id = _formula.Id,
                                        Type = _formula.Type,
                                        FormulaType = _formula.FormulaType,
                                        StartTime = _start,
                                        EndTime = _end,
                                        Value = _value
                                    });
                                }

                                _elecRepository.DeleteEntities(_formula.Id, _formula.Type, _formula.FormulaType, _curTask.Start, _curTask.End);
                                _elecRepository.SaveEntities(_result);
                            } catch (Exception err) {
                                Logger.Error(err.Message);
                                Logger.Error(string.Format("能耗处理发生错误,详见错误日志({0})。", JsonConvert.SerializeObject(_formula)), err);
                            }
                        }
                    } catch (Exception err) {
                        Logger.Error(err.Message, err);
                    } finally {
                        GlobalConfig.SetTaskPloy(_curTask);
                        Logger.Information(string.Format("{0}执行完成，下次执行时间：{1}。", _curTask.Name, CommonHelper.ToDateTimeString(_curTask.Next)));
                    }
                }
            }
        }

        /// <summary>
        /// 电池放电处理任务
        /// </summary>
        private void DoTask002() {
            _allDone.WaitOne();

            var _curTask = GlobalConfig.CurTasks.Find(t => t.Id == "T002");
            if (_curTask == null) return;

            Logger.Information(string.Format("{0}线程已启动。", _curTask.Name));
            if (_curTask.Json == null) {
                Logger.Warning(string.Format("{0}配置无效，线程退出。", _curTask.Name));
                return;
            }

            if (_curTask.Json.EndDate < DateTime.Now) {
                Logger.Warning(string.Format("{0}已过期，线程退出。", _curTask.Name));
                return;
            }

            if (_curTask.Json.StartDate > DateTime.Now) {
                Logger.Warning(string.Format("{0}未到执行日期，将从{1}开始执行。", _curTask.Name, CommonHelper.ToDateString(_curTask.Json.StartDate)));
            }

            while (_runStatus != RunStatus.Stop) {
                Thread.Sleep(1000);
                if (_runStatus == RunStatus.Running) {
                    if (_curTask.Json.StartDate > DateTime.Now) continue;//执行日期未到 
                    if (_curTask.Next > DateTime.Now) continue;//执行时间未到
                    if (_curTask.Json.EndDate < DateTime.Now) {
                        Logger.Warning(string.Format("{0}已过期，线程退出。", _curTask.Name));
                        break;
                    }

                    try {
                        foreach (var model in GlobalConfig.BatModels) {
                            try {
                                var _values = _measureRepository.GetEntities(model.Device.Id, model.Point.Code, model.Point.Number, _curTask.Start, _curTask.End);
                                if (_values.Count == 0) continue;

                                var _details = new List<BatDetail>();
                                BatDetail _current = null;
                                foreach (var _value in _values) {
                                    if (_value.Value < model.Voltage) {
                                        if (_current == null) _current = new BatDetail { Device = model.Device, Point = model.Point, PackId = model.PackId, StartTime = _value.UpdateTime, StartValue = _value.Value, Values = new List<IdValuePair<DateTime, double>>() };
                                        _current.Values.Add(new IdValuePair<DateTime, double> { Id = _value.UpdateTime, Value = _value.Value });
                                    } else if (_value.Value >= model.Voltage && _current != null) {
                                        _current.EndTime = _value.UpdateTime;
                                        _current.EndValue = _value.Value;
                                        _current.Values.Add(new IdValuePair<DateTime, double> { Id = _value.UpdateTime, Value = _value.Value });
                                        _details.Add(_current);
                                        _current = null;
                                    }
                                }

                                if (_current != null) {
                                    var _last = _current.Values.LastOrDefault();
                                    if (_last != null) {
                                        _current.EndTime = _last.Id;
                                        _current.EndValue = _last.Value;
                                        _details.Add(_current);
                                    }

                                    _current = null;
                                }

                                var __details = new List<BatDetail>();
                                foreach (var _point in model.SubPoints) {
                                    foreach (var _detail in _details) {
                                        var __values = _measureRepository.GetEntities(model.Device.Id, _point.Code, _point.Number, _detail.StartTime, _detail.EndTime);
                                        if (__values.Count > 0) {
                                            var _first = __values.First();
                                            var _last = __values.Last();
                                            __details.Add(new BatDetail {
                                                Device = model.Device,
                                                Point = _point,
                                                PackId = model.PackId,
                                                StartTime = _first.UpdateTime,
                                                StartValue = _first.Value,
                                                EndTime = _last.UpdateTime,
                                                EndValue = _last.Value,
                                                Values = __values.Select(v => new IdValuePair<DateTime, double> { Id = v.UpdateTime, Value = v.Value }).ToList()
                                            });
                                        }
                                    }
                                }

                                var batValues = new List<V_Bat>();
                                var batTimes = new List<V_BatTime>();
                                foreach (var _detail in _details) {
                                    batTimes.Add(new V_BatTime {
                                        AreaId = _detail.Device.AreaId,
                                        StationId = _detail.Device.StationId,
                                        RoomId = _detail.Device.RoomId,
                                        DeviceId = _detail.Device.Id,
                                        PointId = _detail.Point.Id,
                                        PackId = _detail.PackId,
                                        StartTime = _detail.StartTime,
                                        StartValue = _detail.StartValue,
                                        EndTime = _detail.EndTime,
                                        EndValue = _detail.EndValue
                                    });

                                    foreach (var _value in _detail.Values) {
                                        batValues.Add(new V_Bat {
                                            AreaId = _detail.Device.AreaId,
                                            StationId = _detail.Device.StationId,
                                            RoomId = _detail.Device.RoomId,
                                            DeviceId = _detail.Device.Id,
                                            PointId = _detail.Point.Id,
                                            PackId = _detail.PackId,
                                            StartTime = _detail.StartTime,
                                            Value = _value.Value,
                                            ValueTime = _value.Id
                                        });
                                    }
                                }

                                foreach (var __detail in __details) {
                                    batTimes.Add(new V_BatTime {
                                        AreaId = __detail.Device.AreaId,
                                        StationId = __detail.Device.StationId,
                                        RoomId = __detail.Device.RoomId,
                                        DeviceId = __detail.Device.Id,
                                        PointId = __detail.Point.Id,
                                        PackId = __detail.PackId,
                                        StartTime = __detail.StartTime,
                                        StartValue = __detail.StartValue,
                                        EndTime = __detail.EndTime,
                                        EndValue = __detail.EndValue
                                    });

                                    foreach (var __value in __detail.Values) {
                                        batValues.Add(new V_Bat {
                                            AreaId = __detail.Device.AreaId,
                                            StationId = __detail.Device.StationId,
                                            RoomId = __detail.Device.RoomId,
                                            DeviceId = __detail.Device.Id,
                                            PointId = __detail.Point.Id,
                                            PackId = __detail.PackId,
                                            StartTime = __detail.StartTime,
                                            Value = __value.Value,
                                            ValueTime = __value.Id
                                        });
                                    }
                                }

                                _batRepository.DeleteEntities(model.Device.Id, model.Point.Id, model.PackId, _curTask.Start, _curTask.End);
                                _batRepository.SaveEntities(batValues);

                                _batTimeRepository.DeleteEntities(model.Device.Id, model.Point.Id, model.PackId, _curTask.Start, _curTask.End);
                                _batTimeRepository.SaveEntities(batTimes);
                            } catch (Exception err) {
                                Logger.Error(err.Message);
                                Logger.Error("电池放电统计发生错误,详见错误日志。", err);
                            }
                        }
                    } catch (Exception err) {
                        Logger.Error(err.Message, err);
                    } finally {
                        GlobalConfig.SetTaskPloy(_curTask);
                        Logger.Information(string.Format("{0}执行完成，下次执行时间：{1}。", _curTask.Name, CommonHelper.ToDateTimeString(_curTask.Next)));
                    }
                }
            }
        }

        /// <summary>
        /// 信号测值统计任务
        /// </summary>
        private void DoTask003() {
            _allDone.WaitOne();

            var _curTask = GlobalConfig.CurTasks.Find(t => t.Id == "T003");
            if (_curTask == null) return;

            Logger.Information(string.Format("{0}线程已启动。", _curTask.Name));
            if (_curTask.Json == null) {
                Logger.Warning(string.Format("{0}配置无效，线程退出。", _curTask.Name));
                return;
            }

            if (_curTask.Json.EndDate < DateTime.Now) {
                Logger.Warning(string.Format("{0}已过期，线程退出。", _curTask.Name));
                return;
            }

            if (_curTask.Json.StartDate > DateTime.Now) {
                Logger.Warning(string.Format("{0}未到执行日期，将从{1}开始执行。", _curTask.Name, CommonHelper.ToDateString(_curTask.Json.StartDate)));
            }

            while (_runStatus != RunStatus.Stop) {
                Thread.Sleep(1000);
                if (_runStatus == RunStatus.Running) {
                    if (_curTask.Json.StartDate > DateTime.Now) continue;//执行日期未到 
                    if (_curTask.Next > DateTime.Now) continue;//执行时间未到
                    if (_curTask.Json.EndDate < DateTime.Now) {
                        Logger.Warning(string.Format("{0}已过期，线程退出。", _curTask.Name));
                        break;
                    }

                    try {
                        foreach (var model in GlobalConfig.StaticModels) {
                            if (model.Interval <= 0) continue;

                            var _result = new List<V_Static>();
                            var _start = _curTask.Start;
                            var _end = _curTask.Start.AddMinutes(model.Interval);
                            while (_curTask.End >= _end) {
                                try {
                                    var _values = _measureRepository.GetEntities(model.Device.Id, model.Point.Code, model.Point.Number, _start, _end);
                                    if (_values.Count > 0) {
                                        var _target = new V_Static {
                                            AreaId = model.Device.AreaId,
                                            StationId = model.Device.StationId,
                                            RoomId = model.Device.RoomId,
                                            DeviceId = model.Device.Id,
                                            PointId = model.Point.Id,
                                            StartTime = _start,
                                            EndTime = _end,
                                            MaxValue = double.MinValue,
                                            MinValue = double.MaxValue,
                                            AvgValue = _values.Average(v => v.Value),
                                            Total = _values.Count
                                        };

                                        foreach (var _v in _values) {
                                            if (_v.Value > _target.MaxValue) {
                                                _target.MaxValue = _v.Value;
                                                _target.MaxTime = _v.UpdateTime;
                                            }

                                            if (_v.Value < _target.MinValue) {
                                                _target.MinValue = _v.Value;
                                                _target.MinTime = _v.UpdateTime;
                                            }
                                        }

                                        _result.Add(_target);
                                    }
                                } catch (Exception err) {
                                    Logger.Error(err.Message);
                                    Logger.Error("信号测值统计发生错误,详见错误日志。", err);
                                } finally {
                                    _start = _end;
                                    _end = _end.AddMinutes(model.Interval);
                                }
                            }

                            _staticRepository.DeleteEntities(model.Device.Id, model.Point.Id, _curTask.Start, _curTask.End);
                            _staticRepository.SaveEntities(_result);
                        }
                    } catch (Exception err) {
                        Logger.Error(err.Message, err);
                    } finally {
                        GlobalConfig.SetTaskPloy(_curTask);
                        Logger.Information(string.Format("{0}执行完成，下次执行时间：{1}。", _curTask.Name, CommonHelper.ToDateTimeString(_curTask.Next)));
                    }
                }
            }
        }

        /// <summary>
        /// 开关电源带载率统计任务
        /// </summary>
        private void DoTask004() {
            _allDone.WaitOne();

            var _curTask = GlobalConfig.CurTasks.Find(t => t.Id == "T004");
            if (_curTask == null) return;

            Logger.Information(string.Format("{0}线程已启动。", _curTask.Name));
            if (_curTask.Json == null) {
                Logger.Warning(string.Format("{0}配置无效，线程退出。", _curTask.Name));
                return;
            }

            if (_curTask.Json.EndDate < DateTime.Now) {
                Logger.Warning(string.Format("{0}已过期，线程退出。", _curTask.Name));
                return;
            }

            if (_curTask.Json.StartDate > DateTime.Now) {
                Logger.Warning(string.Format("{0}未到执行日期，将从{1}开始执行。", _curTask.Name, CommonHelper.ToDateString(_curTask.Json.StartDate)));
            }

            #region 定义变量
            var _FzdlParam = GlobalConfig.CurParams.Find(p => p.Id == ParamId.FZDL);
            if (_FzdlParam == null || string.IsNullOrWhiteSpace(_FzdlParam.Value)) {
                Logger.Warning(string.Format("{0}尚未配置负载电流信号，线程退出。", _curTask.Name));
                return;
            }

            var _FzdlPoint = iPemWorkContext.Points.Find(p => p.Id == _FzdlParam.Value);
            if (_FzdlPoint == null) {
                Logger.Warning(string.Format("{0}负载电流信号配置错误，线程退出。", _curTask.Name));
                return;
            }

            var _GzztParam = GlobalConfig.CurParams.Find(p => p.Id == ParamId.GZZT);
            if (_GzztParam == null || string.IsNullOrWhiteSpace(_GzztParam.Value)) {
                Logger.Warning(string.Format("{0}尚未配置工作状态信号，线程退出。", _curTask.Name));
                return;
            }

            var _GzztPoint = iPemWorkContext.Points.Find(p => p.Id == _GzztParam.Value);
            if (_GzztPoint == null) {
                Logger.Warning(string.Format("{0}工作状态信号配置错误，线程退出。", _curTask.Name));
                return;
            }
            #endregion

            while (_runStatus != RunStatus.Stop) {
                Thread.Sleep(1000);
                if (_runStatus == RunStatus.Running) {
                    if (_curTask.Json.StartDate > DateTime.Now) continue;//执行日期未到 
                    if (_curTask.Next > DateTime.Now) continue;//执行时间未到
                    if (_curTask.Json.EndDate < DateTime.Now) {
                        Logger.Warning(string.Format("{0}已过期，线程退出。", _curTask.Name));
                        break;
                    }

                    try {
                        var _dates = CommonHelper.GetDateSpan(_curTask.Start, _curTask.End);
                        if (_dates.Count == 0) continue;

                        var _comDevices = _combSwitElecSourRepository.GetEntities();
                        var _divDevices = _divSwitElecSourRepository.GetEntities();

                        #region 组合开关电源
                        foreach (var _device in _comDevices) {
                            try {
                                var _current = iPemWorkContext.Devices.Find(d => d.Current.Id == _device.Id);
                                if (_current == null) continue;
                                var _ztPoint = _current.Points.Find(p => p.Current.Type == EnmPoint.DI && p.Current.Id == _FzdlPoint.Id);
                                if (_ztPoint == null) continue;
                                var _fzPoint = _current.Points.Find(p => p.Current.Type == EnmPoint.AI && p.Current.Id == _GzztPoint.Id);
                                if (_fzPoint == null) continue;
                                var _fzCap = _device.SingRModuleRatedOPCap * _device.ExisRModuleCount;
                                if (_fzCap == 0) continue;

                                var _result = new List<V_Load>();
                                var _ztFlag = CommonHelper.GetPointFlag(_ztPoint.Current, "浮充");
                                foreach (var _date in _dates) {
                                    var _load = new V_Load {
                                        AreaId = _device.AreaId,
                                        StationId = _device.StationId,
                                        RoomId = _device.RoomId,
                                        DeviceId = _device.Id,
                                        StartTime = _date,
                                        EndTime = _date.AddDays(1).AddMilliseconds(-1),
                                        Value = 0,
                                    };

                                    var _ztValues = _measureRepository.GetEntities(_device.Id, _ztPoint.Current.Code, _ztPoint.Current.Number, _load.StartTime, _load.EndTime);
                                    var _intervals = new List<IdValuePair<DateTime, DateTime>>();

                                    DateTime? _start = null;
                                    foreach (var _value in _ztValues.OrderBy(v => v.UpdateTime)) {
                                        if (_value.Value == _ztFlag && !_start.HasValue) {
                                            _start = _value.UpdateTime;
                                        } else if (_value.Value != _ztFlag && _start.HasValue) {
                                            _intervals.Add(new IdValuePair<DateTime, DateTime> {
                                                Id = _start.Value,
                                                Value = _value.UpdateTime
                                            });

                                            _start = null;
                                        }
                                    }

                                    if (_start.HasValue) {
                                        _intervals.Add(new IdValuePair<DateTime, DateTime> {
                                            Id = _start.Value,
                                            Value = _load.EndTime
                                        });

                                        _start = null;
                                    }

                                    if (_intervals.Count > 0) {
                                        var _fzValues = _measureRepository.GetEntities(_device.Id, _fzPoint.Current.Code, _fzPoint.Current.Number, _load.StartTime, _load.EndTime);
                                        foreach (var _interval in _intervals) {
                                            var _fzMatch = _fzValues.FindAll(f => f.UpdateTime >= _interval.Id && f.UpdateTime <= _interval.Value);
                                            var _fzMax = _fzMatch.Max(f => f.Value);
                                            if (_fzMax > _load.Value) _load.Value = _fzMax;
                                        }
                                        _load.Value = _load.Value / _fzCap;
                                    }

                                    _result.Add(_load);
                                }

                                _loadRepository.DeleteEntities(_device.Id, _curTask.Start, _curTask.End);
                                _loadRepository.SaveEntities(_result);
                            } catch (Exception err) {
                                Logger.Error(err.Message);
                                Logger.Error("组合开关电源带载率统计发生错误,详见错误日志。", err);
                            }
                        }
                        #endregion

                        #region 分立开关电源
                        foreach (var _device in _divDevices) {
                            try {
                                var _current = iPemWorkContext.Devices.Find(d => d.Current.Id == _device.Id);
                                if (_current == null) continue;
                                var _ztPoint = _current.Points.Find(p => p.Current.Type == EnmPoint.DI && p.Current.Id == _FzdlPoint.Id);
                                if (_ztPoint == null) continue;
                                var _fzPoint = _current.Points.Find(p => p.Current.Type == EnmPoint.AI && p.Current.Id == _GzztPoint.Id);
                                if (_fzPoint == null) continue;
                                var _fzCap = _device.SingRModuleRatedOPCap * _device.ExisRModuleCount;
                                if (_fzCap == 0) continue;

                                var _result = new List<V_Load>();
                                var _ztFlag = CommonHelper.GetPointFlag(_ztPoint.Current, "浮充");
                                foreach (var _date in _dates) {
                                    var _load = new V_Load {
                                        AreaId = _device.AreaId,
                                        StationId = _device.StationId,
                                        RoomId = _device.RoomId,
                                        DeviceId = _device.Id,
                                        StartTime = _date,
                                        EndTime = _date.AddDays(1).AddMilliseconds(-1),
                                        Value = 0,
                                    };

                                    var _ztValues = _measureRepository.GetEntities(_device.Id, _ztPoint.Current.Code, _ztPoint.Current.Number, _load.StartTime, _load.EndTime);
                                    var _intervals = new List<IdValuePair<DateTime, DateTime>>();

                                    DateTime? _start = null;
                                    foreach (var _value in _ztValues.OrderBy(v => v.UpdateTime)) {
                                        if (_value.Value == _ztFlag && !_start.HasValue) {
                                            _start = _value.UpdateTime;
                                        } else if (_value.Value != _ztFlag && _start.HasValue) {
                                            _intervals.Add(new IdValuePair<DateTime, DateTime> {
                                                Id = _start.Value,
                                                Value = _value.UpdateTime
                                            });

                                            _start = null;
                                        }
                                    }

                                    if (_start.HasValue) {
                                        _intervals.Add(new IdValuePair<DateTime, DateTime> {
                                            Id = _start.Value,
                                            Value = _load.EndTime
                                        });

                                        _start = null;
                                    }

                                    if (_intervals.Count > 0) {
                                        var _fzValues = _measureRepository.GetEntities(_device.Id, _fzPoint.Current.Code, _fzPoint.Current.Number, _load.StartTime, _load.EndTime);
                                        foreach (var _interval in _intervals) {
                                            var _fzMatch = _fzValues.FindAll(f => f.UpdateTime >= _interval.Id && f.UpdateTime <= _interval.Value);
                                            var _fzMax = _fzMatch.Max(f => f.Value);
                                            if (_fzMax > _load.Value) _load.Value = _fzMax;
                                        }
                                        _load.Value = _load.Value / _fzCap;
                                    }

                                    _result.Add(_load);
                                }

                                _loadRepository.DeleteEntities(_device.Id, _curTask.Start, _curTask.End);
                                _loadRepository.SaveEntities(_result);
                            } catch (Exception err) {
                                Logger.Error(err.Message);
                                Logger.Error("分立开关电源带载率统计发生错误,详见错误日志。", err);
                            }
                        }
                        #endregion

                    } catch (Exception err) {
                        Logger.Error(err.Message, err);
                    } finally {
                        GlobalConfig.SetTaskPloy(_curTask);
                        Logger.Information(string.Format("{0}执行完成，下次执行时间：{1}。", _curTask.Name, CommonHelper.ToDateTimeString(_curTask.Next)));
                    }
                }
            }
        }

        /// <summary>
        /// 资管接口同步任务
        /// </summary>
        private void DoTask005() {
            _allDone.WaitOne();

            var _curTask = GlobalConfig.CurTasks.Find(t => t.Id == "T005");
            if (_curTask == null) return;

            Logger.Information(string.Format("{0}线程已启动。", _curTask.Name));
            if (_curTask.Json == null) {
                Logger.Warning(string.Format("{0}配置无效，线程退出。", _curTask.Name));
                return;
            }

            if (_curTask.Json.EndDate < DateTime.Now) {
                Logger.Warning(string.Format("{0}已过期，线程退出。", _curTask.Name));
                return;
            }

            if (_curTask.Json.StartDate > DateTime.Now) {
                Logger.Warning(string.Format("{0}未到执行日期，将从{1}开始执行。", _curTask.Name, CommonHelper.ToDateString(_curTask.Json.StartDate)));
            }

            while (_runStatus != RunStatus.Stop) {
                Thread.Sleep(1000);
                if (_runStatus == RunStatus.Running) {
                    if (_curTask.Json.StartDate > DateTime.Now) continue;//执行日期未到 
                    if (_curTask.Next > DateTime.Now) continue;//执行时间未到
                    if (_curTask.Json.EndDate < DateTime.Now) {
                        Logger.Warning(string.Format("{0}已过期，线程退出。", _curTask.Name));
                        break;
                    }

                    try {

                        #region 处理接口设备数据
                        try {
                            var _iDevices = new List<H_IDevice>();
                            foreach (var _device in _deviceRepository.GetEntities()) {
                                _iDevices.Add(new H_IDevice {
                                    Id = _device.Id,
                                    Name = _device.Name,
                                    TypeId = _device.Type.Id,
                                    TypeName = _device.Type.Name,
                                    StationId = _device.StationId
                                });
                            }

                            _iDeviceRepository.SaveEntities(_iDevices,DateTime.Now);
                        } catch (Exception err) {
                            Logger.Error(err.Message);
                            Logger.Error("资管接口同步设备发生错误,详见错误日志。", err);
                        }
                        #endregion

                        #region 处理接口站点数据
                        try {
                            var _iStations = new List<H_IStation>();
                            foreach (var _station in _stationRepository.GetEntities()) {
                                _iStations.Add(new H_IStation {
                                    Id = _station.Id,
                                    Name = _station.Name,
                                    TypeId = _station.Type.Id,
                                    TypeName = _station.Type.Name,
                                    AreaId = _station.AreaId
                                });
                            }

                            _iStationRepository.SaveEntities(_iStations, DateTime.Now);
                        } catch (Exception err) {
                            Logger.Error(err.Message);
                            Logger.Error("资管接口同步站点发生错误,详见错误日志。", err);
                        }
                        #endregion

                        #region 处理接口区域数据
                        try {
                            var _iAreas = new List<H_IArea>();
                            foreach (var _area in _areaRepository.GetEntities()) {
                                _iAreas.Add(new H_IArea {
                                    Id = _area.Id,
                                    Name = _area.Name,
                                    TypeId = _area.Type.Id.ToString(),
                                    TypeName = _area.Type.Value,
                                    ParentId = _area.ParentId
                                });
                            }

                            _iAreaRepository.SaveEntities(_iAreas, DateTime.Now);
                        } catch (Exception err) {
                            Logger.Error(err.Message);
                            Logger.Error("资管接口同步区域发生错误,详见错误日志。", err);
                        }
                        #endregion

                    } catch (Exception err) {
                        Logger.Error(err.Message, err);
                    } finally {
                        GlobalConfig.SetTaskPloy(_curTask);
                        Logger.Information(string.Format("{0}执行完成，下次执行时间：{1}。", _curTask.Name, CommonHelper.ToDateTimeString(_curTask.Next)));
                    }
                }
            }
        }

        /// <summary>
        /// 参数自检处理任务
        /// </summary>
        private void DoTask006() {
            _allDone.WaitOne();

            var _curTask = GlobalConfig.CurTasks.Find(t => t.Id == "T006");
            if (_curTask == null) return;

            Logger.Information(string.Format("{0}线程已启动。", _curTask.Name));
            if (_curTask.Json == null) {
                Logger.Warning(string.Format("{0}配置无效，线程退出。", _curTask.Name));
                return;
            }

            if (_curTask.Json.EndDate < DateTime.Now) {
                Logger.Warning(string.Format("{0}已过期，线程退出。", _curTask.Name));
                return;
            }

            if (_curTask.Json.StartDate > DateTime.Now) {
                Logger.Warning(string.Format("{0}未到执行日期，将从{1}开始执行。", _curTask.Name, CommonHelper.ToDateString(_curTask.Json.StartDate)));
            }

            while (_runStatus != RunStatus.Stop) {
                Thread.Sleep(1000);
                if (_runStatus == RunStatus.Running) {
                    if (_curTask.Json.StartDate > DateTime.Now) continue;//执行日期未到 
                    if (_curTask.Next > DateTime.Now) continue;//执行时间未到
                    if (_curTask.Json.EndDate < DateTime.Now) {
                        Logger.Warning(string.Format("{0}已过期，线程退出。", _curTask.Name));
                        break;
                    }

                    try {
                        var diffs = new List<V_ParamDiff>();
                        foreach (var param in _redefinePointRepository.GetEntities()) {
                            var device = iPemWorkContext.Devices.Find(d => d.Current.Id == param.DeviceId);
                            if (device == null) continue;

                            var point = device.Points.Find(p => p.Current.Id == param.PointId);
                            if (point == null) continue;
                            if (point.SubPoint == null) continue;

                            var diff = new V_ParamDiff();
                            if (param.AlarmLimit != point.SubPoint.AlarmLimit) diff.Threshold = string.Format("{0}&{1}", param.AlarmLimit, point.SubPoint.AlarmLimit);
                            if (param.AlarmLevel != (int)point.SubPoint.AlarmLevel) diff.AlarmLevel = string.Format("{0}&{1}", param.AlarmLevel, (int)point.SubPoint.AlarmLevel);
                            if (param.NMAlarmId != point.Current.NMAlarmId) diff.NMAlarmID = string.Format("{0}&{1}", param.NMAlarmId, point.Current.NMAlarmId);
                            if (param.AbsoluteThreshold != point.SubPoint.AbsoluteThreshold) diff.AbsoluteVal = string.Format("{0}&{1}", param.AbsoluteThreshold, point.SubPoint.AbsoluteThreshold);
                            if (param.PerThreshold != point.SubPoint.PerThreshold) diff.RelativeVal = string.Format("{0}&{1}", param.PerThreshold, point.SubPoint.PerThreshold);
                            if (param.SavedPeriod != point.SubPoint.SavedPeriod) diff.StorageInterval = string.Format("{0}&{1}", param.SavedPeriod, point.SubPoint.SavedPeriod);
                            diffs.Add(diff);
                        }

                        if (diffs.Count > 0) _paramDiffRepository.SaveEntities(diffs, DateTime.Now);
                    } catch (Exception err) {
                        Logger.Error(err.Message, err);
                    } finally {
                        GlobalConfig.SetTaskPloy(_curTask);
                        Logger.Information(string.Format("{0}执行完成，下次执行时间：{1}。", _curTask.Name, CommonHelper.ToDateTimeString(_curTask.Next)));
                    }
                }
            }
        }

        /// <summary>
        /// 重启服务
        /// </summary>
        private void Restart() {
            Logger.Information("重启服务...");
            this.OnStop();
            Environment.Exit(-1);
        }

        /// <summary>
        /// 加载基础数据
        /// </summary>
        private void LoadBase() {
            iPemWorkContext.LogicTypes = _logicTypeRepository.GetEntities();
            iPemWorkContext.SubLogicTypes = _logicTypeRepository.GetSubEntities();
            iPemWorkContext.DeviceTypes = _deviceTypeRepository.GetEntities();
            iPemWorkContext.SubDeviceTypes = _deviceTypeRepository.GetSubEntities();
            iPemWorkContext.RoomTypes = _roomTypeRepository.GetEntities();
            iPemWorkContext.StationTypes = _stationTypeRepository.GetEntities();
            iPemWorkContext.AreaTypes = _enumMethodsRepository.GetEntities(EnmMethodType.Area, "类型");
            iPemWorkContext.Points = _pointRepository.GetEntities();
            iPemWorkContext.SubPoints = _subPointRepository.GetEntities();
            iPemWorkContext.Protocols = new List<WcProtocol>();
            iPemWorkContext.Devices = new List<WcDevice>();
            iPemWorkContext.Fsus = new List<WcFsu>();
            iPemWorkContext.Rooms = new List<WcRoom>();
            iPemWorkContext.Stations = new List<WcStation>();
            iPemWorkContext.Areas = new List<WcArea>();

            //加载模版信号表
            foreach (var _protocol in _protocolRepository.GetEntities()) {
                var _points = _pointRepository.GetEntitiesByProtocol(_protocol.Id);
                iPemWorkContext.Protocols.Add(new WcProtocol {
                    Current = _protocol,
                    Points = _points
                });
            }

            //加载设备表
            var _subPointKeys = new Dictionary<string, List<SubPoint>>();
            foreach (var _device in _deviceRepository.GetEntities()) {
                var _protocol = iPemWorkContext.Protocols.Find(p => p.Current.Id == _device.ProtocolId);
                if (_protocol == null) continue;

                if (!_subPointKeys.ContainsKey(_device.StationType.Id)) {
                    _subPointKeys[_device.StationType.Id] = _subPointRepository.GetEntitiesInStaType(_device.StationType.Id);
                }

                var _subPoints = _subPointKeys[_device.StationType.Id];
                var _wcPoints = (from _point in _protocol.Points
                                 join _subPoint in _subPoints on _point.Id equals _subPoint.PointId into lt
                                 from def in lt.DefaultIfEmpty()
                                 select new WcPoint { 
                                     Current = _point, 
                                     SubPoint = def 
                                 }).ToList();

                iPemWorkContext.Devices.Add(new WcDevice { Current = _device, Points = _wcPoints });
            }

            //加载FSU表
            var _fsus = _fsuRepository.GetEntities();
            var _devsInFsu = iPemWorkContext.Devices.GroupBy(d => d.Current.FsuId);
            iPemWorkContext.Fsus.AddRange(
                from _fsu in _fsus
                join _dev in _devsInFsu on _fsu.Id equals _dev.Key into lt
                from def in lt.DefaultIfEmpty()
                select new WcFsu { 
                    Current = _fsu, 
                    Devices = def != null ? def.ToList() : new List<WcDevice>() 
                }
            );

            //加载机房表
            var _rooms = _roomRepository.GetEntities();
            var _devsInRoom = iPemWorkContext.Devices.GroupBy(d => d.Current.RoomId);
            var _fsusInRoom = iPemWorkContext.Fsus.GroupBy(f => f.Current.RoomId);
            iPemWorkContext.Rooms.AddRange(
                from _room in _rooms
                join _dev in _devsInRoom on _room.Id equals _dev.Key into lt1
                from def1 in lt1.DefaultIfEmpty()
                join _fsu in _fsusInRoom on _room.Id equals _fsu.Key into lt2
                from def2 in lt2.DefaultIfEmpty()
                select new WcRoom {
                    Current = _room,
                    Devices = def1 != null ? def1.ToList() : new List<WcDevice>(),
                    Fsus = def2 != null ? def2.ToList() : new List<WcFsu>()
                }
            );

            //加载站点表
            var _stations = _stationRepository.GetEntities();
            var _roomsInSta = iPemWorkContext.Rooms.GroupBy(r => r.Current.StationId);
            iPemWorkContext.Stations.AddRange(
                from _station in _stations
                join _room in _roomsInSta on _station.Id equals _room.Key into lt
                from def in lt.DefaultIfEmpty()
                select new WcStation {
                    Current = _station,
                    Rooms = def != null ? def.ToList() : new List<WcRoom>()
                }
            );

            //加载区域表
            var _areas = _areaRepository.GetEntities();
            var _stasInArea = iPemWorkContext.Stations.GroupBy(s => s.Current.AreaId);
            iPemWorkContext.Areas.AddRange(
                from _area in _areas
                join _sta in _stasInArea on _area.Id equals _sta.Key into lt
                from def in lt.DefaultIfEmpty()
                select new WcArea {
                    Current = _area,
                    Stations = def != null ? def.ToList() : new List<WcStation>()
                }
            );

            foreach (var current in iPemWorkContext.Areas) {
                current.Initializer(iPemWorkContext.Areas);
            }
        }

        /// <summary>
        /// 加载重定义数据
        /// </summary>
        private void LoadRedefine() {
            GlobalConfig.RedefinePoints = new Dictionary<string, RedefinePoint>();
            foreach (var _redefine in _redefinePointRepository.GetEntities()) {
                (GlobalConfig.RedefinePoints)[CommonHelper.JoinKeys(_redefine.DeviceId, _redefine.PointId)] = _redefine;
            }
        }

        /// <summary>
        /// 加载告警屏蔽数据
        /// </summary>
        private void LoadMaskings() {
            GlobalConfig.Maskings = new HashSet<string>();
            foreach (var mask in _maskingRepository.GetEntities()) {
                if (mask.Type == EnmMaskType.Station) {
                    foreach(var device in iPemWorkContext.Devices.FindAll(d => d.Current.StationId == mask.Id)){
                        GlobalConfig.Maskings.Add(CommonHelper.JoinKeys(device.Current.Id, "masking-all"));
                    }
                } else if (mask.Type == EnmMaskType.Room) {
                    foreach (var device in iPemWorkContext.Devices.FindAll(d => d.Current.RoomId == mask.Id)) {
                        GlobalConfig.Maskings.Add(CommonHelper.JoinKeys(device.Current.Id, "masking-all"));
                    }
                } else if (mask.Type == EnmMaskType.Device) {
                    GlobalConfig.Maskings.Add(CommonHelper.JoinKeys(mask.Id, "masking-all"));
                } else if (mask.Type == EnmMaskType.Point) {
                    var ids = CommonHelper.SplitCondition(mask.Id);
                    if (ids.Length == 2 && !GlobalConfig.Maskings.Contains(CommonHelper.JoinKeys(ids[0], "masking-all")))
                        GlobalConfig.Maskings.Add(CommonHelper.JoinKeys(ids[0], ids[1]));
                }
            }
        }

        /// <summary>
        /// 加载Sc采集组数据
        /// </summary>
        private void LoadScHeartbeats() {
            GlobalConfig.ScHeartbeats = new List<ScHeartbeat>();
            foreach (var group in _groupRepository.GetEntities()) {
                GlobalConfig.ScHeartbeats.Add(new ScHeartbeat(group));
            }
        }

        /// <summary>
        /// 加载工程预约数据
        /// </summary>
        private void LoadReservation() {
            GlobalConfig.Reservations = new List<ReservationModel>();
            var resNodes = _nodesInReservationRepository.GetEntities();
            foreach (var reservation in _reservationRepository.GetEntities()) {
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
        }

        /// <summary>
        /// 加载活动告警数据
        /// </summary>
        private void LoadActiveAlarm() {
            GlobalConfig.InitAlarm();
            foreach (var alarm in _aalmRepository.GetEntities()) {
                GlobalConfig.AddAlarm(alarm);
            }
        }

        /// <summary>
        /// 加载计划任务数据
        /// </summary>
        /// <param name="force">是否重新加载任务数据</param>
        private void LoadTasks(bool force) {
            if (force) {
                _registry.UpdateTasks(GlobalConfig.CurTasks);
                GlobalConfig.CurTasks = _registry.GetTasks();
            }

            foreach (var task in GlobalConfig.CurTasks) {
                if (DateTime.Today.Subtract(task.Start).TotalDays > 180) task.Start = DateTime.Today.AddDays(-1);
                if (DateTime.Today.Subtract(task.End).TotalDays > 180) task.End = DateTime.Today.AddSeconds(-1);
                if (DateTime.Today.Subtract(task.Next).TotalDays > 180) task.Next = DateTime.Now;
            }
        }

        /// <summary>
        /// 加载服务基本数据
        /// </summary>
        private void LoadParams() {
            GlobalConfig.CurParams = _registry.GetParams();
        }

        /// <summary>
        /// 加载信号测值统计策略数据
        /// </summary>
        private void LoadStatic() {
            GlobalConfig.StaticModels = new List<StaticModel>();

            var pattern = @"\{统计曲线:\d+\}";
            foreach (var _device in iPemWorkContext.Devices) {
                foreach (var _point in _device.Points) {
                    if (string.IsNullOrWhiteSpace(_point.Current.ExtSet1)) continue;
                    foreach (Match match in Regex.Matches(_point.Current.ExtSet1, pattern)) {
                        int interval;
                        if (int.TryParse(match.Value.Replace("{统计曲线:", "").Replace("}", ""), out interval)) {
                            GlobalConfig.StaticModels.Add(new StaticModel { Device = _device.Current, Point = _point.Current, Interval = interval });
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 加载电池放电统计策略数据
        /// </summary>
        private void LoadBat() {
            GlobalConfig.BatModels = new List<BatModel>();

            var pattern = @"\{电池放电:\d+&\d+(\.\d+)?\}";
            var models = new List<BatParam>();
            foreach (var _value in GlobalConfig.RedefinePoints.Values) {
                if (string.IsNullOrWhiteSpace(_value.Extend)) continue;
                foreach (Match match in Regex.Matches(_value.Extend, pattern)) {
                    var _tactics = match.Value.Replace("{电池放电:", "").Replace("}", "").Split('&');
                    if (_tactics.Length != 2) continue;

                    int _pack;
                    if (!int.TryParse(_tactics[0], out _pack)) continue;

                    double _voltage;
                    if (!double.TryParse(_tactics[1], out _voltage)) continue;

                    models.Add(new BatParam { DeviceId = _value.DeviceId, PointId = _value.PointId, PackId = _pack, Voltage = _voltage });
                }
            }

            if (models.Count > 0) {
                var _packGroup = models.GroupBy(m => new { m.DeviceId, m.PackId });
                foreach (var _pack in _packGroup) {
                    var _master = _pack.FirstOrDefault(g => g.Voltage > 0);
                    if (_master == null) continue;

                    var _device = iPemWorkContext.Devices.Find(d => d.Current.Id == _pack.Key.DeviceId);
                    if (_device == null) continue;

                    var _point = _device.Points.Find(p => p.Current.Id == _master.PointId);
                    if (_point == null) continue;

                    var _subIds = _pack.Where(p => p != _master).Select(g => g.PointId).ToArray();
                    var _subPoints = _device.Points.FindAll(p => _subIds.Contains(p.Current.Id));

                    GlobalConfig.BatModels.Add(new BatModel {
                        Device = _device.Current,
                        Point = _point.Current,
                        PackId = _master.PackId,
                        Voltage = _master.Voltage,
                        SubPoints = _subPoints.Select(g => g.Current).ToList()
                    });
                }
            }
        }

        /// <summary>
        /// 加载断站、停电、发电策略数据
        /// </summary>
        private void LoadCut() {
            GlobalConfig.Offs = new HashSet<string>();
            GlobalConfig.Cuttings = new HashSet<string>();
            GlobalConfig.Powers = new HashSet<string>();

            var pattern1 = @"\{站点断站\}";
            var pattern2 = @"\{站点停电\}";
            var pattern3 = @"\{站点发电\}";
            foreach (var value in GlobalConfig.RedefinePoints.Values) {
                if (string.IsNullOrWhiteSpace(value.Extend)) continue;

                if (Regex.IsMatch(value.Extend, pattern1))
                    GlobalConfig.Offs.Add(CommonHelper.JoinKeys(value.DeviceId, value.PointId));

                if (Regex.IsMatch(value.Extend, pattern2))
                    GlobalConfig.Cuttings.Add(CommonHelper.JoinKeys(value.DeviceId, value.PointId));

                if (Regex.IsMatch(value.Extend, pattern3))
                    GlobalConfig.Powers.Add(CommonHelper.JoinKeys(value.DeviceId, value.PointId));
            }
        }
    }
}
