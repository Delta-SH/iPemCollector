﻿using iPem.Core;
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
using System.Xml;

namespace iPem.TaskService {
    public partial class TaskService : ServiceBase {

        #region 全局变量
        private RunStatus _runStatus;
        private Registry _registry;
        private Thread _workerThread;
        private List<Thread> _workerThreads;
        private EventWaitHandle _allDone;
        private DataTable _computer;
        private XmlDocument _scXmlDoc;
        private XmlDocument _fsuXmlDoc;
        private Queue<A_IAlarm> _iAlarmQueue;
        private Queue<A_MAlarm> _mAlarmQueue;
        private Queue<A_SAlarm> _sAlarmQueue;
        private String _macId;

        private const string SC_ALARM_CFG_DIR = "cfg";
        private const string SC_ALARM_CFG_FILE = "sc_alarm_cfg.xml";
        private const string FSU_ALARM_CFG_DIR = "cfg";
        private const string FSU_ALARM_CFG_FILE = "fsu_alarm_cfg.xml";

        //静态读写锁
        static ReaderWriterLockSlim _configWriterLock = new ReaderWriterLockSlim();
        static ReaderWriterLockSlim _iAlarmQueueLock = new ReaderWriterLockSlim();
        static ReaderWriterLockSlim _mAlarmQueueLock = new ReaderWriterLockSlim();
        static ReaderWriterLockSlim _sAlarmQueueLock = new ReaderWriterLockSlim();
        #endregion

        #region 资源数据类
        private PointRepository _pointRepository;
        private SubPointRepository _subPointRepository;
        private DeviceRepository _deviceRepository;
        private FsuRepository _fsuRepository;
        private RoomRepository _roomRepository;
        private StationRepository _stationRepository;
        private AreaRepository _areaRepository;
        private SignalRepository _signalRepository;
        private CombSwitElecSourRepository _combSwitElecSourRepository;
        private DivSwitElecSourRepository _divSwitElecSourRepository;
        private MaskingRepository _maskingRepository;
        private GroupRepository _groupRepository;
        private NoticeRepository _noticeRepository;
        #endregion

        #region 历史数据类
        private A_AAlarmRepository _aalmRepository;
        private A_FAlarmRepository _falmRepository;
        private A_HAlarmRepository _halmRepository;
        private A_TAlarmRepository _talmRepository;
        private H_IDeviceRepository _iDeviceRepository;
        private H_IStationRepository _iStationRepository;
        private H_IAreaRepository _iAreaRepository;
        private V_ACabinetRepository _acabinetRepository;
        private V_AMeasureRepository _ameasureRepository;
        private V_HMeasureRepository _hmeasureRepository;
        private V_BatRepository _batRepository;
        private V_BatTimeRepository _batTimeRepository;
        private V_BatCurveRepository _batCurveRepository;
        private V_ElecRepository _elecRepository;
        private V_StaticRepository _staticRepository;
        private V_LoadRepository _loadRepository;
        private V_OfflineRepository _offlineRepository;
        private V_ParamDiffRepository _paramDiffRepository;
        private Alarmer _alarmer;
        private Indexer _indexer;
        #endregion

        #region 应用数据类
        private ReservationRepository _reservationRepository;
        private NodesInReservationRepository _nodesInReservationRepository;
        private FormulaRepository _formulaRepository;
        private DictionaryRepository _dictionaryRepository;
        private Serialer _serialer;
        #endregion

        #region 构造函数
        public TaskService() {
            InitializeComponent();
        }
        #endregion

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

                //清空服务内部命令表
                _registry.CleanOrders();

                //初始化服务参数
                iPemWorkContext.Params = _registry.GetParams();
                if (iPemWorkContext.Params.Count == 0) 
                    throw new Exception("服务尚未配置基本参数。");

                //初始化服务任务信息
                iPemWorkContext.Tasks = _registry.GetTasks();
                if (iPemWorkContext.Tasks.Count == 0) 
                    throw new Exception("服务尚未配置计划任务。");

                //初始化告警翻转哈希表
                iPemWorkContext.Reversals = new Dictionary<string, ReversalModel>();

                //初始化告警接口队列
                _iAlarmQueue = new Queue<A_IAlarm>();
                _mAlarmQueue = new Queue<A_MAlarm>();
                _sAlarmQueue = new Queue<A_SAlarm>();

                //初始化资源数据类
                _pointRepository = new PointRepository();
                _subPointRepository = new SubPointRepository();
                _deviceRepository = new DeviceRepository();
                _fsuRepository = new FsuRepository();
                _roomRepository = new RoomRepository();
                _stationRepository = new StationRepository();
                _areaRepository = new AreaRepository();
                _signalRepository = new SignalRepository();
                _combSwitElecSourRepository = new CombSwitElecSourRepository();
                _divSwitElecSourRepository = new DivSwitElecSourRepository();
                _maskingRepository = new MaskingRepository();
                _groupRepository = new GroupRepository();
                _noticeRepository = new NoticeRepository();

                //初始化历史数据类
                _aalmRepository = new A_AAlarmRepository();
                _falmRepository = new A_FAlarmRepository();
                _halmRepository = new A_HAlarmRepository();
                _talmRepository = new A_TAlarmRepository();
                _iDeviceRepository = new H_IDeviceRepository();
                _iStationRepository = new H_IStationRepository();
                _iAreaRepository = new H_IAreaRepository();
                _acabinetRepository = new V_ACabinetRepository();
                _ameasureRepository = new V_AMeasureRepository();
                _hmeasureRepository = new V_HMeasureRepository();
                _batRepository = new V_BatRepository();
                _batTimeRepository = new V_BatTimeRepository();
                _batCurveRepository = new V_BatCurveRepository();
                _elecRepository = new V_ElecRepository();
                _staticRepository = new V_StaticRepository();
                _loadRepository = new V_LoadRepository();
                _offlineRepository = new V_OfflineRepository();
                _paramDiffRepository = new V_ParamDiffRepository();
                _alarmer = new Alarmer();
                _indexer = new Indexer();

                //初始化应用数据类
                _reservationRepository = new ReservationRepository();
                _nodesInReservationRepository = new NodesInReservationRepository();
                _formulaRepository = new FormulaRepository();
                _dictionaryRepository = new DictionaryRepository();
                _serialer = new Serialer();

                //清空同步配置命令表
                _noticeRepository.Clear();

                //生成机器标识
                _macId = CommonHelper.GetMacId();

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

                //创建告警接口处理线程
                _workerThread = new Thread(new ThreadStart(DoiAlarm));
                _workerThread.IsBackground = true;
                _workerThread.Start();
                _workerThreads.Add(_workerThread);

                //创建系统自检线程
                _workerThread = new Thread(new ThreadStart(DoChecking));
                _workerThread.IsBackground = true;
                _workerThread.Start();
                _workerThreads.Add(_workerThread);

                //创建SC心跳监测线程
                _workerThread = new Thread(new ThreadStart(DoScHeartbeat));
                _workerThread.IsBackground = true;
                _workerThread.Start();
                _workerThreads.Add(_workerThread);

                //创建实时能耗处理线程
                _workerThread = new Thread(new ThreadStart(DoFormula));
                _workerThread.IsBackground = true;
                _workerThread.Start();
                _workerThreads.Add(_workerThread);

                //创建虚拟信号处理线程
                _workerThread = new Thread(new ThreadStart(DoVSignal));
                _workerThread.IsBackground = true;
                _workerThread.Start();
                _workerThreads.Add(_workerThread);

                //创建电池数据处理线程
                _workerThread = new Thread(new ThreadStart(DoBat));
                _workerThread.IsBackground = true;
                _workerThread.Start();
                _workerThreads.Add(_workerThread);

                //创建数据库索引处理线程
                _workerThread = new Thread(new ThreadStart(DoIndex));
                _workerThread.IsBackground = true;
                _workerThread.Start();
                _workerThreads.Add(_workerThread);

                //创建能耗处理线程
                _workerThread = new Thread(new ThreadStart(DoTask001));
                _workerThread.IsBackground = true;
                _workerThread.Start();
                _workerThreads.Add(_workerThread);

                //创建电池曲线处理线程
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

                //创建参数巡检线程
                _workerThread = new Thread(new ThreadStart(DoTask006));
                _workerThread.IsBackground = true;
                _workerThread.Start();
                _workerThreads.Add(_workerThread);

                //创建告警同步线程
                _workerThread = new Thread(new ThreadStart(DoTask007));
                _workerThread.IsBackground = true;
                _workerThread.Start();
                _workerThreads.Add(_workerThread);

