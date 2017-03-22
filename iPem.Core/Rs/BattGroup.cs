using System;

namespace iPem.Core {
    /// <summary>
    /// Represents an battery
    /// </summary>
    [Serializable]
    public partial class BattGroup : Device {
        public string SingGroupCap { get; set; }

        public int SingVoltGrade { get; set; }

        public string SingGroupBattNumber { get; set; }
    }
}
