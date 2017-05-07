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
            var _dates = CommonHelper.GetDateSpan(this.Last, this.Next);
            if(_dates.Count == 0) return;

            try {
                var _dictionaryRepository = new DictionaryRepository();
                var _param = _dictionaryRepository.GetEntity(4);
                if(_param == null || string.IsNullOrWhiteSpace(_param.ValuesJson)) 
                    throw new Exception("未找到报表参数。");

                var _variable = JsonConvert.DeserializeObject<RtValues>(_param.ValuesJson);
                if(_variable.qtxdchbschglLeiXing == null || _variable.qtxdchbschglLeiXing.Length == 0)
                    throw new Exception("未设置开关电源设备类型参数。");
                if(_variable.qtxdchbschgldyXinHao == null || _variable.qtxdchbschgldyXinHao.Length == 0)
                    throw new Exception("未设置直流输出电压参数。");

                var _hisValueRepository = new HisValueRepository();
                var _hisBatTimeRepository = new HisBatTimeRepository();
                var _devices = iPemWorkContext.Devices.FindAll(d => _variable.qtxdchbschglLeiXing.Contains(d.Current.SubType.Id));
                foreach(var _device in _devices) {
                    try {
                        var _dyPoint = _device.Protocol.Points.FirstOrDefault(p => p.Type == EnmPoint.AI && _variable.qtxdchbschgldyXinHao.Contains(p.Id));
                        if(_dyPoint == null) continue;
                        foreach(var _date in _dates) {
                            var _end = _date.AddDays(1).AddMilliseconds(-1);
                            var _result = new List<HisBatTime>();
                            var _procedure = new List<HisValue>();

                            var _dyValues = _hisValueRepository.GetEntities(_device.Current.Id, _dyPoint.Id, _date, _end);
                            foreach(var _value in _dyValues.OrderBy(v => v.UpdateTime)) {
                                if(_value.Value < _variable.qtxdchbschglMax) {
                                    _procedure.Add(_value);
                                } else if(_value.Value >= _variable.qtxdchbschglMax && _procedure.Count > 0) {
                                    var _ordered = _procedure.OrderBy(p => p.Value);
                                    var _min = _ordered.First();
                                    var _max = _ordered.Last();
                                    _result.Add(new HisBatTime {
                                        AreaId = _value.AreaId,
                                        StationId = _value.StationId,
                                        RoomId = _value.RoomId,
                                        DeviceId = _value.DeviceId,
                                        StartTime = _max.UpdateTime,
                                        EndTime = _min.UpdateTime,
                                        StartValue = _max.Value,
                                        EndValue = _min.Value,
                                        CreatedTime = DateTime.Now
                                    });

                                    _procedure.Clear();
                                }
                            }

                            if(_procedure.Count > 0) {
                                var _ordered = _procedure.OrderBy(p => p.Value);
                                var _min = _ordered.First();
                                var _max = _ordered.Last();
                                _result.Add(new HisBatTime {
                                    AreaId = _max.AreaId,
                                    StationId = _max.StationId,
                                    RoomId = _max.RoomId,
                                    DeviceId = _max.DeviceId,
                                    StartTime = _max.UpdateTime,
                                    EndTime = _min.UpdateTime,
                                    StartValue = _max.Value,
                                    EndValue = _min.Value,
                                    CreatedTime = DateTime.Now
                                });

                                _procedure.Clear();
                            }

                            _hisBatTimeRepository.DeleteEntities(_date, _end);
                            _hisBatTimeRepository.SaveEntities(_result);
                        }
                    } catch(Exception err) {
                        this.Events.Add(new Event {
                            Id = Guid.NewGuid(),
                            Type = EventType.Error,
                            Time = DateTime.Now,
                            Message = string.Format("{0}({1})", err.Message, _device.Current.Id),
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
