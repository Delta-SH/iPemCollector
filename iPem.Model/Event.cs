using iPem.Core;
using System;

namespace iPem.Model {
    /// <summary>
    /// Represents a system event
    /// </summary>
    public partial class Event {
        /// <summary>
        /// EventTime
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// EventTime
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// EventType
        /// </summary>
        public EventType Type { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        public String Message { get; set; }

        /// <summary>
        /// FullMessage
        /// </summary>
        public String FullMessage { get; set; }
    }
}
