using iPem.Core;
using System;

namespace iPem.Model {
    /// <summary>
    /// SC心跳策略信息
    /// </summary>
    public partial class ScHeartbeat {
        public ScHeartbeat(Group current) {
            if (current == null) throw new ArgumentNullException("current");
            this.Current = current;
            this.SetInterval(true);
            this.SetCount(true);
        }

        /// <summary>
        /// SC信息类
        /// </summary>
        public Group Current { get; set; }

        /// <summary>
        /// 心跳间隔时间
        /// </summary>
        private int Interval { get; set; }

        /// <summary>
        /// 心跳次数
        /// </summary>
        private int Count { get; set; }

        /// <summary>
        /// 是否需要发送心跳
        /// </summary>
        public bool IsOk {
            get { return this.Interval >= 20; }
        }

        /// <summary>
        /// 通信是否中断
        /// </summary>
        public bool IsOff {
            get { return this.Count >= 3; }
        }

        /// <summary>
        /// 发送心跳
        /// </summary>
        public GetFsuInfoAckPackage Keep() {
            return BIPackMgr.GetFsuInfo(new UriBuilder("http", this.Current.IP, this.Current.Port, "/services/FSUService").ToString(), new GetFsuInfoPackage { FsuId = this.Current.Id });
        }

        /// <summary>
        /// 递增计时器
        /// </summary>
        public void SetInterval(bool force = false) {
            if (force || this.Interval >= 20)
                this.Interval = 1;
            else
                this.Interval++;
        }

        /// <summary>
        /// 递增失败次数
        /// </summary>
        public void SetCount(bool force = false) {
            if (force || this.Count >= 3)
                this.Count = 1;
            else
                this.Count++;
        }

    }
}
