using iPem.Core;
using iPem.Data;
using iPem.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iPem.Task {
    public partial class HisTask02 : IHisTask {
        public string Id {
            get { return "hisTask02"; }
        }

        public string Name {
            get { return "后备时长处理任务"; }
        }

        public long Seconds { get; set; }

        public DateTime Time { get; set; }

        public DateTime Last { get; set; }

        public DateTime Next { get; set; }

        public List<Event> Events { get; set; }

        public int Order {
            get { return 2; }
        }

        public void Execute() {
            //TODO: 蓄电池电压、蓄电池电流、蓄电池工作状态(均充、浮充、放电)
            //TODO: 蓄电池后备时长 = (蓄电池单组容量 * 单组蓄电池个数) / 蓄电池放电状态下的放电电流
            var _dates = CommonHelper.GetDateSpan(this.Last, this.Next);
            if(_dates.Count == 0) return;

            try {
                var _dictionaryRepository = new DictionaryRepository();
                var _param = _dictionaryRepository.GetEntity(4);
                if(_param == null || string.IsNullOrWhiteSpace(_param.ValuesJson)) 
                    throw new Exception("未找到报表参数。");

                var _variable = JsonConvert.DeserializeObject<RtValues>(_param.ValuesJson);
                if(_variable.qtxdchbschglLeiXing == null || _variable.qtxdchbschglLeiXing.Length == 0)
                    throw new Exception("未设置蓄电池设备类型参数。");
                if(_variable.qtxdchbschglztXinHao == null || _variable.qtxdchbschglztXinHao.Length == 0)
                    throw new Exception("未设置蓄电池工作状态参数。");
                if(_variable.qtxdchbschglfzXinHao == null || _variable.qtxdchbschglfzXinHao.Length == 0)
                    throw new Exception("未设置蓄电池负载电流参数。");

                var _hisValueRepository = new HisValueRepository();
                var _battGroupRepository = new BattGroupRepository();
                var _hisBatTimeRepository = new HisBatTimeRepository();
                var _devices = _battGroupRepository.GetEntities().FindAll(d => _variable.qtxdchbschglLeiXing.Contains(d.SubType.Id));
                foreach(var _device in _devices) {
                    try {
                        var _current = iPemWorkContext.Devices.Find(d=>d.Current.Id == _device.Id);
                        if(_current == null) continue;
                        var _ztPoint = _current.Protocol.Points.FindAll(p => p.Type == EnmPoint.DI && _variable.qtxdchbschglztXinHao.Contains(p.Id)).FirstOrDefault();
                        if(_ztPoint == null) continue;
                        var _fzPoint = _current.Protocol.Points.FindAll(p => p.Type == EnmPoint.AI && _variable.qtxdchbschglfzXinHao.Contains(p.Id)).FirstOrDefault();
                        if(_fzPoint == null) continue;
                        var _ztFlag = CommonHelper.GetPointFlag(_ztPoint, "放电");
                        foreach(var _date in _dates) {
                            var _end = _date.AddDays(1).AddMilliseconds(-1);
                            var _ztValues = _hisValueRepository.GetEntities(_device.Id, _ztPoint.Id, _date, _end);
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

                            var _fzMatchs = new List<HisValue>();
                            if(_intervals.Count > 0) {
                                var _fzValues = _hisValueRepository.GetEntities(_device.Id, _fzPoint.Id, _date, _end);
                                foreach(var _interval in _intervals) {
                                    var _fzMatch = _fzValues.FindAll(f => f.UpdateTime >= _interval.Id && f.UpdateTime <= _interval.Value);
                                    if(_fzMatch.Count > 0) _fzMatchs.AddRange(_fzMatch);
                                }
                            }

                            var _batValue = -1d;
                            if(_fzMatchs.Count > 0) {
                                var _fzAverage = _fzMatchs.Average(f => f.Value);
                                if(_fzAverage != 0) {
                                    _batValue = (double.Parse(_device.SingGroupCap) * int.Parse(_device.SingGroupBattNumber)) / _fzAverage;
                                }
                            }

                            _hisBatTimeRepository.DeleteEntities(_date, _end);
                            _hisBatTimeRepository.SaveEntities(new List<HisBatTime> {
                                new HisBatTime {
                                    DeviceId = _device.Id,
                                    Period = _date,
                                    Value = _batValue,
                                    CreatedTime = DateTime.Now
                                }
                            });
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
        }
    }
}
