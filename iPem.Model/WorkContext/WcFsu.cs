using iPem.Core;
using System;
using System.Collections.Generic;

namespace iPem.Model {
    [Serializable]
    public partial class WcFsu {
        public Fsu Current { get; set; }

        public List<WcDevice> Devices { get; set; }
    }
}
