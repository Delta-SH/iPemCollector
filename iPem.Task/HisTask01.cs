using iPem.Core;
using iPem.Data;
using iPem.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace iPem.Task {
    public partial class HisTask01 : IHisTask {
        public string Id {
            get { return "hisTask01"; }
        }

        public string Name {
            get { return "能耗数据处理任务"; }
        }

        public long Seconds { get; set; }

        public DateTime Time { get; set; }

        public DateTime Last { get; set; }

        public DateTime Next { get; set; }

        public List<Event> Events { get; set; }

        public int Order {
            get { return 1; }
        }

        public void Execute() {
            var _dates = CommonHelper.GetDateSpan(this.Last, this.Next);
            if(_dates.Count == 0) return;

            var _computer = new DataTable();
            var _formulaRepository = new FormulaRepository();
            var _formulas = _formulaRepository.GetEntities();
            foreach(var _formula in _formulas) {
                try {
                    var _formulaText = _formula.FormulaText;
                    if(string.IsNullOrWhiteSpace(_formulaText)) continue;
                    if(!CommonHelper.ValidateFormula(_formulaText)) throw new Exception("无效的公式。");
                    var _variables = CommonHelper.GetFormulaVariables(_formulaText);
                    if(_variables == null) throw new Exception("无效的公式。");

                    var _devices = new List<WcDevice>();
                    if(_formula.Type == EnmOrganization.Station) {
                        var _current = iPemWorkContext.Stations.Find(c => c.Current.Id == _formula.Id);
                        if(_current == null) throw new Exception("未找到公式所对应的站点。");
                        _devices.AddRange(_current.Rooms.SelectMany(r => r.Devices));
                    } else if(_formula.Type == EnmOrganization.Room) {
                        var _current = iPemWorkContext.Rooms.Find(c => c.Current.Id == _formula.Id);
                        if(_current == null) throw new Exception("未找到公式所对应的机房。");
                        _devices.AddRange(_current.Devices);
                    }

                    var _details = new List<VariableDetail>();
                    foreach(var _variable in _variables) {
                        var _factors = _variable.Split(new string[] { ">>" }, StringSplitOptions.None);
                        var _devkey = _factors[0].Substring(1);
                        var _potkey = _factors[1];
                        var _device = _devices.Find(d => d.Current.Name == _devkey);
                        if(_device == null) throw new Exception(string.Format("未找到变量{0}中的设备信息。", _variable));
                        var _point = _device.Protocol.Points.Find(p => p.Name == _potkey);
                        if(_point == null) throw new Exception(string.Format("未找到变量{0}中的信号信息。", _variable));
                        _details.Add(new VariableDetail { Device = _device.Current, Point = _point, Variable = _variable });
                    }

                    var _hisMeasureRepository = new HisMeasureRepository();
                    var _result = new List<HisElec>();
                    foreach(var _date in _dates) {
                        var _current = _formulaText;
                        var _value = 0d;
                        try {
                            foreach(var _detail in _details) {
                                var _diff = _hisMeasureRepository.GetValDiff(_detail.Device.Id, _detail.Point.Id, _date, _date.AddDays(1).AddMilliseconds(-1));
                                _current = _current.Replace(_detail.Variable, _diff.ToString());
                            }

                            _value = (double)_computer.Compute(_current, "");
                        } catch {}

                        _result.Add(new HisElec {
                            Id = _formula.Id,
                            Type = _formula.Type,
                            FormulaType = _formula.FormulaType,
                            Period = _date,
                            Value = double.IsNaN(_value) || double.IsInfinity(_value) ? 0d : _value
                        });
                    }

                    if(_result.Count > 0) {
                        var _hisElecRepository = new HisElecRepository();
                        _hisElecRepository.SaveEntities(_result);
                    }
                } catch(Exception err) {
                    this.Events.Add(new Event {
                        Id = Guid.NewGuid(),
                        Type = EventType.Error,
                        Time = DateTime.Now,
                        Message = string.Format("{0}({1},{2},{3},{4})", err.Message, _formula.Id, (int)_formula.Type, (int)_formula.FormulaType, _formula.FormulaText),
                        FullMessage = err.StackTrace
                    });
                }
            }

            if(this.Events.Count > 0) throw new Exception(string.Format("执行完成，发生{0}次错误(详见日志)。", this.Events.Count));
        }
    }
}