                _runStatus = RunStatus.Init;
            } catch (Exception exc) {
                _runStatus = RunStatus.Stop;
                Logger.Warning("启动服务失败，详见错误日志。");
                Logger.Error("启动服务失败", exc);
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

                _registry.UpdateTasks(iPemWorkContext.Tasks);
                Logger.Information("服务已停止");
            } catch (Exception exc) {
                Logger.Warning("停止服务失败，详见错误日志。");
                Logger.Error("停止服务失败", exc);
            }
        }

        /// <summary>
        /// 初始化任务
        /// </summary>
        private void DoInit() {
            var maxRepeat = 3;
            var interval = 86400;
            while (_runStatus < RunStatus.Running) {
                Thread.Sleep(1000);
                try {
                    if (_runStatus == RunStatus.Init) {
                        if (interval++ < 30) continue; 
                        interval = 1;
                        maxRepeat--;
                        Logger.Information("数据初始化...");

                        LoadBase();
                        LoadTasks(false);
                        LoadAlarms(true);
                        LoadMaskings();
                        LoadReservations();
                        LoadFormulas();
                        LoadStatics();
                        LoadBatteries();
                        LoadScHeartbeats();

                        Logger.Information("数据初始化完成");
                        _runStatus = RunStatus.Running;
                        _allDone.Set();
                        break;
                    }
                } catch (Exception err) {
                    Logger.Error(err.Message, err);
                    if (maxRepeat <= 0) {
                        Logger.Warning("数据初始化错误,详见错误日志。");
                        _runStatus = RunStatus.Stop;
                        _allDone.Set();
                        break;
                    } else {
                        Logger.Warning("数据初始化错误,稍后将重试。");
                    }
                }
            }
        }

        /// <summary>
        /// 同步配置任务
        /// </summary>
        private void DoConfig() {
            _allDone.WaitOne();

            if (_runStatus != RunStatus.Running) return;
            Logger.Information("命令监听线程已启动。");
            var interval = 86400;
            while (_runStatus != RunStatus.Stop) {
                Thread.Sleep(1000);
                if (_runStatus == RunStatus.Running) {
                    if (interval++ <= 10) continue; 
                    interval = 1;

                    try {

                        #region 执行命令
                        var orders = _registry.GetOrders();
                        var notices = _noticeRepository.GetEntities();
                        if (notices.Count > 0) {
                            if (!orders.Any(o => o.Id == OrderId.Restart || o.Id == OrderId.Reload || o.Id == OrderId.SyncBase)) {
                                if (notices.Any(n => string.IsNullOrWhiteSpace(n.Name))) {
                                    orders.Add(new OrderEntity { Id = OrderId.SyncBase, Time = DateTime.Now });
                                } else {
                                    orders.Add(new OrderEntity { Id = OrderId.SyncData, Time = DateTime.Now });
                                }
                            }
                                
                            _noticeRepository.Delete(notices);
                        }

                        if (orders.Count == 0) continue;
                        #endregion

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
                                    Logger.Warning("同步基本配置数据错误，详见错误日志。");
                                    Logger.Error("同步基本配置数据错误", err);
                                }

                                try {
                                    LoadParams();
                                    LoadTasks(true);
                                    LoadAlarms();
                                    LoadMaskings();
                                    LoadReservations();
                                    LoadFormulas();
                                    LoadStatics();
                                    LoadBatteries();
                                    LoadScHeartbeats();
                                } catch (Exception err) {
                                    Logger.Warning("同步基础数据错误，详见错误日志。");
                                    Logger.Error("同步基础数据错误", err);
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
                            if (order.Id == OrderId.SyncBase) {
                                #region 同步基础配置
                                try {
                                    //获取写资源锁
                                    _configWriterLock.EnterWriteLock();

                                    LoadBase();
                                    Logger.Information("同步基础配置完成。");
                                } catch (Exception err) {
                                    Logger.Warning("同步基本配置数据错误，详见错误日志。");
                                    Logger.Error("同步基本配置数据错误", err);
                                } finally {
                                    _configWriterLock.ExitWriteLock();
                                }
                                #endregion
                            } else if (order.Id == OrderId.SyncData) {
                                #region 同步基础数据
                                try {
                                    //获取写资源锁
                                    _configWriterLock.EnterWriteLock();

                                    LoadParams();
                                    LoadTasks(true);
                                    LoadAlarms();
                                    LoadMaskings();
                                    LoadReservations();
                                    LoadFormulas();
                                    LoadStatics();
                                    LoadBatteries();
                                    LoadScHeartbeats();
                                    Logger.Information("同步基础数据完成。");
                                } catch (Exception err) {
                                    Logger.Warning("同步活动告警数据错误，详见错误日志。");
                                    Logger.Error("同步活动告警数据错误", err);
                                } finally {
                                    _configWriterLock.ExitWriteLock();
                                }
                                #endregion
                            } else if (order.Id == OrderId.ExTask001) {
                                #region Task001
                                try {
                                    //获取读资源锁
                                    _configWriterLock.EnterReadLock();

                                    var task = JsonConvert.DeserializeObject<ExTask>(order.Param);
                                    if (task != null) this.ExTask001(task.Start, task.End);
                                    Logger.Information("能耗数据处理命令执行完成。");
                                } catch (Exception err) {
                                    Logger.Warning("能耗数据处理命令执行错误，详见错误日志。");
                                    Logger.Error("能耗数据处理命令执行错误", err);
                                } finally {
                                    _configWriterLock.ExitReadLock();
                                }
                                #endregion
                            } else if (order.Id == OrderId.ExTask002) {
                                #region Task002
                                try {
                                    //获取读资源锁
                                    _configWriterLock.EnterReadLock();

                                    var task = JsonConvert.DeserializeObject<ExTask>(order.Param);
                                    if (task != null) this.ExTask002(task.Start, task.End);
                                    Logger.Information("电池曲线处理命令执行完成。");
                                } catch (Exception err) {
                                    Logger.Warning("电池曲线处理命令执行错误，详见错误日志。");
                                    Logger.Error("电池曲线处理命令执行错误", err);
                                } finally {
                                    _configWriterLock.ExitReadLock();
                                }
                                #endregion
                            } else if (order.Id == OrderId.ExTask003) {
                                #region Task003
                                try {
                                    //获取读资源锁
                                    _configWriterLock.EnterReadLock();

                                    var task = JsonConvert.DeserializeObject<ExTask>(order.Param);
                                    if (task != null) this.ExTask003(task.Start, task.End);
                                    Logger.Information("信号测值统计命令执行完成。");
                                } catch (Exception err) {
                                    Logger.Warning("信号测值统计命令执行错误，详见错误日志。");
                                    Logger.Error("信号测值统计命令执行错误", err);
                                } finally {
                                    _configWriterLock.ExitReadLock();
                                }
                                #endregion
                            } else if (order.Id == OrderId.ExTask004) {
                                #region Task004
                                try {
                                    //获取读资源锁
                                    _configWriterLock.EnterReadLock();

                                    var task = JsonConvert.DeserializeObject<ExTask>(order.Param);
                                    if (task != null) {
                                        var _FzdlParam = iPemWorkContext.Params.Find(p => p.Id == ParamId.FZDL);
                                        if (_FzdlParam == null || string.IsNullOrWhiteSpace(_FzdlParam.Value))
                                            throw new Exception("尚未配置负载电流信号");

                                        if(!iPemWorkContext.Points.ContainsKey(_FzdlParam.Value))
                                            throw new Exception("负载电流信号配置错误");

                                        var _FzdlPoint = iPemWorkContext.Points[_FzdlParam.Value];

                                        var _GzztParam = iPemWorkContext.Params.Find(p => p.Id == ParamId.GZZT);
                                        if (_GzztParam == null || string.IsNullOrWhiteSpace(_GzztParam.Value))
                                            throw new Exception("尚未配置工作状态信号");

                                        if (!iPemWorkContext.Points.ContainsKey(_GzztParam.Value))
                                            throw new Exception("工作状态信号配置错误");

                                        var _GzztPoint = iPemWorkContext.Points[_GzztParam.Value];

                                        this.ExTask004(task.Start, task.End, _FzdlPoint, _GzztPoint);
                                        Logger.Information("开关电源带载率统计命令执行完成。");
                                    }
                                } catch (Exception err) {
                                    Logger.Warning("开关电源带载率统计命令执行错误，详见错误日志。");
                                    Logger.Error("开关电源带载率统计命令执行错误", err);
                                } finally {
                                    _configWriterLock.ExitReadLock();
                                }
                                #endregion
                            } else if (order.Id == OrderId.ExTask005) {
                                #region Task005
                                try {
                                    //获取读资源锁
                                    _configWriterLock.EnterReadLock();

                                    this.ExTask005();
                                    Logger.Information("资管接口同步命令执行完成。");
                                } catch (Exception err) {
                                    Logger.Warning("资管接口同步命令执行错误，详见错误日志。");
                                    Logger.Error("资管接口同步命令执行错误", err);
                                } finally {
                                    _configWriterLock.ExitReadLock();
                                }
                                #endregion
                            } else if (order.Id == OrderId.ExTask006) {
                                #region Task006
                                try {
                                    //获取读资源锁
                                    _configWriterLock.EnterReadLock();

                                    this.ExTask006();
                                    Logger.Information("参数自动巡检命令执行完成。");
                                } catch (Exception err) {
                                    Logger.Warning("参数自动巡检命令执行错误，详见错误日志。");
                                    Logger.Error("参数自动巡检命令执行错误", err);
                                } finally {
                                    _configWriterLock.ExitReadLock();
                                }
                                #endregion
                            } else if (order.Id == OrderId.ExTask007) {
                                #region Task007
                                try {
                                    //获取写资源锁
                                    _configWriterLock.EnterWriteLock();

                                    var task = JsonConvert.DeserializeObject<ExTask>(order.Param);
                                    if (task != null) this.ExTask007(task.Start, task.End);
                                    Logger.Information("告警同步处理命令执行完成。");
                                } catch (Exception err) {
                                    Logger.Warning("告警同步处理命令执行错误，详见错误日志。");
                                    Logger.Error("告警同步处理命令执行错误", err);
                                } finally {
                                    _configWriterLock.ExitWriteLock();
                                }
                                #endregion
                            }
                        }
                        #endregion

                        _registry.DelOrders(orders);
                    } catch (Exception err) {
                        Logger.Warning(err.Message);
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

            if (_runStatus != RunStatus.Running) return;
            Logger.Information("告警处理线程已启动。");

            var interval = 86400;
            while (_runStatus != RunStatus.Stop) {
                Thread.Sleep(1000);
                if (_runStatus == RunStatus.Running) {
                    if (interval++ <= 5) continue; interval = 1;

                    try {
                        // 获取读资源锁
                        _configWriterLock.EnterReadLock();

                        // 告警接口标识
                        var _iparam = iPemWorkContext.Params.Find(p => p.Id == ParamId.GJJK);
                        var _ienabled = _iparam != null && "1".Equals(_iparam.Value);

                        // 短信接口标识
                        var _mparam = iPemWorkContext.Params.Find(p => p.Id == ParamId.DXGJ);
                        var _menabled = _mparam != null && "1".Equals(_mparam.Value);

                        // 语音接口标识
                        var _sparam = iPemWorkContext.Params.Find(p => p.Id == ParamId.YYGJ);
                        var _senabled = _sparam != null && "1".Equals(_sparam.Value);

                        // 需要处理的告警
                        var _alarmSystem = new List<TAlarmModel>();
                        var _alarmNormal = new List<TAlarmModel>();
                        var _alarmDelay = new List<TAlarmModel>();

                        #region 告警预处理
                        var _talarms = _talmRepository.GetEntities();
                        foreach (var _alarm in _talarms) {
                            try {
                                if (_alarm.FsuId.Equals("-1")) {
                                    _alarmSystem.Add(new TAlarmModel { Alarm = _alarm });
                                } else if (_alarm.FsuId.Equals("-2")) {
                                    _alarmSystem.Add(new TAlarmModel { Alarm = _alarm });
                                } else {
                                    if (!iPemWorkContext.DeviceSet.ContainsKey(_alarm.DeviceId)) continue;

                                    var _device = iPemWorkContext.DeviceSet[_alarm.DeviceId];
                                    var _signal = _device.Signals.Find(p => p.PointId == _alarm.PointId);
                                    if (_signal == null) continue;

                                    var _current = new TAlarmModel { Device = _device.Current, Signal = _signal, Alarm = _alarm };

                                    //处理告警开始延迟\恢复延迟
                                    if (_current.Alarm.AlarmFlag == EnmFlag.Begin && _current.Signal.AlarmDelay > 0 && _current.Alarm.AlarmTime.AddSeconds(_current.Signal.AlarmDelay) > DateTime.Now) {
                                        _alarmDelay.Add(_current);
                                        continue;
                                    } else if (_current.Alarm.AlarmFlag == EnmFlag.End && _current.Signal.AlarmRecoveryDelay > 0 && _current.Alarm.AlarmTime.AddSeconds(_current.Signal.AlarmRecoveryDelay) > DateTime.Now) {
                                        continue;
                                    }

                                    //当告警延迟时间未到，而告警已经结束，则直接删除此告警流水
                                    if (_alarmDelay.Count > 0 && _current.Alarm.AlarmFlag == EnmFlag.End) {
                                        var _delayStart = _alarmDelay.Find(m => m.Device.Id == _current.Device.Id && m.Signal.PointId == _current.Signal.PointId);
                                        if (_delayStart != null) {
                                            _talmRepository.Delete(_delayStart.Alarm, _current.Alarm);
                                            _alarmDelay.Remove(_delayStart);
                                            continue;
                                        }
                                    }

                                    _alarmNormal.Add(_current);
                                }
                            } catch { }
                        }
                        #endregion

                        #region 系统告警
                        foreach (var _alarm in _alarmSystem) {
                            try {
                                if (_alarm.Alarm.AlarmFlag == EnmFlag.Begin) {
                                    #region 开始告警
                                    try {
                                        var _id = CommonHelper.GetMD5(string.Format("{0}-{1}-{2}", _alarm.Alarm.DeviceId, _alarm.Alarm.PointId, _alarm.Alarm.AlarmTime.Ticks));
                                        var _active = _aalmRepository.GetEntity(_id);
                                        if (_active == null) {

                                            #region 初始化告警
                                            var _current = new AlarmStart {
                                                Id = _id,
                                                TId = _alarm.Alarm.Id,
                                                AreaId = "",
                                                StationId = "",
                                                RoomId = "",
                                                FsuId = _alarm.Alarm.FsuId,
                                                DeviceId = _alarm.Alarm.DeviceId,
                                                PointId = _alarm.Alarm.PointId,
                                                SerialNo = CommonHelper.GetAlarmSerialNo(_alarm.Alarm.Id),
                                                NMAlarmId = _alarm.Alarm.NMAlarmId,
                                                AlarmTime = _alarm.Alarm.AlarmTime,
                                                AlarmLevel = _alarm.Alarm.AlarmLevel,
                                                AlarmFlag = _alarm.Alarm.AlarmFlag,
                                                AlarmValue = _alarm.Alarm.AlarmValue,
                                                AlarmDesc = _alarm.Alarm.AlarmDesc,
                                                AlarmRemark = _alarm.Alarm.AlarmRemark
                                            };
                                            #endregion

                                            #region 开始告警
                                            _alarmer.Start(_current);
                                            iPemWorkContext.AddAlarm(_current);
                                            #endregion

                                            #region 告警接口
                                            if (_ienabled) {
                                                try {
                                                    _iAlarmQueueLock.EnterWriteLock();
                                                    _iAlarmQueue.Enqueue(new A_IAlarm {
                                                        Id = _current.TId,
                                                        AreaId = _current.AreaId,
                                                        StationId = _current.StationId,
                                                        RoomId = _current.RoomId,
                                                        FsuId = _current.FsuId,
                                                        DeviceId = _current.DeviceId,
                                                        PointId = _current.PointId,
                                                        SerialNo = _current.SerialNo,
                                                        NMAlarmId = _current.NMAlarmId,
                                                        AlarmTime = _current.AlarmTime,
                                                        AlarmLevel = _current.AlarmLevel,
                                                        AlarmFlag = _current.AlarmFlag,
                                                        AlarmValue = _current.AlarmValue,
                                                        AlarmDesc = _current.AlarmDesc,
                                                        AlarmRemark = _current.AlarmRemark,
                                                        Confirmed = EnmConfirm.Unconfirmed,
                                                        Confirmer = null,
                                                        ConfirmedTime = null,
                                                        ReservationId = _current.ReservationId,
                                                        ReservationName = _current.ReservationName,
                                                        ReservationStart = _current.ReservationStart,
                                                        ReservationEnd = _current.ReservationEnd,
                                                        PrimaryId = _current.PrimaryId,
                                                        RelatedId = _current.RelatedId,
                                                        FilterId = _current.FilterId,
                                                        ReversalId = _current.ReversalId,
                                                        Masked = _current.Masked,
                                                        CreatedTime = DateTime.Now
                                                    });
                                                } finally {
                                                    _iAlarmQueueLock.ExitWriteLock();
                                                }
                                            }
                                            #endregion

                                            #region 短信告警
                                            if (_menabled) {
                                                try {
                                                    _mAlarmQueueLock.EnterWriteLock();
                                                    _mAlarmQueue.Enqueue(new A_MAlarm {
                                                        Id = _current.TId,
                                                        AreaId = _current.AreaId,
                                                        AreaName = "",
                                                        StationId = _current.StationId,
                                                        StationName = "",
                                                        RoomId = _current.RoomId,
                                                        RoomName = "",
                                                        DeviceId = _current.DeviceId,
                                                        DeviceName = "",
                                                        PointId = _current.PointId,
                                                        PointName = _current.AlarmDesc,
                                                        SerialNo = _current.SerialNo,
                                                        NMAlarmId = _current.NMAlarmId,
                                                        AlarmTime = _current.AlarmTime,
                                                        AlarmLevel = _current.AlarmLevel,
                                                        AlarmFlag = _current.AlarmFlag,
                                                        AlarmValue = _current.AlarmValue,
                                                        AlarmDesc = _current.AlarmDesc,
                                                        AlarmRemark = _current.AlarmRemark,
                                                        Confirmed = EnmConfirm.Unconfirmed,
                                                        Confirmer = null,
                                                        ConfirmedTime = null,
                                                        ReservationId = _current.ReservationId,
                                                        PrimaryId = _current.PrimaryId,
                                                        RelatedId = _current.RelatedId,
                                                        FilterId = _current.FilterId,
                                                        ReversalId = _current.ReversalId,
                                                        Masked = _current.Masked,
                                                        CreatedTime = DateTime.Now
                                                    });
                                                } finally {
                                                    _mAlarmQueueLock.ExitWriteLock();
                                                }
                                            }
                                            #endregion

                                            #region 语音告警
                                            if (_senabled) {
                                                try {
                                                    _sAlarmQueueLock.EnterWriteLock();
                                                    _sAlarmQueue.Enqueue(new A_SAlarm {
                                                        Id = _current.TId,
                                                        AreaId = _current.AreaId,
                                                        AreaName = "",
                                                        StationId = _current.StationId,
                                                        StationName = "",
                                                        RoomId = _current.RoomId,
                                                        RoomName = "",
                                                        DeviceId = _current.DeviceId,
                                                        DeviceName = "",
                                                        PointId = _current.PointId,
                                                        PointName = _current.AlarmDesc,
                                                        SerialNo = _current.SerialNo,
                                                        NMAlarmId = _current.NMAlarmId,
                                                        AlarmTime = _current.AlarmTime,
                                                        AlarmLevel = _current.AlarmLevel,
                                                        AlarmFlag = _current.AlarmFlag,
                                                        AlarmValue = _current.AlarmValue,
                                                        AlarmDesc = _current.AlarmDesc,
                                                        AlarmRemark = _current.AlarmRemark,
                                                        Confirmed = EnmConfirm.Unconfirmed,
                                                        Confirmer = null,
                                                        ConfirmedTime = null,
                                                        ReservationId = _current.ReservationId,
                                                        PrimaryId = _current.PrimaryId,
                                                        RelatedId = _current.RelatedId,
                                                        FilterId = _current.FilterId,
                                                        ReversalId = _current.ReversalId,
                                                        Masked = _current.Masked,
                                                        CreatedTime = DateTime.Now
                                                    });
                                                } finally {
                                                    _sAlarmQueueLock.ExitWriteLock();
                                                }
                                            }
                                            #endregion

                                        } else {
                                            _talmRepository.Delete(_alarm.Alarm);
                                        }
                                    } catch (Exception err) {
                                        err.Source = JsonConvert.SerializeObject(_alarm.Alarm);

                                        Logger.Warning("开始告警发生错误，详见错误日志。");
                                        Logger.Error("开始告警发生错误", err);
                                    }
                                    #endregion
                                } else if (_alarm.Alarm.AlarmFlag == EnmFlag.End) {
                                    #region 结束告警
                                    try {
                                        var _active = _aalmRepository.GetEntityInPoint(_alarm.Alarm.DeviceId, _alarm.Alarm.PointId);
                                        if (_active != null && _alarm.Alarm.AlarmTime > _active.AlarmTime) {

                                            #region 初始化告警
                                            var _current = new AlarmEnd {
                                                Id = _active.Id,
                                                TId = _alarm.Alarm.Id,
                                                AreaId = _active.AreaId,
                                                StationId = _active.StationId,
                                                RoomId = _active.RoomId,
                                                FsuId = _active.FsuId,
                                                DeviceId = _active.DeviceId,
                                                PointId = _active.PointId,
                                                SerialNo = _active.SerialNo,
                                                NMAlarmId = _active.NMAlarmId,
                                                StartTime = _active.AlarmTime,
                                                EndTime = _alarm.Alarm.AlarmTime,
                                                AlarmLevel = _active.AlarmLevel,
                                                AlarmFlag = _alarm.Alarm.AlarmFlag,
                                                StartValue = _active.AlarmValue,
                                                EndValue = _alarm.Alarm.AlarmValue,
                                                AlarmDesc = _active.AlarmDesc,
                                                AlarmRemark = _active.AlarmRemark,
                                                Confirmed = _active.Confirmed,
                                                Confirmer = _active.Confirmer,
                                                ConfirmedTime = _active.ConfirmedTime,
                                                ReservationId = _active.ReservationId,
                                                PrimaryId = _active.PrimaryId,
                                                RelatedId = _active.RelatedId,
                                                FilterId = _active.FilterId,
                                                ReversalId = _active.ReversalId,
                                                ReversalCount = _active.ReversalCount,
                                                Masked = _active.Masked
                                            };
                                            #endregion

                                            #region 结束告警
                                            _alarmer.End(_current);
                                            iPemWorkContext.RemoveAlarm(_current);
                                            #endregion

                                            #region 告警接口
                                            if (_ienabled) {
                                                try {
                                                    _iAlarmQueueLock.EnterWriteLock();
                                                    _iAlarmQueue.Enqueue(new A_IAlarm {
                                                        Id = _current.TId,
                                                        AreaId = _current.AreaId,
                                                        StationId = _current.StationId,
                                                        RoomId = _current.RoomId,
                                                        FsuId = _current.FsuId,
                                                        DeviceId = _current.DeviceId,
                                                        PointId = _current.PointId,
                                                        SerialNo = _current.SerialNo,
                                                        NMAlarmId = _current.NMAlarmId,
                                                        AlarmTime = _current.EndTime,
                                                        AlarmLevel = _current.AlarmLevel,
                                                        AlarmFlag = _current.AlarmFlag,
                                                        AlarmValue = _current.EndValue,
                                                        AlarmDesc = _current.AlarmDesc,
                                                        AlarmRemark = _current.AlarmRemark,
                                                        Confirmed = _current.Confirmed,
                                                        Confirmer = _current.Confirmer,
                                                        ConfirmedTime = _current.ConfirmedTime,
                                                        ReservationId = _current.ReservationId,
                                                        ReservationName = _current.ReservationName,
                                                        ReservationStart = _current.ReservationStart,
                                                        ReservationEnd = _current.ReservationEnd,
                                                        PrimaryId = _current.PrimaryId,
                                                        RelatedId = _current.RelatedId,
                                                        FilterId = _current.FilterId,
                                                        ReversalId = _current.ReversalId,
                                                        Masked = _current.Masked,
                                                        CreatedTime = DateTime.Now
                                                    });
                                                } finally {
                                                    _iAlarmQueueLock.ExitWriteLock();
                                                }
                                            }
                                            #endregion

                                            #region 短信告警
                                            if (_menabled) {
                                                try {
                                                    _mAlarmQueueLock.EnterWriteLock();
                                                    _mAlarmQueue.Enqueue(new A_MAlarm {
                                                        Id = _current.TId,
                                                        AreaId = _current.AreaId,
                                                        AreaName = "",
                                                        StationId = _current.StationId,
                                                        StationName = "",
                                                        RoomId = _current.RoomId,
                                                        RoomName = "",
                                                        DeviceId = _current.DeviceId,
                                                        DeviceName = "",
                                                        PointId = _current.PointId,
                                                        PointName = _current.AlarmDesc,
                                                        SerialNo = _current.SerialNo,
                                                        NMAlarmId = _current.NMAlarmId,
                                                        AlarmTime = _current.EndTime,
                                                        AlarmLevel = _current.AlarmLevel,
                                                        AlarmFlag = _current.AlarmFlag,
                                                        AlarmValue = _current.EndValue,
                                                        AlarmDesc = _current.AlarmDesc,
                                                        AlarmRemark = _current.AlarmRemark,
                                                        Confirmed = _current.Confirmed,
                                                        Confirmer = _current.Confirmer,
                                                        ConfirmedTime = _current.ConfirmedTime,
                                                        ReservationId = _current.ReservationId,
                                                        PrimaryId = _current.PrimaryId,
                                                        RelatedId = _current.RelatedId,
                                                        FilterId = _current.FilterId,
                                                        ReversalId = _current.ReversalId,
                                                        Masked = _current.Masked,
                                                        CreatedTime = DateTime.Now
                                                    });
                                                } finally {
                                                    _mAlarmQueueLock.ExitWriteLock();
                                                }
                                            }
                                            #endregion

                                            #region 语音告警
                                            if (_senabled) {
                                                try {
                                                    _sAlarmQueueLock.EnterWriteLock();
                                                    _sAlarmQueue.Enqueue(new A_SAlarm {
                                                        Id = _current.TId,
                                                        AreaId = _current.AreaId,
                                                        AreaName = "",
                                                        StationId = _current.StationId,
                                                        StationName = "",
                                                        RoomId = _current.RoomId,
                                                        RoomName = "",
                                                        DeviceId = _current.DeviceId,
                                                        DeviceName = "",
                                                        PointId = _current.PointId,
                                                        PointName = _current.AlarmDesc,
                                                        SerialNo = _current.SerialNo,
                                                        NMAlarmId = _current.NMAlarmId,
                                                        AlarmTime = _current.EndTime,
                                                        AlarmLevel = _current.AlarmLevel,
                                                        AlarmFlag = _current.AlarmFlag,
                                                        AlarmValue = _current.EndValue,
                                                        AlarmDesc = _current.AlarmDesc,
                                                        AlarmRemark = _current.AlarmRemark,
                                                        Confirmed = _current.Confirmed,
                                                        Confirmer = _current.Confirmer,
                                                        ConfirmedTime = _current.ConfirmedTime,
                                                        ReservationId = _current.ReservationId,
                                                        PrimaryId = _current.PrimaryId,
                                                        RelatedId = _current.RelatedId,
                                                        FilterId = _current.FilterId,
                                                        ReversalId = _current.ReversalId,
                                                        Masked = _current.Masked,
                                                        CreatedTime = DateTime.Now
                                                    });
                                                } finally {
                                                    _sAlarmQueueLock.ExitWriteLock();
                                                }
                                            }
                                            #endregion

                                        } else {
                                            _talmRepository.Delete(_alarm.Alarm);
                                        }
                                    } catch (Exception err) {
                                        err.Source = JsonConvert.SerializeObject(_alarm.Alarm);

                                        Logger.Warning("结束告警发生错误，详见错误日志。");
                                        Logger.Error("结束告警发生错误", err);
                                    }
                                    #endregion
                                }
                            } catch (Exception err) {
                                err.Source = JsonConvert.SerializeObject(_alarm.Alarm);

                                Logger.Warning("处理告警发生错误，详见错误日志。");
                                Logger.Error("处理告警发生错误", err);
                            }
                        }
                        #endregion

                        #region 正常告警
                        foreach (var _alarm in _alarmNormal) {
                            try {
                                var _key = CommonHelper.JoinKeys(_alarm.Device.Id, _alarm.Signal.PointId);
                                if (_alarm.Alarm.AlarmFlag == EnmFlag.Begin) {
                                    #region 开始告警
                                    try {
                                        var _id = CommonHelper.GetMD5(string.Format("{0}-{1}-{2}", _alarm.Device.Id, _alarm.Signal.PointId, _alarm.Alarm.AlarmTime.Ticks));
                                        var _active = _aalmRepository.GetEntity(_id);
                                        if (_active == null) {

                                            #region 初始化告警
                                            var _current = new AlarmStart {
                                                Id = _id,
                                                TId = _alarm.Alarm.Id,
                                                AreaId = _alarm.Device.AreaId,
                                                StationId = _alarm.Device.StationId,
                                                RoomId = _alarm.Device.RoomId,
                                                FsuId = _alarm.Device.FsuId,
                                                DeviceId = _alarm.Device.Id,
                                                PointId = _alarm.Signal.PointId,
                                                SerialNo = CommonHelper.GetAlarmSerialNo(_alarm.Alarm.Id),
                                                NMAlarmId = _alarm.Alarm.NMAlarmId,
                                                AlarmTime = _alarm.Alarm.AlarmTime,
                                                AlarmLevel = _alarm.Alarm.AlarmLevel,
                                                AlarmFlag = _alarm.Alarm.AlarmFlag,
                                                AlarmValue = _alarm.Alarm.AlarmValue,
                                                AlarmDesc = _alarm.Alarm.AlarmDesc,
                                                AlarmRemark = _alarm.Alarm.AlarmRemark
                                            };
                                            #endregion

                                            #region 工程预约
                                            foreach (var reservation in iPemWorkContext.Reservations) {
                                                if (_alarm.Alarm.AlarmTime >= reservation.Reservation.StartTime
                                                    && _alarm.Alarm.AlarmTime <= reservation.Reservation.EndTime
                                                    && reservation.Devices.Contains(_current.DeviceId)) {
                                                    _current.ReservationId = reservation.Reservation.Id;
                                                    _current.ReservationName = reservation.Reservation.Name;
                                                    _current.ReservationStart = reservation.Reservation.StartTime;
                                                    _current.ReservationEnd = reservation.Reservation.EndTime;
                                                    break;
                                                }
                                            }
                                            #endregion

                                            #region 主次告警
                                            if (!string.IsNullOrWhiteSpace(_alarm.Signal.InferiorAlarmStr)) {
                                                var key = CommonHelper.JoinKeys(_alarm.Signal.InferiorAlarmStr.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
                                                if (iPemWorkContext.Alarms.ContainsKey(key)) {
                                                    _current.PrimaryId = iPemWorkContext.Alarms[key].Id;
                                                }
                                            }
                                            #endregion

                                            #region 关联告警
                                            if (!string.IsNullOrWhiteSpace(_alarm.Signal.ConnAlarmStr)) {
                                                var key = CommonHelper.JoinKeys(_alarm.Signal.ConnAlarmStr.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
                                                if (iPemWorkContext.Alarms.ContainsKey(key)) {
                                                    _current.RelatedId = iPemWorkContext.Alarms[key].Id;
                                                }
                                            }
                                            #endregion

                                            #region 告警过滤
                                            if (!string.IsNullOrWhiteSpace(_alarm.Signal.AlarmFilteringStr)) {
                                                var key = CommonHelper.JoinKeys(_alarm.Signal.AlarmFilteringStr.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
                                                if (iPemWorkContext.Alarms.ContainsKey(key)) {
                                                    _current.FilterId = iPemWorkContext.Alarms[key].Id;
                                                }
                                            }
                                            #endregion

                                            #region 告警翻转
                                            if (!string.IsNullOrWhiteSpace(_alarm.Signal.AlarmReversalStr)) {
                                                int reversalInterval;
                                                if (int.TryParse(_alarm.Signal.AlarmReversalStr, out reversalInterval) && reversalInterval > 0) {
                                                    if (iPemWorkContext.Reversals.ContainsKey(_key)) {
                                                        var reversalTarget = iPemWorkContext.Reversals[_key];
                                                        if (reversalTarget.AlarmTime.AddMinutes(reversalInterval) >= _current.AlarmTime) {
                                                            _current.ReversalId = reversalTarget.AlarmId;
                                                            _current.ReversalCount = ++reversalTarget.ReversalCount;
                                                        } else {
                                                            reversalTarget.AlarmId = _current.Id;
                                                            reversalTarget.AlarmTime = _current.AlarmTime;
                                                            reversalTarget.ReversalCount = 0;
                                                            _current.ReversalId = _current.Id;
                                                            _current.ReversalCount = 0;
                                                        }
                                                    } else {
                                                        iPemWorkContext.Reversals[_key] = new ReversalModel { AlarmId = _current.Id, AlarmTime = _current.AlarmTime, ReversalCount = 0 };
                                                        _current.ReversalId = _current.Id;
                                                        _current.ReversalCount = 0;
                                                    }
                                                }
                                            }
                                            #endregion

                                            #region 告警屏蔽

                                            _current.Masked = iPemWorkContext.Maskings.Contains(CommonHelper.JoinKeys(_current.DeviceId, "masking-all"));
                                            if (!_current.Masked) _current.Masked = iPemWorkContext.Maskings.Contains(_key);

                                            #endregion

                                            #region 开始告警
                                            _alarmer.Start(_current);
                                            iPemWorkContext.AddAlarm(_current);
                                            #endregion

                                            #region 停电告警
                                            var offlines = new List<V_Offline>();
                                            if (iPemWorkContext.Cuttings.ContainsKey(_key)) {
                                                var kvs = iPemWorkContext.Cuttings[_key];
                                                foreach (var kv in kvs) {
                                                    offlines.Add(new V_Offline {
                                                        Id = kv.Key,
                                                        Type = kv.Value,
                                                        FormulaType = EnmFormula.TD,
                                                        StartTime = _current.AlarmTime,
                                                        EndTime = DateTime.Now
                                                    });
                                                }
                                            }
                                            #endregion

                                            #region 发电告警
                                            if (iPemWorkContext.Powers.ContainsKey(_key)) {
                                                var kvs = iPemWorkContext.Powers[_key];
                                                foreach (var kv in kvs) {
                                                    offlines.Add(new V_Offline {
                                                        Id = kv.Key,
                                                        Type = kv.Value,
                                                        FormulaType = EnmFormula.FD,
                                                        StartTime = _current.AlarmTime,
                                                        EndTime = DateTime.Now,
                                                        Value = 0
                                                    });
                                                }
                                            }

                                            //保存停电、发电数据
                                            if (offlines.Count > 0) {
                                                _offlineRepository.SaveActive(offlines);
                                            }
                                            #endregion

                                            #region 告警接口
                                            if (_ienabled) {
                                                try {
                                                    _iAlarmQueueLock.EnterWriteLock();
                                                    _iAlarmQueue.Enqueue(new A_IAlarm {
                                                        Id = _current.TId,
                                                        AreaId = _current.AreaId,
                                                        StationId = _current.StationId,
                                                        RoomId = _current.RoomId,
                                                        FsuId = _current.FsuId,
                                                        DeviceId = _current.DeviceId,
                                                        PointId = _current.PointId,
                                                        SerialNo = _current.SerialNo,
                                                        NMAlarmId = _current.NMAlarmId,
                                                        AlarmTime = _current.AlarmTime,
                                                        AlarmLevel = _current.AlarmLevel,
                                                        AlarmFlag = _current.AlarmFlag,
                                                        AlarmValue = _current.AlarmValue,
                                                        AlarmDesc = _current.AlarmDesc,
                                                        AlarmRemark = _current.AlarmRemark,
                                                        Confirmed = EnmConfirm.Unconfirmed,
                                                        Confirmer = null,
                                                        ConfirmedTime = null,
                                                        ReservationId = _current.ReservationId,
                                                        ReservationName = _current.ReservationName,
                                                        ReservationStart = _current.ReservationStart,
                                                        ReservationEnd = _current.ReservationEnd,
                                                        PrimaryId = _current.PrimaryId,
                                                        RelatedId = _current.RelatedId,
                                                        FilterId = _current.FilterId,
                                                        ReversalId = _current.ReversalId,
                                                        Masked = _current.Masked,
                                                        CreatedTime = DateTime.Now
                                                    });
                                                } finally {
                                                    _iAlarmQueueLock.ExitWriteLock();
                                                }
                                            }
                                            #endregion

                                            #region 短信告警
                                            if (_menabled) {
                                                try {
                                                    _mAlarmQueueLock.EnterWriteLock();
                                                    _mAlarmQueue.Enqueue(new A_MAlarm {
                                                        Id = _current.TId,
                                                        AreaId = _current.AreaId,
                                                        AreaName = _alarm.Device.AreaName,
                                                        StationId = _current.StationId,
                                                        StationName = _alarm.Device.StationName,
                                                        RoomId = _current.RoomId,
                                                        RoomName = _alarm.Device.RoomName,
                                                        DeviceId = _current.DeviceId,
                                                        DeviceName = _alarm.Device.Name,
                                                        PointId = _current.PointId,
                                                        PointName = _alarm.Signal.Name,
                                                        SerialNo = _current.SerialNo,
                                                        NMAlarmId = _current.NMAlarmId,
                                                        AlarmTime = _current.AlarmTime,
                                                        AlarmLevel = _current.AlarmLevel,
                                                        AlarmFlag = _current.AlarmFlag,
                                                        AlarmValue = _current.AlarmValue,
                                                        AlarmDesc = _current.AlarmDesc,
                                                        AlarmRemark = _current.AlarmRemark,
                                                        Confirmed = EnmConfirm.Unconfirmed,
                                                        Confirmer = null,
                                                        ConfirmedTime = null,
                                                        ReservationId = _current.ReservationId,
                                                        PrimaryId = _current.PrimaryId,
                                                        RelatedId = _current.RelatedId,
                                                        FilterId = _current.FilterId,
                                                        ReversalId = _current.ReversalId,
                                                        Masked = _current.Masked,
                                                        CreatedTime = DateTime.Now
                                                    });
                                                } finally {
                                                    _mAlarmQueueLock.ExitWriteLock();
                                                }
                                            }
                                            #endregion

                                            #region 语音告警
                                            if (_senabled) {
                                                try {
                                                    _sAlarmQueueLock.EnterWriteLock();
                                                    _sAlarmQueue.Enqueue(new A_SAlarm {
                                                        Id = _current.TId,
                                                        AreaId = _current.AreaId,
                                                        AreaName = _alarm.Device.AreaName,
                                                        StationId = _current.StationId,
                                                        StationName = _alarm.Device.StationName,
                                                        RoomId = _current.RoomId,
                                                        RoomName = _alarm.Device.RoomName,
                                                        DeviceId = _current.DeviceId,
                                                        DeviceName = _alarm.Device.Name,
                                                        PointId = _current.PointId,
                                                        PointName = _alarm.Signal.Name,
                                                        SerialNo = _current.SerialNo,
                                                        NMAlarmId = _current.NMAlarmId,
                                                        AlarmTime = _current.AlarmTime,
                                                        AlarmLevel = _current.AlarmLevel,
                                                        AlarmFlag = _current.AlarmFlag,
                                                        AlarmValue = _current.AlarmValue,
                                                        AlarmDesc = _current.AlarmDesc,
                                                        AlarmRemark = _current.AlarmRemark,
                                                        Confirmed = EnmConfirm.Unconfirmed,
                                                        Confirmer = null,
                                                        ConfirmedTime = null,
                                                        ReservationId = _current.ReservationId,
                                                        PrimaryId = _current.PrimaryId,
                                                        RelatedId = _current.RelatedId,
                                                        FilterId = _current.FilterId,
                                                        ReversalId = _current.ReversalId,
                                                        Masked = _current.Masked,
                                                        CreatedTime = DateTime.Now
                                                    });
                                                } finally {
                                                    _sAlarmQueueLock.ExitWriteLock();
                                                }
                                            }
                                            #endregion

                                        } else {
                                            _talmRepository.Delete(_alarm.Alarm);
                                        }
                                    } catch (Exception err) {
                                        err.Source = JsonConvert.SerializeObject(_alarm.Alarm);

                                        Logger.Warning("开始告警发生错误，详见错误日志。");
                                        Logger.Error("开始告警发生错误", err);
                                    }
                                    #endregion
                                } else if (_alarm.Alarm.AlarmFlag == EnmFlag.End) {
                                    #region 结束告警
                                    try {
                                        var _active = _aalmRepository.GetEntityInPoint(_alarm.Device.Id, _alarm.Signal.PointId);
                                        if (_active != null && _alarm.Alarm.AlarmTime > _active.AlarmTime) {

                                            #region 初始化告警
                                            var _current = new AlarmEnd {
                                                Id = _active.Id,
                                                TId = _alarm.Alarm.Id,
                                                AreaId = _active.AreaId,
                                                StationId = _active.StationId,
                                                RoomId = _active.RoomId,
                                                FsuId = _active.FsuId,
                                                DeviceId = _active.DeviceId,
                                                PointId = _active.PointId,
                                                SerialNo = _active.SerialNo,
                                                NMAlarmId = _active.NMAlarmId,
                                                StartTime = _active.AlarmTime,
                                                EndTime = _alarm.Alarm.AlarmTime,
                                                AlarmLevel = _active.AlarmLevel,
                                                AlarmFlag = _alarm.Alarm.AlarmFlag,
                                                StartValue = _active.AlarmValue,
                                                EndValue = _alarm.Alarm.AlarmValue,
                                                AlarmDesc = _active.AlarmDesc,
                                                AlarmRemark = _active.AlarmRemark,
                                                Confirmed = _active.Confirmed,
                                                Confirmer = _active.Confirmer,
                                                ConfirmedTime = _active.ConfirmedTime,
                                                ReservationId = _active.ReservationId,
                                                PrimaryId = _active.PrimaryId,
                                                RelatedId = _active.RelatedId,
                                                FilterId = _active.FilterId,
                                                ReversalId = _active.ReversalId,
                                                ReversalCount = _active.ReversalCount,
                                                Masked = _active.Masked
                                            };
                                            #endregion

                                            #region 工程预约
                                            if (_ienabled && !string.IsNullOrWhiteSpace(_current.ReservationId)) {
                                                var _reservation = iPemWorkContext.Reservations.Find(r => r.Reservation.Id == _current.ReservationId);
                                                if (_reservation != null) {
                                                    _current.ReservationName = _reservation.Reservation.Name;
                                                    _current.ReservationStart = _reservation.Reservation.StartTime;
                                                    _current.ReservationEnd = _reservation.Reservation.EndTime;
                                                }
                                            }
                                            #endregion

                                            #region 结束告警
                                            _alarmer.End(_current);
                                            iPemWorkContext.RemoveAlarm(_current);
                                            #endregion

                                            #region 停电告警
                                            var offlines = new List<V_Offline>();
                                            if (iPemWorkContext.Cuttings.ContainsKey(_key)) {
                                                var kvs = iPemWorkContext.Cuttings[_key];
                                                foreach (var kv in kvs) {
                                                    offlines.Add(new V_Offline {
                                                        Id = kv.Key,
                                                        Type = kv.Value,
                                                        FormulaType = EnmFormula.TD,
                                                        StartTime = _current.StartTime,
                                                        EndTime = _current.EndTime
                                                    });
                                                }
                                            }
                                            #endregion

                                            #region 发电告警
                                            if (iPemWorkContext.Powers.ContainsKey(_key)) {
                                                var kvs = iPemWorkContext.Powers[_key];
                                                foreach (var kv in kvs) {
                                                    offlines.Add(new V_Offline {
                                                        Id = kv.Key,
                                                        Type = kv.Value,
                                                        FormulaType = EnmFormula.FD,
                                                        StartTime = _current.StartTime,
                                                        EndTime = _current.EndTime,
                                                        Value = 0
                                                    });
                                                }
                                            }

                                            //保存停电、发电数据
                                            if (offlines.Count > 0) {
                                                _offlineRepository.SaveHistory(offlines);
                                            }
                                            #endregion

                                            #region 告警接口
                                            if (_ienabled) {
                                                try {
                                                    _iAlarmQueueLock.EnterWriteLock();
                                                    _iAlarmQueue.Enqueue(new A_IAlarm {
                                                        Id = _current.TId,
                                                        AreaId = _current.AreaId,
                                                        StationId = _current.StationId,
                                                        RoomId = _current.RoomId,
                                                        FsuId = _current.FsuId,
                                                        DeviceId = _current.DeviceId,
                                                        PointId = _current.PointId,
                                                        SerialNo = _current.SerialNo,
                                                        NMAlarmId = _current.NMAlarmId,
                                                        AlarmTime = _current.EndTime,
                                                        AlarmLevel = _current.AlarmLevel,
                                                        AlarmFlag = _current.AlarmFlag,
                                                        AlarmValue = _current.EndValue,
                                                        AlarmDesc = _current.AlarmDesc,
                                                        AlarmRemark = _current.AlarmRemark,
                                                        Confirmed = _current.Confirmed,
                                                        Confirmer = _current.Confirmer,
                                                        ConfirmedTime = _current.ConfirmedTime,
                                                        ReservationId = _current.ReservationId,
                                                        ReservationName = _current.ReservationName,
                                                        ReservationStart = _current.ReservationStart,
                                                        ReservationEnd = _current.ReservationEnd,
                                                        PrimaryId = _current.PrimaryId,
                                                        RelatedId = _current.RelatedId,
                                                        FilterId = _current.FilterId,
                                                        ReversalId = _current.ReversalId,
                                                        Masked = _current.Masked,
                                                        CreatedTime = DateTime.Now
                                                    });
                                                } finally {
                                                    _iAlarmQueueLock.ExitWriteLock();
                                                }
                                            }
                                            #endregion

                                            #region 短信告警
                                            if (_menabled) {
                                                try {
                                                    _mAlarmQueueLock.EnterWriteLock();
                                                    _mAlarmQueue.Enqueue(new A_MAlarm {
                                                        Id = _current.TId,
                                                        AreaId = _current.AreaId,
                                                        AreaName = _alarm.Device.AreaName,
                                                        StationId = _current.StationId,
                                                        StationName = _alarm.Device.StationName,
                                                        RoomId = _current.RoomId,
                                                        RoomName = _alarm.Device.RoomName,
                                                        DeviceId = _current.DeviceId,
                                                        DeviceName = _alarm.Device.Name,
                                                        PointId = _current.PointId,
                                                        PointName = _alarm.Signal.Name,
                                                        SerialNo = _current.SerialNo,
                                                        NMAlarmId = _current.NMAlarmId,
                                                        AlarmTime = _current.EndTime,
                                                        AlarmLevel = _current.AlarmLevel,
                                                        AlarmFlag = _current.AlarmFlag,
                                                        AlarmValue = _current.EndValue,
                                                        AlarmDesc = _current.AlarmDesc,
                                                        AlarmRemark = _current.AlarmRemark,
                                                        Confirmed = _current.Confirmed,
                                                        Confirmer = _current.Confirmer,
                                                        ConfirmedTime = _current.ConfirmedTime,
                                                        ReservationId = _current.ReservationId,
                                                        PrimaryId = _current.PrimaryId,
                                                        RelatedId = _current.RelatedId,
                                                        FilterId = _current.FilterId,
                                                        ReversalId = _current.ReversalId,
                                                        Masked = _current.Masked,
                                                        CreatedTime = DateTime.Now
                                                    });
                                                } finally {
                                                    _mAlarmQueueLock.ExitWriteLock();
                                                }
                                            }
                                            #endregion

                                            #region 语音告警
                                            if (_senabled) {
                                                try {
                                                    _sAlarmQueueLock.EnterWriteLock();
                                                    _sAlarmQueue.Enqueue(new A_SAlarm {
                                                        Id = _current.TId,
                                                        AreaId = _current.AreaId,
                                                        AreaName = _alarm.Device.AreaName,
                                                        StationId = _current.StationId,
                                                        StationName = _alarm.Device.StationName,
                                                        RoomId = _current.RoomId,
                                                        RoomName = _alarm.Device.RoomName,
                                                        DeviceId = _current.DeviceId,
                                                        DeviceName = _alarm.Device.Name,
                                                        PointId = _current.PointId,
                                                        PointName = _alarm.Signal.Name,
                                                        SerialNo = _current.SerialNo,
                                                        NMAlarmId = _current.NMAlarmId,
                                                        AlarmTime = _current.EndTime,
                                                        AlarmLevel = _current.AlarmLevel,
                                                        AlarmFlag = _current.AlarmFlag,
                                                        AlarmValue = _current.EndValue,
                                                        AlarmDesc = _current.AlarmDesc,
                                                        AlarmRemark = _current.AlarmRemark,
                                                        Confirmed = _current.Confirmed,
                                                        Confirmer = _current.Confirmer,
                                                        ConfirmedTime = _current.ConfirmedTime,
                                                        ReservationId = _current.ReservationId,
                                                        PrimaryId = _current.PrimaryId,
                                                        RelatedId = _current.RelatedId,
                                                        FilterId = _current.FilterId,
                                                        ReversalId = _current.ReversalId,
                                                        Masked = _current.Masked,
                                                        CreatedTime = DateTime.Now
                                                    });
                                                } finally {
                                                    _sAlarmQueueLock.ExitWriteLock();
                                                }
                                            }
                                            #endregion

                                        } else {
                                            _talmRepository.Delete(_alarm.Alarm);
                                        }
                                    } catch (Exception err) {
                                        err.Source = JsonConvert.SerializeObject(_alarm.Alarm);

                                        Logger.Warning("结束告警发生错误，详见错误日志。");
                                        Logger.Error("结束告警发生错误", err);
                                    }
                                    #endregion
                                }
                            } catch (Exception err) {
                                err.Source = JsonConvert.SerializeObject(_alarm.Alarm);

                                Logger.Warning("处理告警发生错误，详见错误日志。");
                                Logger.Error("处理告警发生错误", err);
                            }
                        }
                        #endregion

                    } catch (Exception err) {
                        Logger.Warning(err.Message);
                        Logger.Error(err.Message, err);
                    } finally {
                        _configWriterLock.ExitReadLock();
                    }
                }
            }
        }

        /// <summary>
        /// 告警接口处理任务
        /// </summary>
        private void DoiAlarm() {
            _allDone.WaitOne();

            if (_runStatus != RunStatus.Running) return;
            Logger.Information("告警接口处理线程已启动。");

            var interval = 86400;
            while (_runStatus != RunStatus.Stop) {
                Thread.Sleep(1000);
                if (_runStatus == RunStatus.Running) {
                    if (interval++ <= 5) continue; interval = 1;

                    #region 处理告警接口
                    try {
                        var _ialarms = new List<A_IAlarm>();
                        try {
                            _iAlarmQueueLock.EnterWriteLock();

                            while (_iAlarmQueue.Count > 0) {
                                _ialarms.Add(_iAlarmQueue.Dequeue());
                            }
                        } finally {
                            _iAlarmQueueLock.ExitWriteLock();
                        }

                        if (_ialarms.Count > 0) _alarmer.SaveInterface(_ialarms);
                    } catch (Exception err) {
                        Logger.Warning(err.Message);
                        Logger.Error(err.Message, err);
                    }
                    #endregion

                    #region 处理短信告警 需要根据角色判断需要发送给哪些用户
                    try {
                        var _malarms = new List<A_MAlarm>();
                        try {
                            _mAlarmQueueLock.EnterWriteLock();

                            while (_mAlarmQueue.Count > 0) {
                                _malarms.Add(_mAlarmQueue.Dequeue());
                            }
                        } finally {
                            _mAlarmQueueLock.ExitWriteLock();
                        }

                        if (_malarms.Count > 0) {
                            //TODO: 需要根据角色判断需要发送给哪些用户
                            _alarmer.SaveMessage(_malarms);
                        }
                    } catch (Exception err) {
                        Logger.Warning(err.Message);
                        Logger.Error(err.Message, err);
                    }
                    #endregion

                    #region 处理语音告警 需要根据角色判断需要发送给哪些用户
                    try {
                        var _salarms = new List<A_SAlarm>();
                        try {
                            _sAlarmQueueLock.EnterWriteLock();

                            while (_sAlarmQueue.Count > 0) {
                                _salarms.Add(_sAlarmQueue.Dequeue());
                            }
                        } finally {
                            _sAlarmQueueLock.ExitWriteLock();
                        }

                        if (_salarms.Count > 0) {
                            //TODO: 需要根据角色判断需要发送给哪些用户
                            _alarmer.SaveSpeech(_salarms);
                        }
                    } catch (Exception err) {
                        Logger.Warning(err.Message);
                        Logger.Error(err.Message, err);
                    }
                    #endregion

                }
            }
        }

        /// <summary>
        /// 系统自检任务
        /// </summary>
        private void DoChecking() {
            _allDone.WaitOne();

            if (_runStatus != RunStatus.Running) return;
            Logger.Information("系统自检线程已启动。");

            #region 定义变量

            //SC中断告警变量
            var _ScOffPointId = "076013000";
            var _ScOffSignalId = "076013";
            var _ScOffSignalNumber = "000";
            var _ScOffPointLevel = EnmAlarm.Level2;
            var _ScOffPointNMAlarmId = "603-076-00-076013";

            //FSU中断告警变量
            var _FsuOffPointId = "076010000";
            var _FsuOffSignalId = "076010";
            var _FsuOffSignalNumber = "000";
            var _FsuOffPointLevel = EnmAlarm.Level2;
            var _FsuOffPointNMAlarmId = "603-076-00-076010";

            //计算SC中断变量
            var _scParam = iPemWorkContext.Params.Find(p => p.Id == ParamId.ScOff);
            if (_scParam != null && !string.IsNullOrWhiteSpace(_scParam.Value)) {
                _ScOffPointId = _scParam.Value;
                if (iPemWorkContext.Points.ContainsKey(_ScOffPointId)) {
                    var _point = iPemWorkContext.Points[_ScOffPointId];
                    _ScOffSignalId = _point.Code;
                    _ScOffSignalNumber = _point.Number;
                    _ScOffPointNMAlarmId = _point.NMAlarmId;

                    var _scSubPoint = iPemWorkContext.SubPoints.Values.FirstOrDefault(p => p.PointId == _ScOffPointId);
                    if (_scSubPoint != null) {
                        _ScOffPointLevel = _scSubPoint.AlarmLevel;
                    }
                }
            }

            //计算FSU中断变量
            var _fsuParam = iPemWorkContext.Params.Find(p => p.Id == ParamId.FsuOff);
            if (_fsuParam != null && !string.IsNullOrWhiteSpace(_fsuParam.Value)) {
                _FsuOffPointId = _fsuParam.Value;
                if (iPemWorkContext.Points.ContainsKey(_FsuOffPointId)) {
                    var _point = iPemWorkContext.Points[_FsuOffPointId];
                    _FsuOffSignalId = _point.Code;
                    _FsuOffSignalNumber = _point.Number;
                    _FsuOffPointNMAlarmId = _point.NMAlarmId;

                    var _fsuSubPoint = iPemWorkContext.SubPoints.Values.FirstOrDefault(p => p.PointId == _FsuOffPointId);
                    if (_fsuSubPoint != null) {
                        _FsuOffPointLevel = _fsuSubPoint.AlarmLevel;
                    }
                }
            }

            //加载SC\FSU告警文件对象
            if (_scXmlDoc == null)
                _scXmlDoc = CommonHelper.GetXmlDocument(SC_ALARM_CFG_DIR, SC_ALARM_CFG_FILE);

            if(_fsuXmlDoc == null)
                _fsuXmlDoc = CommonHelper.GetXmlDocument(FSU_ALARM_CFG_DIR, FSU_ALARM_CFG_FILE);

            #endregion

            var interval = 86400;
            while (_runStatus != RunStatus.Stop) {
                Thread.Sleep(1000);
                if (_runStatus == RunStatus.Running) {
                    if (interval++ <= 10) continue; interval = 1;

                    try {
                        _configWriterLock.EnterReadLock();

                        #region SC通信中断
                        try {
                            foreach (var _sc in iPemWorkContext.ScHeartbeats) {
                                var _node = (XmlElement)_scXmlDoc.SelectSingleNode(string.Format("/root/sc[@id='{0}']", _sc.Current.Id));
                                if (!_sc.Current.Status && _node == null) {
                                    #region 开始告警
                                    var _now = DateTime.Now;
                                    var _current = new A_TAlarm {
                                        FsuId = "-1",
                                        DeviceId = _sc.Current.Id,
                                        PointId = _ScOffPointId,
                                        SignalId = _ScOffSignalId,
                                        SignalNumber = _ScOffSignalNumber,
                                        SerialNo = CommonHelper.GetAlarmSerialNo(_now.Ticks),
                                        NMAlarmId = _ScOffPointNMAlarmId,
                                        AlarmTime = _now,
                                        AlarmLevel = _ScOffPointLevel,
                                        AlarmFlag = EnmFlag.Begin,
                                        AlarmValue = 1,
                                        AlarmDesc = "LSC通信前置机通信状态异常告警",
                                        AlarmRemark = "负责B接口协议数据采集软件获硬件的通信状态的告警"
                                    };

                                    _talmRepository.Save(_current);

                                    //保存告警标记
                                    _node = _scXmlDoc.CreateElement("sc");
                                    _node.SetAttribute("id", _current.DeviceId);
                                    _node.SetAttribute("point", _current.PointId);
                                    _node.SetAttribute("level", ((int)_current.AlarmLevel).ToString());
                                    _node.SetAttribute("time", _current.AlarmTime.Ticks.ToString());
                                    _scXmlDoc.DocumentElement.AppendChild(_node);
                                    CommonHelper.SaveXmlDocument(SC_ALARM_CFG_DIR, SC_ALARM_CFG_FILE, _scXmlDoc);
                                    #endregion
                                } else if (_sc.Current.Status && _node != null) {
                                    #region 结束告警
                                    var _now = DateTime.Now;
                                    var _current = new A_TAlarm {
                                        FsuId = "-1",
                                        DeviceId = _sc.Current.Id,
                                        PointId = _ScOffPointId,
                                        SignalId = _ScOffSignalId,
                                        SignalNumber = _ScOffSignalNumber,
                                        SerialNo = CommonHelper.GetAlarmSerialNo(long.Parse(_node.GetAttribute("time"))),
                                        NMAlarmId = _ScOffPointNMAlarmId,
                                        AlarmTime = _now,
                                        AlarmLevel = _ScOffPointLevel,
                                        AlarmFlag = EnmFlag.End,
                                        AlarmValue = 0,
                                        AlarmDesc = "LSC通信前置机通信状态异常告警",
                                        AlarmRemark = "负责B接口协议数据采集软件获硬件的通信状态的告警"
                                    };

                                    _talmRepository.Save(_current);

                                    //删除告警标记
                                    _scXmlDoc.DocumentElement.RemoveChild(_node);
                                    CommonHelper.SaveXmlDocument(SC_ALARM_CFG_DIR, SC_ALARM_CFG_FILE, _scXmlDoc);
                                    #endregion
                                }
                            }
                        } catch (Exception err) {
                            Logger.Warning(err.Message);
                            Logger.Error(err.Message, err);
                        }
                        #endregion

                        #region FSU通信中断
                        try {
                            var _extFsus = _fsuRepository.GetExtEntities();
                            foreach (var _ext in _extFsus) {
                                var _node = (XmlElement)_fsuXmlDoc.SelectSingleNode(string.Format("/root/fsu[@id='{0}']", _ext.Id));
                                if (!_ext.Status && _node == null) {
                                    #region 开始告警
                                    var _now = DateTime.Now;
                                    var _current = new A_TAlarm {
                                        FsuId = "-2",
                                        DeviceId = _ext.Id,
                                        PointId = _FsuOffPointId,
                                        SignalId = _FsuOffSignalId,
                                        SignalNumber = _FsuOffSignalNumber,
                                        SerialNo = CommonHelper.GetAlarmSerialNo(_now.Ticks),
                                        NMAlarmId = _FsuOffPointNMAlarmId,
                                        AlarmTime = _now,
                                        AlarmLevel = _FsuOffPointLevel,
                                        AlarmFlag = EnmFlag.Begin,
                                        AlarmValue = 1,
                                        AlarmDesc = "FSU通信中断",
                                        AlarmRemark = "FSU监控采集设备通信状态的告警"
                                    };

                                    _talmRepository.Save(_current);

                                    //保存告警标记
                                    _node = _fsuXmlDoc.CreateElement("fsu");
                                    _node.SetAttribute("id", _current.DeviceId);
                                    _node.SetAttribute("point", _current.PointId);
                                    _node.SetAttribute("level", ((int)_current.AlarmLevel).ToString());
                                    _node.SetAttribute("time", _current.AlarmTime.Ticks.ToString());
                                    _fsuXmlDoc.DocumentElement.AppendChild(_node);
                                    CommonHelper.SaveXmlDocument(FSU_ALARM_CFG_DIR, FSU_ALARM_CFG_FILE, _fsuXmlDoc);
                                    #endregion
                                } else if (_ext.Status && _node != null) {
                                    #region 结束告警
                                    var _now = DateTime.Now;
                                    var _current = new A_TAlarm {
                                        FsuId = "-2",
                                        DeviceId = _ext.Id,
                                        PointId = _FsuOffPointId,
                                        SignalId = _FsuOffSignalId,
                                        SignalNumber = _FsuOffSignalNumber,
                                        SerialNo = CommonHelper.GetAlarmSerialNo(long.Parse(_node.GetAttribute("time"))),
                                        NMAlarmId = _FsuOffPointNMAlarmId,
                                        AlarmTime = _now,
                                        AlarmLevel = _FsuOffPointLevel,
                                        AlarmFlag = EnmFlag.End,
                                        AlarmValue = 0,
                                        AlarmDesc = "FSU通信中断",
                                        AlarmRemark = "FSU监控采集设备通信状态的告警"
                                    };

                                    _talmRepository.Save(_current);

                                    //删除告警标记
                                    _fsuXmlDoc.DocumentElement.RemoveChild(_node);
                                    CommonHelper.SaveXmlDocument(FSU_ALARM_CFG_DIR, FSU_ALARM_CFG_FILE, _fsuXmlDoc);
                                    #endregion
                                }
                            }
                        } catch (Exception err) {
                            Logger.Warning(err.Message);
                            Logger.Error(err.Message, err);
                        }
                        #endregion

                    } catch (Exception err) {
                        Logger.Warning(err.Message);
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

            if (_runStatus != RunStatus.Running) return;
            Logger.Information("SC心跳线程已启动。");
            var interval = 86400;
            while (_runStatus != RunStatus.Stop) {
                Thread.Sleep(1000);
                if (_runStatus == RunStatus.Running) {
                    if (interval++ <= 20) continue; interval = 1;

                    try {
                        _configWriterLock.EnterReadLock();

                        foreach (var beat in iPemWorkContext.ScHeartbeats) {
                            try {
                                var package = beat.KeepAlive();
                                if (package.Result == EnmBIResult.FAILURE) 
                                    throw new Exception("SC通信中断");

                                #region 通信正常
                                try {
                                    if (!beat.Current.Status) {
                                        var now = DateTime.Now;
                                        _groupRepository.SetOn(beat.Current.Id, now);
                                        beat.Current.Status = true;
                                        beat.Current.ChangeTime = now;
                                    }
                                } catch (Exception err) {
                                    Logger.Warning(err.Message);
                                    Logger.Error(err.Message, err);
                                } finally {
                                    beat.SetCount(true);
                                }
                                #endregion
                            } catch {
                                #region 通信中断
                                try {
                                    if (beat.IsOff && beat.Current.Status) {
                                        var _now = DateTime.Now;
                                        _groupRepository.SetOff(beat.Current.Id, _now);
                                        beat.Current.Status = false;
                                        beat.Current.LastTime = _now;
                                    }
                                } catch (Exception err) {
                                    Logger.Warning(err.Message);
                                    Logger.Error(err.Message, err);
                                } finally {
                                    beat.SetCount();
                                }
                                #endregion
                            }
                        }
                    } catch (Exception err) {
                        Logger.Warning(err.Message);
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
        private void DoFormula() {
            _allDone.WaitOne();

            if (_runStatus != RunStatus.Running) return;
            Logger.Information("实时能耗线程已启动。");
            var interval = 86400;
            while (_runStatus != RunStatus.Stop) {
                Thread.Sleep(1000);
                if (_runStatus == RunStatus.Running) {
                    if (interval++ <= 60) continue;
                    interval = 1;

                    try {
                        _configWriterLock.EnterReadLock();

                        if (iPemWorkContext.Formulas == null || iPemWorkContext.Formulas.Count == 0) 
                            continue;

                        var _offlines = _offlineRepository.GetActive(EnmFormula.FD);
                        var _param = iPemWorkContext.Params.Find(p => p.Id == ParamId.NHZQ);
                        var _period = _param == null ? PeriodType.Hour : (PeriodType)int.Parse(_param.Value);
                        var _eleResult = new List<V_Elec>();
                        var _offResult = new List<V_Offline>();
                        foreach (var _formula in iPemWorkContext.Formulas) {
                            try {
                                var _formulaValue = _formula.FormulaValue;
                                if (string.IsNullOrWhiteSpace(_formulaValue)) continue;
                                if (!CommonHelper.ValidateFormula(_formulaValue)) continue;

                                var _variables = CommonHelper.GetFormulaVariables(_formulaValue);
                                if (_formula.FormulaType == EnmFormula.WD || _formula.FormulaType == EnmFormula.SD) {
                                    #region 温湿度
                                    if (_variables.Count == 0) continue;
                                    foreach (var _variable in _variables) {
                                        var _factors = _variable.Split(new string[] { ">>" }, StringSplitOptions.None);
                                        if (_factors.Length != 2) continue;

                                        var _device = _factors[0].Substring(1);
                                        var _point = _factors[1];
                                        var _current = _ameasureRepository.GetEntity(_device, _point);
                                        if (_current != null) {
                                            _eleResult.Add(new V_Elec {
                                                Id = _formula.Id,
                                                Type = _formula.Type,
                                                FormulaType = _formula.FormulaType,
                                                StartTime = _current.UpdateTime,
                                                EndTime = DateTime.Now,
                                                Value = _current.Value
                                            });
                                        }
                                        break;
                                    }
                                    #endregion
                                } else if (_formula.FormulaType == EnmFormula.XS) {
                                    #region 线损
                                    if (_variables.Count == 0) {
                                        #region 公式中没有变量
                                        var _data = 0d;
                                        var _value = _computer.Compute(_formulaValue, "");
                                        if (_value != DBNull.Value) _data = Convert.ToDouble(_value);
                                        if (double.IsNaN(_data) || double.IsInfinity(_data)) _data = 0d;
                                        _data = Math.Round(_data, 3, MidpointRounding.AwayFromZero);

                                        _eleResult.Add(new V_Elec {
                                            Id = _formula.Id,
                                            Type = _formula.Type,
                                            FormulaType = _formula.FormulaType,
                                            StartTime = DateTime.Now,
                                            EndTime = DateTime.Now,
                                            Value = _data
                                        });
                                        #endregion
                                    } else {
                                        #region 公式中有变量
                                        var ignore = false;
                                        var _current = _formulaValue;
                                        var _details = new List<VariableDetail>();
                                        foreach (var _variable in _variables) {
                                            var _factors = _variable.Split(new string[] { ">>" }, StringSplitOptions.None);
                                            if (_factors.Length != 2) {
                                                ignore = true;
                                                break;
                                            }

                                            _details.Add(new VariableDetail {
                                                DeviceId = _factors[0].Substring(1),
                                                PointId = _factors[1],
                                                Variable = _variable
                                            });
                                        }

                                        if (ignore) continue;

                                        ignore = false;
                                        var _values = _ameasureRepository.GetEntities(_details);
                                        foreach (var _detail in _details) {
                                            var _curval = _values.Find(v => v.DeviceId.Equals(_detail.DeviceId) && v.PointId.Equals(_detail.PointId));
                                            if (_curval == null || _curval.Value == double.MinValue) { ignore = true; break; }
                                            _current = _current.Replace(_detail.Variable, _curval.Value.ToString());
                                        }

                                        if (ignore) continue;

                                        var _data = 0d;
                                        var _value = _computer.Compute(_current, "");
                                        if (_value != DBNull.Value) _data = Convert.ToDouble(_value);
                                        if (double.IsNaN(_data) || double.IsInfinity(_data)) _data = 0d;
                                        _data = Math.Round(_data, 3, MidpointRounding.AwayFromZero);

                                        _eleResult.Add(new V_Elec {
                                            Id = _formula.Id,
                                            Type = _formula.Type,
                                            FormulaType = _formula.FormulaType,
                                            StartTime = DateTime.Now,
                                            EndTime = DateTime.Now,
                                            Value = _data
                                        });
                                        #endregion
                                    }
                                    #endregion
                                } else if (_formula.FormulaType == EnmFormula.FDL) {
                                    #region 发电量
                                    var _offline = _offlines.Find(o => o.Id.Equals(_formula.Id) && o.Type.Equals(_formula.Type));
                                    if (_offline == null) continue;

                                    var _start = _offline.StartTime;
                                    var _end = DateTime.Now;
                                    if (_variables.Count == 0) {
                                        #region 公式中没有变量
                                        var _data = 0d;
                                        var _value = _computer.Compute(_formulaValue, "");
                                        if (_value != DBNull.Value) _data = Convert.ToDouble(_value);
                                        if (double.IsNaN(_data) || double.IsInfinity(_data)) _data = 0d;
                                        _offline.Value = Math.Round(_data, 3, MidpointRounding.AwayFromZero);
                                        _offResult.Add(_offline);
                                        #endregion
                                    } else {
                                        #region 公式中有变量
                                        var ignore = false;
                                        var _current = _formulaValue;
                                        var _details = new List<VariableDetail>();
                                        foreach (var _variable in _variables) {
                                            var _factors = _variable.Split(new string[] { ">>" }, StringSplitOptions.None);
                                            if (_factors.Length != 2) {
                                                ignore = true;
                                                break;
                                            }

                                            _details.Add(new VariableDetail {
                                                DeviceId = _factors[0].Substring(1),
                                                PointId = _factors[1],
                                                Variable = _variable
                                            });
                                        }

                                        if (ignore) continue;

                                        ignore = false;
                                        foreach (var _detail in _details) {
                                            var _vvalue = 0d;
                                            if (_formula.ComputeType == EnmCompute.Kwh) {
                                                var _first = _hmeasureRepository.GetFirst(_detail.DeviceId, _detail.PointId, _start, _end);
                                                if (_first == null) { ignore = true; break; }

                                                var _last = _ameasureRepository.GetEntity(_detail.DeviceId, _detail.PointId);
                                                if (_last == null) { ignore = true; break; }

                                                _vvalue = Math.Round(_last.Value - _first.Value, 3, MidpointRounding.AwayFromZero);
                                            } else if (_formula.ComputeType == EnmCompute.Power) {
                                                _vvalue = _hmeasureRepository.GetAvg(_detail.DeviceId, _detail.PointId, _start, _end);
                                                if (_vvalue == double.MinValue) { ignore = true; break; }
                                            }

                                            _current = _current.Replace(_detail.Variable, _vvalue.ToString());
                                        }

                                        if (ignore) continue;

                                        var _data = 0d;
                                        var _value = _computer.Compute(_current, "");
                                        if (_value != DBNull.Value) _data = Convert.ToDouble(_value);
                                        if (double.IsNaN(_data) || double.IsInfinity(_data)) _data = 0d;
                                        _offline.Value = Math.Round(_data * (_formula.ComputeType == EnmCompute.Kwh ? 1 : _end.Subtract(_start).TotalHours), 3, MidpointRounding.AwayFromZero);
                                        _offResult.Add(_offline);
                                        #endregion
                                    }
                                    #endregion
                                } else {
                                    #region 能耗
                                    var _start = GetPeriod(_period);
                                    var _end = DateTime.Now;
                                    if (_variables.Count == 0) {
                                        #region 公式中没有变量
                                        var _data = 0d;
                                        var _value = _computer.Compute(_formulaValue, "");
                                        if (_value != DBNull.Value) _data = Convert.ToDouble(_value);
                                        if (double.IsNaN(_data) || double.IsInfinity(_data)) _data = 0d;
                                        _data = Math.Round(_data * (_formula.ComputeType == EnmCompute.Kwh ? 1 : _end.Subtract(_start).TotalHours), 3, MidpointRounding.AwayFromZero);

                                        _eleResult.Add(new V_Elec {
                                            Id = _formula.Id,
                                            Type = _formula.Type,
                                            FormulaType = _formula.FormulaType,
                                            StartTime = _start,
                                            EndTime = _end,
                                            Value = _data
                                        });
                                        #endregion
                                    } else {
                                        #region 公式中有变量
                                        var ignore = false;
                                        var _current = _formulaValue;
                                        var _details = new List<VariableDetail>();
                                        foreach (var _variable in _variables) {
                                            var _factors = _variable.Split(new string[] { ">>" }, StringSplitOptions.None);
                                            if (_factors.Length != 2) {
                                                ignore = true;
                                                break;
                                            }

                                            _details.Add(new VariableDetail {
                                                DeviceId = _factors[0].Substring(1),
                                                PointId = _factors[1],
                                                Variable = _variable
                                            });
                                        }

                                        if (ignore) continue;

                                        ignore = false;
                                        foreach (var _detail in _details) {
                                            var _vvalue = 0d;
                                            if (_formula.ComputeType == EnmCompute.Kwh) {
                                                var _first = _hmeasureRepository.GetFirst(_detail.DeviceId, _detail.PointId, _start, _end);
                                                if (_first == null) { ignore = true; break; }

                                                var _last = _ameasureRepository.GetEntity(_detail.DeviceId, _detail.PointId);
                                                if (_last == null) { ignore = true; break; }

                                                _vvalue = Math.Round(_last.Value - _first.Value, 3, MidpointRounding.AwayFromZero);
                                            } else if (_formula.ComputeType == EnmCompute.Power) {
                                                _vvalue = _hmeasureRepository.GetAvg(_detail.DeviceId, _detail.PointId, _start, _end);
                                                if (_vvalue == double.MinValue) { ignore = true; break; }
                                            }

                                            _current = _current.Replace(_detail.Variable, _vvalue.ToString());
                                        }

                                        if (ignore) continue;

                                        var _data = 0d;
                                        var _value = _computer.Compute(_current, "");
                                        if (_value != DBNull.Value) _data = Convert.ToDouble(_value);
                                        if (double.IsNaN(_data) || double.IsInfinity(_data)) _data = 0d;
                                        _data = Math.Round(_data * (_formula.ComputeType == EnmCompute.Kwh ? 1 : _end.Subtract(_start).TotalHours), 3, MidpointRounding.AwayFromZero);

                                        _eleResult.Add(new V_Elec {
                                            Id = _formula.Id,
                                            Type = _formula.Type,
                                            FormulaType = _formula.FormulaType,
                                            StartTime = _start,
                                            EndTime = _end,
                                            Value = _data
                                        });
                                        #endregion
                                    }
                                    #endregion
                                }
                            } catch { }
                        }

                        if (_eleResult.Count > 0) {
                            #region PUE
                            var its = _eleResult.FindAll(r => r.FormulaType == EnmFormula.IT && r.Value > 0);
                            foreach (var it in its) {
                                var tt = _eleResult.Find(r => r.Id.Equals(it.Id) && r.Type == it.Type && r.FormulaType == EnmFormula.TT);
                                if (tt != null && tt.Value > 0) {
                                    _eleResult.Add(new V_Elec {
                                        Id = it.Id,
                                        Type = it.Type,
                                        FormulaType = EnmFormula.PUE,
                                        StartTime = it.StartTime,
                                        EndTime = it.EndTime,
                                        Value = Math.Round(tt.Value / it.Value, 3, MidpointRounding.AwayFromZero)
                                    });
                                }
                            }
                            #endregion

                            _elecRepository.SaveActive(_eleResult);
                        }

                        if (_offResult.Count > 0) {
                            _offlineRepository.UpdateActive(_offResult);
                        }
                    } catch (Exception err) {
                        Logger.Warning(err.Message);
                        Logger.Error(err.Message, err);
                    } finally {
                        _configWriterLock.ExitReadLock();
                    }
                }
            }
        }

        /// <summary>
        /// 虚拟信号处理任务
        /// </summary>
        private void DoVSignal() {
            _allDone.WaitOne();

            if (_runStatus != RunStatus.Running) return;
            var seconds = 0;
            var _curParam = iPemWorkContext.Params.Find(p => p.Id == ParamId.XXPL);
            if (_curParam == null || !int.TryParse(_curParam.Value, out seconds) || seconds <= 0) {
                Logger.Warning("虚拟信号处理线程未开启，线程退出。");
                return;
            }

            Logger.Information("虚拟信号处理线程已启动。");
            var interval = 86400;
            var cabkeys = new Dictionary<string, DateTime>();
            while (_runStatus != RunStatus.Stop) {
                Thread.Sleep(1000);
                if (_runStatus == RunStatus.Running) {
                    if (interval++ <= seconds) continue;
                    interval = 1;
                    
                    try {
                        _configWriterLock.EnterReadLock();

                        if (iPemWorkContext.VSignals == null || iPemWorkContext.VSignals.Count == 0) 
                            continue;

                        var _actives = new List<V_AMeasure>();
                        var _histories = new List<V_HMeasure>();
                        var _cabinets = new List<V_ACabinet>();
                        foreach (var _vsignal in iPemWorkContext.VSignals) {
                            try {
                                var _formulaValue = _vsignal.FormulaValue;
                                if (string.IsNullOrWhiteSpace(_formulaValue)) continue;
                                if (!CommonHelper.ValidateFormula(_formulaValue)) continue;

                                var _now = DateTime.Now;
                                var _pre = _now.Date.AddHours(_now.Hour - 1);
                                var _key = string.Format("{0}-{1}", _vsignal.DeviceId, _vsignal.PointId);
                                var _iscabinet = (_vsignal.Category == EnmVSignalCategory.Category03 || _vsignal.Category == EnmVSignalCategory.Category04 || _vsignal.Category == EnmVSignalCategory.Category05);
                                
                                V_ACabinet _cabinet = null;
                                if (_iscabinet) {
                                    if (!cabkeys.ContainsKey(_key)) {
                                        cabkeys[_key] = _pre;
                                    } else {
                                        var _last = cabkeys[_key];
                                        if (_pre > _last) {
                                            _cabinet = new V_ACabinet {
                                                DeviceId = _vsignal.DeviceId,
                                                PointId = _vsignal.PointId,
                                                Category = _vsignal.Category,
                                                Value = 0,
                                                ValueTime = _pre,
                                                AValue = double.MinValue,
                                                BValue = double.MinValue,
                                                CValue = double.MinValue
                                            };
                                        }
                                    }
                                }

                                var _variables = CommonHelper.GetFormulaVariables(_formulaValue);
                                if (_variables.Count == 0) {
                                    #region 公式中没有变量
                                    var _data = 0d;
                                    var _value = _computer.Compute(_formulaValue, "");
                                    if (_value != DBNull.Value) _data = Convert.ToDouble(_value);
                                    if (double.IsNaN(_data) || double.IsInfinity(_data)) _data = 0d;
                                    _data = Math.Round(_data, 3, MidpointRounding.AwayFromZero);

                                    _actives.Add(new V_AMeasure {
                                        AreaId = _vsignal.AreaId,
                                        StationId = _vsignal.StationId,
                                        RoomId = _vsignal.RoomId,
                                        FsuId = _vsignal.FsuId,
                                        DeviceId = _vsignal.DeviceId,
                                        PointId = _vsignal.PointId,
                                        SignalDesc = _vsignal.UnitState,
                                        Status = EnmState.Normal,
                                        Value = _data,
                                        UpdateTime = _now
                                    });

                                    if (_vsignal.SavedPeriod > 0
                                        && _vsignal.SavedTime.AddSeconds(_vsignal.SavedPeriod) <= _now) {
                                        _vsignal.SavedTime = _now;
                                        _histories.Add(new V_HMeasure {
                                            AreaId = _vsignal.AreaId,
                                            StationId = _vsignal.StationId,
                                            RoomId = _vsignal.RoomId,
                                            FsuId = _vsignal.FsuId,
                                            DeviceId = _vsignal.DeviceId,
                                            PointId = _vsignal.PointId,
                                            SignalDesc = _vsignal.UnitState,
                                            Type = 0,
                                            Value = _data,
                                            UpdateTime = _now
                                        });
                                    }

                                    if (_cabinet != null) {
                                        _cabinet.Value = _data;
                                        _cabinets.Add(_cabinet);
                                        cabkeys[_key] = _pre;
                                    }
                                    #endregion
                                } else {
                                    #region 公式中有变量
                                    var ignore = false;
                                    var _current = _formulaValue;
                                    var _details = new List<VariableDetail>();
                                    foreach (var _variable in _variables) {
                                        var _factors = _variable.Split(new string[] { ">>" }, StringSplitOptions.None);
                                        if (_factors.Length != 2) {
                                            ignore = true;
                                            break;
                                        }

                                        _details.Add(new VariableDetail {
                                            DeviceId = _factors[0].Substring(1),
                                            PointId = _factors[1],
                                            Variable = _variable
                                        });
                                    }

                                    if (ignore) continue;

                                    ignore = false;
                                    var _values = _ameasureRepository.GetEntities(_details);
                                    for (var i = 0; i < _details.Count; i++) {
                                        var _detail = _details[i];
                                        var _curval = _values.Find(v => v.DeviceId.Equals(_detail.DeviceId) && v.PointId.Equals(_detail.PointId));
                                        if (_curval == null || _curval.Value == double.MinValue) { ignore = true; break; }
                                        _current = _current.Replace(_detail.Variable, _curval.Value.ToString());

                                        if (_cabinet != null) {
                                            if (i == 0) {
                                                _cabinet.AValue = _curval.Value;
                                                _cabinet.AValueTime = _curval.UpdateTime;
                                            } else if (i == 1) {
                                                _cabinet.BValue = _curval.Value;
                                                _cabinet.BValueTime = _curval.UpdateTime;
                                            } else if (i == 2) {
                                                _cabinet.CValue = _curval.Value;
                                                _cabinet.CValueTime = _curval.UpdateTime;
                                            }
                                        }
                                    }

                                    if (ignore) {
                                        _actives.Add(new V_AMeasure {
                                            AreaId = _vsignal.AreaId,
                                            StationId = _vsignal.StationId,
                                            RoomId = _vsignal.RoomId,
                                            FsuId = _vsignal.FsuId,
                                            DeviceId = _vsignal.DeviceId,
                                            PointId = _vsignal.PointId,
                                            SignalDesc = _vsignal.UnitState,
                                            Status = EnmState.Invalid,
                                            Value = 0,
                                            UpdateTime = DateTime.Now
                                        });
                                        continue;
                                    }

                                    var _data = 0d;
                                    var _value = _computer.Compute(_current, "");
                                    if (_value != DBNull.Value) _data = Convert.ToDouble(_value);
                                    if (double.IsNaN(_data) || double.IsInfinity(_data)) _data = 0d;
                                    _data = Math.Round(_data, 3, MidpointRounding.AwayFromZero);

                                    _actives.Add(new V_AMeasure {
                                        AreaId = _vsignal.AreaId,
                                        StationId = _vsignal.StationId,
                                        RoomId = _vsignal.RoomId,
                                        FsuId = _vsignal.FsuId,
                                        DeviceId = _vsignal.DeviceId,
                                        PointId = _vsignal.PointId,
                                        SignalDesc = _vsignal.UnitState,
                                        Status = EnmState.Normal,
                                        Value = _data,
                                        UpdateTime = _now
                                    });

                                    if (_vsignal.SavedPeriod > 0
                                        && _vsignal.SavedTime.AddSeconds(_vsignal.SavedPeriod) <= _now) {
                                        _vsignal.SavedTime = _now;
                                        _histories.Add(new V_HMeasure {
                                            AreaId = _vsignal.AreaId,
                                            StationId = _vsignal.StationId,
                                            RoomId = _vsignal.RoomId,
                                            FsuId = _vsignal.FsuId,
                                            DeviceId = _vsignal.DeviceId,
                                            PointId = _vsignal.PointId,
                                            SignalDesc = _vsignal.UnitState,
                                            Type = 0,
                                            Value = _data,
                                            UpdateTime = _now
                                        });
                                    }

                                    if (_cabinet != null) {
                                        _cabinet.Value = _data;
                                        _cabinets.Add(_cabinet);
                                        cabkeys[_key] = _pre;
                                    }
                                    #endregion
                                }
                            } catch { }
                        }

                        if (_actives.Count > 0) {
                            _ameasureRepository.Save(_actives);
                        }

                        if (_histories.Count > 0) {
                            _hmeasureRepository.Save(_histories);
                        }

                        if (_cabinets.Count > 0) {
                            _acabinetRepository.Save(_cabinets);
                        }
                    } catch (Exception err) {
                        Logger.Warning(err.Message);
                        Logger.Error(err.Message, err);
                    } finally {
                        _configWriterLock.ExitReadLock();
                    }
                }
            }
        }

        /// <summary>
        /// 电池数据处理任务
        /// </summary>
        private void DoBat() {
            _allDone.WaitOne();

            if (_runStatus != RunStatus.Running) return;
            var _curParam = iPemWorkContext.Params.Find(p => p.Id == ParamId.DCSJ);
            if (_curParam == null || !"1".Equals(_curParam.Value)) {
                Logger.Warning("电池数据处理功能未开启，线程退出。");
                return;
            }

            Logger.Information("电池数据处理线程已启动。");
            var interval = 86400;
            while (_runStatus != RunStatus.Stop) {
                Thread.Sleep(1000);
                if (_runStatus == RunStatus.Running) {
                    if (interval++ <= 60) continue; 
                    interval = 1;

                    #region 屏蔽电池数据处理
                    /*
                    try {
                        _configWriterLock.EnterReadLock();
                        //以下时间是错误的，需要修改。。。
                        var _start = DateTime.Now;
                        var _end = DateTime.Now;
                        foreach (var _model in GlobalConfig.BatModels) {
                            try {
                                var _values = _hmeasureRepository.GetEntities(_model.DeviceId, _start, _end);
                                if (_values.Count == 0) continue;

                                var _mvalues = _values.FindAll(v => v.PointId == _model.PointId);
                                if (_mvalues.Count == 0) continue;

                                BatDetail _current = null;
                                var _details = new List<BatDetail>();
                                foreach (var _value in _mvalues) {
                                    if (_value.Value < _model.Voltage) {
                                        if (_current == null)
                                            _current = new BatDetail {
                                                AreaId = _model.AreaId,
                                                StationId = _model.StationId,
                                                RoomId = _model.RoomId,
                                                FsuId = _model.FsuId,
                                                DeviceId = _model.DeviceId,
                                                PointId = _model.PointId,
                                                PackId = _model.PackId,
                                                StartTime = _value.UpdateTime,
                                                StartValue = _value.Value,
                                                Values = new List<KV<DateTime, double>>()
                                            };

                                        _current.Values.Add(new KV<DateTime, double> { Key = _value.UpdateTime, Value = _value.Value });
                                    } else if (_value.Value >= _model.Voltage && _current != null) {
                                        _current.EndTime = _value.UpdateTime;
                                        _current.EndValue = _value.Value;
                                        _current.Values.Add(new KV<DateTime, double> { Key = _value.UpdateTime, Value = _value.Value });
                                        _details.Add(_current);
                                        _current = null;
                                    }
                                }

                                if (_current != null) {
                                    var _last = _current.Values.LastOrDefault();
                                    if (_last != null) {
                                        _current.EndTime = _last.Key;
                                        _current.EndValue = _last.Value;
                                        _details.Add(_current);
                                    }

                                    _current = null;
                                }

                                // 放电时间大于15分钟为正常放电
                                _details = _details.FindAll(d => d.EndTime.Subtract(d.StartTime).TotalMinutes >= 15);

                                var __details = new List<BatDetail>();
                                foreach (var _point in _model.SubSignals) {
                                    foreach (var _detail in _details) {
                                        var __values = _values.FindAll(v => v.PointId == _point && v.UpdateTime >= _detail.StartTime && v.UpdateTime <= _detail.EndTime);
                                        if (__values.Count > 0) {
                                            var _first = __values.First();
                                            var _last = __values.Last();
                                            __details.Add(new BatDetail {
                                                AreaId = _model.AreaId,
                                                StationId = _model.StationId,
                                                RoomId = _model.RoomId,
                                                FsuId = _model.FsuId,
                                                DeviceId = _model.DeviceId,
                                                PointId = _point,
                                                PackId = _model.PackId,
                                                StartTime = _first.UpdateTime,
                                                StartValue = _first.Value,
                                                EndTime = _last.UpdateTime,
                                                EndValue = _last.Value,
                                                Values = __values.Select(v => new KV<DateTime, double> { Key = v.UpdateTime, Value = v.Value }).ToList()
                                            });
                                        }
                                    }
                                }

                                var batValues = new List<V_Bat>();
                                foreach (var _detail in _details) {
                                    foreach (var _value in _detail.Values) {
                                        batValues.Add(new V_Bat {
                                            AreaId = _detail.AreaId,
                                            StationId = _detail.StationId,
                                            RoomId = _detail.RoomId,
                                            DeviceId = _detail.DeviceId,
                                            PointId = _detail.PointId,
                                            PackId = _detail.PackId,
                                            StartTime = _detail.StartTime,
                                            Value = _value.Value,
                                            ValueTime = _value.Key
                                        });
                                    }
                                }

                                foreach (var __detail in __details) {
                                    foreach (var __value in __detail.Values) {
                                        batValues.Add(new V_Bat {
                                            AreaId = __detail.AreaId,
                                            StationId = __detail.StationId,
                                            RoomId = __detail.RoomId,
                                            DeviceId = __detail.DeviceId,
                                            PointId = __detail.PointId,
                                            PackId = __detail.PackId,
                                            StartTime = __detail.StartTime,
                                            Value = __value.Value,
                                            ValueTime = __value.Key
                                        });
                                    }
                                }

                                _batRepository.DeleteEntities(_model.DeviceId, _model.PointId, _start, _end);
                                _batRepository.SaveEntities(batValues);
                            } catch (Exception err) {
                                Logger.Warning("电池数据处理发生错误，详见错误日志。");
                                Logger.Error("电池数据处理发生错误", err);
                            }
                        }
                    } catch (Exception err) {
                        Logger.Warning(err.Message);
                        Logger.Error(err.Message, err);
                    } finally {
                        _configWriterLock.ExitReadLock();
                    }
                    */
                    #endregion
                }
            }
        }

        /// <summary>
        /// 数据库索引任务
        /// </summary>
        private void DoIndex() {
            _allDone.WaitOne();

            if (_runStatus != RunStatus.Running) return;
            Logger.Information("数据库索引线程已启动。");

            #region 定义变量

            //上次检索数据库索引的时间
            DateTime _NextIndexer = DateTime.Today;

            //上次重启IIS的时间
            DateTime _NextIISReset = DateTime.Today;

            //上次日志清理时间
            DateTime _NextLogClean = DateTime.Today;

            //上次机器标识监测时间
            DateTime _NextMacCheck = DateTime.Today;

            #endregion

            var interval = 86400;
            while (_runStatus != RunStatus.Stop) {
                Thread.Sleep(1000);
                if (_runStatus == RunStatus.Running) {
                    if (interval++ <= 60) continue; interval = 1;

                    try {
                        _configWriterLock.EnterReadLock();

                        #region 机器标识监测
                        if (!string.IsNullOrWhiteSpace(_macId) && DateTime.Now > _NextMacCheck) {
                            try {
                                var dictionary = _dictionaryRepository.GetEntity(5);
                                if(dictionary != null) {
                                    if (string.IsNullOrWhiteSpace(dictionary.ValuesJson)) {
                                        dictionary.ValuesJson = _macId;
                                        dictionary.LastUpdatedDate = DateTime.Now;
                                        _dictionaryRepository.Update(dictionary);
                                        Logger.Information(string.Format("生成机器标识码，[{0}]。", _macId));
                                    } else {
                                        var macId = dictionary.ValuesJson;
                                        if (!_macId.Equals(macId)) {
                                            dictionary.ValuesJson = _macId;
                                            dictionary.LastUpdatedDate = DateTime.Now;
                                            _dictionaryRepository.Update(dictionary);
                                            Logger.Information(string.Format("更新机器标识码，[{0}]=>[{1}]。", macId, _macId));
                                        }
                                    }
                                }
                            } catch (Exception err) {
                                Logger.Warning(err.Message);
                                Logger.Error(err.Message, err);
                            } finally {
                                _NextMacCheck = DateTime.Now.AddMinutes(30);
                            }
                        }
                        #endregion

                        #region 重启IIS服务
                        if (DateTime.Now > _NextIISReset) {
                            try {
                                CommonHelper.ResetIIS();
                            } catch (Exception err) {
                                Logger.Warning(err.Message);
                                Logger.Error(err.Message, err);
                            } finally {
                                //每月重启一次IIS
                                _NextIISReset = DateTime.Today.AddMonths(1);
                            }
                        }
                        #endregion

                        #region 日志容量管理
                        if (DateTime.Now > _NextLogClean) {
                            try {
                                Logger.Clear();
                            } catch (Exception err) {
                                Logger.Warning(err.Message);
                                Logger.Error(err.Message, err);
                            } finally {
                                _NextLogClean = DateTime.Today.AddHours(25);
                            }
                        }
                        #endregion

                        #region 检查数据库索引
                        if (DateTime.Now > _NextIndexer) {
                            for (var i = 1; i <= 3; i++) {
                                try {
                                    _indexer.Check(_NextIndexer.AddMonths(i * -1));
                                } catch (Exception err) {
                                    Logger.Warning(err.Message);
                                    Logger.Error(err.Message, err);
                                }
                            }

                            _NextIndexer = DateTime.Today.AddHours(26);
                        }
                        #endregion

                    } catch (Exception err) {
                        Logger.Warning(err.Message);
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

            if (_runStatus != RunStatus.Running) return;
            var _curTask = iPemWorkContext.Tasks.Find(t => t.Id == "T001");
            if (_curTask == null) return;

            if (_curTask.Json == null) {
                Logger.Warning(string.Format("配置无效，{0}线程退出。", _curTask.Name));
                return;
            }

            if (_curTask.Json.EndDate < DateTime.Now) {
                Logger.Warning(string.Format("计划已过期，{0}线程退出。", _curTask.Name));
                return;
            }

            Logger.Information(string.Format("{0}线程已启动。", _curTask.Name));
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
                        //获取读资源锁
                        _configWriterLock.EnterReadLock();

                        this.ExTask001(_curTask.Start, _curTask.End);
                    } catch (Exception err) {
                        Logger.Warning(err.Message);
                        Logger.Error(err.Message, err);
                    } finally {
                        _configWriterLock.ExitReadLock();
                        iPemWorkContext.SetTaskPloy(_curTask);
                        Logger.Information(string.Format("{0}执行完成，下次执行时间：{1}。", _curTask.Name, CommonHelper.ToDateTimeString(_curTask.Next)));
                    }
                }
            }
        }

        /// <summary>
        /// 能耗处理方法
        /// </summary>
        private void ExTask001(DateTime start, DateTime end) {
            if (iPemWorkContext.Formulas == null || iPemWorkContext.Formulas.Count == 0) 
                return;

            var _hours = 1;
            var _times = CommonHelper.GetHourSpan(start, end);
            if (_times.Count == 0) return;

            var _eleResult = new List<V_Elec>();
            var _offResult = new List<V_Offline>();
            var _offlines = _offlineRepository.GetHistory(EnmFormula.FD, start, end);
            foreach (var _formula in iPemWorkContext.Formulas) {
                try {
                    var _formulaValue = _formula.FormulaValue;
                    if (string.IsNullOrWhiteSpace(_formulaValue)) continue;
                    if (!CommonHelper.ValidateFormula(_formulaValue))
                        throw new Exception("无效的公式。");

                    var _variables = CommonHelper.GetFormulaVariables(_formulaValue);
                    if (_formula.FormulaType == EnmFormula.WD || _formula.FormulaType == EnmFormula.SD) {
                        continue;
                    } else if (_formula.FormulaType == EnmFormula.XS) {
                        #region 线损
                        if (_variables.Count == 0) {
                            #region 公式中没有变量
                            var _data = 0d;
                            var _value = _computer.Compute(_formulaValue, "");
                            if (_value != DBNull.Value) _data = Convert.ToDouble(_value);
                            if (double.IsNaN(_data) || double.IsInfinity(_data)) _data = 0d;
                            _data = Math.Round(_data * (_formula.ComputeType == EnmCompute.Kwh ? 1 : _hours), 3, MidpointRounding.AwayFromZero);

                            foreach (var _time in _times) {
                                _eleResult.Add(new V_Elec {
                                    Id = _formula.Id,
                                    Type = _formula.Type,
                                    FormulaType = _formula.FormulaType,
                                    StartTime = _time,
                                    EndTime = _time.AddHours(_hours),
                                    Value = _data
                                });
                            }
                            #endregion
                        } else {
                            #region 公式中有变量
                            var ignore = false;
                            var _details = new List<VariableDetail>();
                            foreach (var _variable in _variables) {
                                var _factors = _variable.Split(new string[] { ">>" }, StringSplitOptions.None);
                                if (_factors.Length != 2) {
                                    ignore = true;
                                    break;
                                }

                                _details.Add(new VariableDetail {
                                    DeviceId = _factors[0].Substring(1),
                                    PointId = _factors[1],
                                    Variable = _variable
                                });
                            }

                            if (ignore) continue;

                            foreach (var _time in _times) {
                                var _start = _time;
                                var _end = _start.AddHours(_hours);
                                var _current = _formulaValue;

                                ignore = false;
                                foreach (var _detail in _details) {
                                    var _vvalue = 0d;
                                    if (_formula.ComputeType == EnmCompute.Kwh) {
                                        var _first = _hmeasureRepository.GetFirst(_detail.DeviceId, _detail.PointId, _start, _end);
                                        if (_first == null) { ignore = true; break; }

                                        var _last = _hmeasureRepository.GetFirst(_detail.DeviceId, _detail.PointId, _end, _end.AddHours(_hours));
                                        if (_last == null) _last = _hmeasureRepository.GetLast(_detail.DeviceId, _detail.PointId, _start, _end);
                                        if (_last == null) { ignore = true; break; }

                                        _vvalue = Math.Round(_last.Value - _first.Value, 3, MidpointRounding.AwayFromZero);
                                    } else if (_formula.ComputeType == EnmCompute.Power) {
                                        _vvalue = _hmeasureRepository.GetAvg(_detail.DeviceId, _detail.PointId, _start, _end);
                                        if (_vvalue == double.MinValue) { ignore = true; break; }
                                    }

                                    _current = _current.Replace(_detail.Variable, _vvalue.ToString());
                                }

                                if (ignore) continue;

                                var _data = 0d;
                                var _value = _computer.Compute(_current, "");
                                if (_value != DBNull.Value) _data = Convert.ToDouble(_value);
                                if (double.IsNaN(_data) || double.IsInfinity(_data)) _data = 0d;
                                _data = Math.Round(_data * (_formula.ComputeType == EnmCompute.Kwh ? 1 : _hours), 3, MidpointRounding.AwayFromZero);

                                _eleResult.Add(new V_Elec {
                                    Id = _formula.Id,
                                    Type = _formula.Type,
                                    FormulaType = _formula.FormulaType,
                                    StartTime = _start,
                                    EndTime = _end,
                                    Value = _data
                                });
                            }
                            #endregion
                        }
                        #endregion
                    } else if (_formula.FormulaType == EnmFormula.FDL) {
                        #region 发电量
                        var _offs = _offlines.FindAll(o => o.Id.Equals(_formula.Id) && o.Type.Equals(_formula.Type));
                        if (_offs.Count == 0) continue;

                        if (_variables.Count == 0) {
                            #region 公式中没有变量
                            var _data = 0d;
                            var _value = _computer.Compute(_formulaValue, "");
                            if (_value != DBNull.Value) _data = Convert.ToDouble(_value);
                            if (double.IsNaN(_data) || double.IsInfinity(_data)) _data = 0d;
                            _data = Math.Round(_data * (_formula.ComputeType == EnmCompute.Kwh ? 1 : _hours), 3, MidpointRounding.AwayFromZero);

                            #region 油机发电量
                            foreach (var _off in _offs) {
                                var _start = _off.StartTime;
                                var _end = _off.EndTime;
                                if (_end.Subtract(_start).TotalMinutes <= 15) continue;
                                _off.Value = _data;
                                _offResult.Add(_off);
                            }
                            #endregion

                            #region 油机能耗
                            /*
                            foreach (var _time in _times) {
                                var _start = _time;
                                var _end = _start.AddHours(_hours);
                                if (_offs.Any(o => !(o.StartTime > _end || o.EndTime < _start))) {
                                    _eleResult.Add(new V_Elec {
                                        Id = _formula.Id,
                                        Type = _formula.Type,
                                        FormulaType = _formula.FormulaType,
                                        StartTime = _start,
                                        EndTime = _end,
                                        Value = _data
                                    });
                                }
                            }
                             * */
                            #endregion

                            #endregion
                        } else {
                            #region 公式中有变量
                            var ignore = false;
                            var _details = new List<VariableDetail>();
                            foreach (var _variable in _variables) {
                                var _factors = _variable.Split(new string[] { ">>" }, StringSplitOptions.None);
                                if (_factors.Length != 2) {
                                    ignore = true;
                                    break;
                                }

                                _details.Add(new VariableDetail {
                                    DeviceId = _factors[0].Substring(1),
                                    PointId = _factors[1],
                                    Variable = _variable
                                });
                            }

                            if (ignore) continue;

                            #region 油机发电量
                            foreach (var _off in _offs) {
                                var _start = _off.StartTime;
                                var _end = _off.EndTime;
                                if (_end.Subtract(_start).TotalMinutes <= 15) continue;

                                ignore = false;
                                var _current = _formulaValue;
                                foreach (var _detail in _details) {
                                    var _vvalue = 0d;
                                    if (_formula.ComputeType == EnmCompute.Kwh) {
                                        var _first = _hmeasureRepository.GetFirst(_detail.DeviceId, _detail.PointId, _start, _end);
                                        if (_first == null) { ignore = true; break; }

                                        var _last = _ameasureRepository.GetEntity(_detail.DeviceId, _detail.PointId);
                                        if (_last == null) { ignore = true; break; }

                                        _vvalue = Math.Round(_last.Value - _first.Value, 3, MidpointRounding.AwayFromZero);
                                    } else if (_formula.ComputeType == EnmCompute.Power) {
                                        _vvalue = _hmeasureRepository.GetAvg(_detail.DeviceId, _detail.PointId, _start, _end);
                                        if (_vvalue == double.MinValue) { ignore = true; break; }
                                    }

                                    _current = _current.Replace(_detail.Variable, _vvalue.ToString());
                                }

                                if (ignore) continue;

                                var _data = 0d;
                                var _value = _computer.Compute(_current, "");
                                if (_value != DBNull.Value) _data = Convert.ToDouble(_value);
                                if (double.IsNaN(_data) || double.IsInfinity(_data)) _data = 0d;
                                _off.Value = Math.Round(_data * (_formula.ComputeType == EnmCompute.Kwh ? 1 : _hours), 3, MidpointRounding.AwayFromZero);
                                _offResult.Add(_off);
                            }
                            #endregion

                            #region 油机能耗
                            /*
                            foreach (var _time in _times) {
                                var _total = 0d;
                                foreach (var _off in _offs) {
                                    var _start = _time;
                                    var _end = _start.AddHours(_hours);
                                    if (_off.StartTime > _end || _off.EndTime < _start)
                                        continue;

                                    if (_off.StartTime > _start) _start = _off.StartTime;
                                    if (_off.EndTime < _end) _end = _off.EndTime;
                                    
                                    ignore = false;
                                    var _current = _formulaValue;
                                    foreach (var _detail in _details) {
                                        var _vvalue = 0d;
                                        if (_formula.ComputeType == EnmCompute.Kwh) {
                                            var _first = _hmeasureRepository.GetFirst(_detail.DeviceId, _detail.PointId, _start, _end);
                                            if (_first == null) { ignore = true; break; }

                                            var _last = _hmeasureRepository.GetFirst(_detail.DeviceId, _detail.PointId, _end, _end.AddHours(_hours));
                                            if (_last == null) _last = _hmeasureRepository.GetLast(_detail.DeviceId, _detail.PointId, _start, _end);
                                            if (_last == null) { ignore = true; break; }

                                            _vvalue = Math.Round(_last.Value - _first.Value, 3, MidpointRounding.AwayFromZero);
                                        } else if (_formula.ComputeType == EnmCompute.Power) {
                                            _vvalue = _hmeasureRepository.GetAvg(_detail.DeviceId, _detail.PointId, _start, _end);
                                            if (_vvalue == double.MinValue) { ignore = true; break; }
                                        }

                                        _current = _current.Replace(_detail.Variable, _vvalue.ToString());
                                    }

                                    if (ignore) continue;

                                    var _data = 0d;
                                    var _value = _computer.Compute(_current, "");
                                    if (_value != DBNull.Value) _data = Convert.ToDouble(_value);
                                    if (double.IsNaN(_data) || double.IsInfinity(_data)) _data = 0d;
                                    _total += Math.Round(_data * (_formula.ComputeType == EnmCompute.Kwh ? 1 : _hours), 3, MidpointRounding.AwayFromZero);
                                }

                                _eleResult.Add(new V_Elec {
                                    Id = _formula.Id,
                                    Type = _formula.Type,
                                    FormulaType = _formula.FormulaType,
                                    StartTime = _time,
                                    EndTime = _time.AddHours(_hours),
                                    Value = _total
                                });
                            }
                             * */
                            #endregion

                            #endregion
                        }

                        #endregion
                    } else {
                        #region 能耗
                        if (_variables.Count == 0) {
                            #region 公式中没有变量
                            var _data = 0d;
                            var _value = _computer.Compute(_formulaValue, "");
                            if (_value != DBNull.Value) _data = Convert.ToDouble(_value);
                            if (double.IsNaN(_data) || double.IsInfinity(_data)) _data = 0d;
                            _data = Math.Round(_data * (_formula.ComputeType == EnmCompute.Kwh ? 1 : _hours), 3, MidpointRounding.AwayFromZero);

                            foreach (var _time in _times) {
                                _eleResult.Add(new V_Elec {
                                    Id = _formula.Id,
                                    Type = _formula.Type,
                                    FormulaType = _formula.FormulaType,
                                    StartTime = _time,
                                    EndTime = _time.AddHours(_hours),
                                    Value = _data
                                });
                            }
                            #endregion
                        } else {
                            #region 公式中有变量
                            var ignore = false;
                            var _details = new List<VariableDetail>();
                            foreach (var _variable in _variables) {
                                var _factors = _variable.Split(new string[] { ">>" }, StringSplitOptions.None);
                                if (_factors.Length != 2) {
                                    ignore = true;
                                    break;
                                }

                                _details.Add(new VariableDetail {
                                    DeviceId = _factors[0].Substring(1),
                                    PointId = _factors[1],
                                    Variable = _variable
                                });
                            }

                            if (ignore) continue;

                            foreach (var _time in _times) {
                                var _start = _time;
                                var _end = _start.AddHours(_hours);
                                var _current = _formulaValue;

                                ignore = false;
                                foreach (var _detail in _details) {
                                    var _vvalue = 0d;
                                    if (_formula.ComputeType == EnmCompute.Kwh) {
                                        var _first = _hmeasureRepository.GetFirst(_detail.DeviceId, _detail.PointId, _start, _end);
                                        if (_first == null) { ignore = true; break; }

                                        var _last = _hmeasureRepository.GetFirst(_detail.DeviceId, _detail.PointId, _end, _end.AddHours(_hours));
                                        if (_last == null) _last = _hmeasureRepository.GetLast(_detail.DeviceId, _detail.PointId, _start, _end);
                                        if (_last == null) { ignore = true; break; }

                                        _vvalue = Math.Round(_last.Value - _first.Value, 3, MidpointRounding.AwayFromZero);
                                    } else if (_formula.ComputeType == EnmCompute.Power) {
                                        _vvalue = _hmeasureRepository.GetAvg(_detail.DeviceId, _detail.PointId, _start, _end);
                                        if (_vvalue == double.MinValue) { ignore = true; break; }
                                    }

                                    _current = _current.Replace(_detail.Variable, _vvalue.ToString());
                                }

                                if (ignore) continue;

                                var _data = 0d;
                                var _value = _computer.Compute(_current, "");
                                if (_value != DBNull.Value) _data = Convert.ToDouble(_value);
                                if (double.IsNaN(_data) || double.IsInfinity(_data)) _data = 0d;
                                _data = Math.Round(_data * (_formula.ComputeType == EnmCompute.Kwh ? 1 : _hours), 3, MidpointRounding.AwayFromZero);

                                _eleResult.Add(new V_Elec {
                                    Id = _formula.Id,
                                    Type = _formula.Type,
                                    FormulaType = _formula.FormulaType,
                                    StartTime = _start,
                                    EndTime = _end,
                                    Value = _data
                                });
                            }
                            #endregion
                        }
                        #endregion
                    }
                } catch (Exception err) {
                    err.Source = JsonConvert.SerializeObject(_formula);

                    Logger.Warning("能耗处理发生错误，详见错误日志。");
                    Logger.Error("能耗处理发生错误", err);
                }
            }

            if (_eleResult.Count > 0) {
                #region PUE
                var its = _eleResult.FindAll(r => r.FormulaType == EnmFormula.IT && r.Value > 0);
                foreach (var it in its) {
                    var tt = _eleResult.Find(r => r.Id.Equals(it.Id) && r.Type == it.Type && r.FormulaType == EnmFormula.TT);
                    if (tt != null && tt.Value > 0) {
                        _eleResult.Add(new V_Elec {
                            Id = it.Id,
                            Type = it.Type,
                            FormulaType = EnmFormula.PUE,
                            StartTime = it.StartTime,
                            EndTime = it.EndTime,
                            Value = Math.Round(tt.Value / it.Value, 3, MidpointRounding.AwayFromZero)
                        });
                    }
                }
                #endregion

                _elecRepository.DeleteHistory(start, end);
                _elecRepository.SaveHistory(_eleResult);
            }

            if (_offResult.Count > 0) {
                _offlineRepository.UpdateHistory(_offResult);
            }
        }

        /// <summary>
        /// 电池曲线处理任务
        /// </summary>
        private void DoTask002() {
            _allDone.WaitOne();

            if (_runStatus != RunStatus.Running) return;
            var _curTask = iPemWorkContext.Tasks.Find(t => t.Id == "T002");
            if (_curTask == null) return;

            if (_curTask.Json == null) {
                Logger.Warning(string.Format("配置无效，{0}线程退出。", _curTask.Name));
                return;
            }

            if (_curTask.Json.EndDate < DateTime.Now) {
                Logger.Warning(string.Format("计划已过期，{0}线程退出。", _curTask.Name));
                return;
            }

            Logger.Information(string.Format("{0}线程已启动。", _curTask.Name));
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
                        //获取读资源锁
                        _configWriterLock.EnterReadLock();

                        this.ExTask002(_curTask.Start, _curTask.End);
                    } catch (Exception err) {
                        Logger.Warning(err.Message);
                        Logger.Error(err.Message, err);
                    } finally {
                        _configWriterLock.ExitReadLock();
                        iPemWorkContext.SetTaskPloy(_curTask);
                        Logger.Information(string.Format("{0}执行完成，下次执行时间：{1}。", _curTask.Name, CommonHelper.ToDateTimeString(_curTask.Next)));
                    }
                }
            }
        }

        /// <summary>
        /// 电池曲线处理方法
        /// </summary>
        private void ExTask002(DateTime start, DateTime end) {
            //默认一次电池充放电时长大于15分钟且小于3天
            var _minInterval = 15 * 60;
            var _maxInterval = 3 * 24 * 3600;
            var _curveCount = 240;

            //需要统计的电池信号
            var _zdyPoints = new List<string>();
            var _zdlPoints = new List<string>();
            var _dyPoints = new List<string>();
            var _wdPoints = new List<string>();
            
            var _zdyParam = iPemWorkContext.Params.Find(p => p.Id == ParamId.DCZDY);
            if (_zdyParam != null && !string.IsNullOrWhiteSpace(_zdyParam.Value)) {
                _zdyPoints.AddRange(CommonHelper.SplitCondition(_zdyParam.Value));
            }
            var _zdlParam = iPemWorkContext.Params.Find(p => p.Id == ParamId.DCZDL);
            if (_zdlParam != null && !string.IsNullOrWhiteSpace(_zdlParam.Value)) {
                _zdlPoints.AddRange(CommonHelper.SplitCondition(_zdlParam.Value));
            }
            var _dyParam = iPemWorkContext.Params.Find(p => p.Id == ParamId.DCDY);
            if (_dyParam != null && !string.IsNullOrWhiteSpace(_dyParam.Value)) {
                _dyPoints.AddRange(CommonHelper.SplitCondition(_dyParam.Value));
            }
            var _wdParam = iPemWorkContext.Params.Find(p => p.Id == ParamId.DCWD);
            if (_wdParam != null && !string.IsNullOrWhiteSpace(_wdParam.Value)) {
                _wdPoints.AddRange(CommonHelper.SplitCondition(_wdParam.Value));
            }
            
            try {
                var _batTimes = new List<V_BatTime>();
                var _batCurves = new List<V_BatCurve>();

                var _procedures = _batTimeRepository.GetProcedures(start, end);
                foreach (var _procedure in _procedures) {
                    var _zdyDetails = new List<V_Bat>();
                    var _zdlDetails = new List<V_Bat>();
                    var _dyDetails = new List<V_Bat>();
                    var _wdDetails = new List<V_Bat>();

                    #region 电池充放电过程处理
                    try {
                        var _details = _batRepository.GetProcDetails(_procedure.DeviceId, _procedure.PackId, _procedure.Type, _procedure.StartTime, _procedure.StartTime.AddSeconds(_maxInterval));
                        foreach (var _detail in _details) {
                            var pid = _detail.PointId.Substring(0,6);
                            if (_zdyPoints.Contains(pid)) {
                                _zdyDetails.Add(_detail);
                            } else if (_zdlPoints.Contains(pid)) {
                                _zdlDetails.Add(_detail);
                            } else if (_dyPoints.Contains(pid)) {
                                _dyDetails.Add(_detail);
                            } else if (_wdPoints.Contains(pid)) {
                                _wdDetails.Add(_detail);
                            }
                        }

                        if (_zdyDetails.Count > 0) {
                            var _last = _zdyDetails.Last();
                            _procedure.AreaId = _last.AreaId;
                            _procedure.StationId = _last.StationId;
                            _procedure.RoomId = _last.RoomId;
                            _procedure.EndTime = _last.ValueTime;
                        } else if (_zdlDetails.Count > 0) {
                            var _last = _zdlDetails.Last();
                            _procedure.AreaId = _last.AreaId;
                            _procedure.StationId = _last.StationId;
                            _procedure.RoomId = _last.RoomId;
                            _procedure.EndTime = _last.ValueTime;
                        } else if (_dyDetails.Count > 0) {
                            var _last = _dyDetails.Last();
                            _procedure.AreaId = _last.AreaId;
                            _procedure.StationId = _last.StationId;
                            _procedure.RoomId = _last.RoomId;
                            _procedure.EndTime = _last.ValueTime;
                        } else if (_wdDetails.Count > 0) {
                            var _last = _wdDetails.Last();
                            _procedure.AreaId = _last.AreaId;
                            _procedure.StationId = _last.StationId;
                            _procedure.RoomId = _last.RoomId;
                            _procedure.EndTime = _last.ValueTime;
                        }

                        if (_procedure.EndTime.Subtract(_procedure.StartTime).TotalSeconds < _minInterval) 
                            continue;

                        if (_procedure.Type == EnmBatType.Charge) {
                            var _discharge = _batTimeRepository.GetPreProcedure(_procedure.DeviceId, _procedure.PackId, _procedure.StartTime.AddSeconds(_maxInterval * -1), _procedure.StartTime);
                            if (_discharge != null && _discharge.Type == EnmBatType.Discharge) _procedure.ProcTime = _discharge.StartTime;
                        }

                        _batTimes.Add(_procedure);
                    } catch (Exception err) {
                        Logger.Warning("电池充放电过程处理发生错误，详见错误日志。");
                        Logger.Error("电池充放电过程处理发生错误", err);
                    }
                    #endregion

                    #region 电池充放电曲线处理
                    try {
                        //电池总电压曲线
                        _batCurves.AddRange(this.GetBatCurves(_zdyDetails, _curveCount, EnmBatPoint.DCZDY, _procedure.ProcTime));

                        //电池总电流曲线
                        _batCurves.AddRange(this.GetBatCurves(_zdlDetails, _curveCount, EnmBatPoint.DCZDL, _procedure.ProcTime));

                        //电池单体电压曲线
                        var _dyGroups = _dyDetails.GroupBy(d => d.PointId);
                        foreach (var _dg in _dyGroups) {
                            _batCurves.AddRange(this.GetBatCurves(_dg.AsEnumerable(), _curveCount, EnmBatPoint.DCDY, _procedure.ProcTime));
                        }

                        //电池单体温度曲线
                        var _wdGroups = _wdDetails.GroupBy(d => d.PointId);
                        foreach (var _wg in _wdGroups) {
                            _batCurves.AddRange(this.GetBatCurves(_wg.AsEnumerable(), _curveCount, EnmBatPoint.DCWD, _procedure.ProcTime));
                        }
                    } catch (Exception err) {
                        Logger.Warning("电池充放电曲线处理发生错误，详见错误日志。");
                        Logger.Error("电池充放电曲线处理发生错误", err);
                    }
                    #endregion
                }

                if (_batTimes.Count > 0) {
                    _batTimeRepository.DeleteEntities(start, end);
                    _batTimeRepository.SaveEntities(_batTimes);
                }

                if (_batCurves.Count > 0) {
                    _batCurveRepository.DeleteEntities(start, end);
                    _batCurveRepository.SaveEntities(_batCurves);
                }
            } catch (Exception err) {
                Logger.Warning("电池数据处理发生错误，详见错误日志。");
                Logger.Error("电池数据处理发生错误", err);
            }
        }

        /// <summary>
        /// 信号测值统计任务
        /// </summary>
        private void DoTask003() {
            _allDone.WaitOne();

            if (_runStatus != RunStatus.Running) return;
            var _curTask = iPemWorkContext.Tasks.Find(t => t.Id == "T003");
            if (_curTask == null) return;

            if (_curTask.Json == null) {
                Logger.Warning(string.Format("配置无效，{0}线程退出。", _curTask.Name));
                return;
            }

            if (_curTask.Json.EndDate < DateTime.Now) {
                Logger.Warning(string.Format("计划已过期，{0}线程退出。", _curTask.Name));
                return;
            }

            Logger.Information(string.Format("{0}线程已启动。", _curTask.Name));
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
                        //获取读资源锁
                        _configWriterLock.EnterReadLock();

                        this.ExTask003(_curTask.Start, _curTask.End);
                    } catch (Exception err) {
                        Logger.Warning(err.Message);
                        Logger.Error(err.Message, err);
                    } finally {
                        _configWriterLock.ExitReadLock();
                        iPemWorkContext.SetTaskPloy(_curTask);
                        Logger.Information(string.Format("{0}执行完成，下次执行时间：{1}。", _curTask.Name, CommonHelper.ToDateTimeString(_curTask.Next)));
                    }
                }
            }
        }

        /// <summary>
        /// 信号测值统计方法
        /// </summary>
        private void ExTask003(DateTime start, DateTime end) {
            foreach (var model in iPemWorkContext.Statics) {
                if (model.Interval <= 0) continue;

                var _values = _hmeasureRepository.GetEntities(model.DeviceId, model.PointId, start, end);
                if (_values.Count == 0) continue;

                var _result = new List<V_Static>();
                var _start = start;
                var _end = start.AddMinutes(model.Interval);
                while (end >= _end) {
                    try {
                        var __values = _values.FindAll(v => v.UpdateTime >= _start && v.UpdateTime <= _end);
                        if (__values.Count > 0) {
                            var _target = new V_Static {
                                AreaId = model.AreaId,
                                StationId = model.StationId,
                                RoomId = model.RoomId,
                                DeviceId = model.DeviceId,
                                PointId = model.PointId,
                                StartTime = _start,
                                EndTime = _end,
                                MaxValue = double.MinValue,
                                MinValue = double.MaxValue,
                                AvgValue = __values.Average(v => v.Value),
                                Total = __values.Count
                            };

                            foreach (var __value in __values) {
                                if (__value.Value > _target.MaxValue) {
                                    _target.MaxValue = __value.Value;
                                    _target.MaxTime = __value.UpdateTime;
                                }

                                if (__value.Value < _target.MinValue) {
                                    _target.MinValue = __value.Value;
                                    _target.MinTime = __value.UpdateTime;
                                }
                            }

                            _result.Add(_target);
                        }
                    } catch (Exception err) {
                        Logger.Warning("信号测值统计发生错误，详见错误日志。");
                        Logger.Error("信号测值统计发生错误", err);
                    } finally {
                        _start = _end;
                        _end = _end.AddMinutes(model.Interval);
                    }
                }

                _staticRepository.DeleteEntities(model.DeviceId, model.PointId, start, end);
                _staticRepository.SaveEntities(_result);
            }
        }

        /// <summary>
        /// 开关电源带载率统计任务
        /// </summary>
        private void DoTask004() {
            _allDone.WaitOne();

            if (_runStatus != RunStatus.Running) return;
            var _curTask = iPemWorkContext.Tasks.Find(t => t.Id == "T004");
            if (_curTask == null) return;

            if (_curTask.Json == null) {
                Logger.Warning(string.Format("配置无效，{0}线程退出。", _curTask.Name));
                return;
            }

            if (_curTask.Json.EndDate < DateTime.Now) {
                Logger.Warning(string.Format("计划已过期，{0}线程退出。", _curTask.Name));
                return;
            }

            Logger.Information(string.Format("{0}线程已启动。", _curTask.Name));
            if (_curTask.Json.StartDate > DateTime.Now) {
                Logger.Warning(string.Format("{0}未到执行日期，将从{1}开始执行。", _curTask.Name, CommonHelper.ToDateString(_curTask.Json.StartDate)));
            }

            #region 定义变量
            var _FzdlParam = iPemWorkContext.Params.Find(p => p.Id == ParamId.FZDL);
            if (_FzdlParam == null || string.IsNullOrWhiteSpace(_FzdlParam.Value)) {
                Logger.Warning(string.Format("{0}尚未配置负载电流信号，线程退出。", _curTask.Name));
                return;
            }

            if (!iPemWorkContext.Points.ContainsKey(_FzdlParam.Value)) {
                Logger.Warning(string.Format("{0}负载电流信号配置错误，线程退出。", _curTask.Name));
                return;
            }

            var _FzdlPoint = iPemWorkContext.Points[_FzdlParam.Value];

            var _GzztParam = iPemWorkContext.Params.Find(p => p.Id == ParamId.GZZT);
            if (_GzztParam == null || string.IsNullOrWhiteSpace(_GzztParam.Value)) {
                Logger.Warning(string.Format("{0}尚未配置工作状态信号，线程退出。", _curTask.Name));
                return;
            }

            if (!iPemWorkContext.Points.ContainsKey(_GzztParam.Value)) {
                Logger.Warning(string.Format("{0}工作状态信号配置错误，线程退出。", _curTask.Name));
                return;
            }

            var _GzztPoint = iPemWorkContext.Points[_GzztParam.Value];
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
                        //获取读资源锁
                        _configWriterLock.EnterReadLock();

                        this.ExTask004(_curTask.Start, _curTask.End, _FzdlPoint, _GzztPoint);
                    } catch (Exception err) {
                        Logger.Warning(err.Message);
                        Logger.Error(err.Message, err);
                    } finally {
                        _configWriterLock.ExitReadLock();
                        iPemWorkContext.SetTaskPloy(_curTask);
                        Logger.Information(string.Format("{0}执行完成，下次执行时间：{1}。", _curTask.Name, CommonHelper.ToDateTimeString(_curTask.Next)));
                    }
                }
            }
        }

        /// <summary>
        /// 开关电源带载率统计方法
        /// </summary>
        private void ExTask004(DateTime start, DateTime end, iPem.Core.Point fzdlPoint, iPem.Core.Point gzztPoint) {
            var _dates = CommonHelper.GetDateSpan(start, end);
            if (_dates.Count == 0) return;

            var _comDevices = _combSwitElecSourRepository.GetEntities();
            var _divDevices = _divSwitElecSourRepository.GetEntities();

            #region 组合开关电源
            foreach (var _device in _comDevices) {
                try {
                    if (!iPemWorkContext.DeviceSet.ContainsKey(_device.Id)) continue;
                    var _current = iPemWorkContext.DeviceSet[_device.Id];
                    var _ztPoint = _current.Signals.Find(p => p.Type == EnmPoint.DI && p.PointId == fzdlPoint.Id);
                    if (_ztPoint == null) continue;
                    var _fzPoint = _current.Signals.Find(p => p.Type == EnmPoint.AI && p.PointId == gzztPoint.Id);
                    if (_fzPoint == null) continue;
                    var _fzCap = _device.SingRModuleRatedOPCap * _device.ExisRModuleCount;
                    if (_fzCap == 0) continue;

                    var _result = new List<V_Load>();
                    var _ztFlag = CommonHelper.GetUnitState(_ztPoint.UnitState, "浮充");
                    var _ztValues = _hmeasureRepository.GetEntities(_device.Id, _ztPoint.PointId, start, end);
                    var _fzValues = _hmeasureRepository.GetEntities(_device.Id, _fzPoint.PointId, start, end);
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

                        DateTime? _start = null;
                        var __ztValues = _ztValues.FindAll(v => v.UpdateTime >= _load.StartTime && v.UpdateTime <= _load.EndTime);
                        var _intervals = new List<KV<DateTime, DateTime>>();
                        foreach (var __value in __ztValues) {
                            if (__value.Value == _ztFlag && !_start.HasValue) {
                                _start = __value.UpdateTime;
                            } else if (__value.Value != _ztFlag && _start.HasValue) {
                                _intervals.Add(new KV<DateTime, DateTime> {
                                    Key = _start.Value,
                                    Value = __value.UpdateTime
                                });

                                _start = null;
                            }
                        }

                        if (_start.HasValue) {
                            _intervals.Add(new KV<DateTime, DateTime> {
                                Key = _start.Value,
                                Value = _load.EndTime
                            });

                            _start = null;
                        }

                        if (_intervals.Count > 0) {
                            var __fzValues = _fzValues.FindAll(v => v.UpdateTime >= _load.StartTime && v.UpdateTime <= _load.EndTime);
                            foreach (var _interval in _intervals) {
                                var _fzMatch = __fzValues.FindAll(f => f.UpdateTime >= _interval.Key && f.UpdateTime <= _interval.Value);
                                var _fzMax = _fzMatch.Max(f => f.Value);
                                if (_fzMax > _load.Value) _load.Value = _fzMax;
                            }
                            _load.Value = _load.Value / _fzCap;
                        }

                        _result.Add(_load);
                    }

                    _loadRepository.DeleteEntities(_device.Id, start, end);
                    _loadRepository.SaveEntities(_result);
                } catch (Exception err) {
                    Logger.Warning("组合开关电源带载率统计发生错误，详见错误日志。");
                    Logger.Error("组合开关电源带载率统计发生错误", err);
                }
            }
            #endregion

            #region 分立开关电源
            foreach (var _device in _divDevices) {
                try {
                    if (!iPemWorkContext.DeviceSet.ContainsKey(_device.Id)) continue;
                    var _current = iPemWorkContext.DeviceSet[_device.Id];
                    var _ztPoint = _current.Signals.Find(p => p.Type == EnmPoint.DI && p.PointId == fzdlPoint.Id);
                    if (_ztPoint == null) continue;
                    var _fzPoint = _current.Signals.Find(p => p.Type == EnmPoint.AI && p.PointId == gzztPoint.Id);
                    if (_fzPoint == null) continue;
                    var _fzCap = _device.SingRModuleRatedOPCap * _device.ExisRModuleCount;
                    if (_fzCap == 0) continue;

                    var _result = new List<V_Load>();
                    var _ztFlag = CommonHelper.GetUnitState(_ztPoint.UnitState, "浮充");
                    var _ztValues = _hmeasureRepository.GetEntities(_device.Id, _ztPoint.PointId, start, end);
                    var _fzValues = _hmeasureRepository.GetEntities(_device.Id, _fzPoint.PointId, start, end);
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

                        DateTime? _start = null;
                        var __ztValues = _ztValues.FindAll(v => v.UpdateTime >= _load.StartTime && v.UpdateTime <= _load.EndTime);
                        var _intervals = new List<KV<DateTime, DateTime>>();
                        foreach (var __value in __ztValues) {
                            if (__value.Value == _ztFlag && !_start.HasValue) {
                                _start = __value.UpdateTime;
                            } else if (__value.Value != _ztFlag && _start.HasValue) {
                                _intervals.Add(new KV<DateTime, DateTime> {
                                    Key = _start.Value,
                                    Value = __value.UpdateTime
                                });

                                _start = null;
                            }
                        }

                        if (_start.HasValue) {
                            _intervals.Add(new KV<DateTime, DateTime> {
                                Key = _start.Value,
                                Value = _load.EndTime
                            });

                            _start = null;
                        }

                        if (_intervals.Count > 0) {
                            var __fzValues = _fzValues.FindAll(v => v.UpdateTime >= _load.StartTime && v.UpdateTime <= _load.EndTime);
                            foreach (var _interval in _intervals) {
                                var _fzMatch = __fzValues.FindAll(f => f.UpdateTime >= _interval.Key && f.UpdateTime <= _interval.Value);
                                var _fzMax = _fzMatch.Max(f => f.Value);
                                if (_fzMax > _load.Value) _load.Value = _fzMax;
                            }
                            _load.Value = _load.Value / _fzCap;
                        }

                        _result.Add(_load);
                    }

                    _loadRepository.DeleteEntities(_device.Id, start, end);
                    _loadRepository.SaveEntities(_result);
                } catch (Exception err) {
                    Logger.Warning("分立开关电源带载率统计发生错误，详见错误日志。");
                    Logger.Error("分立开关电源带载率统计发生错误", err);
                }
            }
            #endregion
        }

        /// <summary>
        /// 资管接口同步任务
        /// </summary>
        private void DoTask005() {
            _allDone.WaitOne();

            if (_runStatus != RunStatus.Running) return;
            var _curTask = iPemWorkContext.Tasks.Find(t => t.Id == "T005");
            if (_curTask == null) return;

            if (_curTask.Json == null) {
                Logger.Warning(string.Format("配置无效，{0}线程退出。", _curTask.Name));
                return;
            }

            if (_curTask.Json.EndDate < DateTime.Now) {
                Logger.Warning(string.Format("计划已过期，{0}线程退出。", _curTask.Name));
                return;
            }

            Logger.Information(string.Format("{0}线程已启动。", _curTask.Name));
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
                        //获取读资源锁
                        _configWriterLock.EnterReadLock();

                        this.ExTask005();
                    } catch (Exception err) {
                        Logger.Warning(err.Message);
                        Logger.Error(err.Message, err);
                    } finally {
                        _configWriterLock.ExitReadLock();
                        iPemWorkContext.SetTaskPloy(_curTask);
                        Logger.Information(string.Format("{0}执行完成，下次执行时间：{1}。", _curTask.Name, CommonHelper.ToDateTimeString(_curTask.Next)));
                    }
                }
            }
        }

        /// <summary>
        /// 资管接口同步方法
        /// </summary>
        private void ExTask005() {

            #region 处理接口设备数据
            try {
                var _iDevices = new List<H_IDevice>();
                var _devices = _deviceRepository.GetEntities();
                foreach (var _device in _devices) {
                    _iDevices.Add(new H_IDevice {
                        Id = _device.Id,
                        Name = _device.Name,
                        TypeId = _device.SubType.Id,
                        TypeName = _device.SubType.Name,
                        StationId = _device.StationId
                    });
                }

                _iDeviceRepository.SaveEntities(_iDevices, DateTime.Now);
            } catch (Exception err) {
                Logger.Warning("资管接口同步设备发生错误，详见错误日志。");
                Logger.Error("资管接口同步设备发生错误", err);
            }
            #endregion

            #region 处理接口站点数据
            try {
                var _iStations = new List<H_IStation>();
                var _stations = _stationRepository.GetEntities();
                foreach (var _station in _stations) {
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
                Logger.Warning("资管接口同步站点发生错误，详见错误日志。");
                Logger.Error("资管接口同步站点发生错误", err);
            }
            #endregion

            #region 处理接口区域数据
            try {
                var _iAreas = new List<H_IArea>();
                var _areas = _areaRepository.GetEntities();
                foreach (var _area in _areas) {
                    _iAreas.Add(new H_IArea {
                        Id = _area.Id,
                        Name = _area.Name,
                        TypeId = _area.Type.Key.ToString(),
                        TypeName = _area.Type.Value,
                        ParentId = _area.ParentId
                    });
                }

                _iAreaRepository.SaveEntities(_iAreas, DateTime.Now);
            } catch (Exception err) {
                Logger.Warning("资管接口同步区域发生错误，详见错误日志。");
                Logger.Error("资管接口同步区域发生错误", err);
            }
            #endregion

        }

        /// <summary>
        /// 参数自动巡检任务
        /// </summary>
        private void DoTask006() {
            _allDone.WaitOne();

            if (_runStatus != RunStatus.Running) return;
            var _curTask = iPemWorkContext.Tasks.Find(t => t.Id == "T006");
            if (_curTask == null) return;

            if (_curTask.Json == null) {
                Logger.Warning(string.Format("配置无效，{0}线程退出。", _curTask.Name));
                return;
            }

            if (_curTask.Json.EndDate < DateTime.Now) {
                Logger.Warning(string.Format("计划已过期，{0}线程退出。", _curTask.Name));
                return;
            }

            Logger.Information(string.Format("{0}线程已启动。", _curTask.Name));
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
                        //获取读资源锁
                        _configWriterLock.EnterReadLock();

                        this.ExTask006();
                    } catch (Exception err) {
                        Logger.Warning(err.Message);
                        Logger.Error(err.Message, err);
                    } finally {
                        _configWriterLock.ExitReadLock();
                        iPemWorkContext.SetTaskPloy(_curTask);
                        Logger.Information(string.Format("{0}执行完成，下次执行时间：{1}。", _curTask.Name, CommonHelper.ToDateTimeString(_curTask.Next)));
                    }
                }
            }
        }

        /// <summary>
        /// 参数自动巡检处理方法
        /// </summary>
        private void ExTask006() {
            var _diffs = new List<V_ParamDiff>();
            var _signals = _signalRepository.GetEntities();
            foreach (var _signal in _signals) {
                if (!iPemWorkContext.DeviceSet.ContainsKey(_signal.DeviceId)) continue;
                if (!iPemWorkContext.Points.ContainsKey(_signal.PointId)) continue;

                var _device = iPemWorkContext.DeviceSet[_signal.DeviceId];
                var _point = iPemWorkContext.Points[_signal.PointId];
                var _skey = CommonHelper.JoinKeys(_point.Id, _device.Current.StationType.Id);
                if (!iPemWorkContext.SubPoints.ContainsKey(_skey)) continue;
                var _subpoint = iPemWorkContext.SubPoints[_skey];

                var _diff = new V_ParamDiff { DeviceId = _device.Current.Id, PointId = _point.Id };
                if (string.IsNullOrWhiteSpace(_signal.NMAlarmId)) _signal.NMAlarmId = null;
                if (string.IsNullOrWhiteSpace(_point.NMAlarmId)) _point.NMAlarmId = null;
                if (string.IsNullOrWhiteSpace(_signal.StorageRefTime)) _signal.StorageRefTime = null;
                if (string.IsNullOrWhiteSpace(_subpoint.StorageRefTime)) _subpoint.StorageRefTime = null;

                var _ignore = true;
                if (_signal.AlarmLimit != _subpoint.AlarmLimit) {
                    _diff.Threshold = string.Format("{0}&{1}", CommonHelper.DoubleToString(_signal.AlarmLimit), CommonHelper.DoubleToString(_subpoint.AlarmLimit));
                    _ignore = false;
                }

                if (_signal.AlarmLevel != (int)_subpoint.AlarmLevel) {
                    _diff.AlarmLevel = string.Format("{0}&{1}", CommonHelper.Int32ToString(_signal.AlarmLevel), (int)_subpoint.AlarmLevel);
                    _ignore = false;
                }

                if (_signal.NMAlarmId != _point.NMAlarmId) {
                    _diff.NMAlarmID = string.Format("{0}&{1}", CommonHelper.StringToString(_signal.NMAlarmId), CommonHelper.StringToString(_point.NMAlarmId));
                    _ignore = false;
                }

                if (_signal.AbsoluteThreshold != _subpoint.AbsoluteThreshold) {
                    _diff.AbsoluteVal = string.Format("{0}&{1}", CommonHelper.DoubleToString(_signal.AbsoluteThreshold), CommonHelper.DoubleToString(_subpoint.AbsoluteThreshold));
                    _ignore = false;
                }

                if (_signal.PerThreshold != _subpoint.PerThreshold) {
                    _diff.RelativeVal = string.Format("{0}&{1}", CommonHelper.DoubleToString(_signal.PerThreshold), CommonHelper.DoubleToString(_subpoint.PerThreshold));
                    _ignore = false;
                }

                if (_signal.SavedPeriod != _subpoint.SavedPeriod) {
                    _diff.StorageInterval = string.Format("{0}&{1}", CommonHelper.Int32ToString(_signal.SavedPeriod), CommonHelper.Int32ToString(_subpoint.SavedPeriod));
                    _ignore = false;
                }

                if (_signal.StorageRefTime != _subpoint.StorageRefTime) {
                    _diff.StorageRefTime = string.Format("{0}&{1}", CommonHelper.StringToString(_signal.StorageRefTime), CommonHelper.StringToString(_subpoint.StorageRefTime));
                    _ignore = false;
                }

                _diff.Masked = iPemWorkContext.Maskings.Contains(CommonHelper.JoinKeys(_device.Current.Id, "masking-all"));
                if (!_diff.Masked) _diff.Masked = iPemWorkContext.Maskings.Contains(CommonHelper.JoinKeys(_device.Current.Id, _point.Id));
                if (_diff.Masked) {
                    _ignore = false;
                }

                if (_ignore) continue;
                _diffs.Add(_diff);
            }

            if (_diffs.Count > 0) {
                _paramDiffRepository.SaveEntities(_diffs, DateTime.Now);
            }
        }

        /// <summary>
        /// 告警同步任务
        /// </summary>
        private void DoTask007() {
            _allDone.WaitOne();

            if (_runStatus != RunStatus.Running) return;
            var _curTask = iPemWorkContext.Tasks.Find(t => t.Id == "T007");
            if (_curTask == null) return;

            if (_curTask.Json == null) {
                Logger.Warning(string.Format("配置无效，{0}线程退出。", _curTask.Name));
                return;
            }

            if (_curTask.Json.EndDate < DateTime.Now) {
                Logger.Warning(string.Format("计划已过期，{0}线程退出。", _curTask.Name));
                return;
            }

            Logger.Information(string.Format("{0}线程已启动。", _curTask.Name));
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
                        //获取写资源锁
                        _configWriterLock.EnterWriteLock();

                        this.ExTask007(_curTask.Start, _curTask.End);
                    } catch (Exception err) {
                        Logger.Warning(err.Message);
                        Logger.Error(err.Message, err);
                    } finally {
                        _configWriterLock.ExitWriteLock();
                        iPemWorkContext.SetTaskPloy(_curTask);
                        Logger.Information(string.Format("{0}执行完成，下次执行时间：{1}。", _curTask.Name, CommonHelper.ToDateTimeString(_curTask.Next)));
                    }
                }
            }
        }

        /// <summary>
        /// 告警同步处理方法
        /// </summary>
        private void ExTask007(DateTime start, DateTime end) {

            #region 同步告警变量
            LoadAlarms();
            #endregion

            var alarms = _falmRepository.GetEntities(start, end);
            if (alarms.Count > 0) {
                var hisKeys = new HashSet<string>(_halmRepository.GetEntities(start, end).Select(a => a.Id));

                #region 告警分类
                var starts = new List<FAlarmModel>();
                var ends = new List<FAlarmModel>();
                var finished = new List<KV<FAlarmModel, FAlarmModel>>();
                foreach (var alarm in alarms) {
                    var key = alarm.DeviceId;
                    if (!iPemWorkContext.DeviceSet.ContainsKey(key)) continue;

                    var device = iPemWorkContext.DeviceSet[key];
                    var signal = device.Signals.Find(p => p.PointId == alarm.PointId);
                    if (signal == null) continue;

                    var current = new FAlarmModel { Device = device.Current, Signal = signal, Alarm = alarm };

                    if (alarm.AlarmFlag == EnmFlag.Begin) {
                        starts.Add(current);
                    } else if (alarm.AlarmFlag == EnmFlag.End) {
                        var started = starts.Find(m => m.Device.Id == current.Device.Id && m.Signal.PointId == current.Signal.PointId);
                        if (started != null) {
                            finished.Add(new KV<FAlarmModel, FAlarmModel>(started, current));
                            starts.Remove(started);
                        } else {
                            ends.Add(current);
                        }
                    }
                }
                #endregion

                #region 仅有开始告警
                foreach (var alarm in starts) {
                    var key = CommonHelper.GetMD5(string.Format("{0}-{1}-{2}", alarm.Device.Id, alarm.Signal.PointId, alarm.Alarm.AlarmTime.Ticks));
                    if (iPemWorkContext.AlarmIds.Contains(key)) continue;
                    if (hisKeys.Contains(key)) continue;
                    _talmRepository.Save(alarm.Alarm);
                }
                #endregion

                #region 仅有结束告警

                foreach (var alarm in ends) {
                    var key = CommonHelper.JoinKeys(alarm.Device.Id, alarm.Signal.PointId);
                    if (!iPemWorkContext.Alarms.ContainsKey(key)) continue;
                    if (iPemWorkContext.Alarms[key].AlarmTime >= alarm.Alarm.AlarmTime) continue;
                    _talmRepository.Save(alarm.Alarm);
                }

                #endregion

                #region 既有开始又有结束

                foreach (var alarm in finished) {
                    var key = CommonHelper.GetMD5(string.Format("{0}-{1}-{2}", alarm.Key.Device.Id, alarm.Key.Signal.PointId, alarm.Key.Alarm.AlarmTime.Ticks));
                    if (hisKeys.Contains(key)) continue;
                    if (iPemWorkContext.AlarmIds.Contains(key)) {
                        _talmRepository.Save(alarm.Value.Alarm);
                    } else {
                        _talmRepository.Save(alarm.Key.Alarm, alarm.Value.Alarm);
                    }
                }

                #endregion

                _falmRepository.Delete(alarms);
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
            //加载区域
            iPemWorkContext.Areas = new List<WcArea>();
            var _areas = _areaRepository.GetEntities();
            foreach (var _area in _areas) {
                iPemWorkContext.Areas.Add(new WcArea(_area));
            }
            foreach (var current in iPemWorkContext.Areas) {
                current.Initializer(iPemWorkContext.Areas);
            }

            //加载站点
            iPemWorkContext.Stations = _stationRepository.GetEntities();

            //加载机房
            iPemWorkContext.Rooms = _roomRepository.GetEntities();

            //加载FSU
            iPemWorkContext.ExtFsus = _fsuRepository.GetExtEntities();

            //加载设备
            iPemWorkContext.Devices = new List<WcDevice>();
            iPemWorkContext.DeviceSet = new Dictionary<string, WcDevice>();

            var _signals = _signalRepository.GetEntities();
            var _devices = _deviceRepository.GetEntities();
            var _groups = _signals.GroupBy(s => s.DeviceId);
            foreach (var _device in _devices) {
                var _current = new WcDevice(_device);

                var _group = _groups.FirstOrDefault(g => g.Key.Equals(_device.Id));
                _current.Signals = _group != null ? _group.ToList() : new List<Signal>();

                iPemWorkContext.Devices.Add(_current);
                iPemWorkContext.DeviceSet[_device.Id] = _current;
            }

            //加载信号
            iPemWorkContext.Points = _pointRepository.GetEntities().ToDictionary(k => k.Id, v => v);

            //加载信号子表
            iPemWorkContext.SubPoints = _subPointRepository.GetEntities().ToDictionary(k => CommonHelper.JoinKeys(k.PointId, k.StationTypeId), v => v);

            //加载虚拟信号
            iPemWorkContext.VSignals = _signalRepository.GetVEntities();
        }

        /// <summary>
        /// 加载告警屏蔽数据
        /// </summary>
        private void LoadMaskings() {
            iPemWorkContext.Maskings = new HashSet<string>();
            var _masks = _maskingRepository.GetEntities();
            foreach (var _mask in _masks) {
                if (_mask.Type == EnmMaskType.Station) {
                    var _devices = iPemWorkContext.Devices.FindAll(d => d.Current.StationId.Equals(_mask.Id));
                    foreach (var _device in _devices) {
                        iPemWorkContext.Maskings.Add(CommonHelper.JoinKeys(_device.Current.Id, "masking-all"));
                    }
                } else if (_mask.Type == EnmMaskType.Room) {
                    var _devices = iPemWorkContext.Devices.FindAll(d => d.Current.RoomId.Equals(_mask.Id));
                    foreach (var _device in _devices) {
                        iPemWorkContext.Maskings.Add(CommonHelper.JoinKeys(_device.Current.Id, "masking-all"));
                    }
                } else if (_mask.Type == EnmMaskType.Device) {
                    iPemWorkContext.Maskings.Add(CommonHelper.JoinKeys(_mask.Id, "masking-all"));
                } else if (_mask.Type == EnmMaskType.Point) {
                    var _keys = CommonHelper.SplitCondition(_mask.Id);
                    if (_keys.Length == 2 && !iPemWorkContext.Maskings.Contains(CommonHelper.JoinKeys(_keys[0], "masking-all")))
                        iPemWorkContext.Maskings.Add(CommonHelper.JoinKeys(_keys[0], _keys[1]));
                }
            }
        }

        /// <summary>
        /// 加载Sc采集组数据
        /// </summary>
        private void LoadScHeartbeats() {
            iPemWorkContext.ScHeartbeats = new List<ScHeartbeat>();

            //初始化SC心跳包
            var _scs = _groupRepository.GetEntities();
            foreach (var _sc in _scs) {
                iPemWorkContext.ScHeartbeats.Add(new ScHeartbeat(_sc));
            }

            //初始化SC告警配置文件
            _scXmlDoc = CommonHelper.GetXmlDocument(SC_ALARM_CFG_DIR, SC_ALARM_CFG_FILE);
            if (_scXmlDoc.DocumentElement.ChildNodes.Count > 0) {
                var _scKeys = _scs.Select(g => g.Id).ToArray();
                for (int i = _scXmlDoc.DocumentElement.ChildNodes.Count - 1; i >= 0; i--) {
                    var node = _scXmlDoc.DocumentElement.ChildNodes[i];
                    var id = node.Attributes["id"].Value;
                    if (!_scKeys.Contains(id)) {
                        _aalmRepository.Delete(new KV<string, string>("-1", id));
                        node.ParentNode.RemoveChild(node);
                    }
                }
            }
            CommonHelper.SaveXmlDocument(SC_ALARM_CFG_DIR, SC_ALARM_CFG_FILE, _scXmlDoc);
            
            //初始化FSU告警配置文件
            var _fsus = _fsuRepository.GetExtEntities();
            _fsuXmlDoc = CommonHelper.GetXmlDocument(FSU_ALARM_CFG_DIR, FSU_ALARM_CFG_FILE);
            if (_fsuXmlDoc.DocumentElement.ChildNodes.Count > 0) {
                var _fsuKeys = _fsus.Select(f => f.Id).ToArray();
                for (int i = _fsuXmlDoc.DocumentElement.ChildNodes.Count - 1; i >= 0; i--) {
                    var node = _fsuXmlDoc.DocumentElement.ChildNodes[i];
                    var id = node.Attributes["id"].Value;
                    if (!_fsuKeys.Contains(id)) {
                        _aalmRepository.Delete(new KV<string, string>("-2", id));
                        node.ParentNode.RemoveChild(node);
                    }
                }
            }
            CommonHelper.SaveXmlDocument(FSU_ALARM_CFG_DIR, FSU_ALARM_CFG_FILE, _fsuXmlDoc);
        }

        /// <summary>
        /// 加载工程预约数据
        /// <para>忽略一个月之前的工程预约</para>
        /// </summary>
        private void LoadReservations() {
            iPemWorkContext.Reservations = new List<ReservationModel>();

            var _reservations = _reservationRepository.GetEntities(DateTime.Today.AddMonths(-1)).FindAll(r => r.Status == EnmResult.Success);
            var _resnodes = _nodesInReservationRepository.GetEntities(_reservations.Select(r => r.Id).ToArray());
            foreach (var _reservation in _reservations) {
                var _nodes = _resnodes.FindAll(n => n.ReservationId.Equals(_reservation.Id));
                if (_nodes.Count == 0) continue;

                var _resdevices = new HashSet<string>();
                foreach (var _node in _nodes) {
                    if (_node.NodeType == EnmSSH.Area) {
                        var _current = iPemWorkContext.Areas.Find(a => a.Current.Id.Equals(_node.NodeId));
                        if (_current == null) continue;
                        var _devices = iPemWorkContext.Devices.FindAll(d => _current.Keys.Contains(d.Current.AreaId));
                        foreach (var _device in _devices) {
                            _resdevices.Add(_device.Current.Id);
                        }
                    } else if (_node.NodeType == EnmSSH.Station) {
                        var _devices = iPemWorkContext.Devices.FindAll(d => d.Current.StationId.Equals(_node.NodeId));
                        foreach (var _device in _devices) {
                            _resdevices.Add(_device.Current.Id);
                        }
                    } else if (_node.NodeType == EnmSSH.Room) {
                        var _devices = iPemWorkContext.Devices.FindAll(d => d.Current.RoomId.Equals(_node.NodeId));
                        foreach (var _device in _devices) {
                            _resdevices.Add(_device.Current.Id);
                        }
                    } else if (_node.NodeType == EnmSSH.Device) {
                        _resdevices.Add(_node.NodeId);
                    }
                }

                iPemWorkContext.Reservations.Add(new ReservationModel { Reservation = _reservation, Devices = _resdevices });
            }
        }

        /// <summary>
        /// 加载告警数据
        /// </summary>
        private void LoadAlarms(bool clean = false) {
            iPemWorkContext.InitAlarm();
            var _alarms = _aalmRepository.GetEntities();
            foreach (var _alarm in _alarms) {
                iPemWorkContext.AddAlarm(_alarm);
            }

            if (!clean) return;

            #region 清理流水告警
            try {
                var _tcleans = new List<A_TAlarm>();
                var _talarms = _talmRepository.GetEntities();
                foreach (var _alarm in _talarms) {
                    if (_alarm.FsuId.Equals("-1") || _alarm.FsuId.Equals("-2"))
                        continue;

                    if (!iPemWorkContext.DeviceSet.ContainsKey(_alarm.DeviceId)) {
                        _tcleans.Add(_alarm);
                    }
                }

                if (_tcleans.Count > 0) 
                    _talmRepository.Delete(_tcleans.ToArray());
            } catch (Exception err) {
                Logger.Warning("清理流水告警错误，详见错误日志。");
                Logger.Error("清理流水告警错误", err);
            }
            #endregion

            #region 清理同步告警
            try {
                var _fcleans = new List<A_FAlarm>();
                var _falarms = _falmRepository.GetEntities();
                foreach (var _alarm in _falarms) {
                    if (!iPemWorkContext.DeviceSet.ContainsKey(_alarm.DeviceId)) {
                        _fcleans.Add(_alarm);
                    }
                }

                if (_fcleans.Count > 0) 
                    _falmRepository.Delete(_fcleans);
            } catch (Exception err) {
                Logger.Warning("清理同步告警错误，详见错误日志。");
                Logger.Error("清理同步告警错误", err);
            }
            #endregion
        }

        /// <summary>
        /// 加载计划任务数据
        /// </summary>
        /// <param name="force">是否重新加载任务数据</param>
        private void LoadTasks(bool force) {
            if (force) {
                _registry.UpdateTasks(iPemWorkContext.Tasks);
                iPemWorkContext.Tasks = _registry.GetTasks();
            }

            foreach (var task in iPemWorkContext.Tasks) {
                if (DateTime.Today.Subtract(task.Start).TotalDays > 30) task.Start = DateTime.Today.AddDays(-1);
                if (DateTime.Today.Subtract(task.End).TotalDays > 30) task.End = DateTime.Today.AddSeconds(-1);
                if (DateTime.Today.Subtract(task.Next).TotalDays > 30) task.Next = DateTime.Now;
            }
        }

        /// <summary>
        /// 加载服务参数数据
        /// </summary>
        private void LoadParams() {
            iPemWorkContext.Params = _registry.GetParams();
        }

        /// <summary>
        /// 加载测值统计数据
        /// </summary>
        private void LoadStatics() {
            iPemWorkContext.Statics = new List<StaticModel>();
            foreach (var _device in iPemWorkContext.Devices) {
                foreach (var _signal in _device.Signals) {
                    if (_signal.Type == EnmPoint.AI && _signal.StaticPeriod > 0) {
                        iPemWorkContext.Statics.Add(new StaticModel {
                            AreaId = _device.Current.AreaId,
                            StationId = _device.Current.StationId,
                            RoomId = _device.Current.RoomId,
                            FsuId = _device.Current.FsuId,
                            DeviceId = _device.Current.Id,
                            PointId = _signal.PointId,
                            Interval = _signal.StaticPeriod
                        });
                    }
                }
            }

            foreach (var _signal in iPemWorkContext.VSignals) {
                if (_signal.Type == EnmPoint.AI && _signal.StaticPeriod > 0) {
                    iPemWorkContext.Statics.Add(new StaticModel {
                        AreaId = _signal.AreaId,
                        StationId = _signal.StationId,
                        RoomId = _signal.RoomId,
                        FsuId = _signal.FsuId,
                        DeviceId = _signal.DeviceId,
                        PointId = _signal.PointId,
                        Interval = _signal.StaticPeriod
                    });
                }
            }
        }

        /// <summary>
        /// 加载电池统计数据
        /// </summary>
        private void LoadBatteries() {
            iPemWorkContext.Batteries = new List<BatModel>();

            #region 屏蔽电池数据处理
            /*
            var pattern = @"\{电池放电:\d+&\d+(\.\d+)?\}";
            var models = new List<BatParam>();
            foreach (var _device in iPemWorkContext.Devices) {
                foreach (var _signal in _device.Signals) {
                    if (string.IsNullOrWhiteSpace(_signal.Extend)) continue;
                    foreach (Match match in Regex.Matches(_signal.Extend, pattern)) {
                        var _tactics = match.Value.Replace("{电池放电:", "").Replace("}", "").Split('&');
                        if (_tactics.Length != 2) continue;

                        int _pack;
                        if (!int.TryParse(_tactics[0], out _pack)) continue;

                        double _voltage;
                        if (!double.TryParse(_tactics[1], out _voltage)) continue;

                        models.Add(new BatParam { DeviceId = _signal.DeviceId, PointId = _signal.PointId, PackId = _pack, Voltage = _voltage });
                    }
                }
            }

            if (models.Count > 0) {
                var _packGroup = models.GroupBy(m => new { m.DeviceId, m.PackId });
                foreach (var _pack in _packGroup) {
                    var _master = _pack.FirstOrDefault(g => g.Voltage > 0);
                    if (_master == null) continue;

                    if (!iPemWorkContext.DeviceSet.ContainsKey(_pack.Key.DeviceId))
                        continue;

                    var _device = iPemWorkContext.DeviceSet[_pack.Key.DeviceId];
                    var _signal = _device.Signals.Find(p => p.PointId == _master.PointId);
                    if (_signal == null) continue;

                    var _subIds = _pack.Where(p => p != _master).Select(g => g.PointId).ToArray();
                    var _subPoints = _device.Signals.FindAll(p => _subIds.Contains(p.PointId));
                    iPemWorkContext.Batteries.Add(new BatModel {
                        AreaId = _device.Current.AreaId,
                        StationId = _device.Current.StationId,
                        RoomId = _device.Current.RoomId,
                        FsuId = _device.Current.FsuId,
                        DeviceId = _device.Current.Id,
                        PointId = _signal.PointId,
                        PackId = _master.PackId,
                        Voltage = _master.Voltage,
                        SubSignals = _subPoints.Select(g => g.PointId).ToList()
                    });
                }
            }
            */
            #endregion
        }

        /// <summary>
        /// 加载能耗公式数据
        /// </summary>
        private void LoadFormulas() {
            iPemWorkContext.Offlines = new Dictionary<string, List<KV<string, EnmSSH>>>();
            iPemWorkContext.Cuttings = new Dictionary<string, List<KV<string, EnmSSH>>>();
            iPemWorkContext.Powers = new Dictionary<string, List<KV<string, EnmSSH>>>();
            iPemWorkContext.Formulas = new List<Formula>();

            var _formulas = _formulaRepository.GetEntities();
            foreach (var _formula in _formulas) {
                if (_formula.FormulaType == EnmFormula.TD) {
                    var _formulaValue = _formula.FormulaValue;
                    if (string.IsNullOrWhiteSpace(_formulaValue)) continue;
                    if (!CommonHelper.ValidateFormula(_formulaValue)) continue;

                    var _variables = CommonHelper.GetFormulaVariables(_formulaValue);
                    foreach (var _variable in _variables) {
                        var _factors = _variable.Split(new string[] { ">>" }, StringSplitOptions.None);
                        if (_factors.Length != 2) continue;

                        var _device = _factors[0].Substring(1);
                        var _point = _factors[1];
                        var _key = CommonHelper.JoinKeys(_device, _point);
                        if (iPemWorkContext.Cuttings.ContainsKey(_key)) {
                            iPemWorkContext.Cuttings[_key].Add(new KV<string, EnmSSH>(_formula.Id, _formula.Type));
                        } else {
                            iPemWorkContext.Cuttings[_key] = new List<KV<string, EnmSSH>> { new KV<string, EnmSSH>(_formula.Id, _formula.Type) };
                        }
                        break;
                    }
                } else if (_formula.FormulaType == EnmFormula.FD) {
                    var _formulaValue = _formula.FormulaValue;
                    if (string.IsNullOrWhiteSpace(_formulaValue)) continue;
                    if (!CommonHelper.ValidateFormula(_formulaValue)) continue;

                    var _variables = CommonHelper.GetFormulaVariables(_formulaValue);
                    foreach (var _variable in _variables) {
                        var _factors = _variable.Split(new string[] { ">>" }, StringSplitOptions.None);
                        if (_factors.Length != 2) continue;

                        var _device = _factors[0].Substring(1);
                        var _point = _factors[1];
                        var _key = CommonHelper.JoinKeys(_device, _point);
                        if (iPemWorkContext.Powers.ContainsKey(_key)) {
                            iPemWorkContext.Powers[_key].Add(new KV<string, EnmSSH>(_formula.Id, _formula.Type));
                        } else {
                            iPemWorkContext.Powers[_key] = new List<KV<string, EnmSSH>> { new KV<string, EnmSSH>(_formula.Id, _formula.Type) };
                        }
                        break;
                    }
                } else {
                    iPemWorkContext.Formulas.Add(_formula);
                }
            }
        }

        /// <summary>
        /// 获得实时能耗统计周期
        /// </summary>
        private DateTime GetPeriod(PeriodType period) {
            switch (period) {
                case PeriodType.Hour:
                    return DateTime.Today.AddHours(DateTime.Now.Hour);
                case PeriodType.Day:
                    return DateTime.Today;
                case PeriodType.Month:
                    return new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                default:
                    return DateTime.Today;
            }
        }

        /// <summary>
        /// 获得指定数量的电池曲线
        /// </summary>
        private List<V_BatCurve> GetBatCurves(IEnumerable<V_Bat> _values, int _count, EnmBatPoint _ptype, DateTime _proctime) {
            var _result = new List<V_BatCurve>();
            var _length = _values.Count();
            if (_length <= _count) {
                foreach (var _detail in _values) {
                    _result.Add(new V_BatCurve {
                        AreaId = _detail.AreaId,
                        StationId = _detail.StationId,
                        RoomId = _detail.RoomId,
                        DeviceId = _detail.DeviceId,
                        PointId = _detail.PointId,
                        PackId = _detail.PackId,
                        Type = _detail.Type,
                        PType = _ptype,
                        StartTime = _detail.StartTime,
                        Value = _detail.Value,
                        ValueTime = _detail.ValueTime,
                        ProcTime = _proctime
                    });
                }
            } else {
                var _ticks = _length / (double)_count;
                var _target = _values.First();
                for (var i = 0; i < _count; i++) {
                    var __start = (int)Math.Floor(i * _ticks);
                    var __end = (int)Math.Floor((i + 1) * _ticks);
                    var __count = __end - __start;
                    if (__count > 0) {
                        var __values = _values.Skip(__start).Take(__count);
                        if (__values.Any()) _target = __values.Last();
                    }

                    _result.Add(new V_BatCurve {
                        AreaId = _target.AreaId,
                        StationId = _target.StationId,
                        RoomId = _target.RoomId,
                        DeviceId = _target.DeviceId,
                        PointId = _target.PointId,
                        PackId = _target.PackId,
                        Type = _target.Type,
                        PType = _ptype,
                        StartTime = _target.StartTime,
                        Value = _target.Value,
                        ValueTime = _target.ValueTime,
                        ProcTime = _proctime
                    });
                }
            }

            return _result;
        }
    }
}