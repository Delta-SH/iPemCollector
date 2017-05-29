using iPem.Core;
using System;

namespace iPem.Model {
    /// <summary>
    /// 日志信息
    /// </summary>
    public partial class Event {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 日志时间
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// 日志类型
        /// </summary>
        public EventType Type { get; set; }

        /// <summary>
        /// 日志摘要
        /// </summary>
        public String Message { get; set; }

        /// <summary>
        /// 详细信息
        /// </summary>
        public String FullMessage { get; set; }
    }
}
