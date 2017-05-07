using iPem.Core;
using iPem.Data;
using iPem.Model;
using System;
using System.Collections.Generic;

namespace iPem.Task {
    public partial class HisTask04 : IHisTask {
        public string Id {
            get { return "hisTask04"; }
        }

        public string Name {
            get { return "接口数据处理任务"; }
        }

        public long Seconds { get; set; }

        public DateTime Time { get; set; }

        public DateTime Last { get; set; }

        public DateTime Next { get; set; }

        public List<Event> Events { get; set; }

        public int Order {
            get { return 4; }
        }

        public void Execute() {
            try {
                if(this.Last.Year == DateTime.Today.Year && this.Last.Month == DateTime.Today.Month)
                    throw new Exception("此任务本月已经执行，本次将被忽略。");

                #region 处理接口设备数据
                var amDevices = new List<AmDevice>();
                foreach(var device in iPemWorkContext.Devices) {
                    amDevices.Add(new AmDevice {
                        Id = device.Current.Id,
                        Name = device.Current.Name,
                        Type = device.Current.Type.Name,
                        ParentId = device.Current.StationId,
                        CreatedTime = DateTime.Now
                    });
                }

                var _amDeviceRepository = new AmDeviceRepository();
                _amDeviceRepository.DeleteEntities();
                _amDeviceRepository.SaveEntities(amDevices);
                #endregion

                #region 处理接口站点数据
                var amStations = new List<AmStation>();
                foreach(var station in iPemWorkContext.Stations) {
                    var parent = iPemWorkContext.Areas.Find(a => a.Current.Id == station.Current.AreaId);
                    if(parent == null) continue;
                    amStations.Add(new AmStation {
                        Id = station.Current.Id,
                        Name = station.Current.Name,
                        Type = station.Current.Type.Name,
                        Parent = parent.Current.Name,
                        CreatedTime = DateTime.Now
                    });
                }

                var _amStationRepository = new AmStationRepository();
                _amStationRepository.DeleteEntities();
                _amStationRepository.SaveEntities(amStations);
                #endregion

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
