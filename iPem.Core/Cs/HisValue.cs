using System;

namespace iPem.Core {
    [Serializable]
    public partial class HisValue {
        /// <summary>
        /// Gets or sets the area identifier
        /// </summary>
        public string AreaId { get; set; }

        /// <summary>
        /// Gets or sets the station identifier
        /// </summary>
        public string StationId { get; set; }

        /// <summary>
        /// Gets or sets the room identifier
        /// </summary>
        public string RoomId { get; set; }

        /// <summary>
        /// Gets or sets the fsu identifier
        /// </summary>
        public string FsuId { get; set; }

        /// <summary>
        /// Gets or sets the device identifier
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// Gets or sets the point identifier
        /// </summary>
        public string PointId { get; set; }

        /// <summary>
        /// Gets or sets the signal identifier
        /// </summary>
        public string SignalId { get; set; }

        /// <summary>
        /// Gets or sets the signal number
        /// </summary>
        public string SignalNumber { get; set; }

        /// <summary>
        /// Gets or sets the signal description
        /// </summary>
        public string SignalDesc { get; set; }

        /// <summary>
        /// Gets or sets the type
        /// </summary>
        public EnmPoint Type { get; set; }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Gets or sets the datetime
        /// </summary>
        public DateTime UpdateTime { get; set; }
    }
}
