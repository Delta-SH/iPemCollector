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
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace iPem.TaskServer {
    public partial class Simulator : Form {

        #region 全局变量
        private RunStatus _runStatus;
        private Registry _registry;
        private Thread _workerThread;
        private List<Thread> _workerThreads;
        private EventWaitHandle _allDone;
        private DataTable _computer;

        //静态配置读写锁
        static ReaderWriterLockSlim _configWriterLock = new ReaderWriterLockSlim();
        #endregion

        #region 资源数据类
        private LogicTypeRepository _logicTypeRepository;
        private DeviceTypeRepository _deviceTypeRepository;
        private RoomTypeRepository _roomTypeRepository;
        private StationTypeRepository _stationTypeRepository;
        private EnumMethodsRepository _enumMethodsRepository;
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
        private V_AMeasureRepository _ameasureRepository;
        private V_HMeasureRepository _hmeasureRepository;
        private V_BatRepository _batRepository;
        private V_BatTimeRepository _batTimeRepository;
        private V_BatCurveRepository _batCurveRepository;
        private V_ElecRepository _elecRepository;
        private V_StaticRepository _staticRepository;
        private V_LoadRepository _loadRepository;
        private V_CuttingRepository _cuttingRepository;
        private V_CutedRepository _cutedRepository;
        private V_ParamDiffRepository _paramDiffRepository;
        private Alarmer _alarmer;
        private Indexer _indexer;
        #endregion

        #region 应用数据类
        private ReservationRepository _reservationRepository;
        private NodesInReservationRepository _nodesInReservationRepository;
        private FormulaRepository _formulaRepository;
        #endregion

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
                _ameasureRepository = new V_AMeasureRepository();
                _hmeasureRepository = new V_HMeasureRepository();
                _batRepository = new V_BatRepository();
                _batTimeRepository = new V_BatTimeRepository();
                _batCurveRepository = new V_BatCurveRepository();
                _elecRepository = new V_ElecRepository();
                _staticRepository = new V_StaticRepository();
                _loadRepository = new V_LoadRepository();
                _cuttingRepository = new V_CuttingRepository();
                _cutedRepository = new V_CutedRepository();
                _paramDiffRepository = new V_ParamDiffRepository();
                _alarmer = new Alarmer();
                _indexer = new Indexer();

                //初始化应用数据类
                _reservationRepository = new ReservationRepository();
                _nodesInReservationRepository = new NodesInReservationRepository();
                _formulaRepository = new FormulaRepository();

                //清空同步配置命令表
                _noticeRepository.Clear();

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

                //创建系统自检线程
                _workerThread = new Thread(new ThreadStart(DoChecking));
                _workerThread.IsBackground = true;
                _workerThread.Start();
                _workerThreads.Add(_workerThread);

                //创建SC心跳线程
                _workerThread = new Thread(new ThreadStart(DoScHeartbeat));
                _workerThread.IsBackground = true;
                _workerThread.Start();
                _workerThreads.Add(_workerThread);

                //创建实时能耗线程
                _workerThread = new Thread(new ThreadStart(DoFormula));
                _workerThread.IsBackground = true;
                _workerThread.Start();
                _workerThreads.Add(_workerThread);

                //创建电池数据处理线程
                _workerThread = new Thread(new ThreadStart(DoBat));
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
        private void stopButton_Click(object sender, EventArgs e) {
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
                Logger.Warning("停止服务失败，详见错误日志。");
                Logger.Error("停止服务失败", exc);
            }
        }

        /// <summary>
        /// 服务初始化任务
        /// </summary>
        private void DoInit() {
            var maxRepeat = 3;
            var interval = 86400;
            while (_runStatus < RunStatus.Running) {
                Thread.Sleep(1000);
                try {
                    if (_runStatus == RunStatus.Init) {
                        if (interval++ < 30) continue; interval = 1;
                        maxRepeat--;
                        Logger.Information("数据初始化...");

                        LoadBase();
                        LoadMaskings();
                        LoadScHeartbeats();
                        LoadReservations();
                        LoadTasks(false);
                        LoadFormulas();
                        LoadStaticPolicies();
                        LoadBatPolicies();
                        LoadCutPolicies();
                        LoadAlarms(true);

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
        /// 5秒检查一次配置表
        /// </summary>
        private void DoConfig() {
            _allDone.WaitOne();

            if (_runStatus != RunStatus.Running) return;
            Logger.Information("命令监听线程已启动。");
            var interval = 86400;
            while (_runStatus != RunStatus.Stop) {
                Thread.Sleep(1000);
                if (_runStatus == RunStatus.Running) {
                    if (interval++ <= 10) continue; interval = 1;

                    try {

                        #region 执行命令
                        var orders = _registry.GetOrders();
                        var notices = _noticeRepository.GetEntities();
                        if (notices.Count > 0) {
                            if (!orders.Any(o => o.Id == OrderId.Restart || o.Id == OrderId.Reload || o.Id == OrderId.SyncConfig)) {
                                if (notices.Any(n => string.IsNullOrWhiteSpace(n.Name))) {
                                    orders.Add(new OrderEntity { Id = OrderId.SyncConfig, Time = DateTime.Now });
                                } else {
                                    if (notices.Any(n => n.Name.Contains("M_Reservations"))) {
                                        orders.Add(new OrderEntity { Id = OrderId.SyncReservation, Time = DateTime.Now });
                                    }

                                    if (notices.Any(n => n.Name.Contains("M_Formulas"))) {
                                        orders.Add(new OrderEntity { Id = OrderId.SyncFormula, Time = DateTime.Now });
                                    }
                                    
                                    if (notices.Any(n => n.Name.Contains("H_Masking"))) {
                                        orders.Add(new OrderEntity { Id = OrderId.SyncMasking, Time = DateTime.Now });
                                    }

                                    if (notices.Any(n => n.Name.Contains("D_FSU"))) {
                                        orders.Add(new OrderEntity { Id = OrderId.SyncFsu, Time = DateTime.Now });
                                    }
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
                                    LoadMaskings();
                                } catch (Exception err) {
                                    Logger.Warning("同步告警屏蔽数据错误，详见错误日志。");
                                    Logger.Error("同步告警屏蔽数据错误", err);
                                }

                                try {
                                    LoadScHeartbeats();
                                } catch (Exception err) {
                                    Logger.Warning("同步SC采集组数据错误，详见错误日志。");
                                    Logger.Error("同步SC采集组数据错误", err);
                                }

                                try {
                                    LoadReservations();
                                } catch (Exception err) {
                                    Logger.Warning("同步工程预约数据错误，详见错误日志。");
                                    Logger.Error("同步工程预约数据错误", err);
                                }

                                try {
                                    LoadTasks(true);
                                } catch (Exception err) {
                                    Logger.Warning("同步计划任务数据错误，详见错误日志。");
                                    Logger.Error("同步计划任务数据错误", err);
                                }

                                try {
                                    LoadParams();
                                } catch (Exception err) {
                                    Logger.Warning("同步服务配置数据错误，详见错误日志。");
                                    Logger.Error("同步服务配置数据错误", err);
                                }

                                try {
                                    LoadFormulas();
                                } catch (Exception err) {
                                    Logger.Warning("同步能耗公式数据错误，详见错误日志。");
                                    Logger.Error("同步能耗公式数据错误", err);
                                }

                                try {
                                    LoadStaticPolicies();
                                } catch (Exception err) {
                                    Logger.Warning("同步信号测值统计策略错误，详见错误日志。");
                                    Logger.Error("同步信号测值统计策略错误", err);
                                }

                                try {
                                    LoadBatPolicies();
                                } catch (Exception err) {
                                    Logger.Warning("同步电池数据处理策略错误，详见错误日志。");
                                    Logger.Error("同步电池数据处理策略错误", err);
                                }

                                try {
                                    LoadCutPolicies();
                                } catch (Exception err) {
                                    Logger.Warning("同步断站、停电、发电策略错误，详见错误日志。");
                                    Logger.Error("同步断站、停电、发电策略错误", err);
                                }

                                try {
                                    LoadAlarms();
                                } catch (Exception err) {
                                    Logger.Warning("同步活动告警数据错误，详见错误日志。");
                                    Logger.Error("同步活动告警数据错误", err);
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
                                #region 同步配置数据
                                try {
                                    //获取写资源锁
                                    _configWriterLock.EnterWriteLock();
                                    Logger.Information("同步配置...");

                                    try {
                                        LoadBase();
                                    } catch (Exception err) {
                                        Logger.Warning("同步基本配置数据错误，详见错误日志。");
                                        Logger.Error("同步基本配置数据错误", err);
                                    }

                                    try {
                                        LoadMaskings();
                                    } catch (Exception err) {
                                        Logger.Warning("同步告警屏蔽数据错误，详见错误日志。");
                                        Logger.Error("同步告警屏蔽数据错误", err);
                                    }

                                    try {
                                        LoadScHeartbeats();
                                    } catch (Exception err) {
                                        Logger.Warning("同步SC采集组数据错误，详见错误日志。");
                                        Logger.Error("采集组数据错误", err);
                                    }

                                    try {
                                        LoadReservations();
                                    } catch (Exception err) {
                                        Logger.Warning("同步工程预约数据错误，详见错误日志。");
                                        Logger.Error("同步工程预约数据错误", err);
                                    }

                                    try {
                                        LoadFormulas();
                                    } catch (Exception err) {
                                        Logger.Warning("同步能耗公式数据错误，详见错误日志。");
                                        Logger.Error("同步能耗公式数据错误", err);
                                    }

                                    try {
                                        LoadStaticPolicies();
                                    } catch (Exception err) {
                                        Logger.Warning("同步信号测值统计策略错误，详见错误日志。");
                                        Logger.Error("同步信号测值统计策略错误", err);
                                    }

                                    try {
                                        LoadBatPolicies();
                                    } catch (Exception err) {
                                        Logger.Warning("同步电池数据处理策略错误，详见错误日志。");
                                        Logger.Error("同步电池数据处理策略错误", err);
                                    }

                                    try {
                                        LoadCutPolicies();
                                    } catch (Exception err) {
                                        Logger.Warning("同步断站、停电、发电策略错误，详见错误日志。");
                                        Logger.Error("同步断站、停电、发电策略错误", err);
                                    }

                                    Logger.Information("同步配置完成。");
                                } finally {
                                    _configWriterLock.ExitWriteLock();
                                }
                                #endregion
                            } else if (order.Id == OrderId.SyncAlarm) {
                                #region 同步活动告警
                                try {
                                    //获取写资源锁
                                    _configWriterLock.EnterWriteLock();

                                    Logger.Information("同步告警...");
                                    LoadAlarms();
                                    Logger.Information("同步告警完成。");
                                } catch (Exception err) {
                                    Logger.Warning("同步活动告警数据错误，详见错误日志。");
                                    Logger.Error("同步活动告警数据错误", err);
                                } finally {
                                    _configWriterLock.ExitWriteLock();
                                }
                                #endregion
                            } else if (order.Id == OrderId.SyncReservation) {
                                #region 同步工程预约
                                try {
                                    //获取写资源锁
                                    _configWriterLock.EnterWriteLock();

                                    Logger.Information("同步工程预约...");
                                    LoadReservations();
                                    Logger.Information("同步工程预约完成。");
                                } catch (Exception err) {
                                    Logger.Warning("同步工程预约数据错误，详见错误日志。");
                                    Logger.Error("同步工程预约数据错误", err);
                                } finally {
                                    _configWriterLock.ExitWriteLock();
                                }
                                #endregion
                            } else if (order.Id == OrderId.SyncFormula) {
                                #region 同步能耗公式
                                try {
                                    //获取写资源锁
                                    _configWriterLock.EnterWriteLock();

                                    Logger.Information("同步能耗公式...");
                                    LoadFormulas();
                                    Logger.Information("同步能耗公式完成。");
                                } catch (Exception err) {
                                    Logger.Warning("同步能耗公式数据错误，详见错误日志。");
                                    Logger.Error("同步能耗公式数据错误", err);
                                } finally {
                                    _configWriterLock.ExitWriteLock();
                                }
                                #endregion
                            } else if (order.Id == OrderId.SyncMasking) {
                                #region 同步告警屏蔽
                                try {
                                    //获取写资源锁
                                    _configWriterLock.EnterWriteLock();

                                    Logger.Information("同步告警屏蔽数据...");
                                    LoadMaskings();
                                    Logger.Information("同步告警屏蔽数据完成。");
                                } catch (Exception err) {
                                    Logger.Warning("同步告警屏蔽数据错误，详见错误日志。");
                                    Logger.Error("同步告警屏蔽数据错误", err);
                                } finally {
                                    _configWriterLock.ExitWriteLock();
                                }
                                #endregion
                            } else if (order.Id == OrderId.SyncFsu) {
                                #region 同步FSU数据
                                try {
                                    //获取写资源锁
                                    _configWriterLock.EnterWriteLock();

                                    Logger.Information("同步FSU数据...");
                                    iPemWorkContext.Fsus = _fsuRepository.GetEntities();
                                    Logger.Information("同步FSU数据完成。");
                                } catch (Exception err) {
                                    Logger.Warning("同步FSU数据错误，详见错误日志。");
                                    Logger.Error("同步FSU数据错误", err);
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
                                        var _FzdlParam = GlobalConfig.CurParams.Find(p => p.Id == ParamId.FZDL);
                                        if (_FzdlParam == null || string.IsNullOrWhiteSpace(_FzdlParam.Value))
                                            throw new Exception("尚未配置负载电流信号");

                                        if(!iPemWorkContext.Points.ContainsKey(_FzdlParam.Value))
                                            throw new Exception("负载电流信号配置错误");

                                        var _FzdlPoint = iPemWorkContext.Points[_FzdlParam.Value];

                                        var _GzztParam = GlobalConfig.CurParams.Find(p => p.Id == ParamId.GZZT);
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
                        //获取读资源锁
                        _configWriterLock.EnterReadLock();

                        var _alarmModels = new List<TAlarmModel>();
                        var _alarms = _talmRepository.GetEntities();
                        var _skips = new List<A_TAlarm>();
                        foreach (var _alarm in _alarms) {
                            try {
                                if (_skips.Contains(_alarm)) continue;

                                var key = CommonHelper.JoinKeys(_alarm.FsuId, _alarm.DeviceId);
                                if (!iPemWorkContext.DeviceSet2.ContainsKey(key)) continue;

                                var _device = iPemWorkContext.DeviceSet2[key];
                                var _signal = _device.Signals.Find(p => p.PointId == _alarm.PointId);
                                if (_signal == null) continue;

                                //当告警延迟时间未到，而告警已经结束，则直接删除此告警流水
                                if (_alarm.AlarmFlag == EnmFlag.Begin && _signal.AlarmDelay > 0) {
                                    var _endalarm = _alarms.Find(a => a.FsuId == _alarm.FsuId && a.SerialNo == _alarm.SerialNo && a.AlarmFlag == EnmFlag.End);
                                    if (_endalarm != null && _alarm.AlarmTime.AddSeconds(_signal.AlarmDelay) >= _endalarm.AlarmTime) {
                                        _talmRepository.Delete(new List<A_TAlarm> { _alarm, _endalarm });
                                        _skips.Add(_endalarm);
                                        continue;
                                    }
                                }

                                _alarmModels.Add(new TAlarmModel { Device = _device.Current, Signal = _signal, Alarm = _alarm });
                            } catch { }
                        }

                        if (_alarmModels.Count > 0) {
                            var _validModels = new List<TAlarmModel>();

                            #region 处理告警触发延迟、告警恢复延迟
                            foreach (var _alarm in _alarmModels) {
                                if (_alarm.Alarm.AlarmFlag == EnmFlag.Begin && _alarm.Signal.AlarmDelay > 0 && _alarm.Alarm.AlarmTime.AddSeconds(_alarm.Signal.AlarmDelay) > DateTime.Now) {
                                    continue;
                                } else if (_alarm.Alarm.AlarmFlag == EnmFlag.End && _alarm.Signal.AlarmRecoveryDelay > 0 && _alarm.Alarm.AlarmTime.AddSeconds(_alarm.Signal.AlarmRecoveryDelay) > DateTime.Now) {
                                    continue;
                                }

                                _validModels.Add(_alarm);
                            }
                            #endregion

                            // 为了防止服务器和FSU时间不一致，允许服务器和FSU时间有1小时的误差。
                            var _reservations = GlobalConfig.Reservations.FindAll(res => DateTime.Now >= res.Reservation.StartTime.AddHours(-1) && DateTime.Now <= res.Reservation.EndTime.AddHours(1));
                            // 关闭/打开实时告警接口
                            var _iparam = GlobalConfig.CurParams.Find(p => p.Id == ParamId.GJJK);
                            var _ienabled = _iparam != null && "1".Equals(_iparam.Value) ? true : false;
                            // 处理有效告警
                            foreach (var _alarm in _validModels) {
                                try {
                                    var _id = string.Format("{0}-{1}", _alarm.Alarm.SerialNo, _alarm.Device.FsuId);
                                    var _key = CommonHelper.JoinKeys(_alarm.Device.Id, _alarm.Signal.PointId);
                                    if (_alarm.Alarm.AlarmFlag == EnmFlag.Begin) {
                                        #region 开始告警
                                        try {
                                            var _active = _aalmRepository.GetEntity(_id);
                                            if (_active == null) {

                                                #region 初始化告警
                                                var _current = new AlarmStart {
                                                    Id = _id,
                                                    AreaId = _alarm.Device.AreaId,
                                                    StationId = _alarm.Device.StationId,
                                                    RoomId = _alarm.Device.RoomId,
                                                    FsuId = _alarm.Device.FsuId,
                                                    FsuCode = _alarm.Device.FsuCode,
                                                    DeviceId = _alarm.Device.Id,
                                                    DeviceCode = _alarm.Device.Code,
                                                    PointId = _alarm.Signal.PointId,
                                                    SerialNo = _alarm.Alarm.SerialNo,
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
                                                foreach (var _res in _reservations) {
                                                    if (_res.Devices.Contains(_current.DeviceId)) {
                                                        _current.ReservationId = _res.Reservation.Id;
                                                        _current.ReservationName = _res.Reservation.Name;
                                                        _current.ReservationStart = _res.Reservation.StartTime;
                                                        _current.ReservationEnd = _res.Reservation.EndTime;
                                                        break;
                                                    }
                                                }
                                                #endregion

                                                #region 主次告警
                                                if (!string.IsNullOrWhiteSpace(_alarm.Signal.InferiorAlarmStr)) {
                                                    var dicKey = CommonHelper.JoinKeys(_alarm.Signal.InferiorAlarmStr.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
                                                    if (GlobalConfig.AlarmKeys2.ContainsKey(dicKey)) {
                                                        _current.PrimaryId = GlobalConfig.AlarmKeys2[dicKey].Id;
                                                    }
                                                }
                                                #endregion

                                                #region 关联告警
                                                if (!string.IsNullOrWhiteSpace(_alarm.Signal.ConnAlarmStr)) {
                                                    var dicKey = CommonHelper.JoinKeys(_alarm.Signal.ConnAlarmStr.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
                                                    if (GlobalConfig.AlarmKeys2.ContainsKey(dicKey)) {
                                                        _current.RelatedId = GlobalConfig.AlarmKeys2[dicKey].Id;
                                                    }
                                                }
                                                #endregion

                                                #region 告警过滤
                                                if (!string.IsNullOrWhiteSpace(_alarm.Signal.AlarmFilteringStr)) {
                                                    var dicKey = CommonHelper.JoinKeys(_alarm.Signal.AlarmFilteringStr.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
                                                    if (GlobalConfig.AlarmKeys2.ContainsKey(dicKey)) {
                                                        _current.FilterId = GlobalConfig.AlarmKeys2[dicKey].Id;
                                                    }
                                                }
                                                #endregion

                                                #region 告警翻转
                                                if (!string.IsNullOrWhiteSpace(_alarm.Signal.AlarmReversalStr)) {
                                                    int reversalInterval;
                                                    if (int.TryParse(_alarm.Signal.AlarmReversalStr, out reversalInterval) && reversalInterval > 0) {
                                                        if (GlobalConfig.ReversalKeys.ContainsKey(_key)) {
                                                            var reversalTarget = GlobalConfig.ReversalKeys[_key];
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
                                                            GlobalConfig.ReversalKeys[_key] = new ReversalModel { AlarmId = _current.Id, AlarmTime = _current.AlarmTime, ReversalCount = 0 };
                                                            _current.ReversalId = _current.Id;
                                                            _current.ReversalCount = 0;
                                                        }
                                                    }
                                                }
                                                #endregion

                                                #region 告警屏蔽

                                                _current.Masked = GlobalConfig.Maskings.Contains(CommonHelper.JoinKeys(_current.DeviceId, "masking-all"));
                                                if (!_current.Masked) _current.Masked = GlobalConfig.Maskings.Contains(_key);

                                                #endregion

                                                #region 开始告警
                                                _alarmer.Start(_current);
                                                GlobalConfig.AddAlarm(_current);

                                                //启用/关闭 实时告警接口
                                                if (_ienabled) _alarmer.StartInterface(_current);
                                                #endregion

                                                #region 断站、停电、发电
                                                var _cuttings = new List<V_Cutting>();

                                                #region 断站告警
                                                if (GlobalConfig.Offs.Contains(_key)) {
                                                    _cuttings.Add(new V_Cutting {
                                                        Id = _current.Id,
                                                        Type = EnmCutType.Off,
                                                        AreaId = _current.AreaId,
                                                        StationId = _current.StationId,
                                                        RoomId = _current.RoomId,
                                                        FsuId = _current.FsuId,
                                                        DeviceId = _current.DeviceId,
                                                        PointId = _current.PointId,
                                                        StartTime = _current.AlarmTime
                                                    });
                                                }
                                                #endregion

                                                #region 停电告警
                                                if (GlobalConfig.Cuttings.Contains(_key)) {
                                                    _cuttings.Add(new V_Cutting {
                                                        Id = _current.Id,
                                                        Type = EnmCutType.Cut,
                                                        AreaId = _current.AreaId,
                                                        StationId = _current.StationId,
                                                        RoomId = _current.RoomId,
                                                        FsuId = _current.FsuId,
                                                        DeviceId = _current.DeviceId,
                                                        PointId = _current.PointId,
                                                        StartTime = _current.AlarmTime
                                                    });
                                                }
                                                #endregion

                                                #region 发电告警
                                                if (GlobalConfig.Powers.Contains(_key)) {
                                                    _cuttings.Add(new V_Cutting {
                                                        Id = _current.Id,
                                                        Type = EnmCutType.Power,
                                                        AreaId = _current.AreaId,
                                                        StationId = _current.StationId,
                                                        RoomId = _current.RoomId,
                                                        FsuId = _current.FsuId,
                                                        DeviceId = _current.DeviceId,
                                                        PointId = _current.PointId,
                                                        StartTime = _current.AlarmTime
                                                    });
                                                }
                                                #endregion

                                                if(_cuttings.Count > 0) _cuttingRepository.SaveEntities(_cuttings);
                                                #endregion

                                            } else {
                                                _alarmer.Delete(_alarm.Alarm);
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
                                            var _active = _aalmRepository.GetEntity(_id);
                                            if (_active != null) {

                                                #region 初始化告警
                                                var _current = new AlarmEnd {
                                                    Id = _active.Id,
                                                    AreaId = _alarm.Device.AreaId,
                                                    StationId = _alarm.Device.StationId,
                                                    RoomId = _alarm.Device.RoomId,
                                                    FsuId = _alarm.Device.FsuId,
                                                    FsuCode = _alarm.Device.FsuCode,
                                                    DeviceId = _alarm.Device.Id,
                                                    DeviceCode = _alarm.Device.Code,
                                                    PointId = _alarm.Signal.PointId,
                                                    SerialNo = _alarm.Alarm.SerialNo,
                                                    NMAlarmId = _alarm.Alarm.NMAlarmId,
                                                    StartTime = _active.AlarmTime,
                                                    EndTime = _alarm.Alarm.AlarmTime,
                                                    AlarmLevel = _alarm.Alarm.AlarmLevel,
                                                    AlarmFlag = _alarm.Alarm.AlarmFlag,
                                                    StartValue = _active.AlarmValue,
                                                    EndValue = _alarm.Alarm.AlarmValue,
                                                    AlarmDesc = _alarm.Alarm.AlarmDesc,
                                                    AlarmRemark = _alarm.Alarm.AlarmRemark,
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

                                                #region 工程预约(为实时告警接口写入工程名称、开始时间、结束时间等)
                                                if (_ienabled && !string.IsNullOrWhiteSpace(_current.ReservationId)) {
                                                    var _res = GlobalConfig.Reservations.Find(r => r.Reservation.Id == _current.ReservationId);
                                                    if (_res != null) {
                                                        _current.ReservationName = _res.Reservation.Name;
                                                        _current.ReservationStart = _res.Reservation.StartTime;
                                                        _current.ReservationEnd = _res.Reservation.EndTime;
                                                    }
                                                }
                                                #endregion

                                                #region 结束告警
                                                _alarmer.End(_current);
                                                GlobalConfig.RemoveAlarm(_current);

                                                //启用/关闭 实时告警接口
                                                if (_ienabled) _alarmer.EndInterface(_current);
                                                #endregion

                                                #region 断站、停电、发电
                                                var _cuteds = new List<V_Cuted>();

                                                #region 断站告警
                                                if (GlobalConfig.Offs.Contains(_key)) {
                                                    _cuteds.Add(new V_Cuted {
                                                        Id = _current.Id,
                                                        Type = EnmCutType.Off,
                                                        AreaId = _current.AreaId,
                                                        StationId = _current.StationId,
                                                        RoomId = _current.RoomId,
                                                        FsuId = _current.FsuId,
                                                        DeviceId = _current.DeviceId,
                                                        PointId = _current.PointId,
                                                        StartTime = _current.StartTime,
                                                        EndTime = _current.EndTime
                                                    });
                                                }
                                                #endregion

                                                #region 停电告警
                                                if (GlobalConfig.Cuttings.Contains(_key)) {
                                                    _cuteds.Add(new V_Cuted {
                                                        Id = _current.Id,
                                                        Type = EnmCutType.Cut,
                                                        AreaId = _current.AreaId,
                                                        StationId = _current.StationId,
                                                        RoomId = _current.RoomId,
                                                        FsuId = _current.FsuId,
                                                        DeviceId = _current.DeviceId,
                                                        PointId = _current.PointId,
                                                        StartTime = _current.StartTime,
                                                        EndTime = _current.EndTime
                                                    });
                                                }
                                                #endregion

                                                #region 发电告警
                                                if (GlobalConfig.Powers.Contains(_key)) {
                                                    _cuteds.Add(new V_Cuted {
                                                        Id = _current.Id,
                                                        Type = EnmCutType.Power,
                                                        AreaId = _current.AreaId,
                                                        StationId = _current.StationId,
                                                        RoomId = _current.RoomId,
                                                        FsuId = _current.FsuId,
                                                        DeviceId = _current.DeviceId,
                                                        PointId = _current.PointId,
                                                        StartTime = _current.StartTime,
                                                        EndTime = _current.EndTime
                                                    });
                                                }
                                                #endregion

                                                if (_cuteds.Count > 0) _cutedRepository.SaveEntities(_cuteds);
                                                #endregion
                                                
                                            } else {
                                                _alarmer.Delete(_alarm.Alarm);
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
        /// 系统自检任务
        /// 5秒自检一次
        /// </summary>
        private void DoChecking() {
            _allDone.WaitOne();

            if (_runStatus != RunStatus.Running) return;
            Logger.Information("系统自检线程已启动。");

            #region 定义变量
            KV<iPem.Core.Point, SubPoint> _ScOffPoint = null;
            //每个FSU所对应的SubPoint都不同，需要动态获取
            iPem.Core.Point _FsuOffPoint = null;
            //上次检索数据库索引的时间
            DateTime _NextIndexer = DateTime.Today;
            //上次重启IIS的时间
            DateTime _NextIISReset = DateTime.Today;
            try {
                var _scOff = GlobalConfig.CurParams.Find(p => p.Id == ParamId.ScOff);
                if (_scOff != null && !string.IsNullOrWhiteSpace(_scOff.Value)) {
                    if (!iPemWorkContext.Points.ContainsKey(_scOff.Value)) {
                        Logger.Warning("SC通信中断告警编码配置错误");
                    } else {
                        var _scPoint = iPemWorkContext.Points[_scOff.Value];
                        var _scSubPoint = iPemWorkContext.SubPoints.Values.FirstOrDefault(p => p.PointId == _scPoint.Id);
                        _ScOffPoint = new KV<iPem.Core.Point, SubPoint> { Key = _scPoint, Value = _scSubPoint };
                    }
                } else {
                    Logger.Warning("SC通信中断告警编码尚未配置");
                }

                var _fsuOff = GlobalConfig.CurParams.Find(p => p.Id == ParamId.FsuOff);
                if (_fsuOff != null && !string.IsNullOrWhiteSpace(_fsuOff.Value)) {
                    if (!iPemWorkContext.Points.ContainsKey(_fsuOff.Value)) {
                        Logger.Warning("FSU通信中断告警编码配置错误");
                    } else {
                        _FsuOffPoint = iPemWorkContext.Points[_fsuOff.Value];
                    }
                } else {
                    Logger.Warning("FSU通信中断告警编码尚未配置");
                }
            } catch (Exception err) {
                Logger.Warning(err.Message);
                Logger.Error(err.Message, err);
            }
            #endregion

            var interval = 86400;
            while (_runStatus != RunStatus.Stop) {
                Thread.Sleep(1000);
                if (_runStatus == RunStatus.Running) {
                    if (interval++ <= 10) continue; interval = 1;
                    if (_ScOffPoint == null && _FsuOffPoint == null) continue;

                    try {
                        _configWriterLock.EnterReadLock();

                        #region SC通信中断
                        try {
                            if (_ScOffPoint != null) {
                                foreach (var _sc in GlobalConfig.ScHeartbeats) {
                                    var _key = CommonHelper.JoinKeys(_sc.Current.Id, _ScOffPoint.Key.Id);
                                    if (_sc.Current.Status && GlobalConfig.AlarmKeys2.ContainsKey(_key)) {
                                        var _alarm = _aalmRepository.GetEntityInPoint(_sc.Current.Id, _ScOffPoint.Key.Id);
                                        if (_alarm != null) {
                                            #region 结束告警
                                            var _current = new AlarmEnd {
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

                                            _alarmer.End(_current);
                                            GlobalConfig.RemoveAlarm(_current);
                                            #endregion
                                        }
                                    } else if (!_sc.Current.Status && !GlobalConfig.AlarmKeys2.ContainsKey(_key)) {
                                        #region 开始告警
                                        var _current = new AlarmStart {
                                            Id = null,
                                            AreaId = "-1",
                                            StationId = "-1",
                                            RoomId = "-1",
                                            FsuId = "-1",
                                            FsuCode = "-1",
                                            DeviceId = _sc.Current.Id,
                                            DeviceCode = _sc.Current.Id,
                                            PointId = _ScOffPoint.Key.Id,
                                            SerialNo = CommonHelper.GetIdAsString(),
                                            NMAlarmId = _ScOffPoint.Key.NMAlarmId,
                                            AlarmTime = DateTime.Now,
                                            AlarmLevel = _ScOffPoint.Value != null ? _ScOffPoint.Value.AlarmLevel : EnmAlarm.Level2,
                                            AlarmFlag = EnmFlag.Begin,
                                            AlarmValue = 0,
                                            AlarmDesc = null,
                                            AlarmRemark = null
                                        };
                                        _current.Id = string.Format("00-{0}-{1}", _current.SerialNo, _current.DeviceId);

                                        _alarmer.Start(_current);
                                        GlobalConfig.AddAlarm(_current);
                                        #endregion
                                    }
                                }
                            }
                        } catch (Exception err) {
                            Logger.Warning(err.Message);
                            Logger.Error(err.Message, err);
                        }
                        #endregion

                        #region FSU通信中断
                        try {
                            if (_FsuOffPoint != null) {
                                var _extFsus = _fsuRepository.GetExtEntities();
                                foreach (var _ext in _extFsus) {
                                    var _alarm = _aalmRepository.GetEntityInPoint(_ext.Id, _FsuOffPoint.Id);
                                    if (_ext.Status && _alarm != null) {
                                        #region 结束告警
                                        var _current = new AlarmEnd {
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

                                        _alarmer.End(_current);
                                        GlobalConfig.RemoveAlarm(_current);
                                        #endregion
                                    } else if(!_ext.Status && _alarm == null) {
                                        #region 开始告警
                                        var _fsu = iPemWorkContext.Fsus.Find(d => d.Id == _ext.Id);
                                        if (_fsu != null) {
                                            var _skey = CommonHelper.JoinKeys(_FsuOffPoint.Id, _fsu.StaTypeId);
                                            var _subPoint = iPemWorkContext.SubPoints.ContainsKey(_skey) ? iPemWorkContext.SubPoints[_skey] : null;
                                            var _current = new AlarmStart {
                                                Id = null,
                                                AreaId = _fsu.AreaId,
                                                StationId = _fsu.StationId,
                                                RoomId = _fsu.RoomId,
                                                FsuId = _fsu.Id,
                                                FsuCode = _fsu.Code,
                                                DeviceId = _fsu.Id,
                                                DeviceCode = _fsu.Code,
                                                PointId = _FsuOffPoint.Id,
                                                SerialNo = CommonHelper.GetIdAsString(),
                                                NMAlarmId = _FsuOffPoint.NMAlarmId,
                                                AlarmTime = DateTime.Now,
                                                AlarmLevel = _subPoint != null ? _subPoint.AlarmLevel : EnmAlarm.Level2,
                                                AlarmFlag = EnmFlag.Begin,
                                                AlarmValue = 0,
                                                AlarmDesc = null,
                                                AlarmRemark = null
                                            };
                                            _current.Id = string.Format("01-{0}-{1}", _current.SerialNo, _current.FsuId);

                                            _alarmer.Start(_current);
                                            GlobalConfig.AddAlarm(_current);
                                        }
                                        #endregion
                                    }
                                }
                            }
                        } catch (Exception err) {
                            Logger.Warning(err.Message);
                            Logger.Error(err.Message, err);
                        }
                        #endregion

                        #region 检查数据库索引
                        if (DateTime.Now > _NextIndexer) {
                            for (var i = 0; i <= 3; i++) {
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

                        #region 重启IIS服务
                        if (DateTime.Now > _NextIISReset) {
                            try {
                                CommonHelper.ResetIIS();
                            } catch (Exception err) {
                                Logger.Warning(err.Message);
                                Logger.Error(err.Message, err);
                            } finally {
                                _NextIISReset = DateTime.Today.AddHours(25);
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

                        foreach (var beat in GlobalConfig.ScHeartbeats) {
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
                                        var now = DateTime.Now;
                                        _groupRepository.SetOff(beat.Current.Id, now);
                                        beat.Current.Status = false;
                                        beat.Current.LastTime = now;
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
        /// 实时能耗任务
        /// </summary>
        private void DoFormula() {
            _allDone.WaitOne();

            if (_runStatus != RunStatus.Running) return;
            Logger.Information("实时能耗线程已启动。");
            var interval = 86400;
            while (_runStatus != RunStatus.Stop) {
                Thread.Sleep(1000);
                if (_runStatus == RunStatus.Running) {
                    if (interval++ <= 60) continue; interval = 1;

                    try {
                        _configWriterLock.EnterReadLock();

                        if (GlobalConfig.FormulaModels == null) continue;
                        var _param = GlobalConfig.CurParams.Find(p => p.Id == ParamId.SSNH);
                        var _period = _param == null ? PeriodType.Day : (PeriodType)int.Parse(_param.Value);
                        var _result = new List<V_Elec>();

                        #region 能耗数据
                        foreach (var _formula in GlobalConfig.FormulaModels) {
                            try {
                                var _formulaText = _formula.FormulaText;
                                if (string.IsNullOrWhiteSpace(_formulaText)) continue;
                                if (!CommonHelper.ValidateFormula(_formulaText)) continue;

                                var _start = GetPeriod(_period); var _end = DateTime.Now;
                                var _current = _formulaText; var _value = 0d;
                                var _variables = CommonHelper.GetFormulaVariables(_formulaText);
                                if (_variables.Count == 0) {
                                    #region 公式中没有变量
                                    try {
                                        var __value = _computer.Compute(_current, "");
                                        if (__value != DBNull.Value) _value = Convert.ToDouble(__value);
                                        if (double.IsNaN(_value) || double.IsInfinity(_value)) _value = 0d;
                                    } catch { }
                                    #endregion
                                } else {
                                    #region 公式中有变量
                                    List<WcDevice> _devices;
                                    if (_formula.Type == EnmSSH.Station) {
                                        var _station = iPemWorkContext.Stations.Find(c => c.Id == _formula.Id);
                                        if (_station == null) throw new Exception("未找到公式所属的站点。");

                                        _devices = iPemWorkContext.Devices.FindAll(d => d.Current.StationId == _station.Id);
                                    } else if (_formula.Type == EnmSSH.Room) {
                                        var _room = iPemWorkContext.Rooms.Find(c => c.Id == _formula.Id);
                                        if (_room == null) throw new Exception("未找到公式所属的机房。");

                                        _devices = iPemWorkContext.Devices.FindAll(d => d.Current.RoomId == _room.Id);
                                    } else {
                                        throw new Exception("仅支持站点、机房能耗公式。");
                                    }

                                    var _details = new List<VariableDetail>();
                                    foreach (var _variable in _variables) {
                                        var _factors = _variable.Split(new string[] { ">>" }, StringSplitOptions.None);
                                        var _devkey = _factors[0].Substring(1);
                                        var _potkey = _factors[1];
                                        var _device = _devices.Find(d => d.Current.Name == _devkey);
                                        if (_device == null) throw new Exception(string.Format("未找到变量{0}中的设备。", _variable));
                                        var _signal = _device.Signals.Find(p => p.Name == _potkey);
                                        if (_signal == null) throw new Exception(string.Format("未找到变量{0}中的信号。", _variable));
                                        _details.Add(new VariableDetail {
                                            AreaId = _device.Current.AreaId,
                                            StationId = _device.Current.StationId,
                                            RoomId = _device.Current.RoomId,
                                            FsuId = _device.Current.FsuId,
                                            DeviceId = _device.Current.Id,
                                            PointId = _signal.PointId,
                                            Variable = _variable 
                                        });
                                    }

                                    try {
                                        foreach (var _detail in _details) {
                                            var _vvalue = 0d;
                                            if (_formula.ComputeType == EnmCompute.Diff) {
                                                var _first = _hmeasureRepository.GetFirst(_detail.DeviceId, _detail.PointId, _start, _end);
                                                if (_first != null) {
                                                    var _last = _ameasureRepository.GetEntity(_detail.DeviceId, _detail.PointId);
                                                    if (_last != null) _vvalue = Math.Round(_last.Value - _first.Value, 3, MidpointRounding.AwayFromZero);
                                                }
                                            } else if (_formula.ComputeType == EnmCompute.Avg) {
                                                _vvalue = _hmeasureRepository.GetAvg(_detail.DeviceId, _detail.PointId, _start, _end);
                                            }

                                            _current = _current.Replace(_detail.Variable, _vvalue.ToString());
                                        }

                                        var __value = _computer.Compute(_current, "");
                                        if (__value != DBNull.Value) _value = Convert.ToDouble(__value);
                                        if (double.IsNaN(_value) || double.IsInfinity(_value)) _value = 0d;
                                    } catch { }
                                    
                                    #endregion
                                }

                                _result.Add(new V_Elec {
                                    Id = _formula.Id,
                                    Type = _formula.Type,
                                    FormulaType = _formula.FormulaType,
                                    ComputeType = _formula.ComputeType,
                                    StartTime = _start,
                                    EndTime = _end,
                                    Value = Math.Round(_value * (_formula.ComputeType == EnmCompute.Diff ? 1 : _end.Subtract(_start).TotalHours), 3, MidpointRounding.AwayFromZero)
                                });
                            } catch { }
                        }
                        #endregion

                        if (_result.Count > 0) 
                            _elecRepository.SaveActiveEntities(_result);
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
            var _curParam = GlobalConfig.CurParams.Find(p => p.Id == ParamId.DCSJ);
            if (_curParam == null || !"1".Equals(_curParam.Value)) {
                Logger.Warning("电池数据处理功能未开启，线程退出。");
                return;
            }

            Logger.Information("电池数据处理线程已启动。");
            var interval = 86400;
            while (_runStatus != RunStatus.Stop) {
                Thread.Sleep(1000);
                if (_runStatus == RunStatus.Running) {
                    if (interval++ <= 60) continue; interval = 1;

                    return;
                    #region 屏蔽电池数据处理
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
                    #endregion
                }
            }
        }

        /// <summary>
        /// 能耗处理任务
        /// </summary>
        private void DoTask001() {
            _allDone.WaitOne();

            if (_runStatus != RunStatus.Running) return;
            var _curTask = GlobalConfig.CurTasks.Find(t => t.Id == "T001");
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
                        GlobalConfig.SetTaskPloy(_curTask);
                        Logger.Information(string.Format("{0}执行完成，下次执行时间：{1}。", _curTask.Name, CommonHelper.ToDateTimeString(_curTask.Next)));
                    }
                }
            }
        }

        /// <summary>
        /// 能耗处理方法
        /// </summary>
        private void ExTask001(DateTime start, DateTime end) {
            var _times = CommonHelper.GetHourSpan(start, end);
            if (_times.Count == 0) return;

            if (GlobalConfig.FormulaModels == null) return;
            var _result = new List<V_Elec>();

            #region 能耗数据
            foreach (var _formula in GlobalConfig.FormulaModels) {
                try {
                    var _formulaText = _formula.FormulaText;
                    if (string.IsNullOrWhiteSpace(_formulaText)) continue;
                    if (!CommonHelper.ValidateFormula(_formulaText)) 
                        throw new Exception("无效的公式。");

                    var _variables = CommonHelper.GetFormulaVariables(_formulaText);
                    if (_variables.Count == 0) {
                        #region 公式中没有变量
                        foreach (var _time in _times) {
                            var _start = _time; var _end = _time.AddHours(1);
                            var _current = _formulaText; var _value = 0d;

                            try {
                                var __value = _computer.Compute(_current, "");
                                if (__value != DBNull.Value) _value = Convert.ToDouble(__value);
                                if (double.IsNaN(_value) || double.IsInfinity(_value)) _value = 0d;
                            } catch { }

                            _result.Add(new V_Elec {
                                Id = _formula.Id,
                                Type = _formula.Type,
                                FormulaType = _formula.FormulaType,
                                ComputeType = _formula.ComputeType,
                                StartTime = _start,
                                EndTime = _end,
                                Value = Math.Round(_value * (_formula.ComputeType == EnmCompute.Diff ? 1 : _end.Subtract(_start).TotalHours), 3, MidpointRounding.AwayFromZero)
                            });
                        }
                        #endregion
                    } else {
                        #region 公式中有变量

                        #region 解析公式
                        List<WcDevice> _devices;
                        if (_formula.Type == EnmSSH.Station) {
                            var _station = iPemWorkContext.Stations.Find(c => c.Id == _formula.Id);
                            if (_station == null) throw new Exception("未找到公式所属的站点。");

                            _devices = iPemWorkContext.Devices.FindAll(d => d.Current.StationId == _station.Id);
                        } else if (_formula.Type == EnmSSH.Room) {
                            var _room = iPemWorkContext.Rooms.Find(c => c.Id == _formula.Id);
                            if (_room == null) throw new Exception("未找到公式所属的机房。");

                            _devices = iPemWorkContext.Devices.FindAll(d => d.Current.RoomId == _room.Id);
                        } else {
                            throw new Exception("仅支持站点、机房能耗公式。");
                        }

                        var _details = new List<VariableDetail>();
                        foreach (var _variable in _variables) {
                            var _factors = _variable.Split(new string[] { ">>" }, StringSplitOptions.None);
                            var _devkey = _factors[0].Substring(1);
                            var _potkey = _factors[1];
                            var _device = _devices.Find(d => d.Current.Name == _devkey);
                            if (_device == null) throw new Exception(string.Format("未找到变量{0}中的设备。", _variable));
                            var _signal = _device.Signals.Find(p => p.Name == _potkey);
                            if (_signal == null) throw new Exception(string.Format("未找到变量{0}中的信号。", _variable));
                            _details.Add(new VariableDetail {
                                AreaId = _device.Current.AreaId,
                                StationId = _device.Current.StationId,
                                RoomId = _device.Current.RoomId,
                                FsuId = _device.Current.FsuId,
                                DeviceId = _device.Current.Id,
                                PointId = _signal.PointId,
                                Variable = _variable
                            });
                        }
                        #endregion

                        #region 计算公式
                        foreach (var _time in _times) {
                            var _start = _time; var _end = _time.AddHours(1);
                            var _current = _formulaText; var _value = 0d;

                            try {
                                foreach (var _detail in _details) {
                                    var _vvalue = 0d;
                                    if (_formula.ComputeType == EnmCompute.Diff) {
                                        var _first = _hmeasureRepository.GetFirst(_detail.DeviceId, _detail.PointId, _start, _end);
                                        if (_first != null) {
                                            var _last = _hmeasureRepository.GetFirst(_detail.DeviceId, _detail.PointId, _end, _end.AddHours(1));
                                            if (_last == null) _last = _hmeasureRepository.GetLast(_detail.DeviceId, _detail.PointId, _start, _end);
                                            if (_last != null) _vvalue = Math.Round(_last.Value - _first.Value, 3, MidpointRounding.AwayFromZero);
                                        }
                                    } else if (_formula.ComputeType == EnmCompute.Avg) {
                                        _vvalue = _hmeasureRepository.GetAvg(_detail.DeviceId, _detail.PointId, _start, _end);
                                    }

                                    _current = _current.Replace(_detail.Variable, _vvalue.ToString());
                                }

                                var __value = _computer.Compute(_current, "");
                                if (__value != DBNull.Value) _value = Convert.ToDouble(__value);
                                if (double.IsNaN(_value) || double.IsInfinity(_value)) _value = 0d;
                            } catch { }

                            _result.Add(new V_Elec {
                                Id = _formula.Id,
                                Type = _formula.Type,
                                FormulaType = _formula.FormulaType,
                                ComputeType = _formula.ComputeType,
                                StartTime = _start,
                                EndTime = _end,
                                Value = Math.Round(_value * (_formula.ComputeType == EnmCompute.Diff ? 1 : _end.Subtract(_start).TotalHours), 3, MidpointRounding.AwayFromZero)
                            });
                        }
                        #endregion

                        #endregion
                    }
                } catch (Exception err) {
                    err.Source = JsonConvert.SerializeObject(_formula);

                    Logger.Warning("能耗处理发生错误，详见错误日志。");
                    Logger.Error("能耗处理发生错误", err);
                }
            }
            #endregion

            _elecRepository.DeleteEntities(start, end);
            _elecRepository.SaveEntities(_result);
        }

        /// <summary>
        /// 电池曲线处理任务
        /// </summary>
        private void DoTask002() {
            _allDone.WaitOne();

            if (_runStatus != RunStatus.Running) return;
            var _curTask = GlobalConfig.CurTasks.Find(t => t.Id == "T002");
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
                        GlobalConfig.SetTaskPloy(_curTask);
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

            #region 电池充放电过程处理
            try {
                var _batTimes = new List<V_BatTime>();
                foreach (var _procedure in _batRepository.GetProcedures(start, end)) {
                    try {
                        var _details = _batRepository.GetProcDetails(_procedure.DeviceId, _procedure.PointId, _procedure.StartTime, _procedure.StartTime.AddSeconds(_maxInterval));
                        if (_details.Count == 0) continue;
                        var _start = _details.First(); 
                        var _end = _details.Last();

                        if (_end.ValueTime.Subtract(_start.ValueTime).TotalSeconds < _minInterval) continue;

                        var _time = new V_BatTime {
                            AreaId = _start.AreaId,
                            StationId = _start.StationId,
                            RoomId = _start.RoomId,
                            DeviceId = _start.DeviceId,
                            PointId = _start.PointId,
                            PackId = _start.PackId,
                            Type = _start.Type,
                            StartTime = _start.StartTime,
                            StartValue = _start.Value,
                            EndTime = _end.ValueTime,
                            EndValue = _end.Value,
                            ProcTime = _start.StartTime
                        };

                        if (_time.Type == EnmBatStatus.Charge) {
                            var discharge = _batRepository.GetLast(_time.DeviceId, _time.PointId, _time.StartTime.AddSeconds(_maxInterval * -1), _time.StartTime);
                            if (discharge != null && discharge.Type == EnmBatStatus.Discharge) _time.ProcTime = discharge.StartTime;
                        }

                        _batTimes.Add(_time);
                    } catch (Exception err) {
                        Logger.Warning("电池充放电过程处理发生错误，详见错误日志。");
                        Logger.Error("电池充放电过程处理发生错误", err);
                    }
                }

                if (_batTimes.Count > 0) {
                    _batTimeRepository.DeleteEntities(start, end);
                    _batTimeRepository.SaveEntities(_batTimes);
                }
            } catch (Exception err) {
                Logger.Warning("电池充放电过程处理发生错误，详见错误日志。");
                Logger.Error("电池充放电过程处理发生错误", err);
            }
            #endregion

            #region 电池充放电曲线处理
            try {
                foreach (var _procedure in _batTimeRepository.GetProcedures(start, end)) {
                    try {
                        var _details = _batTimeRepository.GetProcDetails(_procedure.DeviceId, _procedure.PointId, _procedure.ProcTime, _procedure.ProcTime.AddSeconds(_maxInterval));
                        if (_details.Count == 0) continue;
                        var _start = _details.First().StartTime;
                        var _end = _details.Last().EndTime;

                        var _values = _batRepository.GetEntities(_procedure.DeviceId, _procedure.PointId, _start, _end);
                        if (_values.Count == 0) continue;

                        var _batCurves = new List<V_BatCurve>();
                        if (_values.Count <= _curveCount) {
                            foreach (var _value in _values) {
                                _batCurves.Add(new V_BatCurve {
                                    AreaId = _value.AreaId,
                                    StationId = _value.StationId,
                                    RoomId = _value.RoomId,
                                    DeviceId = _value.DeviceId,
                                    PointId = _value.PointId,
                                    PackId = _value.PackId,
                                    Type = _value.Type,
                                    StartTime = _value.StartTime,
                                    Value = _value.Value,
                                    ValueTime = _value.ValueTime,
                                    ProcTime = _procedure.ProcTime
                                });
                            }
                        } else {
                            var _ticks = _values.Count / (double)_curveCount;
                            var _target = _values.First();
                            for (var i = 0; i < _curveCount; i++) {
                                var __start = (int)Math.Floor(i * _ticks);
                                var __end = (int)Math.Floor((i + 1) * _ticks);
                                var __count = __end - __start;
                                if (__count > 0) {
                                    var __values = _values.Skip(__start).Take(__count);
                                    if(__values.Any()) _target = __values.Last();
                                }

                                _batCurves.Add(new V_BatCurve {
                                    AreaId = _target.AreaId,
                                    StationId = _target.StationId,
                                    RoomId = _target.RoomId,
                                    DeviceId = _target.DeviceId,
                                    PointId = _target.PointId,
                                    PackId = _target.PackId,
                                    Type = _target.Type,
                                    StartTime = _target.StartTime,
                                    Value = _target.Value,
                                    ValueTime = _target.ValueTime,
                                    ProcTime = _procedure.ProcTime
                                });
                            }
                        }

                        if (_batCurves.Count > 0) {
                            _batCurveRepository.DeleteEntities(_procedure.DeviceId, _procedure.PointId, _procedure.ProcTime, _procedure.ProcTime.AddSeconds(_maxInterval));
                            _batCurveRepository.SaveEntities(_batCurves);
                        }
                    } catch (Exception err) {
                        Logger.Warning("电池曲线处理发生错误，详见错误日志。");
                        Logger.Error("电池曲线处理发生错误", err);
                    }
                }
            } catch (Exception err) {
                Logger.Warning("电池曲线处理发生错误，详见错误日志。");
                Logger.Error("电池曲线处理发生错误", err);
            }
            #endregion
        }

        /// <summary>
        /// 信号测值统计任务
        /// </summary>
        private void DoTask003() {
            _allDone.WaitOne();

            if (_runStatus != RunStatus.Running) return;
            var _curTask = GlobalConfig.CurTasks.Find(t => t.Id == "T003");
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
                        GlobalConfig.SetTaskPloy(_curTask);
                        Logger.Information(string.Format("{0}执行完成，下次执行时间：{1}。", _curTask.Name, CommonHelper.ToDateTimeString(_curTask.Next)));
                    }
                }
            }
        }

        /// <summary>
        /// 信号测值统计方法
        /// </summary>
        private void ExTask003(DateTime start, DateTime end) {
            foreach (var model in GlobalConfig.StaticModels) {
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
            var _curTask = GlobalConfig.CurTasks.Find(t => t.Id == "T004");
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
            var _FzdlParam = GlobalConfig.CurParams.Find(p => p.Id == ParamId.FZDL);
            if (_FzdlParam == null || string.IsNullOrWhiteSpace(_FzdlParam.Value)) {
                Logger.Warning(string.Format("{0}尚未配置负载电流信号，线程退出。", _curTask.Name));
                return;
            }

            if (!iPemWorkContext.Points.ContainsKey(_FzdlParam.Value)) {
                Logger.Warning(string.Format("{0}负载电流信号配置错误，线程退出。", _curTask.Name));
                return;
            }

            var _FzdlPoint = iPemWorkContext.Points[_FzdlParam.Value];

            var _GzztParam = GlobalConfig.CurParams.Find(p => p.Id == ParamId.GZZT);
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
                        GlobalConfig.SetTaskPloy(_curTask);
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
                    if (!iPemWorkContext.DeviceSet1.ContainsKey(_device.Id)) continue;
                    var _current = iPemWorkContext.DeviceSet1[_device.Id];
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
                    if (!iPemWorkContext.DeviceSet1.ContainsKey(_device.Id)) continue;
                    var _current = iPemWorkContext.DeviceSet1[_device.Id];
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
            var _curTask = GlobalConfig.CurTasks.Find(t => t.Id == "T005");
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
                        GlobalConfig.SetTaskPloy(_curTask);
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
                foreach (var _device in _deviceRepository.GetEntities()) {
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
                Logger.Warning("资管接口同步站点发生错误，详见错误日志。");
                Logger.Error("资管接口同步站点发生错误", err);
            }
            #endregion

            #region 处理接口区域数据
            try {
                var _iAreas = new List<H_IArea>();
                foreach (var _area in _areaRepository.GetEntities()) {
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
            var _curTask = GlobalConfig.CurTasks.Find(t => t.Id == "T006");
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
                        GlobalConfig.SetTaskPloy(_curTask);
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
            foreach (var _signal in _signalRepository.GetEntities()) {
                if (!iPemWorkContext.DeviceSet1.ContainsKey(_signal.DeviceId)) continue;
                if (!iPemWorkContext.Points.ContainsKey(_signal.PointId)) continue;

                var _device = iPemWorkContext.DeviceSet1[_signal.DeviceId];
                var _point = iPemWorkContext.Points[_signal.PointId];
                var _skey = CommonHelper.JoinKeys(_point.Id, _device.Current.StationType.Id);
                if (!iPemWorkContext.SubPoints.ContainsKey(_skey)) continue;
                var _subpoint = iPemWorkContext.SubPoints[_skey];

                var _diff = new V_ParamDiff { DeviceId = _device.Current.Id, PointId = _point.Id };
                if (string.IsNullOrWhiteSpace(_signal.NMAlarmId)) _signal.NMAlarmId = null;
                if (string.IsNullOrWhiteSpace(_point.NMAlarmId)) _point.NMAlarmId = null;
                if (string.IsNullOrWhiteSpace(_signal.StorageRefTime)) _signal.StorageRefTime = null;
                if (string.IsNullOrWhiteSpace(_subpoint.StorageRefTime)) _subpoint.StorageRefTime = null;

                if (_signal.AlarmLimit != _subpoint.AlarmLimit) _diff.Threshold = string.Format("{0}&{1}", _signal.AlarmLimit, _subpoint.AlarmLimit);
                if (_signal.AlarmLevel != (int)_subpoint.AlarmLevel) _diff.AlarmLevel = string.Format("{0}&{1}", _signal.AlarmLevel, (int)_subpoint.AlarmLevel);
                if (_signal.NMAlarmId != _point.NMAlarmId) _diff.NMAlarmID = string.Format("{0}&{1}", _signal.NMAlarmId ?? "", _point.NMAlarmId ?? "");
                if (_signal.AbsoluteThreshold != _subpoint.AbsoluteThreshold) _diff.AbsoluteVal = string.Format("{0}&{1}", _signal.AbsoluteThreshold, _subpoint.AbsoluteThreshold);
                if (_signal.PerThreshold != _subpoint.PerThreshold) _diff.RelativeVal = string.Format("{0}&{1}", _signal.PerThreshold, _subpoint.PerThreshold);
                if (_signal.SavedPeriod != _subpoint.SavedPeriod) _diff.StorageInterval = string.Format("{0}&{1}", _signal.SavedPeriod, _subpoint.SavedPeriod);
                if (_signal.StorageRefTime != _subpoint.StorageRefTime) _diff.StorageRefTime = string.Format("{0}&{1}", _signal.StorageRefTime ?? "", _subpoint.StorageRefTime ?? "");

                _diff.Masked = GlobalConfig.Maskings.Contains(CommonHelper.JoinKeys(_device.Current.Id, "masking-all"));
                if (!_diff.Masked) _diff.Masked = GlobalConfig.Maskings.Contains(CommonHelper.JoinKeys(_device.Current.Id, _point.Id));

                if (string.IsNullOrWhiteSpace(_diff.Threshold)
                    && string.IsNullOrWhiteSpace(_diff.AlarmLevel)
                    && string.IsNullOrWhiteSpace(_diff.NMAlarmID)
                    && string.IsNullOrWhiteSpace(_diff.AbsoluteVal)
                    && string.IsNullOrWhiteSpace(_diff.RelativeVal)
                    && string.IsNullOrWhiteSpace(_diff.StorageInterval)
                    && string.IsNullOrWhiteSpace(_diff.StorageRefTime)
                    && !_diff.Masked
                    ) continue;

                _diffs.Add(_diff);
            }

            if (_diffs.Count > 0) 
                _paramDiffRepository.SaveEntities(_diffs, DateTime.Now);
        }

        /// <summary>
        /// 告警同步任务
        /// </summary>
        private void DoTask007() {
            _allDone.WaitOne();

            if (_runStatus != RunStatus.Running) return;
            var _curTask = GlobalConfig.CurTasks.Find(t => t.Id == "T007");
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
                        GlobalConfig.SetTaskPloy(_curTask);
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
                var starts = new List<A_FAlarm>();
                var ends = new List<A_FAlarm>();
                var finished = new List<KV<A_FAlarm, A_FAlarm>>();
                foreach (var alarm in alarms) {
                    var key = string.Format("{0}-{1}", alarm.SerialNo, alarm.FsuId);
                    if (alarm.AlarmFlag == EnmFlag.Begin) {
                        starts.Add(alarm);
                    } else if (alarm.AlarmFlag == EnmFlag.End) {
                        var existed = starts.Find(s => string.Format("{0}-{1}", s.SerialNo, s.FsuId).Equals(key));
                        if (existed != null) {
                            finished.Add(new KV<A_FAlarm, A_FAlarm>(existed, alarm));
                            starts.Remove(existed);
                        } else {
                            ends.Add(alarm);
                        }
                    }
                }
                #endregion

                #region 仅有开始告警
                foreach (var alarm in starts) {
                    var key = string.Format("{0}-{1}", alarm.SerialNo, alarm.FsuId);
                    if (GlobalConfig.AlarmKeys1.ContainsKey(key)) continue;
                    if (hisKeys.Contains(key)) continue;
                    _talmRepository.SaveEntities(alarm);
                }
                #endregion

                #region 仅有结束告警

                foreach (var alarm in ends) {
                    var key = string.Format("{0}-{1}", alarm.SerialNo, alarm.FsuId);
                    if (!GlobalConfig.AlarmKeys1.ContainsKey(key)) continue;
                    _talmRepository.SaveEntities(alarm);
                }

                #endregion

                #region 既有开始又有结束

                foreach (var alarm in finished) {
                    var key = string.Format("{0}-{1}", alarm.Key.SerialNo, alarm.Key.FsuId);
                    if (hisKeys.Contains(key)) continue;
                    _talmRepository.SaveEntities(alarm.Key, alarm.Value);
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
            //this.OnStop();
            //Environment.Exit(-1);
            Application.Restart();
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
            iPemWorkContext.Points = _pointRepository.GetEntities().ToDictionary(k => k.Id, v => v);
            iPemWorkContext.SubPoints = _subPointRepository.GetEntities().ToDictionary(k => CommonHelper.JoinKeys(k.PointId, k.StationTypeId), v => v);
            iPemWorkContext.Fsus = _fsuRepository.GetEntities();
            iPemWorkContext.Rooms = _roomRepository.GetEntities();
            iPemWorkContext.Stations = _stationRepository.GetEntities();

            //加载设备
            iPemWorkContext.Devices = new List<WcDevice>();
            foreach (var _device in _deviceRepository.GetEntities()) {
                var _current = new WcDevice(_device) { Signals = _signalRepository.GetEntities(_device.Id) };
                iPemWorkContext.Devices.Add(_current);
            }

            //加载设备Key
            iPemWorkContext.DeviceSet1 = new Dictionary<string, WcDevice>();
            iPemWorkContext.DeviceSet2 = new Dictionary<string, WcDevice>();
            foreach (var _device in iPemWorkContext.Devices) {
                iPemWorkContext.DeviceSet1[_device.Current.Id] = iPemWorkContext.DeviceSet2[CommonHelper.JoinKeys(_device.Current.FsuCode, _device.Current.Code)] = _device;
            }

            //加载区域
            iPemWorkContext.Areas = new List<WcArea>();
            foreach (var _area in _areaRepository.GetEntities()) {
                iPemWorkContext.Areas.Add(new WcArea(_area));
            }
            foreach (var current in iPemWorkContext.Areas) {
                current.Initializer(iPemWorkContext.Areas);
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
        private void LoadReservations() {
            GlobalConfig.Reservations = new List<ReservationModel>();
            var resNodes = _nodesInReservationRepository.GetEntities();
            foreach (var reservation in _reservationRepository.GetEntities()) {
                var nodes = resNodes.FindAll(a => a.ReservationId == reservation.Id);
                if (nodes.Count == 0) continue;

                var resDevices = new HashSet<string>();
                foreach (var node in nodes) {
                    if (node.NodeType == EnmSSH.Area) {
                        var current = iPemWorkContext.Areas.Find(a => a.Current.Id == node.NodeId);
                        if (current == null) continue;
                        var devices = iPemWorkContext.Devices.FindAll(d => current.Keys.Contains(d.Current.AreaId));
                        foreach (var device in devices) {
                            resDevices.Add(device.Current.Id);
                        }
                    } else if (node.NodeType == EnmSSH.Station) {
                        var devices = iPemWorkContext.Devices.FindAll(d => d.Current.StationId == node.NodeId);
                        foreach (var device in devices) {
                            resDevices.Add(device.Current.Id);
                        }
                    } else if (node.NodeType == EnmSSH.Room) {
                        var devices = iPemWorkContext.Devices.FindAll(d => d.Current.RoomId == node.NodeId);
                        foreach (var device in devices) {
                            resDevices.Add(device.Current.Id);
                        }
                    } else if (node.NodeType == EnmSSH.Device) {
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
        /// 加载告警数据
        /// </summary>
        private void LoadAlarms(bool clean = false) {
            GlobalConfig.InitAlarm();
            foreach (var alarm in _aalmRepository.GetEntities()) {
                GlobalConfig.AddAlarm(alarm);
            }

            if (!clean) return;

            #region 清理流水告警
            var talarms = new List<A_TAlarm>();
            foreach (var _alarm in _talmRepository.GetEntities()) {
                var key = CommonHelper.JoinKeys(_alarm.FsuId, _alarm.DeviceId);
                if (!iPemWorkContext.DeviceSet2.ContainsKey(key)) {
                    talarms.Add(_alarm);
                }
            }

            try {
                if (talarms.Count > 0) 
                    _talmRepository.Delete(talarms);
            } catch (Exception err) {
                Logger.Warning("清理流水告警错误，详见错误日志。");
                Logger.Error("清理流水告警错误", err);
            }
            #endregion

            #region 清理同步告警
            var falarms = new List<A_FAlarm>();
            foreach (var _alarm in _falmRepository.GetEntities()) {
                var key = CommonHelper.JoinKeys(_alarm.FsuId, _alarm.DeviceId);
                if (!iPemWorkContext.DeviceSet2.ContainsKey(key)) {
                    falarms.Add(_alarm);
                }
            }

            try {
                if (falarms.Count > 0)
                    _falmRepository.Delete(falarms);
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
        /// 加载服务参数数据
        /// </summary>
        private void LoadParams() {
            GlobalConfig.CurParams = _registry.GetParams();
        }

        /// <summary>
        /// 加载信号测值统计策略数据
        /// </summary>
        private void LoadStaticPolicies() {
            GlobalConfig.StaticModels = new List<StaticModel>();
            foreach (var _device in iPemWorkContext.Devices) {
                foreach (var _signal in _device.Signals) {
                    if (_signal.StaticPeriod > 0) {
                        GlobalConfig.StaticModels.Add(new StaticModel {
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
        }

        /// <summary>
        /// 加载电池数据处理策略
        /// </summary>
        private void LoadBatPolicies() {
            GlobalConfig.BatModels = new List<BatModel>();
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

                    if (!iPemWorkContext.DeviceSet1.ContainsKey(_pack.Key.DeviceId))
                        continue;

                    var _device = iPemWorkContext.DeviceSet1[_pack.Key.DeviceId];
                    var _signal = _device.Signals.Find(p => p.PointId == _master.PointId);
                    if (_signal == null) continue;

                    var _subIds = _pack.Where(p => p != _master).Select(g => g.PointId).ToArray();
                    var _subPoints = _device.Signals.FindAll(p => _subIds.Contains(p.PointId));
                    GlobalConfig.BatModels.Add(new BatModel {
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
        }

        /// <summary>
        /// 加载断站、停电、发电策略数据
        /// </summary>
        private void LoadCutPolicies() {
            GlobalConfig.Offs = new HashSet<string>();
            GlobalConfig.Cuttings = new HashSet<string>();
            GlobalConfig.Powers = new HashSet<string>();

            var pattern1 = @"\{站点断站\}";
            var pattern2 = @"\{站点停电\}";
            var pattern3 = @"\{站点发电\}";
            foreach (var _device in iPemWorkContext.Devices) {
                foreach (var _signal in _device.Signals) {
                    if (string.IsNullOrWhiteSpace(_signal.Extend)) continue;

                    if (Regex.IsMatch(_signal.Extend, pattern1))
                        GlobalConfig.Offs.Add(CommonHelper.JoinKeys(_signal.DeviceId, _signal.PointId));

                    if (Regex.IsMatch(_signal.Extend, pattern2))
                        GlobalConfig.Cuttings.Add(CommonHelper.JoinKeys(_signal.DeviceId, _signal.PointId));

                    if (Regex.IsMatch(_signal.Extend, pattern3))
                        GlobalConfig.Powers.Add(CommonHelper.JoinKeys(_signal.DeviceId, _signal.PointId));
                }
            }
        }

        /// <summary>
        /// 加载能耗公式数据
        /// </summary>
        private void LoadFormulas() {
            GlobalConfig.FormulaModels = _formulaRepository.GetEntities();
        }

        /// <summary>
        /// 获得实时能耗统计周期
        /// </summary>
        private DateTime GetPeriod(PeriodType period) {
            switch (period) {
                case PeriodType.Day:
                    return DateTime.Today;
                case PeriodType.Month:
                    return new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                default:
                    return DateTime.Today;
            }
        }
    }
}