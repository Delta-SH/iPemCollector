using iPem.Core;
using iPem.Data;
using iPem.Model;
using System;
using System.Collections.Generic;

namespace iPem.Task {
    public partial class ActTask02 : IActTask {
        public string Id {
            get { return "actTask02"; }
        }

        public string Name {
            get { return "历史告警处理任务"; }
        }

        public long Seconds { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public DateTime Last { get; set; }

        public DateTime Next { get; set; }

        public List<Event> Events { get; set; }

        public int Order {
            get { return 2; }
        }

        public void Execute() {
            var start = this.Last;
            var end = this.Next;

            var _hisAlmRepository = new A_HAlarmRepository();
            var _extendAlmRepository = new ExtAlarmRepository();
            var _appointmentRepository = new ReservationRepository();
            var _nodesInAppointmentRepository = new NodesInReservationRepository();

            #region 处理工程预约
            var _appointments = _appointmentRepository.GetEntities(start, end);
            var _appsets = new List<IdValuePair<Reservation, HashSet<string>>>();
            foreach(var _appointment in _appointments) {
                var _appnodes = _nodesInAppointmentRepository.GetEntities(_appointment.Id);
                var _appdevices = new HashSet<string>();
                foreach(var _node in _appnodes) {
                    if(_node.NodeType == EnmOrganization.Area) {
                        var _current = iPemWorkContext.Areas.Find(a => a.Current.Id == _node.NodeId);
                        if(_current == null) continue;
                        var _devices = iPemWorkContext.Devices.FindAll(d => _current.Keys.Contains(d.Current.AreaId));
                        foreach(var _device in _devices) {
                            _appdevices.Add(_device.Current.Id);
                        }
                    } else if(_node.NodeType == EnmOrganization.Station) {
                        var _devices = iPemWorkContext.Devices.FindAll(d => d.Current.StationId == _node.NodeId);
                        foreach(var _device in _devices) {
                            _appdevices.Add(_device.Current.Id);
                        }
                    } else if(_node.NodeType == EnmOrganization.Room) {
                        var _devices = iPemWorkContext.Devices.FindAll(d => d.Current.RoomId == _node.NodeId);
                        foreach(var _device in _devices) {
                            _appdevices.Add(_device.Current.Id);
                        }
                    } else if(_node.NodeType == EnmOrganization.Device) {
                        _appdevices.Add(_node.NodeId);
                    }
                }

                _appsets.Add(new IdValuePair<Reservation, HashSet<string>> {
                    Id = _appointment,
                    Value = _appdevices
                });
            }

            _extendAlmRepository.DeleteEntities(start, end);
            if(_appsets.Count > 0) {
                var _entities = new List<ExtAlarm>();
                var _alarms = _hisAlmRepository.GetEntities(start, end);
                foreach(var _alarm in _alarms) {
                    foreach(var _appset in _appsets) {
                        if(_appset.Value.Contains(_alarm.DeviceId)
                            && _appset.Id.StartTime >= _alarm.AlarmTime
                            && _appset.Id.EndTime <= _alarm.AlarmTime) {
                            _entities.Add(new ExtAlarm {
                                SerialNo = _alarm.SerialNo,
                                Id = _alarm.FsuId,
                                Time = _alarm.AlarmTime,
                                ProjectId = _appset.Id.Id
                            });
                            break;
                        }
                    }
                }

                if(_entities.Count > 0)
                    _extendAlmRepository.SaveEntities(_entities);
            }
            #endregion
        }
    }
}
