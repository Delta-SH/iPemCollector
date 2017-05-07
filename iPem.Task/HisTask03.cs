using iPem.Core;
using iPem.Data;
using iPem.Data.Cs;
using iPem.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iPem.Task {
    public partial class HisTask03 : IHisTask {
        public string Id {
            get { return "hisTask03"; }
        }

        public string Name {
            get { return "带载合格率处理任务"; }
        }

        public long Seconds { get; set; }

        public DateTime Time { get; set; }

        public DateTime Last { get; set; }

        public DateTime Next { get; set; }

        public List<Event> Events { get; set; }

        public int Order {
            get { return 3; }
        }

        public void Execute() {
            var _dates = CommonHelper.GetDateSpan(this.Last, this.Next);
            if(_dates.Count == 0) return;

            try {
                var _dictionaryRepository = new DictionaryRepository();
                var _param = _dictionaryRepository.GetEntity(4);
                if(_param == null || string.IsNullOrWhiteSpace(_param.ValuesJson))
                    throw new Exception("未找到报表参数。");

                var _variable = JsonConvert.DeserializeObject<RtValues>(_param.ValuesJson);
                if(_variable.qtkgdydzhglLeiXing == null || _variable.qtkgdydzhglLeiXing.Length == 0)
                    throw new Exception("未设置开关电源设备类型参数。");
                if(_variable.qtkgdydzhglztXinHao == null || _variable.qtkgdydzhglztXinHao.Length == 0)
                    throw new Exception("未设置开关电源工作状态参数。");
                if(_variable.qtkgdydzhglfzXinHao == null || _variable.qtkgdydzhglfzXinHao.Length == 0)
                    throw new Exception("未设置开关电源负载电流参数。");

                var _hisValueRepository = new HisValueRepository();
                var _combSwitElecSourRepository = new CombSwitElecSourRepository();
                var _divSwitElecSourRepository = new DivSwitElecSourRepository();
                var _hisLoadRateRepository = new HisLoadRateRepository();
                var _comDevices = _combSwitElecSourRepository.GetEntities().FindAll(d => _variable.qtkgdydzhglLeiXing.Contains(d.SubType.Id));
                var _divDevices = _divSwitElecSourRepository.GetEntities().FindAll(d => _variable.qtkgdydzhglLeiXing.Contains(d.SubType.Id));

                //组合开关电源
                foreach(var _device in _comDevices) {
                    try {
                        var _current = iPemWorkContext.Devices.Find(d => d.Current.Id == _device.Id);
                        if(_current == null) continue;
                        var _ztPoint = _current.Protocol.Points.FirstOrDefault(p => p.Type == EnmPoint.DI && _variable.qtkgdydzhglztXinHao.Contains(p.Id));
                        if(_ztPoint == null) continue;
                        var _fzPoint = _current.Protocol.Points.FirstOrDefault(p => p.Type == EnmPoint.AI && _variable.qtkgdydzhglfzXinHao.Contains(p.Id));
                        if(_fzPoint == null) continue;
                        var _fzCap = double.Parse(_device.SingRModuleRatedOPCap) * int.Parse(_device.ExisRModuleCount);
                        if(_fzCap == 0) continue;

                        var _ztFlag = CommonHelper.GetPointFlag(_ztPoint, "浮充");
                        foreach(var _date in _dates) {
                            var _end = _date.AddDays(1).AddMilliseconds(-1);
                            var _ztValues = _hisValueRepository.GetEntities(_device.Id, _ztPoint.Id, _date, _end);
                            var _result = new List<HisLoadRate>();
                            var _intervals = new List<IdValuePair<DateTime, DateTime>>();

                            DateTime? _start = null;
                            foreach(var _value in _ztValues.OrderBy(v => v.UpdateTime)) {
                                if(_value.Value == _ztFlag && !_start.HasValue) {
                                    _start = _value.UpdateTime;
                                } else if(_value.Value != _ztFlag && _start.HasValue) {
                                    _intervals.Add(new IdValuePair<DateTime, DateTime> {
                                        Id = _start.Value,
                                        Value = _value.UpdateTime
                                    });

                                    _start = null;
                                }
                            }

                            if(_start.HasValue) {
                                _intervals.Add(new IdValuePair<DateTime, DateTime> {
                                    Id = _start.Value,
                                    Value = _end
                                });

                                _start = null;
                            }

                            if(_intervals.Count > 0) {
                                var _fzValues = _hisValueRepository.GetEntities(_device.Id, _fzPoint.Id, _date, _end);
                                foreach(var _interval in _intervals) {
                                    var _fzMatch = _fzValues.FindAll(f => f.UpdateTime >= _interval.Id && f.UpdateTime <= _interval.Value);
                                    var _fzMax = _fzMatch.Max(f => f.Value);
                                    var _loadValue = _fzMax / _fzCap;
                                    _result.Add(new HisLoadRate {
                                        AreaId = _device.AreaId,
                                        StationId = _device.StationId,
                                        RoomId = _device.RoomId,
                                        DeviceId = _device.Id,
                                        StartTime = _interval.Id,
                                        EndTime = _interval.Value,
                                        Value = _loadValue,
                                        CreatedTime = DateTime.Now
                                    });
                                }
                            }

                            _hisLoadRateRepository.DeleteEntities(_date, _end);
                            _hisLoadRateRepository.SaveEntities(_result);
                        }
                    } catch(Exception err) {
                        this.Events.Add(new Event {
                            Id = Guid.NewGuid(),
                            Type = EventType.Error,
                            Time = DateTime.Now,
                            Message = string.Format("{0}({1})", err.Message, _device.Id),
                            FullMessage = err.StackTrace
                        });
                    }
                }

                //分列开关电源
                foreach(var _device in _divDevices) {
                    try {
                        var _current = iPemWorkContext.Devices.Find(d => d.Current.Id == _device.Id);
                        if(_current == null) continue;
                        var _ztPoint = _current.Protocol.Points.FirstOrDefault(p => p.Type == EnmPoint.DI && _variable.qtkgdydzhglztXinHao.Contains(p.Id));
                        if(_ztPoint == null) continue;
                        var _fzPoint = _current.Protocol.Points.FirstOrDefault(p => p.Type == EnmPoint.AI && _variable.qtkgdydzhglfzXinHao.Contains(p.Id));
                        if(_fzPoint == null) continue;
                        var _fzCap = double.Parse(_device.SingRModuleRatedOPCap) * int.Parse(_device.ExisRModuleCount);
                        if(_fzCap == 0) continue;

                        var _ztFlag = CommonHelper.GetPointFlag(_ztPoint, "浮充");
                        foreach(var _date in _dates) {
                            var _end = _date.AddDays(1).AddMilliseconds(-1);
                            var _ztValues = _hisValueRepository.GetEntities(_device.Id, _ztPoint.Id, _date, _end);
                            var _result = new List<HisLoadRate>();
                            var _intervals = new List<IdValuePair<DateTime, DateTime>>();

                            DateTime? _start = null;
                            foreach(var _value in _ztValues.OrderBy(v => v.UpdateTime)) {
                                if(_value.Value == _ztFlag && !_start.HasValue) {
                                    _start = _value.UpdateTime;
                                } else if(_value.Value != _ztFlag && _start.HasValue) {
                                    _intervals.Add(new IdValuePair<DateTime, DateTime> {
                                        Id = _start.Value,
                                        Value = _value.UpdateTime
                                    });

                                    _start = null;
                                }
                            }

                            if(_start.HasValue) {
                                _intervals.Add(new IdValuePair<DateTime, DateTime> {
                                    Id = _start.Value,
                                    Value = _end
                                });

                                _start = null;
                            }

                            if(_intervals.Count > 0) {
                                var _fzValues = _hisValueRepository.GetEntities(_device.Id, _fzPoint.Id, _date, _end);
                                foreach(var _interval in _intervals) {
                                    var _fzMatch = _fzValues.FindAll(f => f.UpdateTime >= _interval.Id && f.UpdateTime <= _interval.Value);
                                    var _fzMax = _fzMatch.Max(f => f.Value);
                                    var _loadValue = _fzMax / _fzCap;
                                    _result.Add(new HisLoadRate {
                                        AreaId = _device.AreaId,
                                        StationId = _device.StationId,
                                        RoomId = _device.RoomId,
                                        DeviceId = _device.Id,
                                        StartTime = _interval.Id,
                                        EndTime = _interval.Value,
                                        Value = _loadValue,
                                        CreatedTime = DateTime.Now
                                    });
                                }
                            }

                            _hisLoadRateRepository.DeleteEntities(_date, _end);
                            _hisLoadRateRepository.SaveEntities(_result);
                        }
                    } catch(Exception err) {
                        this.Events.Add(new Event {
                            Id = Guid.NewGuid(),
                            Type = EventType.Error,
                            Time = DateTime.Now,
                            Message = string.Format("{0}({1})", err.Message, _device.Id),
                            FullMessage = err.StackTrace
                        });
                    }
                }
            } catch(Exception err) {
                this.Events.Add(new Event {
                    Id = Guid.NewGuid(),
                    Type = EventType.Error,
                    Time = DateTime.Now,
                    Message = err.Message,
                    FullMessage = err.StackTrace
                });
            }

            if(this.Events.Count > 0) throw new Exception(string.Format("执行完成，发生{0}次错误(详见日志)。", this.Events.Count));
        }
    }
}
