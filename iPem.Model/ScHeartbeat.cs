using iPem.Core;
using System;

namespace iPem.Model {
    /// <summary>
    /// SC心跳策略信息
    /// </summary>
    public partial class ScHeartbeat {
        private int MAX_RETRY_COUNT = 3;

        public ScHeartbeat(Group current) {
            if (current == null) throw new ArgumentNullException("current");
            this.Current = current;
            this.SetCount(true);
        }

        /// <summary>
        /// SC信息类
        /// </summary>
        public Group Current { get; set; }

        /// <summary>
        /// 心跳次数
        /// </summary>
        private int Count { get; set; }

        /// <summary>
        /// 通信是否中断
        /// </summary>
        public bool IsOff {
            get { return this.Count >= MAX_RETRY_COUNT; }
        }

        /// <summary>
        /// 发送心跳
        /// </summary>
        public GetFsuInfoAckPackage KeepAlive() {
            return BIPackMgr.GetFsuInfo(new UriBuilder("http", this.Current.IP, this.Current.Port, "").ToString(), new GetFsuInfoPackage { FsuId = this.Current.Id });
        }

        /// <summary>
        /// 递增失败次数
        /// </summary>
        public void SetCount(bool force = false) {
            if (force || this.Count >= MAX_RETRY_COUNT)
                this.Count = 1;
            else
                this.Count++;
        }

    }
}
