using System;

namespace iPem.Core {
    [Serializable]
    public partial class HisLoadRate {
        public string DeviceId { get; set; }

        public DateTime Period { get; set; }

        public double Value { get; set; }

        public DateTime CreatedTime { get; set; }
    }
}
