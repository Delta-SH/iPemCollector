using iPem.Core;
using System;

namespace iPem.Model {
    [Serializable]
    public partial class WcDevice {
        public Device Current { get; set; }

        public WcProtocol Protocol { get; set; }
    }
}
