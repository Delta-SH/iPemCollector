using System;

namespace iPem.Core {
    /// <summary>
    /// Represents a sub device type
    /// </summary>
    [Serializable]
    public partial class SubDeviceType {
        /// <summary>
        ///Gets or sets the identifier
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///Gets or sets type of the device
        /// </summary>
        public string DeviceTypeId { get; set; }
    }
}
