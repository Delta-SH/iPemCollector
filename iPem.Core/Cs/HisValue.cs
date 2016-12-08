using System;

namespace iPem.Core {
    [Serializable]
    public partial class HisValue {
        /// <summary>
        /// Gets or sets the device identifier
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// Gets or sets the point identifier
        /// </summary>
        public string PointId { get; set; }

        /// <summary>
        /// Gets or sets the type
        /// </summary>
        public EnmValType ValType { get; set; }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Gets or sets the threshold
        /// </summary>
        public double Threshold { get; set; }

        /// <summary>
        /// Gets or sets the state
        /// </summary>
        public EnmState State { get; set; }

        /// <summary>
        /// Gets or sets the datetime
        /// </summary>
        public DateTime Time { get; set; }
    }
}
